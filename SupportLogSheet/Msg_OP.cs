// socket 消息类
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Drawing;

namespace SupportLogSheet
{
    public class Msg_OP
    {
        public string sentFrom;
        public string msgType;
        private StringBuilder  msgContent;
        public bool isCorrectFormat = false;
        private message[] msgs;
        public delegate void Del_MsgOp(Msg_OP msg_Op);

        public Msg_OP(string type, ListViewItem lvi, string[] keys)
        {
            this.msgType = type;       
            buildMsgs(lvi, keys);   
        }

        public Msg_OP(string type,message[]  msgs )
        {
            this.msgType = type;
            this.msgs = msgs;
        }

        public Msg_OP(string socketMsg)
        {
            string[] info = Regex.Split(socketMsg, Config.Msg_Separator1);
            if (info.Length == 3)
            {
                sentFrom = info[0].Trim(' ');
                msgType = info[1].Trim(' ');
                msgContent = new StringBuilder(info[2].Trim(' '));
                isCorrectFormat = true;
            }
        }

        public message[] getMsgs()
        {
            return msgs;
        }

        public void setMsgContent(string s)
        {
            msgContent.Clear();
            msgContent.Append(s);
        }

        public string getMsgContent()
        {
            return msgContent.ToString();
        }

        public void buildMsgs()
        {
            if (msgs == null)
            {
                string[] temp = Regex.Split(getMsgContent(), Config.Msg_Separator3);
                msgs = new message[temp.Length];
                for (int i = 0; i < temp.Length; i++)
                {
                    msgs[i] = new message(temp[i]);
                }
            }
        }

        public void buildMsgs(ListViewItem lvi, string[] keys)
        {
            if (msgs == null)
            {
                msgs = new message[1];
                if (lvi != null)
                {
                    msgs[0] = new message();
                    for (int i = 0; i < keys.Length; i++)
                    {  
                        msgs[0].setKeyValuePair(keys[i], lvi.SubItems[i+1].Text);
                    }
                }
            }
        }

        public string getValueFromPairs(string key)
        {
            buildMsgs();
            return msgs[0].getValueFromPairs(key);
        }

        public void setValueToPairs(string key, string value)
        {
            buildMsgs();
            msgs[0].setKeyValuePair(key, value);
        }

    }
}
