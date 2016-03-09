using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SLS
{
    class Log
    {
        private string filePath;
        private FileStream fs;

        public Log(string path)
        {
            filePath = path;
            fs = new FileStream(filePath, FileMode.Append);
        }
        public void Info(string message)
        {
            StringBuilder logStringBuilder = new StringBuilder();
            logStringBuilder.Append("[" );
            logStringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            logStringBuilder.Append(":");
            logStringBuilder.Append(DateTime.Now.Millisecond.ToString());
            logStringBuilder.Append("]: ");
            logStringBuilder.Append(message);
            logStringBuilder.Append("\r\n");
            byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(logStringBuilder.ToString());
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
        }
        public void Error(Exception ex)
        {
            StringBuilder logStringBuilder = new StringBuilder();
            logStringBuilder.Append("[");
            logStringBuilder.Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            logStringBuilder.Append(":");
            logStringBuilder.Append(DateTime.Now.Millisecond.ToString());
            logStringBuilder.Append("]: Error>");
            logStringBuilder.Append(ex.ToString());
            logStringBuilder.Append("\r\n");
            byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(logStringBuilder.ToString());
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
            //alert function
        }
        public void writeRecovery(string msg)
        {
            byte[] bytes = Encoding.GetEncoding("UTF-8").GetBytes(msg);
            fs.Write(bytes, 0, bytes.Length);
            fs.Flush();
        }
    }
}
