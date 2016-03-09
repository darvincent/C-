// config 信息
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Net.Sockets;
using System.Data.SqlClient;

namespace SupportLogSheet
{
    class Config
    {
        //ini
        public static readonly Log logWriter = new Log("log.txt");
        public static INI_OP Customization_INI;
        public static INI_OP Common_INI;
        //Socket
        public static SLSocket SLS_Sock;

        //version no
        public static readonly string SLS_Version = "2.0.13";
        public static readonly string ZB = new StringBuilder("SupportLogSheet  V_").Append(SLS_Version).Append("\r\nBy  Ian.Z\r\n").ToString();
        
        //XML file content
        public static readonly string ReminderXMLContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<Reminder />";
        public static readonly string MarkedCaseXMLContent = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<config />";

        //Font
        public static Font Font_Content;
 
        //seperate symbol
        public static string Msg_Separator1 = "\u0001";
        public static string Msg_Separator2 = "\u0002";
        public static string Msg_Separator3 = "\u0003";
        public static string Msg_Separator6 = "\u0006";

        // 和server端消息 key-value mapping格式
        public static readonly Dictionary<string, string> KeyValue_pair = new Dictionary<string, string> {
             //SupportLogSheet
            {"1", "CaseID"},
            {"2", "CreateTime"},
            {"3", "HappenAt"},
            {"4", "Client"},
            {"5", "Caller"},
            {"6", "Phone"},
            {"7", "Through"},
            {"8", "Description"},
            {"9", "Type"},
            {"10", "Incharge"},
            {"11", "Status"},
            {"12", "Solution"},
            {"13", "Priority"},
            {"14", "Issue"},
            //CaseProperty
            {"100", "Category"},
            {"101", "Property"},
            //AccountManager
            {"150", "Name"},
            {"151", "Phone"},
            {"152", "Phone2"},
            //CaseFollowUp
            {"50", "TimeModified"},
            {"52", "FollowUp"},
            //CaseHints
            {"73", "Hints"},
            //ClientContact
            {"111", "Contact"},
            {"112", "Title"},
            {"113", "Phone"},
            {"114", "Phone2"},
            {"115", "Email"},
            {"116", "Address"},
            //ClientList
            {"132", "Level"},
            {"133", "AccountManager"},
            //UserFile
            {"80", "UserName"},
            {"81", "ChineseName"},
            {"82", "Password"},
            {"83", "PasswordHash"},
            {"84", "Salt"},
            {"85", "Email"},
            {"86", "SuperUser"},
            {"87", "UserID"},
            {"88", "Active"},
            {"90", "IsSupport"},
            //ProductMaster
            {"160", "Server"},
            {"161", "Active"},
            {"162", "IP"},
            {"163", "LoginUser"},
            {"164", "Password"},
            {"165", "RemoteThrough"},
            {"166", "Category"},
            {"167", "Product"},
            {"168", "Description"},
            {"169", "Version"},
            {"170", "Date"},
            {"171", "UpdateBy"},
            {"172", "Remark"},
            //ProductHint
            {"200", "HintID"},
            {"201", "HintType"},
            {"202", "HintDescription"},
            {"203", "FromVersion"},
            {"204", "ToVersion"},
            {"205", "HintDetail"},
            //MyMarkedCase
            {"210", "CaseCategory"},
            {"211", "NewCaseCategory"},
            {"212", "CategoryDescription"},
            {"213", "FilePath"},
            //LogWriting
            {"220", "LogContent"},
            //Notice
            {"230","NoticeID"},
            {"231","CreateTime"},
            {"232","Sender"},
            {"233","Receiver"},
            {"234","Content"}
        };

        public static readonly string[] shortCutKeys = { "QI_New", "QI_Add", "QI_Clear", "QI_SwitchLeft", "QI_SwitchRight", "Case_New", "Case_Refresh", "Case_Close", "Case_Multifilter", "Tab_Outstanding", "Tab_Search", "Tab_MyTask", "Tab_CIM", "Tab_Notice", "Tab_Reminder" };

