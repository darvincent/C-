// listView 操作类
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Drawing;
namespace SupportLogSheet
{
    class LV_OP
    {
        //Colors
        public static Color Color_NewItem;
        public static Color Color_EditItem;
        public static readonly Color Color_Missing = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
        //public static Color Color_Missing1 = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(248)))), ((int)(((byte)(250)))));   

        public static Dictionary<string, Color> ClientLevelColorDetail = new Dictionary<string, Color>();
        public static Dictionary<string, Color> PriorityColorDetail = new Dictionary<string, Color>();
        public delegate void Del_initialListView(ListView lv, List<ListViewItem> lvis, bool doClear);
        public delegate void Del_initialContent(List<ListViewItem>lvis);

        public static void listviewSort(ListView l, ColumnClickEventArgs e, ref bool sortType)
        {
            if (e.Column != 0)
            {
                l.ListViewItemSorter = new ListViewItemsComparer(e.Column, sortType);
                l.Sort();
                sortType = !sortType;
            }
        }

        public static void listViewAutoSize(ListView l)
        {
            try
            {
                if (l.Columns.Count > 0)
                {
                    l.BeginUpdate();
                    //l.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                    l.Columns[0].Width = 0;
                    int width = l.Width;
                    for (int i = 1; i < l.Columns.Count - 1; i++)
                    {
                        width -= l.Columns[i].Width;
                    }
                    l.Columns[l.Columns.Count - 1].Width = width;
                    l.EndUpdate();
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        public static void initialListView(ListView l, Dictionary<string,List<ListViewItem>>content,bool doClear)
        {
            l.BeginUpdate();
            foreach (List<ListViewItem> items in content.Values)
            {
                l.Items.AddRange(items.ToArray());
            }
            l.EndUpdate();
        }

        public static void initialListView(ListView l, List<ListViewItem> content, bool doClear)
        {
            l.BeginUpdate();
            if (doClear)
            {
                l.Items.Clear();
            }
            if (content != null)
            {
                l.Items.AddRange(content.ToArray());
            }
            l.EndUpdate();
        }

        public static void readColumnOrder(ListView lv, INI_OP INI_Config, string category)
        {
            for (int i = 1; i < lv.Columns.Count; i++)
            {
                if (!INI_Config.readValue(category, lv.Columns[i].Text).Equals(""))
                {
                    lv.Columns[i].DisplayIndex = Int32.Parse(INI_Config.readValue(category, lv.Columns[i].Text));
                }
            }
        }

        public static void writeColumnOrder(ListView lv, INI_OP INI_Config, string category)
        {
            for (int i = 1; i < lv.Columns.Count; i++)
            {
                INI_Config.writeValue(category, lv.Columns[i].Text, lv.Columns[i].DisplayIndex.ToString());
            }
        }

        public static void readColumnWidth(ListView lv, INI_OP INI_Config, string category)
        {
            for (int i = 1; i < lv.Columns.Count; i++)
            {
                if (!INI_Config.readValue(category, lv.Columns[i].Text).Equals(""))
                {
                    lv.Columns[i].Width = Int32.Parse(INI_Config.readValue(category, lv.Columns[i].Text));
                }
            }
        }

        public static void writeColumnWidth(ListView lv, INI_OP INI_Config, string category)
        {
            for (int i = 1; i < lv.Columns.Count; i++)
            {
                INI_Config.writeValue(category, lv.Columns[i].Text, lv.Columns[i].Width.ToString());
            }
        }

        private static bool updateOneListviewItem(ListViewItem lvi, string[] UIkeys, message msg)
        {
            if (lvi != null)
            {
                if (lvi.ListView != null)
                {
                    lvi.ListView.BeginUpdate();
                    for (int i = 0; i < UIkeys.Length; i++)
                    {
                        lvi.SubItems[i + 1].Text = msg.getValueFromPairs(UIkeys[i]);
                    }
                    lvi.ListView.EndUpdate();
                }
                return true;
            }
            return false;
        }

        public static bool updateOneListviewItem(ListView lv, string[] UIkeys, string[] indexKeys, string[] IndexValueKeys, message msg)
        {
            //UIKeys = listviewItem keys, indexKeys=search index keys, valueIndexKeys= search value keys, those keys values all get from msg
            ListViewItem lvi;
            if (IndexValueKeys != null)
            {
                lvi = findListviewItem(lv, UIkeys, indexKeys, msg.getValuesFromKeys(IndexValueKeys));
            }
            else
            {
                lvi = findListviewItem(lv, UIkeys, indexKeys, msg.getValuesFromKeys(indexKeys));
            }
            return updateOneListviewItem(lvi, UIkeys, msg);
        }

        public static bool updateOneListviewItem(List<ListViewItem> items, string[] UIkeys, string[] indexKeys, string[] IndexValueKeys, message msg)
        {
            //UIKeys = listviewItem keys, indexKeys=search index keys, valueIndexKeys= search value keys, those keys values all get from msg
            ListViewItem lvi;
            if (IndexValueKeys != null)
            {
                lvi = findListviewItem(items, UIkeys, indexKeys, msg.getValuesFromKeys(IndexValueKeys));
            }
            else
            {
                lvi = findListviewItem(items, UIkeys, indexKeys, msg.getValuesFromKeys(indexKeys));
            }
            return updateOneListviewItem(lvi, UIkeys, msg);
        }

        public static ListViewItem findListviewItem(ListView  lv, string[] UIkeys,string[] indexKeys, message msg)
        {
            for (int i = 0; i < indexKeys.Length; i++)
            {
                if (!msg.isContainsKey(indexKeys[i]))
                {
                    return null;
                }
            }
            if (UIkeys.Length < indexKeys.Length)
            {
                return null;
            }
            int[] UIindexes = new int[indexKeys.Length];
            for (int i = 0; i < indexKeys.Length; i++)
            {
                UIindexes[i] = Config.getIndex(UIkeys, indexKeys[i]);
                if (UIindexes[i] < 0)
                {
                    return null;
                }
                else
                {
                    UIindexes[i]++;
                }
            }
            string[] indexValues = msg.getValuesFromKeys(indexKeys);
            bool result;
            for (int i = 0; i < lv.Items.Count; i++)
            {
                result = true;
                for (int t = 0; t < UIindexes.Length; t++)
                {
                    if (!lv.Items[i].SubItems[UIindexes[t]].Text.Equals(indexValues[t]))
                    {
                        result = false;
                        break;
                    }
                }
                if (result)
                {
                    return lv.Items[i];
                }
            }
            return null;
        }

        public static ListViewItem findListviewItem(ListView lv, string[] UIkeys, string[] indexKeys, string[] indexValues)
        {
            if (indexKeys.Length != indexValues.Length)
            {
                return null;
            }
            if (UIkeys.Length < indexKeys.Length)
            {
                return null;
            }
            int[] UIindexes = new int[indexKeys.Length];
            for (int i = 0; i < indexKeys.Length; i++)
            {
                UIindexes[i] = Config.getIndex(UIkeys, indexKeys[i]);
                if (UIindexes[i] < 0)
                {
                    return null;
                }
                else
                {
                    UIindexes[i]++;
                }
            }
            bool result;
            for (int i = 0; i < lv.Items.Count; i++)
            {
                result = true;
                for (int t = 0; t < UIindexes.Length; t++)
                {
                    if (!lv.Items[i].SubItems[UIindexes[t]].Text.Equals(indexValues[t]))
                    {
                        result = false;
                        break;
                    }
                }
                if (result)
                {
                    return lv.Items[i];
                }
            }
            return null;
        }

        public static ListViewItem findListviewItem(List<ListViewItem> items, string[] UIkeys, string[] indexKeys, string[] indexValues)
        {
            if (indexKeys.Length != indexValues.Length)
            {
                return null;
            }
            if (UIkeys.Length < indexKeys.Length)
            {
                return null;
            }
            int[] UIindexes = new int[indexKeys.Length];
            for (int i = 0; i < indexKeys.Length; i++)
            {
                UIindexes[i] = Config.getIndex(UIkeys, indexKeys[i]);
                if (UIindexes[i] < 0)
                {
                    return null;
                }
                else
                {
                    UIindexes[i]++;
                }
            }
            bool result;
            for (int i = 0; i < items.Count; i++)
            {
                result = true;
                for (int t = 0; t < UIindexes.Length; t++)
                {
                    if (!items[i].SubItems[UIindexes[t]].Text.Equals(indexValues[t]))
                    {
                        result = false;
                        break;
                    }
                }
                if (result)
                {
                    return items[i];
                }
            }
            return null;
        }

        public static ListViewItem findListviewItem(List<ListViewItem> items, string[] UIkeys, string[] indexKeys, message msg)
        {
            for (int i = 0; i < indexKeys.Length; i++)
            {
                if (!msg.isContainsKey(indexKeys[i]))
                {
                    return null;
                }
            }
            if (UIkeys.Length < indexKeys.Length)
            {
                return null;
            }
            int[] UIindexes = new int[indexKeys.Length];
            for (int i = 0; i < indexKeys.Length; i++)
            {
                UIindexes[i] = Config.getIndex(UIkeys, indexKeys[i]);
                if (UIindexes[i] < 0)
                {
                    return null;
                }
                else
                {
                    UIindexes[i]++;
                }
            }
            string[] indexValues = msg.getValuesFromKeys(indexKeys);
            bool result;
            for (int i = 0; i < items.Count; i++)
            {
                result = true;
                for (int t = 0; t < UIindexes.Length; t++)
                {
                    if (!items[i].SubItems[UIindexes[t]].Text.Equals(indexValues[t]))
                    {
                        result = false;
                        break;
                    }
                }
                if (result)
                {
                    return items[i];
                }
            }
            return null;
        }

        public static bool removeOneListviewItem(ListView lv, ListViewItem lvi)
        {
            if (lvi != null)
            {
                lv.BeginUpdate();
                lv.Items.Remove(lvi);
                lv.EndUpdate();
                return true;
            }
            return false;
        }

        public static bool removeOneListviewItem(List<ListViewItem > items, string[] UIkeys, string[] indexKeys, string[] indexValues)
        {
            ListViewItem lvi = findListviewItem(items, UIkeys, indexKeys, indexValues);
            if (lvi != null)
            {
                items.Remove(lvi);
                return true;
            }
            return false;
        }

        public static bool removeOneListviewItem(ListView lv, string[] UIkeys, string[] indexKeys, string[] indexValues)
        {
            return removeOneListviewItem(lv, findListviewItem(lv, UIkeys, indexKeys, indexValues));
        }

        public static bool removeOneListviewItem(List<ListViewItem> items, string[] UIkeys, string[] indexKeys, message msg)
        {
            ListViewItem lvi = findListviewItem(items, UIkeys, indexKeys, msg);
            if (lvi != null)
            {
                items.Remove(lvi);
                return true;
            }
            return false;
        }

        public static bool removeOneListviewItem(ListView lv, string[] UIkeys, string[] indexKeys, message msg)
        {
            return removeOneListviewItem(lv, findListviewItem(lv, UIkeys, indexKeys, msg));
        }

        public static void insertOneListviewItem(ListView lv, int index, ListViewItem lvi)
        {
            lvi.BackColor = LV_OP.Color_NewItem;
            lv.BeginUpdate();
            if (index >= 0 && index<=lv.Items.Count)
            {
                lv.Items.Insert(index, lvi);
            }
            lv.EndUpdate();
        }

        public static void replaceOneListviewItem(ListView lv, string[] UIkeys, string[] indexKeys, message msg)
        {
            replaceOneListviewItem(lv, findListviewItem(lv, UIkeys, indexKeys, msg), getLVI(msg, UIkeys));
        }

        public static void replaceOneListviewItem(ListView lv, ListViewItem lvi_beReplaced, ListViewItem lvi_toReplace)
        {
            lvi_toReplace.BackColor = LV_OP.Color_EditItem;
            lv.BeginUpdate();
            if (lv.Items.Contains(lvi_beReplaced))
            {
                lv.Items.Remove(lvi_beReplaced);
            }
            if (lvi_toReplace != null)
            {
                lv.Items.Insert(0, lvi_toReplace);
            }
            lv.EndUpdate();
        }

        public static ListViewItem cloneLVI(ListViewItem lvi)
        {
            ListViewItem temp = (ListViewItem)lvi.Clone();
            return temp;
        }

        public static List<ListViewItem> realtimeProductHint(object cmd)
        {
            return SQL.genListLvi(cmd.ToString(), Config.UI_ProductHintsKeys, SQL.dbConnect());
        }

        public static void changeLviBackColor(ListViewItem lvi, string[] keys)
        {
            if (keys != null)
            {
                int index = Config.getIndex(keys, "13");
                if (index >= 0)
                {
                    try
                    {
                        lvi.BackColor = LV_OP.PriorityColorDetail[lvi.SubItems[index + 1].Text];
                    }
                    catch
                    {
                        lvi.BackColor = LV_OP.Color_Missing;
                    }
                }
                else
                {
                    lvi.BackColor = LV_OP.Color_Missing;
                }
            }
            else
            {
                lvi.BackColor = LV_OP.Color_Missing;
            }
        }

        public static void initialClientLevelColor(string clientLevelStr)
        {
            string[] levels = Regex.Split(clientLevelStr, ",");
            ClientLevelColorDetail.Clear();
            for (int i = 0; i < levels.Length; i++)
            {
                string s = Config.Customization_INI.readValue("ClientLevelColor", levels[i]).Trim(' ');
                if (s != "")
                {
                    ClientLevelColorDetail.Add(levels[i], utility.getColor(s));
                }
                else
                {
                    Config.Customization_INI.writeValue("ClientLevelColor", levels[i], "Color [Black]");
                    ClientLevelColorDetail.Add(levels[i], Color.Black);
                }
            }
            ClientLevelColorDetail.Add("", Color.Black);
        }

        public static void initialPriorityColor(string prioritieStr)
        {
            string[] priorities = Regex.Split(prioritieStr, ",");
            PriorityColorDetail.Clear();
            for (int i = 0; i < priorities.Length; i++)
            {
                string s = Config.Customization_INI.readValue("PriorityColor", priorities[i]).Trim(' ');
                if (s != "")
                {
                    PriorityColorDetail.Add(priorities[i], utility.getColor(s));
                }
                else
                {
                    Config.Customization_INI.writeValue("PriorityColor", priorities[i], "Color [A=255, R=232, G=244, B=232]");
                    PriorityColorDetail.Add(priorities[i], LV_OP.Color_Missing);
                }
            }
            PriorityColorDetail.Add("", LV_OP.Color_Missing);
        }

        public static ListViewItem getLVI(message msg,string[] keys)
        {
            ListViewItem lvi = new ListViewItem();
            for (int i = 0; i < keys.Length; i++)
            {
                lvi.SubItems.Add(msg.getValueFromPairs(keys[i]));
            }
            if (keys.Contains("4"))
            {
                foreach (string level in MainForm.ClientLevelDetail.Keys)
                {
                    if (MainForm.ClientLevelDetail[level].Contains(msg.getValueFromPairs("4")))
                    {
                        lvi.ForeColor = ClientLevelColorDetail[level];
                        break;
                    }
                }
            }
            if (keys.Contains("13"))
            {
                try
                {
                    lvi.BackColor = PriorityColorDetail[msg.getValueFromPairs("13")];
                }
                catch
                {
                    lvi.BackColor = LV_OP.Color_Missing;
                }
            }
            else
            {
                lvi.BackColor = LV_OP.Color_Missing;
            }
            return lvi;
        }

        public static ListViewItem[] getLVIs(Msg_OP msg_Op,string[] keys)
        {
            msg_Op.buildMsgs();
            message[] msgs =msg_Op.getMsgs();
            ListViewItem[] lvis = new ListViewItem[msgs.Length];
            for (int i = 0; i < lvis.Length; i++)
            {
                lvis[i] = getLVI(msgs[i],keys);
            }
            return lvis;
        }

        public static bool IsNewOrEdited(ListViewItem lvi)
        {
            if (lvi.BackColor == LV_OP.Color_EditItem || lvi.BackColor == LV_OP.Color_NewItem)
            {
                return true;
            }
            return false;
        }

        public static string getColumnValue(ListViewItem lvi, string[] UI_keys, string key)
        {
            if (UI_keys != null)
            {
                if (lvi.SubItems.Count != UI_keys.Length + 1)
                {
                    return "";
                }
                else
                {
                    int index = Config.getIndex(UI_keys, key);
                    if (index >= 0)
                    {
                        return lvi.SubItems[index + 1].Text;
                    }
                }
            }
            return "";
        }

        public static List<string> getColumnValues(List<ListViewItem> lvis, string[] UI_keys, string key)
        {
            List<string> values = new List<string>();
            if (lvis != null && UI_keys != null)
            {
                if (lvis[0].SubItems.Count == UI_keys.Length + 1)
                {
                    int index = Config.getIndex(UI_keys, key);
                    if (index >= 0)
                    {
                        index++;
                        for (int t = 0; t < lvis.Count; t++)
                        {
                            if (!lvis[t].SubItems[index].Text.Equals(""))
                            {
                                values.Add(lvis[t].SubItems[index].Text);
                            }
                        }
                    }
                }
            }
            return values;
        }

        public static string getColumnValues(ListView lv, string[] UI_keys, string key)
        {
            StringBuilder value = new StringBuilder("");
            if (UI_keys != null && lv.Items.Count>0)
            {
                if (lv.Items[0].SubItems.Count == UI_keys.Length + 1)
                {
                    int index = Config.getIndex(UI_keys, key);
                    if (index >= 0)
                    {
                        index++;
                        value.Append(lv.Items[0].SubItems[index].Text);
                        for (int t = 1; t < lv.Items.Count; t++)
                        {
                            if (!lv.Items[t].SubItems[index].Text.Trim().Equals(""))
                            {
                                value.Append(",").Append(lv.Items[t].SubItems[index].Text);
                            }
                        }
                    }
                }
            }
            return value.ToString();
        }

        public static void clearListView(ListView l)
        {
            l.BeginUpdate();
            l.Items.Clear();
            l.EndUpdate();
        }

        public static int getLviIndex(ListView lv,string[] keys, string key, string comparer)
        {
            int index = Config.getIndex(keys, key);
            if (index >= 0)
            {
                index++;
                for (int i = 0; i < lv.Items.Count; i++)
                {
                    if (lv.Items[i].SubItems[index].Text.Equals(comparer))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static int getLviIndex(List<ListViewItem> lvis, string[] keys, string key, string comparer)
        {
            int index = Config.getIndex(keys, key);
            if (index >= 0)
            {
                index++;
                for (int i = 0; i < lvis.Count; i++)
                {
                    if (lvis[i].SubItems[index].Text.Equals(comparer))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public static void changeLVIBackColor(ListViewItem lvi, string[] keys)
        {
            if (IsNewOrEdited(lvi))
            {
                changeLviBackColor(lvi, keys);
            }
        }

    }
}
