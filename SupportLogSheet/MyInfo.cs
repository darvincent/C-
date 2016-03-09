// 个人信息类
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SupportLogSheet
{

    public class MyInfo
    {
        //My Info
        private string myID;
        private string myName;
        private string myChiName;
        private string myEmail;
        private string myExName;
        private string myLocation;
        public MyInfo(string ID, string name, string chiName, string email, string exName,string location)
        {
            myID = ID;
            myName = name;
            myChiName = chiName;
            myEmail = email;
            myExName = exName;
            myLocation = location;
        }
        public string MyID
        {
            get { return myID; }
            set { myID = value; }
        }
        public string MyName
        {
            get { return myName; }
            set { myName = value; }
        }
        public string MyChiName
        {
            get { return myChiName; }
            set { myChiName = value; }
        }
        public string MyEmail
        {
            get { return myEmail; }
            set { myEmail = value; }
        }
        public string MyExName
        {
            get { return myExName; }
            set { myExName = value; }
        }
        public string MyLocation
        {
            get { return myLocation; }
            set { myLocation = value; }
        }
    }
}
