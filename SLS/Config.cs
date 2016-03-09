using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SLS
{
    class Config
    {
        public static string Msg_Separator1 = "\u0001";
        public static string Msg_Separator2 = "\u0002";
        public static string Msg_Separator3 = "\u0003";
        public static string Msg_Separator6 = "\u0006";
        //DB names
        public static string DBName_SupportLogSheet = "SupportLogSheet";
        public static string DBName_AccountManager = "AccountManager";
        public static string DBName_CaseFollowUp = "CaseFollowUp";
        public static string DBName_CaseHints = "caseHints";
        public static string DBName_ClientContact = "ClientContact";
        public static string DBName_ClientList = "ClientList";
        public static string DBName_UserFile = "UserFile";
        public static string DBName_CaseProperty = "CaseProperty";
        public static string DBName_ClientServer = "ClientServer";
        public static string DBName_Product = "Product";
        public static string DBName_ProductMaster = "ProductMaster";
        public static string DBName_ProductMaster_Update = "ProductMaster_Update";
        public static string DBName_ProductHints = "ProductHints";

        //DB columns corresponding keys
        public static string[] SupportLogSheetKeys = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13", "14" ,"167","160","169"};
        public static string[] CasePropertyKeys = { "100", "101" };
        public static string[] AccountManagerKeys = { "150", "151", "152" };
        public static string[] CaseFollowUpKeys = { "1", "50", "52" };
        public static string[] CaseHintsKeys = { "1", "73" };
        public static string[] ClientContactKeys = { "4", "111", "112", "113", "114", "115", "116" };
        public static string[] ClientListKeys = { "4", "132", "133" };
        public static string[] UserFileKeys = { "80", "81", "82", "83", "84", "85", "86", "87", "88","90" };
        public static string[] ClientServerKeys = { "4", "160", "161", "162", "163", "164", "165" };
        public static string[] ProductKeys = { "166", "167", "168" };
        public static string[] ProductMasterKeys = { "4", "160", "166", "167", "169" };
        public static string[] ProductMaster_UpdateKeys = { "4", "160", "166", "167", "169", "170", "171", "172" };
        public static string[] ProductHintsKeys = { "200", "167", "201", "202", "203", "204", "205" };
        public static string[] UserFile_SuperUser = { "80", "86", "87", "88", "90" };
        public static string[] UserFile_Self = { "80", "81", "85" };
        public static string[] CompareCaseKeys = { "3", "4", "5", "6", "7", "9", "10", "11", "13", "14", "167", "160", "169" };
        public static string[] LoadCaseKeys = { "1", "3", "4", "5", "6", "7", "9", "10", "11", "13", "14", "167", "160", "169" };
        //DB PK columns
        public static string[] PK_SupportLogSheetKeys = { "1" };
        public static string[] PK_CasePropertyKeys = { "100", "101" };
        public static string[] PK_AccountManagerKeys = { "150" };
        public static string[] PK_CaseFollowUpKeys = { "1", "50" };
        public static string[] PK_CaseHintsKeys = { "1" };
        public static string[] PK_ClientContactKeys = { "4", "111" };
        public static string[] PK_ClientListKeys = { "4" };
        public static string[] PK_UserFileKeys = { "80" };
        public static string[] PK_ClientServerKeys = { "4", "160" };
        public static string[] PK_ProductKeys = { "166", "167" };
        public static string[] PK_ProductMasterKeys = { "4", "160", "167" };
        public static string[] PK_ProductMaster_UpdateKeys = { "4", "160", "167", "169" };
        public static string[] PK_ProductHintsKeys = { "200" };

        public static readonly Dictionary<string, string> LocationNamePath_pair = new Dictionary<string, string>{
            {"HK","HK_path"},
            {"CN","CN_path"}
        };

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
            //caseHints
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
            {"220", "LogContent"}
        };

        public static string getValue(string key)
        {
            return KeyValue_pair[key];
        }

        public static string[] getValues(string[] keys)
        {
            string[] values = new string[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                values[i] = KeyValue_pair[keys[i]];
            }
            return values;
        }

        public static Dictionary<string, string> genPair(Dictionary<string, string> input, string[] keys)
        {
            Dictionary<string, string> output = new Dictionary<string, string>();
            for (int i = 0; i < keys.Length; i++)
            {
                output.Add(keys[i], input[keys[i]]);
            }
            return output;
        }
    }
}
