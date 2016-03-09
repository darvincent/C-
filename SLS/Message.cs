// 中间消息类
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Windows.Forms;

namespace SLS
{
    class Message
    {
        public Hashtable ht;

        public Message()
        {
            ht = new Hashtable();
        }
        public Message(string msg)
        {
            this.ht = GetKeyValueHT(msg);
        }
        public string GetMsg()
        {
            StringBuilder sb = new StringBuilder();
            foreach (DictionaryEntry de in ht)
            {
                if (sb.Length != 0)
                {
                    sb.Append(Config.Msg_Separator2).Append(de.Key.ToString().Trim()).Append(Config.Msg_Separator2).Append(de.Value.ToString().Trim());
                }
                else
                {
                    sb.Append(de.Key.ToString().Trim()).Append(Config.Msg_Separator2).Append(de.Value.ToString().Trim());
                }
            }
            return sb.ToString();
        }
        public Hashtable GetKeyValueHT(string s)
        {
            try
            {
                ht = new Hashtable();
                string[] pairs = Regex.Split(s, Config.Msg_Separator2);
                if (pairs.Length % 2 == 0)
                {
                    for (int i = 0; i < pairs.Length; i = i + 2)
                    {
                        ht.Add(pairs[i], pairs[i + 1]);
                    }
                }
                return ht;
            }
            catch (Exception ex)
            {
                Server.logger.Error(ex);
                return null;
            }
        }
        public void RemovePair(string key)
        {
            if (ht.ContainsKey(key))
            {
                ht.Remove(key);
            }
        }
        public string GetValueFromPairs(string key)
        {
            try
            {
                if (ht.ContainsKey(key))
                {
                    return ht[key].ToString().Trim();
                }
                else
                {
                    return "";
                }
            }
            catch (Exception ex)
            {
                Server.logger.Error(ex);
                return "";
            }
        }
        public string[] GetValuesFromHT(string[] keys)
        {
            string[] values = new string[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                values[i] = ModifySocketMsg(GetValueFromPairs(keys[i]));
            }
            return values;
        }
        public void SetKeyValuePair(string key, string value)
        {
            try
            {
                string k = key.Trim(), v = value.Trim();
                if (ht.ContainsKey(k))
                {
                    ht[k] = v;
                }
                else
                {
                    ht.Add(k, v);
                }
            }
            catch (Exception ex)
            {
                Server.logger.Error(ex);
            }
        }
        public string ModifySocketMsg(string s)
        {
            string pattern = @"'";
            string substitution = "''";
            if (Regex.IsMatch(s, pattern))
            {
                s = Regex.Replace(s, pattern, substitution);
            }
            return s;
        }
    }
}
