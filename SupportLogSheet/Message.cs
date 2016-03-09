// 中间消息 类
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Windows.Forms;

namespace SupportLogSheet
{
    public class message
    {
        private Hashtable ht;

        public message()
        {
            ht = new Hashtable();
        }
        public message(string msg)
        {
            this.ht = getKeyValueHT(msg);
        }
        public string getMsg()
        {
            StringBuilder sb = new StringBuilder();
            foreach (DictionaryEntry de in ht)
            {
                string value = removeSeparator(de.Value.ToString().Trim(' '));
                if (sb.Length != 0)
                {
                    sb.Append(Config.Msg_Separator2).Append(de.Key.ToString().Trim(' ')).Append(Config.Msg_Separator2).Append(value);
                }
                else
                {
                    sb.Append(de.Key.ToString().Trim(' ')).Append(Config.Msg_Separator2).Append(value);
                }
            }
            return sb.ToString();
        }
        private string removeSeparator(string input)
        {
            if (input.Contains(Config.Msg_Separator1))
            {
                input = Regex.Replace(input, Config.Msg_Separator1, " ");
            }
            if (input.Contains(Config.Msg_Separator2))
            {
                input = Regex.Replace(input, Config.Msg_Separator2, " ");
            }
            if (input.Contains(Config.Msg_Separator3))
            {
                input = Regex.Replace(input, Config.Msg_Separator3, " ");
            }
            if (input.Contains(Config.Msg_Separator6))            {
                input = Regex.Replace(input, Config.Msg_Separator6, " ");
            }
            return input;
        }
        private Hashtable getKeyValueHT(string s)
        {
            ht = new Hashtable();
            try
            {
                string[] pairs = Regex.Split(s, Config.Msg_Separator2);
                if (pairs.Length % 2 == 0)
                {
                    for (int i = 0; i < pairs.Length; i = i + 2)
                    {
                        ht.Add(pairs[i], pairs[i + 1]);
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
            return ht;
        }
        public void removePair(string key)
        {
            if (ht.ContainsKey(key))
            {
                ht.Remove(key);
            }
        }
        public string getValueFromPairs(string key)
        {
            if (ht.ContainsKey(key))
            {
                return ht[key].ToString();
            }
            else
            {
                return "";
            }
        }
        public string[] getValuesFromKeys(string[] keys)
        {
            string[] values = new string[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                values[i] = getValueFromPairs(keys[i]);
            }
            return values;
        }
        public List<string> getExistingValues()
        {
            List<string> result = new List<string>();
            foreach (DictionaryEntry de in ht)
            {
                result.Add(de.Value.ToString());
            }    
            return result;
        }
        public void setKeyValuePair(string key, string value)
        {
            string k = key.Trim(' ');
            if (ht.ContainsKey(k))
            {
                ht[k] = value.Trim(' ');
            }
            else
            {
                ht.Add(k, value.Trim(' '));
            }
        }
        public bool isContainsKey(string key)
        {
            return ht.ContainsKey(key);
        }
        public bool isContainsKeys(string[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
            {
                if (!isContainsKey(keys[i]))
                {
                    MessageBox.Show(keys[i]);
                    return false;
                }
            }
            return true;
        }
    }
}
