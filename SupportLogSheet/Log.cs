// Log 对象创建类
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SupportLogSheet
{
    public class Log
    {
        private string filePath;
        private FileStream fs;

        public Log(string path)
        {
            filePath = path;
            fs = new FileStream(filePath, FileMode.Append);
        }

        public void writeLog(string message)
        {
            StringBuilder logStringBuilder = new StringBuilder();
            logStringBuilder.Append("[" );
            logStringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            logStringBuilder.Append(":");
            logStringBuilder.Append(DateTime.Now.Millisecond.ToString());
            logStringBuilder.Append("]: ");
            logStringBuilder.Append(message);
            logStringBuilder.Append("\r\n");
            message msg = new message();
            msg.setKeyValuePair("220", logStringBuilder.ToString());
            Msg_OP msg_Op = new Msg_OP("IO9", new message[] { msg });
           Config.SLS_Sock.inMsgQueue_add(msg_Op);
        }

        public void flushToFile(Msg_OP msg_Op)
        {
            try
            {
                byte[] bytes = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(msg_Op.getMsgs()[0].getValueFromPairs("220"));
                fs.Write(bytes, 0, bytes.Length);
                fs.Flush();
            }
            catch 
            {
                fs.Close();
                fs = new FileStream(filePath, FileMode.Append);
               Config.SLS_Sock.inMsgQueue_add(msg_Op);
            }
        }

        public void writeErrorLog(Exception ex)
        {
            StringBuilder logStringBuilder = new StringBuilder();
            logStringBuilder.Append("[");
            logStringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            logStringBuilder.Append(":");
            logStringBuilder.Append(DateTime.Now.Millisecond.ToString());
            logStringBuilder.Append("]: Error>");
            logStringBuilder.Append(ex.ToString());
            logStringBuilder.Append("\r\n");
            message msg = new message();
            msg.setKeyValuePair("220", logStringBuilder.ToString());
            Msg_OP msg_Op = new Msg_OP("IO9", new message[] { msg });
           Config.SLS_Sock.inMsgQueue_add(msg_Op);
        }

        public void Close()
        {
            fs.Close();
        }

        public void writeLogSycn(string message)
        {
            StringBuilder logStringBuilder = new StringBuilder();
            logStringBuilder.Append("[");
            logStringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            logStringBuilder.Append(":");
            logStringBuilder.Append(DateTime.Now.Millisecond.ToString());
            logStringBuilder.Append("]: ");
            logStringBuilder.Append(message);
            logStringBuilder.Append("\r\n");
            byte[] bytes = System.Text.Encoding.GetEncoding("UTF-8").GetBytes(logStringBuilder.ToString());
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
        }
    }
}
