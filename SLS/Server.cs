using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Collections;

namespace SLS
{
    class Server
    {
        public static INIClass configFile;
        public static Log logger;
        private Log recoveryWriter;
        private IoServer ioServer;
        private string latestUIVersion;
        public static int caseSeqNo;
        public static int productHintSeqNo;
        private static string year;
        private Dictionary<string, StringBuilder> followUps;
        private Dictionary<string, StringBuilder> caseHints;
        private Dictionary<string, Dictionary<string, string>> loginUserAndCases;
        private Dictionary<string, string> IDNameDic;
        private Dictionary<string, object[]> outstandingCases;
        private string notice;
        private static int broadcastInterval;
        public static int port;
        private SQLConnectionPool sqlconnectionPool;

        public Server()
        {
            configFile = new INIClass(AppDomain.CurrentDomain.BaseDirectory + "configuration.ini");
            logger = new Log("SLSlog.txt");
            recoveryWriter = new Log("Recovery.txt");
            sqlconnectionPool = new SQLConnectionPool(10, 30, configFile);
            sqlconnectionPool.Inital();
            followUps = new Dictionary<string, StringBuilder>();
            caseHints = new Dictionary<string, StringBuilder>();
            loginUserAndCases = new Dictionary<string, Dictionary<string, string>>();
            IDNameDic = new Dictionary<string, string>();
            outstandingCases = new Dictionary<string, object[]>();
            port = Int32.Parse(configFile.Read("SLS", "port"));
            latestUIVersion = configFile.Read("UIUpgrade", "version");
            notice = GetFormatNotice(configFile.Read("NOTICE", "notice"), "/r/n", "\r\n");
            caseSeqNo = Int32.Parse(configFile.Read("CASE", "seq"));
            productHintSeqNo = Int32.Parse(configFile.Read("ProductHint", "seq"));
            year = configFile.Read("CASE", "year");
            broadcastInterval = Int32.Parse(configFile.Read("BroadCast", "Interval"));
            ThreadPool.QueueUserWorkItem(new WaitCallback(InitialOutstandingCases));
            ThreadPool.QueueUserWorkItem(new WaitCallback(InitialFollowUp));
            ThreadPool.QueueUserWorkItem(new WaitCallback(InitialCaseHints));
            ThreadPool.QueueUserWorkItem(new WaitCallback(MapUserIDToName));
        }

