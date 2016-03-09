// sql 语句操作类
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace SupportLogSheet
{
    public class SQL
    {
        //DB Info
        private static string DB_Source = Config.Common_INI.readValue("DATABASE", "ip");
        private static string DB_UserID = Config.Common_INI.readValue("DATABASE", "user");
        private static string DB_Pwd = Config.Common_INI.readValue("DATABASE", "password");
        private static string DB_InitialCatalog = Config.Common_INI.readValue("DATABASE", "db");
        private static int DB_ConnectTimeout = 30;

        public static SqlConnection dbConnect()
        {
            try
            {
                StringBuilder sb = new StringBuilder("Data Source=");
                sb.Append(DB_Source).Append(";user id =").Append(DB_UserID).Append(";password=").Append(DB_Pwd);
                sb.Append(";Initial Catalog=").Append(DB_InitialCatalog).Append(";Connect Timeout=").Append(DB_ConnectTimeout);
                SqlConnection mySqlConnection = new SqlConnection(sb.ToString());
                mySqlConnection.Open();
                return mySqlConnection;
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
                return null;
            }
        }

        public static string sqlCmd_Cases(string[] cases)
        {
            StringBuilder sb = new StringBuilder("select * from SupportLogSheet where CaseID in ('");
            for (int i = 0; i < cases.Length - 1; i++)
            {
                sb.Append(cases[i]).Append("','");
            }
            sb.Append(cases[cases.Length - 1]).Append("')");
            return sb.ToString();
        }

        public static string selectSqlCmd(string DBName, string[] selectKeys, string[] conditionKeys, string[] conditionValues)
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

        public static string addSqlCmd(string DBName, string[] keys, message msg)
        {
            string[] columns = msg.getValuesFromKeys(keys);
            StringBuilder sb = new StringBuilder("insert into ");
            sb.Append(DBName).Append(" Values ('");
            for (int i = 0; i < columns.Length - 1; i++)
            {
                sb.Append(columns[i]).Append("','");
            }
            sb.Append(columns[columns.Length - 1]).Append("')");
            return sb.ToString();
        }

        public static string getVersionFilterSQL(string fromVersion, string toVersion, bool equal)
        {
            StringBuilder sb = new StringBuilder(" and (not (dbo.f_IP2Int(FromVersion)");
            if (equal)
            {
                sb.Append(">  dbo.f_IP2Int('");
                sb.Append(toVersion);
                sb.Append("')) and not (dbo.f_IP2Int(ToVersion) < dbo.f_IP2Int('");
                sb.Append(fromVersion);
            }
            else
            {
                sb.Append(">=  dbo.f_IP2Int('");
                sb.Append(toVersion);
                sb.Append("')) and not (dbo.f_IP2Int(ToVersion) <= dbo.f_IP2Int('");
                sb.Append(fromVersion);
            }
            sb.Append("') and dbo.f_IP2Int(ToVersion)!=''))");
            return sb.ToString();
        }

        public static List<ListViewItem> genListLvi(string cmd, string[] keys, SqlConnection sqlConnection)
        {
            List<ListViewItem> items = new List<ListViewItem>();
            try
            {
                using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                {
                    using (SqlDataReader re = sqlcmd.ExecuteReader())
                    {
                        if (!re.HasRows)
                        {
                            return items;
                        }
                        string[] DBColumnNames = Config.getValues(keys);
                        message msg = new message();
                        while (re.Read())
                        {
                            for (int i = 0; i < DBColumnNames.Length; i++)
                            {
                                try
                                {
                                    msg.setKeyValuePair(keys[i], re.GetString(re.GetOrdinal(DBColumnNames[i])).Trim(' '));
                                }
                                catch
                                {
                                    msg.setKeyValuePair(keys[i], re.GetDateTime(re.GetOrdinal(DBColumnNames[i])).ToString("yyyy-MM-dd HH:mm:ss").Trim(' '));
                                }
                            }
                            items.Add(LV_OP.getLVI(msg, keys));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
            return items;
        }

        public static Dictionary<string, ListViewItem> genCaseDic(string cmd, string[] keys, SqlConnection sqlConnection)
        {
            Dictionary<string, ListViewItem> items = new Dictionary<string, ListViewItem>();
            try
            {
                using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                {
                    using (SqlDataReader re = sqlcmd.ExecuteReader())
                    {
                        if (!re.HasRows)
                        {
                            return items;
                        }
                        string[] DBColumnNames = Config.getValues(keys);
                        message msg = new message();
                        while (re.Read())
                        {
                            ListViewItem lvi = new ListViewItem();
                            for (int i = 0; i < DBColumnNames.Length; i++)
                            {  
                                try
                                {
                                    msg.setKeyValuePair(keys[i], re.GetString(re.GetOrdinal(DBColumnNames[i])).Trim(' '));
                                }
                                catch
                                {
                                    msg.setKeyValuePair(keys[i], re.GetDateTime(re.GetOrdinal(DBColumnNames[i])).ToString("yyyy-MM-dd HH:mm:ss").Trim(' '));
                                }
                            }
                            items.Add(msg.getValueFromPairs("1"), LV_OP.getLVI(msg, keys));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
            return items;
        }

        public static List<message> queryResults(string cmd, string[] keys, SqlConnection sqlConnection) {
            List<message> items = new List<message> ();
            try
            {
                using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                {
                    using (SqlDataReader re = sqlcmd.ExecuteReader())
                    {
                        if (!re.HasRows)
                        {
                            return items;
                        }
                        string[] DBColumnNames = Config.getValues(keys);
                        message msg = new message();
                        while (re.Read())
                        {
                            ListViewItem lvi = new ListViewItem();
                            for (int i = 0; i < DBColumnNames.Length; i++)
                            {
                                try
                                {
                                    msg.setKeyValuePair(keys[i], re.GetString(re.GetOrdinal(DBColumnNames[i])).Trim(' '));
                                }
                                catch
                                {
                                    msg.setKeyValuePair(keys[i], re.GetDateTime(re.GetOrdinal(DBColumnNames[i])).ToString("yyyy-MM-dd HH:mm:ss").Trim(' '));
                                }
                            }
                            items.Add(msg);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
            return items;
        }
    }
}
