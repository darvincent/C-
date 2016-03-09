// .ini 文件 操作类
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SupportLogSheet
{
    public class INI_OP
    {
        private string inipath;
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string ShortcutKey, string val, string filePath);
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string ShortcutKey, string def, StringBuilder retVal, int size, string filePath);

        public INI_OP(string INIPath)
        {
            if (!File.Exists(INIPath))
            {
                File.Create(INIPath);
            }
            inipath = INIPath;
        }

        public void writeValue(string Section, string Key, string Value)
        {
            try
            {
                WritePrivateProfileString(Section, Key, Value, this.inipath);
            }
            catch (IOException ex)
            {
                Config.logWriter.writeErrorLog(ex);      
            }
        }

        public string readValue(string Section, string Key)
        {
            try
            {
                StringBuilder temp = new StringBuilder(5000);
                int i = GetPrivateProfileString(Section, Key, "", temp, 5000, this.inipath);
                return temp.ToString();
            }
            catch (IOException ex)
            {
                Config.logWriter.writeErrorLog(ex);
                return "";
            }
        }

        public void deleteKey(string Section, string Ident)
        {
             WritePrivateProfileString(Section, Ident, null, this.inipath);
        }

        public void deleteSection(string section)
        {
            WritePrivateProfileString(section, null, null, this.inipath);
        }

        public void updateKeyValue_AppendSubStr(string section, string key, string value)
        {
            string temp = readValue(section, key);
            temp = utility.updateSTR_AppendSubStr(temp,",", value);
            writeValue(section, key, temp);
        }

        public string updateKeyValue_ReplaceSubStr(string section, string key, string pattern,string substitution)
        {
            string temp = readValue(section, key);
            temp = utility.updateSTR_ReplaceSubStr(temp,",", pattern, substitution);
            writeValue(section, key, temp);
            return temp;
        }

        public bool ExistINIFile()
        {
            return File.Exists(inipath);
        }

    }

}