        private void MapUserIDToName(object obj)
        {
            SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
            if (sqlConnection == null)
            {
                logger.Info("Can't get DB connection");
                return;
            }
            try
            {
                string userName_Column = Config.getValue("80"), userID_Column = Config.getValue("87");
                string cmd = new StringBuilder("select ").Append(userName_Column).Append(",").Append(userID_Column).Append(" from ").Append(Config.DBName_UserFile).ToString();

                SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection);
                using (SqlDataReader re = sqlcmd.ExecuteReader())
                {
                    while (re.Read())
                    {
                        IDNameDic.Add(re.GetValue(re.GetOrdinal(userID_Column)).ToString().Trim(), re.GetValue(re.GetOrdinal(userName_Column)).ToString().Trim());
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            sqlconnectionPool.releaseConnection(sqlConnection);
        }

        private void InitialOutstandingCases(object obj)
        {
            SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
            if (sqlConnection == null)
            {
                logger.Info("Can't get DB connection");
                return;
            }
            string cmd = new StringBuilder("select * from ").Append(Config.DBName_SupportLogSheet).Append(" where ").Append(Config.KeyValue_pair["11"]).Append(" != 'Closed'").ToString();
            SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection);
            string[] loadNames = Config.getValues(Config.LoadCaseKeys);
            using (SqlDataReader re = sqlcmd.ExecuteReader())
            {
                while (re.Read())
                {
                    Message msg = new Message();
                    for (int i = 0; i < loadNames.Length; i++)
                    {
                        try
                        {
                            msg.SetKeyValuePair(Config.LoadCaseKeys[i], re.GetValue(re.GetOrdinal(loadNames[i])).ToString().Trim(' '));
                        }
                        catch
                        {
                            msg.SetKeyValuePair(Config.LoadCaseKeys[i], re.GetDateTime(re.GetOrdinal(Config.LoadCaseKeys[i])).ToString("yyyy-MM-dd HH:mm:ss").Trim(' '));
                        }
                    }
                    outstandingCases.Add(msg.GetValueFromPairs("1"), new object[] { new object(), msg });
                }
            }
            sqlconnectionPool.releaseConnection(sqlConnection);
        }

        private void AddOutstandingCase(string caseID)
        {
            Message msg = new Message();
            SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
            if (sqlConnection == null)
            {
                logger.Info("Can't get DB connection");
                return;
            }
            try
            {
                string cmd = GenSelectSql(Config.DBName_SupportLogSheet, null, new string[] { "1" }, new string[] { caseID });
                SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection);
                string[] loadNames = Config.getValues(Config.LoadCaseKeys);
                using (SqlDataReader re = sqlcmd.ExecuteReader())
                {
                    while (re.Read())
                    {
                        for (int i = 0; i < loadNames.Length; i++)
                        {
                            msg.SetKeyValuePair(Config.LoadCaseKeys[i], re.GetValue(re.GetOrdinal(loadNames[i])).ToString().Trim(' '));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            sqlconnectionPool.releaseConnection(sqlConnection);
            outstandingCases.Add(caseID, new object[] { new object(), msg });
        }

        private string CompareCase(ref Message newMsg)
        {
            string caseID = newMsg.GetValueFromPairs("1");
            string[] checkNames = Config.getValues(Config.CompareCaseKeys);
            StringBuilder changes = new StringBuilder();
            try
            {
                lock (outstandingCases[caseID][0])
                {
                    Message msg = (Message)outstandingCases[caseID][1];
                    for (int i = 0; i < Config.CompareCaseKeys.Length; i++)
                    {
                        string key = Config.CompareCaseKeys[i];
                        string beforeValue = msg.GetValueFromPairs(key);
                        string newValue = newMsg.GetValueFromPairs(key);
                        if (!newValue.Equals(beforeValue))
                        {
                            changes.Append("[").Append(checkNames[i]).Append("] ").Append(beforeValue).Append(" —>").Append(newValue).Append("   ");
                        }
                    }
                    if (changes.Length != 0)
                    {
                        outstandingCases[caseID][1] = newMsg;
                    }
                    return changes.ToString();
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return "";
            }
        }

        private string GenCaseID(string incharge)
        {
            StringBuilder sb = new StringBuilder("SLM-");
            sb.Append(year).Append("-").Append(caseSeqNo.ToString("D5")).Append("-").Append(incharge);
            Interlocked.Increment(ref caseSeqNo);
            configFile.Write("CASE", "seq", caseSeqNo.ToString());
            return sb.ToString();
        }

        private string GenProductHintID(string incharge)
        {
            StringBuilder sb = new StringBuilder("PH-");
            sb.Append(productHintSeqNo.ToString("D5")).Append("-").Append(incharge);
            Interlocked.Increment(ref productHintSeqNo);
            configFile.Write("ProductHint", "seq", productHintSeqNo.ToString());
            return sb.ToString();
        }

        private void InitialCaseHints(object obj)
        {
            SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
            if (sqlConnection == null)
            {
                logger.Info("Can't get DB connection");
                return;
            }
            try
            {
                logger.Info("Start Load case Hints");
                string cmd = "select C.CaseID,C.Hints from caseHints C, SupportLogSheet S where C.CaseID=S.CaseID and S.Status != 'Closed'";
                SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection);
                using (SqlDataReader re = sqlcmd.ExecuteReader())
                {
                    Message msg = new Message();
                    string[] DBColumnNames = Config.getValues(Config.CaseHintsKeys);
                    string[] DBColumnKeys = Config.CaseHintsKeys;
                    while (re.Read())
                    {
                        for (int i = 0; i < DBColumnNames.Length; i++)
                        {
                            msg.SetKeyValuePair(DBColumnKeys[i], re.GetValue(re.GetOrdinal(DBColumnNames[i])).ToString().Trim(' '));
                        }
                        string CaseNo = msg.GetValueFromPairs("1");
                        if (caseHints.ContainsKey(CaseNo))
                        {
                            caseHints[CaseNo].Append(Config.Msg_Separator3).Append(msg.GetMsg());
                        }
                        else
                        {
                            caseHints.Add(CaseNo, new StringBuilder(msg.GetMsg()));
                        }
                    }
                }
                logger.Info("Load case hints done!");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            sqlconnectionPool.releaseConnection(sqlConnection);
        }

        private void InitialFollowUp(object obj)
        {
            SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
            if (sqlConnection == null)
            {
                logger.Info("Can't get DB connection");
                return;
            }
            try
            {
                logger.Info("Start Load case followUps");
                string cmd = "select C.CaseID,C.TimeModified,C.FollowUp from CaseFollowUp C inner join SupportLogSheet S on C.CaseID = S.CaseID and S.Status != 'Closed'";
                SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection);
                using (SqlDataReader re = sqlcmd.ExecuteReader())
                {
                    string[] DBColumnNames = Config.getValues(Config.CaseFollowUpKeys);
                    string[] DBColumnKeys = Config.CaseFollowUpKeys;
                    while (re.Read())
                    {
                        Message msg = new Message();
                        for (int i = 0; i < DBColumnNames.Length; i++)
                        {
                            try
                            {
                                msg.SetKeyValuePair(DBColumnKeys[i], re.GetString(re.GetOrdinal(DBColumnNames[i])).Trim(' '));
                            }
                            catch
                            {
                                msg.SetKeyValuePair(DBColumnKeys[i], re.GetDateTime(re.GetOrdinal(DBColumnNames[i])).ToString("yyyy-MM-dd HH:mm:ss").Trim(' '));
                            }
                        }
                        string CaseNo = msg.GetValueFromPairs("1");
                        if (followUps.ContainsKey(CaseNo))
                        {
                            followUps[CaseNo].Append(Config.Msg_Separator3).Append(msg.GetMsg());
                        }
                        else
                        {
                            followUps.Add(CaseNo, new StringBuilder(msg.GetMsg()));
                        }
                    }
                }
                logger.Info("Load followUps done!");
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            sqlconnectionPool.releaseConnection(sqlConnection);
        }

        private void FindCaseHints(string CaseNo, string sentFrom, SocketAsyncEventArgs sockArg)
        {
            SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
            if (sqlConnection == null)
            {
                logger.Info("Can't get DB connection");
                return;
            }
            try
            {
                string cmd = GenSelectSql(Config.DBName_CaseHints, null, new string[] { "1" }, new string[] { CaseNo });
                Message msg = new Message();
                msg.SetKeyValuePair("1", CaseNo);
                msg.SetKeyValuePair("73", "");
                using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                {
                    using (SqlDataReader re = sqlcmd.ExecuteReader())
                    {
                        if (re.HasRows == false)
                        {
                            re.Close();
                            re.Dispose();
                            bool sqlError_hints = false;
                            string hcmd = GenAddSql(Config.DBName_CaseHints, Config.CaseHintsKeys, msg);
                            try
                            {
                                using (SqlCommand hsqlcmd = new SqlCommand(hcmd, sqlConnection))
                                {
                                    hsqlcmd.ExecuteNonQuery();
                                }
                            }
                            catch (SqlException ex)
                            {
                                sqlError_hints = true;
                                logger.Error(ex);
                            }
                            finally
                            {
                                if (!sqlError_hints)
                                {
                                    caseHints.Add(CaseNo, new StringBuilder(msg.GetMsg()));
                                    OutputSocketMsg hsm = new OutputSocketMsg(new List<SocketAsyncEventArgs> { sockArg }, 1, IoServer.GenOutSocketMsg(sentFrom, "H0", msg.GetMsg()));
                                    ioServer.AddOutSockMsg(hsm);
                                }
                            }
                            return;
                        }
                        while (re.Read())
                        {
                            msg.SetKeyValuePair("73", re.GetValue(re.GetOrdinal(Config.getValue("73"))).ToString().Trim(' '));
                            caseHints.Add(CaseNo, new StringBuilder(msg.GetMsg()));
                        }
                        OutputSocketMsg hsm1 = new OutputSocketMsg(new List<SocketAsyncEventArgs> { sockArg }, 1, IoServer.GenOutSocketMsg(sentFrom, "H0", caseHints[CaseNo].ToString()));
                        ioServer.AddOutSockMsg(hsm1);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            sqlconnectionPool.releaseConnection(sqlConnection);
        }

        private void FindFollowUp(string CaseNo, string sentFrom, SocketAsyncEventArgs sockArg)
        {
            SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
            if (sqlConnection == null)
            {
                logger.Info("Can't get DB connection");
                return;
            }
            try
            {
                string cmd = GenSelectSql(Config.DBName_CaseFollowUp, null, new string[] { "1" }, new string[] { CaseNo });
                using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                {
                    Message msg = new Message();
                    using (SqlDataReader re = sqlcmd.ExecuteReader())
                    {
                        if (re.HasRows == false)
                        {
                            re.Close();
                            bool sqlError_followup = false;
                            msg.SetKeyValuePair("1", CaseNo);
                            msg.SetKeyValuePair("50", DateTime.Now.ToString());
                            msg.SetKeyValuePair("52", "");
                            string fcmd = GenAddSql(Config.DBName_CaseFollowUp, Config.CaseFollowUpKeys, msg);
                            try
                            {
                                using (SqlCommand fsqlcmd = new SqlCommand(fcmd, sqlConnection))
                                {
                                    fsqlcmd.ExecuteNonQuery();
                                }
                            }
                            catch (SqlException ex)
                            {
                                sqlError_followup = true;
                                logger.Error(ex);
                            }
                            finally
                            {
                                if (!sqlError_followup)
                                {
                                    string followupContent = msg.GetMsg();
                                    followUps.Add(CaseNo, new StringBuilder(followupContent));
                                    OutputSocketMsg fsm = new OutputSocketMsg(new List<SocketAsyncEventArgs> { sockArg }, 1, IoServer.GenOutSocketMsg(sentFrom, "F0", followupContent));
                                    ioServer.AddOutSockMsg(fsm);
                                }
                            }
                            return;
                        }
                        string[] DBColumnNames = Config.getValues(Config.CaseFollowUpKeys);
                        while (re.Read())
                        {
                            for (int i = 0; i < DBColumnNames.Length; i++)
                            {
                                try
                                {
                                    msg.SetKeyValuePair(Config.CaseFollowUpKeys[i], re.GetString(re.GetOrdinal(DBColumnNames[i])).Trim(' '));
                                }
                                catch
                                {
                                    msg.SetKeyValuePair(Config.CaseFollowUpKeys[i], re.GetDateTime(re.GetOrdinal(DBColumnNames[i])).ToString("yyyy-MM-dd HH:mm:ss").Trim(' '));
                                }
                            }
                            if (followUps.ContainsKey(CaseNo))
                            {
                                followUps[CaseNo].Insert(0, Config.Msg_Separator3).Insert(0, msg.GetMsg());
                            }
                            else
                            {
                                followUps.Add(CaseNo, new StringBuilder(msg.GetMsg()));
                            }
                        }
                        OutputSocketMsg fsm1 = new OutputSocketMsg(new List<SocketAsyncEventArgs> { sockArg }, 1, IoServer.GenOutSocketMsg(sentFrom, "F0", followUps[CaseNo].ToString()));
                        ioServer.AddOutSockMsg(fsm1);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            sqlconnectionPool.releaseConnection(sqlConnection);
        }

        public static string GenSelectSql(string DBName, string[] selectKeys, string[] conditionKeys, string[] conditionValues)
        {
            try
            {
                StringBuilder sb = new StringBuilder("select");
                if (selectKeys == null)
                {
                    sb.Append(" * ");
                }
                else
                {
                    string[] selectNames = Config.getValues(selectKeys);
                    sb.Append(selectNames[0]);
                    for (int i = 1; i < selectNames.Length; i++)
                    {
                        sb.Append(",").Append(selectNames[i]);
                    }
                }
                sb.Append(" from ").Append(DBName);
                if (conditionKeys != null && conditionValues != null && conditionKeys.Length != 0)
                {
                    sb.Append(" where ");
                    string[] conditionNames = Config.getValues(conditionKeys);
                    sb.Append(conditionNames[0]).Append(" = ").Append("'").Append(conditionValues[0]).Append("'");
                    for (int i = 1; i < conditionValues.Length; i++)
                    {
                        sb.Append(" and ").Append(conditionNames[i]).Append(" = ").Append("'").Append(conditionValues[i]).Append("'");
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return "";
            }
        }

        private static string GenAddSql(string DBName, string[] keys, Message msg)
        {
            try
            {
                string[] columns = msg.GetValuesFromHT(keys);
                StringBuilder sb = new StringBuilder("insert into ");
                sb.Append(DBName).Append(" Values ('");
                for (int i = 0; i < columns.Length - 1; i++)
                {
                    sb.Append(columns[i]).Append("','");
                }
                sb.Append(columns[columns.Length - 1]).Append("')");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return "";
            }
        }

        private static string GenEditSql(string DBName, string[] DB_keys, string[] PK_keys, string[] Override_PKKeys, Message msg)
        {
            try
            {
                string[] contents = msg.GetValuesFromHT(DB_keys);
                string[] DBColumnNames = Config.getValues(DB_keys);
                StringBuilder sb = new StringBuilder("update ");
                sb.Append(DBName).Append(" set ");
                for (int i = 0; i < DBColumnNames.Length - 1; i++)
                {
                    sb.Append(DBColumnNames[i]).Append("='").Append(contents[i]).Append("',");
                }
                sb.Append(DBColumnNames[DBColumnNames.Length - 1]).Append("='").Append(contents[DBColumnNames.Length - 1]).Append("' where ");

                string[] PKColumnNames = Config.getValues(PK_keys);
                if (Override_PKKeys == null)
                {
                    string[] PK_contents = msg.GetValuesFromHT(PK_keys);
                    for (int i = 0; i < PKColumnNames.Length - 1; i++)
                    {
                        sb.Append(PKColumnNames[i]).Append("='").Append(PK_contents[i]).Append("' and ");
                    }
                    sb.Append(PKColumnNames[PKColumnNames.Length - 1]).Append("='").Append(PK_contents[PKColumnNames.Length - 1]).Append("'");
                }
                else
                {
                    string[] PK_contents = msg.GetValuesFromHT(Override_PKKeys);
                    for (int i = 0; i < PKColumnNames.Length - 1; i++)
                    {
                        sb.Append(PKColumnNames[i]).Append("='").Append(PK_contents[i]).Append("' and ");
                    }
                    sb.Append(PKColumnNames[PKColumnNames.Length - 1]).Append("='").Append(PK_contents[PKColumnNames.Length - 1]).Append("'");
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return "";
            }
        }

        private static string GenDeleteSql(string DBName, string[] PK_keys, Message msg)
        {
            try
            {
                StringBuilder sb = new StringBuilder("delete from ");
                string[] contents = msg.GetValuesFromHT(PK_keys);
                string[] dbColumnNames = Config.getValues(PK_keys);
                sb.Append(DBName).Append(" where ");
                for (int i = 0; i < dbColumnNames.Length - 1; i++)
                {
                    sb.Append(dbColumnNames[i]).Append("='").Append(contents[i]).Append("' and ");
                }
                sb.Append(dbColumnNames[dbColumnNames.Length - 1]).Append("='").Append(contents[dbColumnNames.Length - 1]).Append("'");
                return sb.ToString();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return "";
            }
        }

        private string GetAllLoginInfo()
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                List<string> users = loginUserAndCases.Keys.ToList();
                if (users.Count > 0)
                {
                    sb.Append(GetUserInputInfo(users[0]));
                    for (int i = 1; i < users.Count; i++)
                    {
                        sb.Append(Config.Msg_Separator3).Append(GetUserInputInfo(users[i]));
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return "";
            }
        }

        private string GetUserInputInfo(string userName)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("80").Append(Config.Msg_Separator2).Append(userName).Append(Config.Msg_Separator2).Append("17").Append(Config.Msg_Separator2);
                List<string> clients = loginUserAndCases[userName].Values.ToList();
                if (clients.Count > 0)
                {
                    sb.Append(clients[0]);
                    for (int i = 1; i < clients.Count; i++)
                    {
                        sb.Append(",").Append(clients[i]);
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return "";
            }
        }

        private string GetFormatNotice(string s, string BeRepalced, string ToReplace)
        {
            try
            {
                if (Regex.IsMatch(s, BeRepalced))
                {
                    s = Regex.Replace(s, BeRepalced, ToReplace);
                }
                return s;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
                return s;
            }
        }

        private void GetErrorMsg(Exception ex, string sentFrom, string errorMsg, SocketAsyncEventArgs sockArg)
        {
            Message msg = new Message();
            msg.SetKeyValuePair("75", errorMsg);
            using (OutputSocketMsg sm = new OutputSocketMsg(new List<SocketAsyncEventArgs> { sockArg }, 1, IoServer.GenOutSocketMsg(sentFrom, "ERROR", msg.GetMsg())))
            {
                ioServer.AddOutSockMsg(sm);
                logger.Error(ex);
            }
        }

        public void Run(int port)
        {
            ioServer = new IoServer(30, 8096, new IoServer.Del_handleInMsg(HandleMsg));
            ioServer.Start(port);
        }

        private void HandleMsg(InputSocketMsg ISM)
        {
            try
            {
                string Msg = ISM.Message;
                logger.Info("UI>" + Msg);
                string[] info = Regex.Split(Msg, Config.Msg_Separator1);
                if (info.Length == 3)
                {
                    string sentFrom = info[0].Trim();
                    string MsgType = info[1].Trim();
                    string MsgContent = info[2].Trim();
                    switch (MsgType)
                    {
                        case "C1":   //add case
                            {
                                string createTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                Message msg = new Message(MsgContent);
                                string caseID = GenCaseID(sentFrom);
                                string inchange = msg.GetValueFromPairs("10");
                                string decription = msg.GetValueFromPairs("8");
                                msg.SetKeyValuePair("1", caseID);
                                msg.SetKeyValuePair("2", createTime);
                                if (msg.GetValueFromPairs("3").Equals(""))
                                {
                                    msg.SetKeyValuePair("3", createTime);
                                }
                                bool sqlError = false;
                                string cmd = GenAddSql(Config.DBName_SupportLogSheet, Config.SupportLogSheetKeys, msg);
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    StringBuilder errorMsg = new StringBuilder("Add case failed, Reason: DB operation ERROR.\r\nCase info>\r\nClient: ").Append(msg.GetValueFromPairs("4"));
                                    errorMsg.Append(", CallPerson: ").Append(msg.GetValueFromPairs("5")).Append(", DesCription: ").Append(decription);
                                    errorMsg.Append("\r\nTips: is this client in clientlist? Or you should add it to clientlist first, from top bar->superuser!");
                                    GetErrorMsg(ex, sentFrom, errorMsg.ToString(), ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(sentFrom, MsgType, msg.GetMsg()));
                                        ioServer.AddOutSockMsg(sm);
                                        outstandingCases.Add(caseID, new object[] { new object(), msg });
                                        // Initial the case hints of this case
                                        bool sqlError_hints = false;
                                        Message msg_caseHint = new Message();
                                        msg_caseHint.SetKeyValuePair("1", caseID);
                                        msg_caseHint.SetKeyValuePair("73", "");
                                        string hcmd = GenAddSql(Config.DBName_CaseHints, Config.CaseHintsKeys, msg_caseHint);
                                        try
                                        {
                                            using (SqlCommand hsqlcmd = new SqlCommand(hcmd, sqlConnection))
                                            {
                                                hsqlcmd.ExecuteNonQuery();
                                            }
                                        }
                                        catch (SqlException ex)
                                        {
                                            sqlError_hints = true;
                                            logger.Error(ex);
                                        }
                                        finally
                                        {
                                            if (!sqlError_hints)
                                            {
                                                caseHints.Add(caseID, new StringBuilder(msg_caseHint.GetMsg()));
                                            }
                                        }

                                        //add the first follow up of this case
                                        bool sqlError_followup = false;
                                        StringBuilder temp = new StringBuilder(IDNameDic[sentFrom]);
                                        temp.Append(" build this case.");
                                        if (IDNameDic[sentFrom] != inchange && inchange != "Unassigned")
                                        {
                                            temp.Append(" And assigned to ").Append(inchange).Append(".");
                                        }
                                        Message msg_followUp = new Message();
                                        msg_followUp.SetKeyValuePair("1", caseID);
                                        msg_followUp.SetKeyValuePair("50", createTime);
                                        msg_followUp.SetKeyValuePair("52", temp.ToString());
                                        string fcmd = GenAddSql(Config.DBName_CaseFollowUp, Config.CaseFollowUpKeys, msg_followUp);

                                        try
                                        {
                                            using (SqlCommand fsqlcmd = new SqlCommand(fcmd, sqlConnection))
                                            {
                                                fsqlcmd.ExecuteNonQuery();
                                            }
                                        }
                                        catch (SqlException ex)
                                        {
                                            sqlError_followup = true;
                                            logger.Error(ex);
                                        }
                                        finally
                                        {
                                            if (!sqlError_followup)
                                            {
                                                followUps.Add(caseID, new StringBuilder(msg_followUp.GetMsg()));
                                            }
                                        }
                                        string name = IDNameDic[sentFrom];
                                        string inputFrom = msg.GetValueFromPairs("17");
                                        if (loginUserAndCases.ContainsKey(name))
                                        {
                                            if (loginUserAndCases[name].ContainsKey(inputFrom))
                                            {
                                                loginUserAndCases[name].Remove(inputFrom);
                                                OutputSocketMsg usm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(sentFrom, "R2", GetUserInputInfo(name)));
                                                ioServer.AddOutSockMsg(usm);
                                            }
                                        }
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "C2": //edit case
                            {
                                Message msg = new Message(MsgContent);
                                string caseID = msg.GetValueFromPairs("1");
                                if (!outstandingCases.ContainsKey(caseID))
                                {
                                    AddOutstandingCase(caseID);
                                }
                                string issueType = msg.GetValueFromPairs("14");
                                string status = msg.GetValueFromPairs("11");
                                string cmd = GenEditSql(Config.DBName_SupportLogSheet, Config.SupportLogSheetKeys, Config.PK_SupportLogSheetKeys, null, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    StringBuilder errorMsg = new StringBuilder("Edit case failed, Reason: DB operation ERROR.\r\nCase info:\r\nCaseID: ").Append(caseID);
                                    errorMsg.Append(", Client: ").Append(msg.GetValueFromPairs("4")).Append(", DesCription: ").Append(msg.GetValueFromPairs("8"));
                                    GetErrorMsg(ex, sentFrom, errorMsg.ToString(), ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(sentFrom, MsgType, MsgContent));
                                        ioServer.AddOutSockMsg(sm);
                                        string changes = CompareCase(ref msg);
                                        if (!changes.Equals(""))
                                        {
                                            bool followUpSqlError = false;
                                            Message msg_followUp = new Message();
                                            msg_followUp.SetKeyValuePair("1", caseID);
                                            msg_followUp.SetKeyValuePair("50", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                            msg_followUp.SetKeyValuePair("52", IDNameDic[sentFrom] + ": " + changes);
                                            string fcmd = GenAddSql(Config.DBName_CaseFollowUp, Config.CaseFollowUpKeys, msg_followUp);
                                            try
                                            {
                                                using (SqlCommand fsqlcmd = new SqlCommand(fcmd, sqlConnection))
                                                {
                                                    fsqlcmd.ExecuteNonQuery();
                                                }
                                            }
                                            catch (SqlException ex)
                                            {
                                                followUpSqlError = true;
                                                logger.Error(ex);
                                            }
                                            finally
                                            {
                                                if (!followUpSqlError)
                                                {
                                                    if (followUps.ContainsKey(caseID))
                                                    {
                                                        followUps[caseID].Append(Config.Msg_Separator3).Append(msg_followUp.GetMsg());
                                                        OutputSocketMsg fsm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(sentFrom, "F1", followUps[caseID].ToString()));
                                                        ioServer.AddOutSockMsg(fsm);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "C3": //delete case
                            {
                                Message msg = new Message(MsgContent);
                                string caseID = msg.GetValueFromPairs("1");
                                string cmd = GenDeleteSql(Config.DBName_SupportLogSheet, Config.PK_SupportLogSheetKeys, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = new StringBuilder("Delete case failed, Reason: DB operation ERROR.\r\nCaseID: ").Append(caseID).ToString();
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(sentFrom, MsgType, MsgContent));
                                        ioServer.AddOutSockMsg(sm);
                                        if (followUps.ContainsKey(caseID))
                                        {
                                            followUps.Remove(caseID);
                                        }
                                        if (caseHints.ContainsKey(caseID))
                                        {
                                            caseHints.Remove(caseID);
                                        }
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "H1":    // modify case hints
                            {
                                string inputPerson = IDNameDic[sentFrom];
                                Message msg = new Message(MsgContent);
                                string hContent = msg.GetValueFromPairs("73");
                                if (!hContent.Equals(""))
                                {
                                    hContent = new StringBuilder(hContent).Append("\r\n-----").Append(inputPerson).Append(" , ").Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")).ToString();
                                }
                                msg.SetKeyValuePair("73", hContent);
                                string cmd = GenEditSql(Config.DBName_CaseHints, Config.CaseHintsKeys, Config.PK_CaseHintsKeys, null, msg);
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                bool sqlError = false;
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    StringBuilder errorMsg = new StringBuilder("Modify case hints failed, Reason: DB operation ERROR.\r\ncase hints info:\r\nCaseID: ").Append(msg.GetValueFromPairs("1"));
                                    errorMsg.Append(", hints: ").Append(msg.GetValueFromPairs("73"));
                                    GetErrorMsg(ex, sentFrom, errorMsg.ToString(), ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg fsm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(sentFrom, MsgType, msg.GetMsg()));
                                        ioServer.AddOutSockMsg(fsm);
                                        caseHints[msg.GetValueFromPairs("1")].Clear().Append(msg.GetMsg());
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "F0":// load follow up and case hints of a case
                            {
                                // follow up
                                Message msg = new Message(MsgContent);
                                string caseID = msg.GetValueFromPairs("1");
                                if (!caseID.Equals(""))
                                {
                                    if (followUps.ContainsKey(caseID))
                                    {
                                        OutputSocketMsg fsm = new OutputSocketMsg(new List<SocketAsyncEventArgs> { ISM.SockArg }, 1, IoServer.GenOutSocketMsg(sentFrom, "F0", followUps[caseID].ToString()));
                                        ioServer.AddOutSockMsg(fsm);
                                    }
                                    else
                                    {
                                        FindFollowUp(caseID, sentFrom, ISM.SockArg);
                                    }
                                    //case hints
                                    if (caseHints.ContainsKey(caseID))
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(new List<SocketAsyncEventArgs> { ISM.SockArg }, 1, IoServer.GenOutSocketMsg(sentFrom, "H0", caseHints[caseID].ToString()));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                    else
                                    {
                                        FindCaseHints(caseID, sentFrom, ISM.SockArg);
                                    }
                                }
                                break;
                            }
                        case "F1":  // new a  follow up
                            {
                                Message msg = new Message(MsgContent);
                                msg.SetKeyValuePair("50", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                msg.SetKeyValuePair("52", new StringBuilder(IDNameDic[sentFrom]).Append(": ").Append(msg.GetValueFromPairs("52")).ToString());
                                string[] SQLColumns = Regex.Split(MsgContent, Config.Msg_Separator2);
                                string fcmd = GenAddSql(Config.DBName_CaseFollowUp, Config.CaseFollowUpKeys, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand fsqlcmd = new SqlCommand(fcmd, sqlConnection))
                                    {
                                        fsqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    StringBuilder errorMsg = new StringBuilder("New case follow up failed, Reason: DB operation ERROR.\r\nFollow up info:\r\nCaseID: ").Append(msg.GetValueFromPairs("1"));
                                    errorMsg.Append(", follow up content: ").Append(msg.GetValueFromPairs("52"));
                                    GetErrorMsg(ex, sentFrom, errorMsg.ToString(), ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        string caseID = msg.GetValueFromPairs("1");
                                        if (followUps.ContainsKey(caseID))
                                        {
                                            followUps[caseID].Append(Config.Msg_Separator3).Append(msg.GetMsg());
                                            OutputSocketMsg fsm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(sentFrom, MsgType, followUps[caseID].ToString()));
                                            ioServer.AddOutSockMsg(fsm);
                                        }
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "F2":  // Edit a follow up
                            {
                                Message msg = new Message(MsgContent);
                                string fcmd = GenEditSql(Config.DBName_CaseFollowUp, new string[] { "52" }, Config.PK_CaseFollowUpKeys, null, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand fsqlcmd = new SqlCommand(fcmd, sqlConnection))
                                    {
                                        fsqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    StringBuilder errorMsg = new StringBuilder("Edit case follow up failed, Reason: DB operation ERROR.\r\nFollow up info:\r\nCaseID: ");
                                    errorMsg.Append(msg.GetValueFromPairs("1")).Append(", follow up content: ").Append(msg.GetValueFromPairs("52"));
                                    GetErrorMsg(ex, sentFrom, errorMsg.ToString(), ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        string caseID = msg.GetValueFromPairs("1");
                                        if (followUps.ContainsKey(caseID))
                                        {
                                            string[] followUp_temp = Regex.Split(followUps[caseID].ToString(), Config.Msg_Separator3);
                                            followUps[caseID].Clear();
                                            for (int i = 0; i < followUp_temp.Length - 1; i++)
                                            {
                                                if (Regex.IsMatch(followUp_temp[i], msg.GetValueFromPairs("50")))
                                                {
                                                    followUps[caseID].Append(msg.GetMsg()).Append(Config.Msg_Separator3);
                                                }
                                                else
                                                {
                                                    followUps[caseID].Append(followUp_temp[i]).Append(Config.Msg_Separator3);
                                                }
                                            }

                                            if (Regex.IsMatch(followUp_temp.Last(), msg.GetValueFromPairs("50")))
                                            {
                                                followUps[caseID].Append(msg.GetMsg());
                                            }
                                            else
                                            {
                                                followUps[caseID].Append(followUp_temp.Last());
                                            }

                                            OutputSocketMsg fsm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(sentFrom, MsgType, followUps[caseID].ToString()));
                                            ioServer.AddOutSockMsg(fsm);
                                        }
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "F3":  // remove a follow up
                            {
                                Message msg = new Message(MsgContent);
                                string fcmd = GenDeleteSql(Config.DBName_CaseFollowUp, Config.PK_CaseFollowUpKeys, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand fsqlcmd = new SqlCommand(fcmd, sqlConnection))
                                    {
                                        fsqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    StringBuilder errorMsg = new StringBuilder("Delete case follow up failed, Reason: DB operation ERROR.\r\nFollow up info:\r\nCaseID: ");
                                    errorMsg.Append(msg.GetValueFromPairs("1")).Append(", follow up Created time: ").Append(msg.GetValueFromPairs("50"));
                                    GetErrorMsg(ex, sentFrom, errorMsg.ToString(), ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        string caseID = msg.GetValueFromPairs("1");
                                        if (followUps.ContainsKey(caseID))
                                        {
                                            string[] followUp_temp = Regex.Split(followUps[caseID].ToString(), Config.Msg_Separator3);
                                            followUps[caseID].Clear();
                                            if (!Regex.IsMatch(followUp_temp[0], msg.GetValueFromPairs("50")))
                                            {
                                                followUps[caseID].Append(followUp_temp[0]);
                                            }
                                            for (int i = 1; i < followUp_temp.Length; i++)
                                            {
                                                if (Regex.IsMatch(followUp_temp[i], msg.GetValueFromPairs("50")))
                                                {
                                                    continue;
                                                }
                                                else
                                                {
                                                    if (followUps[caseID].Length != 0)
                                                    {
                                                        followUps[caseID].Append(Config.Msg_Separator3).Append(followUp_temp[i]);
                                                    }
                                                    else
                                                    {
                                                        followUps[caseID].Append(followUp_temp[i]);
                                                    }
                                                }
                                            }

                                            OutputSocketMsg fsm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(sentFrom, MsgType, followUps[caseID].ToString()));
                                            ioServer.AddOutSockMsg(fsm);
                                        }
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }

                        case "P1":  // Add a contact
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenAddSql(Config.DBName_ClientContact, Config.ClientContactKeys, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand fsqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        fsqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    StringBuilder errorMsg = new StringBuilder("Add contact failed, Reason: DB operation ERROR.\r\nContact info:\r\nClient: ");
                                    errorMsg.Append(msg.GetValueFromPairs("4")).Append(", Contact name: ").Append(msg.GetValueFromPairs("111"));
                                    GetErrorMsg(ex, sentFrom, errorMsg.ToString(), ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }

                        case "P2":  // Edit a contact
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenEditSql(Config.DBName_ClientContact, Config.ClientContactKeys, Config.PK_ClientContactKeys, new string[] { "4", "117" }, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand fsqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        fsqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    StringBuilder errorMsg = new StringBuilder("Edit contact failed, Reason: DB operation ERROR.\r\nContact info:\r\nClient: ");
                                    errorMsg.Append(msg.GetValueFromPairs("4")).Append(", Ex Contact name: ").Append(msg.GetValueFromPairs("117"));
                                    GetErrorMsg(ex, sentFrom, errorMsg.ToString(), ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }

                        case "P3":  // Delete a contact
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenDeleteSql(Config.DBName_ClientContact, Config.PK_ClientContactKeys, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand fsqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        fsqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    StringBuilder errorMsg = new StringBuilder("Delete contact failed, Reason: DB operation ERROR.\r\nContact info:\r\nClient: ");
                                    errorMsg.Append(msg.GetValueFromPairs("4")).Append(", Contact name: ").Append(msg.GetValueFromPairs("111"));
                                    GetErrorMsg(ex, sentFrom, errorMsg.ToString(), ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }

                        case "B1":  // add a Client to ClientList
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenAddSql(Config.DBName_ClientList, Config.ClientListKeys, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand fsqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        fsqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = new StringBuilder("Add client failed, Reason: DB operation ERROR.\r\nClient info:\r\nClient: ").Append(msg.GetValueFromPairs("4")).ToString();
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }

                        case "B2":  // Edit a Client in ClientList
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenEditSql(Config.DBName_ClientList, Config.ClientListKeys, Config.PK_ClientListKeys, new string[] { "135" }, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = new StringBuilder("Edit client failed, Reason: DB operation ERROR.\r\nClient info:\r\nClient: ").Append(msg.GetValueFromPairs("4")).ToString();
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        try
                                        {
                                            string ucmd = new StringBuilder("update SupportLogSheet set Client = '").Append(msg.GetValueFromPairs("4")).Append("' where Client ='").Append(msg.GetValueFromPairs("135")).Append("'").ToString();
                                            string ucmd2 = new StringBuilder("update ClientContact set Client = '").Append(msg.GetValueFromPairs("4")).Append("' where Client ='").Append(msg.GetValueFromPairs("135")).Append("'").ToString();
                                            using (SqlCommand sqlcmd1 = new SqlCommand(ucmd, sqlConnection))
                                            {
                                                sqlcmd1.ExecuteNonQuery();
                                            }
                                            using (SqlCommand sqlcmd2 = new SqlCommand(ucmd2, sqlConnection))
                                            {
                                                sqlcmd2.ExecuteNonQuery();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error(ex);
                                        }
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }

                        case "B3":  // delete a Client in ClientList
                            {
                                Message msg = new Message(MsgContent);
                                string dcmd = GenDeleteSql(Config.DBName_ClientList, Config.PK_ClientListKeys, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand fsqlcmd = new SqlCommand(dcmd, sqlConnection))
                                    {
                                        fsqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = new StringBuilder("Delete client failed, Reason: DB operation ERROR.\r\nClient: ").Append(msg.GetValueFromPairs("4")).ToString();
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }

                        case "U1":  // edit a user in UserFile, By superuser 
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenEditSql(Config.DBName_UserFile, Config.UserFile_SuperUser, Config.PK_UserFileKeys, null, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = new StringBuilder("Edit user failed, Reason: DB operation ERROR.\r\nUser info:\r\nUser name: ").Append(msg.GetValueFromPairs("80")).ToString();
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        string name = msg.GetValueFromPairs("80");
                                        string id = msg.GetValueFromPairs("87");
                                        foreach (string key in IDNameDic.Keys)
                                        {
                                            if (IDNameDic[key].Equals(name))
                                            {
                                                IDNameDic.Remove(key);
                                                IDNameDic.Add(id, name);
                                                break;
                                            }
                                        }
                                        IDNameDic[name] = id;
                                        SocketInfo s = ISM.SockArg.UserToken as SocketInfo;
                                        s.userName = name;
                                        s.userID = id;
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }

                        case "U2":  // edit a user's info, by user himself
                            {
                                Message msg = new Message(MsgContent);
                                string exName = msg.GetValueFromPairs("89"), newName = msg.GetValueFromPairs("80");
                                if (loginUserAndCases.ContainsKey(newName) && !exName.Equals(newName))
                                {
                                    string errorMsg = newName + " is already been used by other users";
                                    GetErrorMsg(null, sentFrom, errorMsg, ISM.SockArg);
                                    break;
                                }
                                string cmd = GenEditSql(Config.DBName_UserFile, Config.UserFile_Self, Config.PK_UserFileKeys, new string[] { "89" }, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = "Change user info failed, Reason: DB operation ERROR.\r\n";
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                        if (loginUserAndCases.ContainsKey(exName))
                                        {
                                            Dictionary<string, string> temp = loginUserAndCases[exName];
                                            loginUserAndCases.Remove(exName);
                                            loginUserAndCases.Add(newName, temp);
                                        }
                                        if (IDNameDic.ContainsKey(sentFrom))
                                        {
                                            IDNameDic[sentFrom] = newName;
                                        }

                                        string ucmd = new StringBuilder("update ").Append(Config.DBName_SupportLogSheet).Append(" set Incharge = '").Append(newName).Append("' where Incharge = '").Append(exName).Append("'").ToString();
                                        recoveryWriter.writeRecovery(ucmd);
                                        try
                                        {
                                            using (SqlCommand sqlcmd = new SqlCommand(ucmd, sqlConnection))
                                            {
                                                sqlcmd.ExecuteNonQuery();
                                            }
                                        }
                                        catch (SqlException ex)
                                        {
                                            logger.Error(ex);
                                        }
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }

                        case "U3":  // delete a user from UserFile
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenDeleteSql(Config.DBName_UserFile, Config.PK_UserFileKeys, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand fsqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        fsqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = new StringBuilder("Delete user failed, Reason: DB operation ERROR.\r\nUser info:\r\nUser name: ").Append(msg.GetValueFromPairs("80")).ToString();
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }

                        case "U4":  // user change password, by user himself
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenEditSql(Config.DBName_UserFile, new string[] { "80", "82", "83", "84" }, Config.PK_UserFileKeys, null, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = "Change password failed, Reason: DB operation ERROR.\r\n";
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(new List<SocketAsyncEventArgs> { ISM.SockArg }, 1, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }

                        case "S": // Change Case Property sections
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenDeleteSql(Config.DBName_CaseProperty, new string[] { "100" }, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = "Change case property sections failed, Reason: DB operation ERROR.\r\n";
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        string[] properties = Regex.Split(msg.GetValueFromPairs("101"), ",");
                                        for (int i = 0; i < properties.Length; i++)
                                        {
                                            msg.SetKeyValuePair("101", properties[i]);
                                            string cmd1 = GenAddSql(Config.DBName_CaseProperty, Config.CasePropertyKeys, msg);
                                            try
                                            {
                                                using (SqlCommand sqlcmd = new SqlCommand(cmd1, sqlConnection))
                                                {
                                                    sqlcmd.ExecuteNonQuery();
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                logger.Error(ex);
                                            }
                                        }
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "L1":  // user login
                            {
                                Message msg = new Message(MsgContent);
                                string userName = msg.GetValueFromPairs("80");
                                if (loginUserAndCases.ContainsKey(userName))
                                {
                                    Message ERRmsg = new Message();
                                    ERRmsg.SetKeyValuePair("75", "Another Login kicks out this login Session.\r\n");
                                    ERRmsg.SetKeyValuePair("76", "true");
                                    loginUserAndCases.Remove(userName);
                                    OutputSocketMsg esm = new OutputSocketMsg(new List<SocketAsyncEventArgs> { ISM.SockArg }, 1, IoServer.GenOutSocketMsg(sentFrom, "Kicked", ERRmsg.GetMsg()));
                                    esm.ForceDisconnect = true;
                                    ioServer.AddOutSockMsg(esm);
                                }
                                if (msg.GetValueFromPairs("60").Equals(latestUIVersion))
                                {
                                    SocketInfo s = ISM.SockArg.UserToken as SocketInfo;
                                    s.userID = sentFrom;
                                    s.userName = userName;
                                    Message msg_Notice = new Message();
                                    msg_Notice.SetKeyValuePair("74", notice);
                                    OutputSocketMsg nsm = new OutputSocketMsg(new List<SocketAsyncEventArgs> { ISM.SockArg }, 1, IoServer.GenOutSocketMsg(sentFrom, "N0", msg_Notice.GetMsg()));
                                    ioServer.AddOutSockMsg(nsm);
                                    loginUserAndCases.Add(userName, new Dictionary<string, string>());
                                    msg.RemovePair("60");
                                    if (!IDNameDic.ContainsKey(sentFrom))
                                    {
                                        IDNameDic.Add(sentFrom, userName);
                                    }
                                    OutputSocketMsg usm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(sentFrom, MsgType, GetAllLoginInfo()));
                                    ioServer.AddOutSockMsg(usm);
                                }
                                else
                                {
                                    string location = msg.GetValueFromPairs("91");
                                    if (Config.LocationNamePath_pair.ContainsKey(location))
                                    {
                                        msg.SetKeyValuePair("61", configFile.Read("UIUpgrade", Config.LocationNamePath_pair[location]));
                                    }
                                    else
                                    {
                                        msg.SetKeyValuePair("61", configFile.Read("UIUpgrade", Config.LocationNamePath_pair["HK"]));
                                    }
                                    OutputSocketMsg nsm = new OutputSocketMsg(new List<SocketAsyncEventArgs> { ISM.SockArg }, 1, IoServer.GenOutSocketMsg(sentFrom, "N1", msg.GetMsg()));
                                    ioServer.AddOutSockMsg(nsm);
                                }
                                break;
                            }
                        case "L0": // user logout
                            {
                                Message msg = new Message(MsgContent);
                                string userName = msg.GetValueFromPairs("80");
                                if (loginUserAndCases.ContainsKey(userName))
                                {
                                    loginUserAndCases.Remove(userName);
                                }
                                OutputSocketMsg usm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(sentFrom, MsgType, msg.GetMsg()));
                                ioServer.AddOutSockMsg(usm);
                                break;
                            }
                        case "R1": // User update msg
                            {
                                Message msg = new Message(MsgContent);
                                string inputFrom = msg.GetValueFromPairs("17");
                                string broker = msg.GetValueFromPairs("1");
                                string name = msg.GetValueFromPairs("80");
                                if (loginUserAndCases.ContainsKey(name))
                                {
                                    if (loginUserAndCases[name].ContainsKey(inputFrom))
                                    {
                                        if (broker.Equals(""))
                                        {
                                            loginUserAndCases[name].Remove(inputFrom);
                                        }
                                        else
                                        {
                                            loginUserAndCases[name][inputFrom] = broker;
                                        }
                                    }
                                    else
                                    {
                                        loginUserAndCases[name].Add(inputFrom, broker);
                                    }
                                }
                                else
                                {
                                    Dictionary<string, string> temp = new Dictionary<string, string>();
                                    temp.Add(inputFrom, broker);
                                    loginUserAndCases.Add(name, temp);
                                }
                                OutputSocketMsg usm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(sentFrom, MsgType, GetUserInputInfo(name)));
                                ioServer.AddOutSockMsg(usm);
                                break;
                            }
                        case "R2":  // when user clear a input client
                            {
                                Message msg = new Message(MsgContent);
                                string username = msg.GetValueFromPairs("80");
                                string inputFrom = msg.GetValueFromPairs("17");
                                if (loginUserAndCases.ContainsKey(username))
                                {
                                    if (loginUserAndCases[username].ContainsKey(inputFrom))
                                    {
                                        loginUserAndCases[username].Remove(inputFrom);
                                    }
                                    OutputSocketMsg usm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(sentFrom, MsgType, GetUserInputInfo(username)));
                                    ioServer.AddOutSockMsg(usm);
                                }
                                break;
                            }
                        case "N0":   // upgrade notice 
                            {
                                Message msg = new Message(MsgContent);
                                string content = msg.GetValueFromPairs("74");
                                string[] notices = Regex.Split(content, "\r\n");
                                StringBuilder noticeBuilder = new StringBuilder(content);
                                if (!Regex.IsMatch(notices[notices.Length - 1], "-----"))
                                {
                                    noticeBuilder.Append("\r\n-----").Append(msg.GetValueFromPairs("80")).Append(" , ").Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                }
                                msg.SetKeyValuePair("74", noticeBuilder.ToString());
                                try
                                {
                                    configFile.Write("NOTICE", "notice", GetFormatNotice(noticeBuilder.ToString(), "\r\n", "/r/n"));
                                }
                                catch (IOException ex)
                                {
                                    logger.Error(ex);
                                    string errorMsg = "Update Notice failed!";
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    this.notice = noticeBuilder.ToString();
                                    OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(sentFrom, MsgType, msg.GetMsg()));
                                    ioServer.AddOutSockMsg(sm);
                                }
                                break;
                            }
                        case "A1": // add AM
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenAddSql(Config.DBName_AccountManager, Config.AccountManagerKeys, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = "Add AM failed, Reason: DB operation ERROR.\r\n";
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "A2": // Edit AM
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenEditSql(Config.DBName_AccountManager, Config.AccountManagerKeys, Config.PK_AccountManagerKeys, new string[] { "153" }, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = "Edit AM failed, Reason: DB operation ERROR.\r\n";
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        string ucmd = new StringBuilder("update ClientList set AccountManager = '").Append(msg.GetValueFromPairs("150")).Append("' where AccountManager = '").Append(msg.GetValueFromPairs("153")).Append("'").ToString();
                                        recoveryWriter.writeRecovery(ucmd);
                                        try
                                        {
                                            using (SqlCommand sqlcmd1 = new SqlCommand(ucmd, sqlConnection))
                                            {
                                                sqlcmd1.ExecuteNonQuery();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error(ex);
                                        }
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "A3": // Delete AM
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenDeleteSql(Config.DBName_AccountManager, Config.PK_AccountManagerKeys, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = "Delete AM failed, Reason: DB operation ERROR.\r\n";
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        string ucmd = new StringBuilder("update ClientList set AccountManager = '' where AccountManager = '").Append(msg.GetValueFromPairs("150")).Append("'").ToString();
                                        recoveryWriter.writeRecovery(ucmd);
                                        try
                                        {
                                            using (SqlCommand sqlcmd1 = new SqlCommand(ucmd, sqlConnection))
                                            {
                                                sqlcmd1.ExecuteNonQuery();
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            logger.Error(ex);
                                        }
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "PM1": // add client server
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenAddSql(Config.DBName_ClientServer, Config.ClientServerKeys, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = "Add ClientServer failed, Reason: DB operation ERROR.\r\n";
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "PM2": // edit client server
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenEditSql(Config.DBName_ClientServer, Config.ClientServerKeys, Config.PK_ClientServerKeys, new string[] { "4", "173" }, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = "Edit ClientServer failed, Reason: DB operation ERROR.\r\n";
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "PM3": // delete client server
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenDeleteSql(Config.DBName_ClientServer, Config.PK_ClientServerKeys, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = "Delete ClientServer failed, Reason: DB operation ERROR.\r\n";
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "PM4": // add  a product of a client server
                            {
                                Message msg = new Message(MsgContent);
                                string client = msg.GetValueFromPairs("4"), server = msg.GetValueFromPairs("160"), product = msg.GetValueFromPairs("167");
                                string cmdCheck = new StringBuilder("select top(1) * from ").Append(Config.DBName_ProductMaster).Append(" where Client = '").Append(client).Append("' and Server = '").Append(server).Append("' and Product = '").Append(product).Append("'").ToString();
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    SqlCommand checkCmd = new SqlCommand(cmdCheck, sqlConnection);
                                    using (SqlDataReader re = checkCmd.ExecuteReader())
                                    {
                                        if (re.HasRows)
                                        {
                                            re.Close();
                                            string cmd = GenEditSql(Config.DBName_ProductMaster, Config.ProductMasterKeys, Config.PK_ProductMasterKeys, null, msg);
                                            using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                            {
                                                sqlcmd.ExecuteNonQuery();
                                            }
                                            MsgType = "PM5";
                                        }
                                        else
                                        {
                                            re.Close();
                                            string cmd = GenAddSql(Config.DBName_ProductMaster, Config.ProductMasterKeys, msg);
                                            using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                            {
                                                sqlcmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    StringBuilder errorMsg = new StringBuilder("Deploy Product failed,client: ");
                                    errorMsg.Append(msg.GetValueFromPairs("4")).Append(",Server:").Append(msg.GetValueFromPairs("160")).Append(",Product:").Append(msg.GetValueFromPairs("167")).Append(".Reason: DB operation ERROR.\r\n");
                                    GetErrorMsg(ex, sentFrom, errorMsg.ToString(), ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(sentFrom, MsgType, MsgContent));
                                        ioServer.AddOutSockMsg(sm);

                                        msg.SetKeyValuePair("170", DateTime.Now.ToString("yyyy-MM-dd"));
                                        msg.SetKeyValuePair("171", IDNameDic[sentFrom]);
                                        string ucmd = GenAddSql(Config.DBName_ProductMaster_Update, Config.ProductMaster_UpdateKeys, msg);
                                        try
                                        {
                                            using (SqlCommand sqlcmd = new SqlCommand(ucmd, sqlConnection))
                                            {
                                                sqlcmd.ExecuteNonQuery();
                                            }
                                        }
                                        catch (SqlException ex)
                                        {
                                            logger.Error(ex);
                                        }
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "PM5": // edit a product of a client server
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenEditSql(Config.DBName_ProductMaster, Config.ProductMasterKeys, Config.PK_ProductMasterKeys, null, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    StringBuilder errorMsg = new StringBuilder("Edit Product failed,client: ");
                                    errorMsg.Append(msg.GetValueFromPairs("4")).Append(",Server:").Append(msg.GetValueFromPairs("160")).Append(",Product:").Append(msg.GetValueFromPairs("167")).Append(".Reason: DB operation ERROR.\r\n");
                                    GetErrorMsg(ex, sentFrom, errorMsg.ToString(), ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                        string ucmd = GenEditSql(Config.DBName_ProductMaster_Update, new string[] { "169" }, Config.PK_ProductMaster_UpdateKeys, new string[] { "4", "160", "167", "174" }, msg);
                                        try
                                        {
                                            using (SqlCommand sqlcmd = new SqlCommand(ucmd, sqlConnection))
                                            {
                                                sqlcmd.ExecuteNonQuery();
                                            }
                                        }
                                        catch (SqlException ex)
                                        {
                                            logger.Error(ex);
                                        }
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "PM6": //delete a product of a client server
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenDeleteSql(Config.DBName_ProductMaster, Config.PK_ProductMasterKeys, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    StringBuilder errorMsg = new StringBuilder("Delete Product failed,client: ");
                                    errorMsg.Append(msg.GetValueFromPairs("4")).Append(",Server:").Append(msg.GetValueFromPairs("160")).Append(",Product:").Append(msg.GetValueFromPairs("167")).Append(".Reason: DB operation ERROR.\r\n");
                                    GetErrorMsg(ex, sentFrom, errorMsg.ToString(), ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "PM7": // edit product update of a client server
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenEditSql(Config.DBName_ProductMaster_Update, Config.ProductMaster_UpdateKeys, Config.PK_ProductMaster_UpdateKeys, null, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    StringBuilder errorMsg = new StringBuilder("Edit Product Update failed,client: ");
                                    errorMsg.Append(msg.GetValueFromPairs("4")).Append(",Server:").Append(msg.GetValueFromPairs("160")).Append(",Product:").Append(msg.GetValueFromPairs("167")).Append(",Version:").Append(msg.GetValueFromPairs("169")).Append(".Reason: DB operation ERROR.\r\n");
                                    GetErrorMsg(ex, sentFrom, errorMsg.ToString(), ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "PM8": // delete product update of a client server
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenDeleteSql(Config.DBName_ProductMaster_Update, Config.PK_ProductMaster_UpdateKeys, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    StringBuilder errorMsg = new StringBuilder("Delete Product Update failed,client: ");
                                    errorMsg.Append(msg.GetValueFromPairs("4")).Append(",Server:").Append(msg.GetValueFromPairs("160")).Append(",Product:").Append(msg.GetValueFromPairs("167")).Append(",Version:").Append(msg.GetValueFromPairs("169")).Append(".Reason: DB operation ERROR.\r\n");
                                    GetErrorMsg(ex, sentFrom, errorMsg.ToString(), ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "PM9":   // add a product type
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenAddSql(Config.DBName_Product, Config.ProductKeys, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = new StringBuilder("Add Product failed,product name:").Append(msg.GetValueFromPairs("167")).Append(". Reason: DB operation ERROR.\r\n").ToString();
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "PM10": // edit a product type
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenEditSql(Config.DBName_Product, Config.ProductKeys, Config.PK_ProductKeys, new string[] { "175", "176" }, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = new StringBuilder("Edit Product failed,product name:").Append(msg.GetValueFromPairs("167")).Append(". Reason: DB operation ERROR.\r\n").ToString();
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "PM11": // delete a product type
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenDeleteSql(Config.DBName_Product, Config.PK_ProductKeys, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = new StringBuilder("Delete Product failed,product name:").Append(msg.GetValueFromPairs("167")).Append(". Reason: DB operation ERROR.\r\n").ToString();
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "PH1": // add a product hint
                            {
                                Message msg = new Message(MsgContent);
                                msg.SetKeyValuePair("200", GenProductHintID(sentFrom));
                                string cmd = GenAddSql(Config.DBName_ProductHints, Config.ProductHintsKeys, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = new StringBuilder("Add ProductHints failed,product name:").Append(msg.GetValueFromPairs("167")).Append(". Reason: DB operation ERROR.\r\n").ToString();
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(sentFrom, "PH1", msg.GetMsg()));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "PH2": // edit a product hint
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenEditSql(Config.DBName_ProductHints, Config.ProductHintsKeys, Config.PK_ProductHintsKeys, null, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = "Edit ProductHints failed, Reason: DB operation ERROR.\r\n";
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        case "PH3": // delete a product hint
                            {
                                Message msg = new Message(MsgContent);
                                string cmd = GenDeleteSql(Config.DBName_ProductHints, Config.PK_ProductHintsKeys, msg);
                                bool sqlError = false;
                                SqlConnection sqlConnection = sqlconnectionPool.GetConnction();
                                try
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                                    {
                                        sqlcmd.ExecuteNonQuery();
                                    }
                                }
                                catch (SqlException ex)
                                {
                                    sqlError = true;
                                    logger.Error(ex);
                                    string errorMsg = "Delete ProductHints failed, Reason: DB operation ERROR.\r\n";
                                    GetErrorMsg(ex, sentFrom, errorMsg, ISM.SockArg);
                                }
                                finally
                                {
                                    if (!sqlError)
                                    {
                                        OutputSocketMsg sm = new OutputSocketMsg(null, 0, IoServer.GenOutSocketMsg(Msg));
                                        ioServer.AddOutSockMsg(sm);
                                    }
                                }
                                sqlconnectionPool.releaseConnection(sqlConnection);
                                break;
                            }
                        default:
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }

}

