using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;

namespace SLS
{
    public class INIClass
    {
        private string inipath;
        private object writeLock;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        public INIClass(string INIPath)
        {
            inipath = INIPath;
            writeLock = new object();
        }

        public void Write(string Section, string Key, string Value)
        {
            lock (writeLock)
            {
                try
                {
                    WritePrivateProfileString(Section, Key, Value, this.inipath);
                }
                catch (IOException ex)
                {
                    Server.logger.Error(ex);
                }
            }
        }

        public string Read(string Section, string Key)
        {
            try
            {
                StringBuilder temp = new StringBuilder(50000);
                int i = GetPrivateProfileString(Section, Key, "", temp, 50000, this.inipath);
                return temp.ToString();
            }
            catch (IOException ex)
            {
                Server.logger.Error(ex);
                return "Error";
            }
        }

        public bool Exist()
        {
            return File.Exists(inipath);
        }
    }
}