        // action types
        public static readonly string[] lowFrequencyType = { "R1", "R2", "L0", "L1", "L2","L3", "U1", "U2", "U3", "U4", "S", "B1", "B2", "B3", "A1", "A2", "A3", "P1", "P2", "P3", "PM1", "PM2", "PM3", "PM4", "PM5", "PM6", "PM7", "PM8", "PM9", "PM10", "PM11", "PH1", "PH2", "PH3", "H1", "F1", "F2", "F3", "ERROR", "Kicked", "N0", "N1", "N2", "N3" };
        public static readonly string[] highFrequencyType = { "F0", "H0" };
        public static readonly string[] caseType = { "C1", "C2", "C3", "C4", "C5", "C6", "C7" };
        public static readonly string[] IOType = { "IO1", "IO2", "IO3", "IO4", "IO5", "IO6", "IO7", "IO8", "IO9"};
        public static readonly string[] PM_SeparateType = { "PM1", "PM2", "PM3", "PM4", "PM5", "PM6", "PM7", "PM8", "PM9", "PM10", "PM11", "IO9", "N1","ERROR","Kicked" };

        //DB  table names
        public static readonly string DBName_SupportLogSheet = "SupportLogSheet";
        public static readonly string DBName_AccountManager = "AccountManager";
        public static readonly string DBName_CaseFollowUp = "CaseFollowUp";
        public static readonly string DBName_CaseHints = "CaseHints";
        public static readonly string DBName_ClientContact = "ClientContact";
        public static readonly string DBName_ClientList = "ClientList";
        public static readonly string DBName_UserFile = "UserFile";
        public static readonly string DBName_CaseProperty = "CaseProperty";
        public static readonly string DBName_ClientServer = "ClientServer";
        public static readonly string DBName_Product = "Product";
        public static readonly string DBName_ProductMaster = "ProductMaster";
        public static readonly string DBName_ProductMaster_Update = "ProductMaster_Update";
        public static readonly string DBName_Notice = "Notice";

        public static readonly string[] CaseProperty_Category = { "Priority", "Through", "Type", "Status", "ClientLevel", "Issue", "ProductCate" };
        public static readonly string[] CategoryType1 = { "Client", "Incharge", "Product", "Priority", "Issue", "Type", "Status" };
        public static readonly string[] CategoryType2 = { "Setting", "Operation", "Bug" };

        //DB columns corresponding keys
        public static readonly string[] UserFileKeys = { "80", "81", "82", "83", "84", "85", "86", "87", "88", "90" };
        public static readonly string[] ClientServerKeys = { "4", "160", "161", "162", "163", "164", "165" };
        public static readonly string[] ProductKeys = { "166", "167", "168" };
        public static readonly string[] ProductMasterKeys = { "4", "160", "166", "167", "169" };

