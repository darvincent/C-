// 提醒操作类
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.IO;

namespace SupportLogSheet
{
    public class Reminder_OP
    {
        private XElement config;
        private string XMLPath;
        private Dictionary<string, System.Timers.Timer> RemindTimers = new Dictionary<string, System.Timers.Timer>();
        private delegate void Del_timer(object obj, System.Timers.ElapsedEventArgs e);
        private Dictionary<string, string> CaseRemindTime = new Dictionary<string, string>();
        private string init_NoExpiredCases = "";
        private string init_ExpiredCases = "";

        public Reminder_OP(string path)
        {
            this.XMLPath = path;
        }

        public void loadConfig()
        {
            if (!File.Exists(XMLPath))
            {
                createXML();
            }
            config = XElement.Load(@XMLPath);
            getReminders();
        }

        public void refresh()
        {
            RemindTimers.Clear();
            CaseRemindTime.Clear();
            init_NoExpiredCases = "";
            init_ExpiredCases = "";
            getReminders();
        }

        private void createXML()
        {
            FileStream Fs = new FileStream(XMLPath, FileMode.Create);
            StreamWriter Sw = new StreamWriter(Fs);
            Sw.Write(Config.ReminderXMLContent);
            Sw.Close();
            Fs.Close();
            MessageBox.Show("No Reminder config file: Reminder.XML!\r\nCreated a new one.");
        }

        public string getRemindTime(string caseID)
        {
            try
            {
                return CaseRemindTime[caseID];
            }
            catch
            {
                return "";
            }
        }

        public System.Timers.Timer initialTimer(int seconds)
        {
            if (seconds <= 0)
            {
                seconds = 10;
            }
            System.Timers.Timer timer = new System.Timers.Timer(1000 * seconds);
            try
            {
                timer.Elapsed += new System.Timers.ElapsedEventHandler(new Del_timer(timeUpAction));
                timer.AutoReset = false;
                timer.Enabled = true;
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
            return timer;
        }

        public bool addReminder(Msg_OP msg_Op)
        {
            try
            {
                string caseID = msg_Op.getValueFromPairs("1");
                string remindTime = msg_Op.getValueFromPairs("180");
                int seconds = utility.returnSeconds(DateTime.Now, Convert.ToDateTime(remindTime));
                IEnumerable<XElement> cases = from c in config.Elements("case")
                                              where c.Attribute("name").Value == caseID
                                              select c;
                if (cases.Count() > 0)
                {
                    cases.First().Value = remindTime;
                    RemindTimers[caseID] = initialTimer(seconds);
                    config.Save(XMLPath);
                }
                else
                {
                    RemindTimers.Add(caseID, initialTimer(seconds));
                    config.Add(new XElement("case",
                        new XAttribute("name", caseID),
                       remindTime));

                    config.Save(XMLPath);
                }

                if (CaseRemindTime.ContainsKey(caseID))
                {
                    CaseRemindTime[caseID] = remindTime;
                }
                else
                {
                    CaseRemindTime.Add(caseID, remindTime);
                }
                return true;
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
                return false;
            }
        }

        public void removeReminder(string caseID)
        {
            try
            {
                IEnumerable<XElement> cases = from c in config.Elements("case")
                                              where c.Attribute("name").Value == caseID
                                              select c;
                if (cases.Count() > 0)
                {
                    cases.First().Remove();
                }
                if (CaseRemindTime.ContainsKey(caseID))
                {
                    CaseRemindTime.Remove(caseID);
                }
                config.Save(XMLPath);
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        public void Th_removeReminder(object obj)
        {
            removeReminder(obj.ToString());
        }

        public void removeReminders(object obj)
        {
            string[] caseIDs = Regex.Split(obj.ToString(), ",");
            for (int i = 0; i < caseIDs.Length; i++)
            {
                removeReminder(caseIDs[i]);
            }       
        }

        private void getReminders()
        {
            IEnumerable<XElement> cases = from c in config.Elements("case")
                                          select c;
            foreach (var aCase in cases)
            {
                string caseID = aCase.Attribute("name").Value;
                string remindTime = aCase.Value;
                CaseRemindTime.Add(caseID, remindTime);
                int seconds = utility.returnSeconds(DateTime.Now, Convert.ToDateTime(remindTime));
                if (seconds> 0)
                {
                    RemindTimers.Add(caseID, initialTimer(seconds));
                    init_NoExpiredCases = utility.updateSTR_AppendSubStr(init_NoExpiredCases, ",", caseID);
                }
                else
                {
                    init_ExpiredCases = utility.updateSTR_AppendSubStr(init_ExpiredCases, ",", caseID);
                }
            }
        }

        public List<ListViewItem> getNoExpiredCases()
        {
            List<ListViewItem> ReminderCases = new List<ListViewItem>();
            using (SqlConnection sqlConnection = SQL.dbConnect())
            {
                ReminderCases = SQL.genListLvi(SQL.sqlCmd_Cases(Regex.Split(init_NoExpiredCases, ",")), Config.UI_CaseKeys, sqlConnection);
            }
            return ReminderCases;
        }

        public List<ListViewItem> getExpiredCases()
        {
            List<ListViewItem> ReminderCases = new List<ListViewItem>();
            using (SqlConnection sqlConnection = SQL.dbConnect())
            {
                ReminderCases = SQL.genListLvi(SQL.sqlCmd_Cases(Regex.Split(init_ExpiredCases, ",")), Config.UI_CaseKeys, sqlConnection);
            }
            return ReminderCases;
        }

        private void timeUpAction(object obj, System.Timers.ElapsedEventArgs e)
        {
            foreach (string caseID in RemindTimers.Keys.ToList())
            {
                if (obj.Equals(RemindTimers[caseID]))
                {
                    message msg = new message();
                    msg.setKeyValuePair("1", caseID);
                    Msg_OP msg_Op = new Msg_OP("N3", new message[] { msg });
                    Config.SLS_Sock.inMsgQueue_add(msg_Op);
                    break;
                }
            }
        }

    }
}