        //UI keys
        public static readonly string[] UI_CaseKeys = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "167", "160", "169" };
        public static readonly string[] UI_CasePropertyKeys = { "100", "101" };
        public static readonly string[] UI_AccountManagerKeys = { "150", "151", "152" };
        public static readonly string[] UI_CaseFollowUpKeys = { "50", "52" };
        public static readonly string[] UI_CaseHintsKeys = { "1", "73" };
        public static readonly string[] UI_ClientContactKeys = { "4", "111", "112", "113", "114", "115", "116" };
        public static readonly string[] UI_ClientListKeys = { "4", "132", "133" };
        public static readonly string[] UI_UserFileKeys = { "80", "81", "85", "86", "87", "88", "90" };
        public static readonly string[] UI_UserUpdateKeys = { "80", "17", "19" };
        public static readonly string[] UI_ClientServerKeys = { "160", "161", "162" };
        public static readonly string[] UI_FilterProductVersionsKeys = { "4", "160", "169" };
        public static readonly string[] UI_ProductKeys = { "166", "167", "168" };
        public static readonly string[] UI_ProductMasterKeys = { "4", "160", "166", "167", "169" };
        public static readonly string[] UI_ProductMaster_UpdateKeys = { "4", "160", "166", "167", "169", "171", "170", "172" };
        public static readonly string[] UI_MultiFilterProductKeys = { "4", "160" };
        public static readonly string[] UI_CaseAnalysisKeys = { "1", "13", "4", "160", "167", "169", "9", "14", "10", "11" };
        public static readonly string[] UI_ProductHintsKeys = { "200","167", "201", "202", "203", "204","205" };
        public static readonly string[] UI_NoticeKeys = { "231", "233", "234" };
        //UI_Index Keys
        public static readonly string[] UI_Index_CaseKeys = { "1"};
        public static readonly string[] UI_Index_CasePropertyKeys = { "100", "101" };
        public static readonly string[] UI_Index_AccountManagerKeys = { "150" };
        public static readonly string[] UI_Index_CaseFollowUpKeys = { "50", "52" };
        public static readonly string[] UI_Index_CaseHintsKeys = { "1" };
        public static readonly string[] UI_Index_ClientContactKeys = { "4", "111"};
        public static readonly string[] UI_Index_ClientListKeys = { "4", "132" };
        public static readonly string[] UI_Index_UserFileKeys = { "80" };
        public static readonly string[] UI_Index_UserUpdateKeys = { "80" };
        public static readonly string[] UI_Index_ClientServerKeys = { "160" };
        public static readonly string[] UI_Index_ProductKeys = { "166", "167" };
        public static readonly string[] UI_Index_ProductMasterKeys = { "4", "160", "166"};
        public static readonly string[] UI_Index_ProductMaster_UpdateKeys = { "4", "160", "167", "169"};
        public static readonly string[] UI_Index_CaseAnalysisKeys = { "1" };
        public static readonly string[] UI_Index_ProductHintsKeys = {  "200"};
        //UI_Index_value Keys
        //public static readonly string[] UI_IndexValue_CaseKeys = { "1" };
        //public static readonly string[] UI_IndexValue_CasePropertyKeys = { "100", "101" };
        //public static readonly string[] UI_IndexValue_AccountManagerKeys = { "150" };
        //public static readonly string[] UI_IndexValue_CaseFollowUpKeys = { "50", "52" };
        //public static readonly string[] UI_IndexValue_CaseHintsKeys = { "1" };
        public static readonly string[] UI_IndexValue_ClientContactKeys = { "4", "117" };
        //public static readonly string[] UI_IndexValue_ClientListKeys = { "4", "132", "133" };
        public static readonly string[] UI_IndexValue_UserFileKeys = { "89" };
        //public static readonly string[] UI_IndexValue_UserUpdateKeys = { "80", "17", "19" };
        //public static readonly string[] UI_IndexValue_ClientServerKeys = { "160" };
        public static readonly string[] UI_IndexValue_ProductKeys = { "175", "176" };
        //public static readonly string[] UI_IndexValue_ProductMasterKeys = { "4", "160", "166" };
        //public static readonly string[] UI_IndexValue_ProductMaster_UpdateKeys = { "4", "160", "166", "167", "169", "171", "170", "172" };
        //public static readonly string[] UI_IndexValue_CaseAnalysisKeys = { "1", "13", "4", "160", "167", "169", "9", "14", "10", "11" };
        //public static readonly string[] UI_IndexValue_ProductHintsKeys = { "167", "200", "201", "202", "203", "204", "205" };
        
        public static int getIndex(string[] keys,string key)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (key.Equals(keys[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        public static string getValue(string key)
        {
            if (KeyValue_pair.ContainsKey(key))
            {
                return KeyValue_pair[key];
            }
            return "";
        }

        public static string[] getValues(string[] keys)
        {
            if (keys != null)
            {
                string[] values = new string[keys.Length];
                for (int i = 0; i < keys.Length; i++)
                {
                    try
                    {
                        values[i] = KeyValue_pair[keys[i]];
                    }
                    catch
                    {
                        values[i] = "";
                    }
                }
                return values;
            }
            return new string[0];
        }

        public static Dictionary<string,string> genPair(Dictionary<string,string> input,string[] keys)
        {
            Dictionary<string,string> output = new Dictionary<string,string>();
            for(int i=0;i<keys.Length;i++)
            {
                output.Add(keys[i], input[keys[i]]);
            }
            return output;
        }

    }
}
