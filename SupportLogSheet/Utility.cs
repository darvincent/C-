// 工具类
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Windows.Forms;
using System.Threading;
using System.Text.RegularExpressions;
using System.IO;
using System.Collections;
using System.Runtime.InteropServices;
using System.Net.Sockets;
using System.Net;
using System.Drawing;
using Microsoft.Office.Interop.Excel;
using System.Reflection;

namespace SupportLogSheet
{
    public class utility
    {       
        public delegate void Del_NoArgs();
        public delegate void Del_switchTab(TabControl tbc, int index);
        public delegate void Del_setText(Control obj, string text);

        public static void loadClientContact(ComboBox client, ComboBox contact, Dictionary<string, List<ListViewItem>> CIMList)
        {
            string s = client.Text.Trim(' ');
            contact.BeginUpdate();
            contact.Text = "";
            contact.Items.Clear();
            if (CIMList.ContainsKey(s))
            {
                contact.Items.AddRange(LV_OP.getColumnValues(CIMList[s], Config.UI_ClientContactKeys, "111").ToArray());
            }
            contact.EndUpdate();
        }

        public static void loadContactPhone(ComboBox clientCombo, ComboBox contactCombo, ComboBox phoneCombo, Dictionary<string, List<ListViewItem>> CIMList)
        {
            string clientStr = clientCombo.Text.Trim(' ');
            string contactStr = contactCombo.Text.Trim(' ');
            phoneCombo.BeginUpdate();
            phoneCombo.Text = "";
            phoneCombo.Items.Clear();
            int contactIndex = Config.getIndex(Config.UI_ClientContactKeys, "111") + 1;
            int phone1Index = Config.getIndex(Config.UI_ClientContactKeys, "113") + 1;
            int phone2Index = Config.getIndex(Config.UI_ClientContactKeys, "114") + 1;
            if (CIMList.ContainsKey(clientStr))
            {
                for (int i = 0; i < CIMList[clientStr].Count; i++)
                {
                    if (contactIndex != 0 && CIMList[clientStr][i].SubItems[contactIndex].Text.Equals(contactStr))
                    {
                        if (phone1Index != 0 && !CIMList[clientStr][i].SubItems[phone1Index].Text.Equals(""))
                        {
                            phoneCombo.Items.Add(CIMList[clientStr][i].SubItems[phone1Index].Text);
                        }
                        if (phone2Index != 0 && !CIMList[clientStr][i].SubItems[phone2Index].Text.Equals(""))
                        {
                            phoneCombo.Items.Add(CIMList[clientStr][i].SubItems[phone2Index].Text);
                        }
                        break;
                    }
                }
            }
            phoneCombo.EndUpdate();
        }

        public static bool comPareVersion(string fromVersion, string toVersion)
        {
            string[] temp1 = fromVersion.Split('.');
            string[] temp2 = toVersion.Split('.');
            int sum1 = 1000 * Int32.Parse(temp1[0]) + 100 * Int32.Parse(temp1[1]) + 10 * Int32.Parse(temp1[2]) + Int32.Parse(temp1[3]);
            int sum2 = 1000 * Int32.Parse(temp2[0]) + 100 * Int32.Parse(temp2[1]) + 10 * Int32.Parse(temp2[2]) + Int32.Parse(temp2[3]);
            return sum1 < sum2;
        }

        public static bool isCorrectVersionFormat(string product,string version)
        {
            product = product.Trim(' ');
            if (product == "OSKey" || product == "Windows OS")
            {
                return true;
            }
            version = version.Trim(' ');
            if (version == "")
            {
                return true;
            }
            string[] numbers = version.Split('.');
            if (numbers.Length != 4)
            {
                return false;
            }
            for (int i = 0; i < 4; i++)
            {
                try
                {
                    int var1 = Convert.ToInt32(numbers[i]);
                }
                catch
                {
                    return false;
                }
            }
            return true;
        }

        public static void focusToTabpage(TabControl tbc, int index)
        {
            tbc.SelectedIndex = index;
        }

        public static void setText(Control obj, string text)
        {
            obj.Text = text;
        }

        string toHexString(byte[] bytes)
        {
            var hex = new StringBuilder();
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }
  
        public static int checkLogin(string userName, string password)
        {
            try
            {
                SqlConnection mySqlConnection = SQL.dbConnect();
                string sqlcmd = "select * from UserFile where UserName = '" + userName.Trim(' ') + "'";
                SqlCommand cmd = new SqlCommand(sqlcmd, mySqlConnection);
                using (SqlDataReader re = cmd.ExecuteReader())
                {
                    if (re.HasRows == false)
                    {
                        return -1; //user not exist
                    }
                    string username, passwordHash, salt, active;
                    while (re.Read())
                    {
                        username = re.GetValue(re.GetOrdinal(Config.getValue("80"))).ToString().Trim(' ');
                        salt = re.GetValue(re.GetOrdinal(Config.getValue("84"))).ToString().Trim(' ');
                        passwordHash = re.GetValue(re.GetOrdinal(Config.getValue("83"))).ToString().Trim(' ');
                        active = re.GetValue(re.GetOrdinal(Config.getValue("88"))).ToString().Trim(' ');
                        LoginForm.LoginUser = username;
                        LoginForm.IsSupport = re.GetValue(re.GetOrdinal(Config.getValue("90"))).ToString().Trim(' ') == "N" ? false : true;
                        byte[] passwordAndSaltBytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
                        byte[] hashBytes = new System.Security.Cryptography.SHA256Managed().ComputeHash(passwordAndSaltBytes);
                        string hashString = Convert.ToBase64String(hashBytes);
                        if (hashString == passwordHash)
                        {
                            if (active == "N")
                            {
                                return -2; // inactive user
                            }
                            else
                            {
                                LoginForm.MyID = re.GetValue(re.GetOrdinal(Config.getValue("87"))).ToString().Trim(' ');
                                if (LoginForm.MyID == "")
                                {
                                    LoginForm.MyID = "-1";
                                }
                                return 1; // login successfully
                            }
                        }
                        else
                        {
                            return -3; // wrong password
                        }
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
                return 0;
            }
        }
        //to avoid the ' bring error when insert DB
        public static string modifySocketMsg(string s)
        {
            string pattern = @"'";
            string substitution = "''";
            if (Regex.IsMatch(s, pattern))
            {
                s = Regex.Replace(s, pattern, substitution);
            }
            return s;
        }

        public static string getAMInfo(string client, Dictionary<string, string> ClientAM, Dictionary<string, string> AMPhone)
        {
            if (ClientAM.ContainsKey(client))
            {
                if (!ClientAM[client].Equals(""))
                {
                    string AM = ClientAM[client];
                    if (AMPhone.ContainsKey(AM) && !AMPhone[AM].Equals( ""))
                    {
                        return new StringBuilder(AM).Append("(").Append(AMPhone[AM]).Append(")").ToString();
                    }
                    else
                    {
                        return AM;
                    }
                }
            }
            return "";
        }

        public static int minusToUnNegative(ref int i)
        {
            return i > 0 ? Interlocked.Decrement(ref i) : i;
        }

        public static string getPositiveNo(int i)
        {
            return i > 0 ? "+" + i.ToString() : "";
        }

        public static Color getColor(string s) 
        {
            //Color [A=255, R=0, G=128, B=192]
            try
            {
                if (s == "")
                {
                    return LV_OP.Color_Missing;
                }
                string pattern = @"Color \[";
                string pattern1 = @"\]";
                string substitution = "";
                int[] rgb = new int[3];
                Color color = new Color();
                if (Regex.IsMatch(s, pattern))
                {
                    s = Regex.Replace(s, pattern, substitution);
                }
                if (Regex.IsMatch(s, pattern1))
                {
                    s = Regex.Replace(s, pattern1, substitution);
                }
                string[] contents = Regex.Split(s, ",");
                if (contents.Length == 1)
                {
                    color = Color.FromName(contents[0]);
                    return color;
                }
                for (int i = 1; i < contents.Length; i++)
                {
                    string[] temp = Regex.Split(contents[i].Trim(' '), "=");
                    rgb[i - 1] = Int32.Parse(temp[1]);
                }
                color = Color.FromArgb(rgb[0], rgb[1], rgb[2]);
                return color;
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
                return LV_OP.Color_Missing;
            }
        }

        public static void exportToExcel(ListView  lv,string fileName)
        {
            try
            {
                if (lv.Items.Count > 0)
                {
                    using (SaveFileDialog dialog = new SaveFileDialog())
                    {
                        dialog.Filter = "Excel(*.xlsx)|*.xlsx|Excel2003(*.xls)|*.xls|All Files(*.*)|*.*";
                        dialog.FileName = fileName + ".xlsx";
                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            Microsoft.Office.Interop.Excel.Application xlApp = new Microsoft.Office.Interop.Excel.Application();
                            object missing = System.Reflection.Missing.Value;
                            if (xlApp == null)
                            {
                                MessageBox.Show("You have to install Excel first !");
                                return;
                            }
                            try
                            {
                                Microsoft.Office.Interop.Excel.Workbooks xlBooks = xlApp.Workbooks;
                                Microsoft.Office.Interop.Excel.Workbook xlBook = xlBooks.Add(Microsoft.Office.Interop.Excel.XlWBATemplate.xlWBATWorksheet);
                                Microsoft.Office.Interop.Excel.Worksheet xlSheet = (Microsoft.Office.Interop.Excel.Worksheet)xlBook.Worksheets[1];
                                Microsoft.Office.Interop.Excel.Range range = null;

                                // header
                                range = xlSheet.Range[xlSheet.Cells[1, 1], xlSheet.Cells[1, lv.Items[0].SubItems.Count - 1]];
                                range.Borders[XlBordersIndex.xlEdgeLeft].Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThick;
                                range.Borders[XlBordersIndex.xlEdgeRight].Weight = Microsoft.Office.Interop.Excel.XlBorderWeight.xlThick;
                                range.Borders.LineStyle = 1;
                                range.RowHeight = 25;
                                range.ColumnWidth = 20;
                                range.Interior.Color = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));

                                for (int i = 1; i < lv.Items[0].SubItems.Count; i++)
                                {
                                    xlSheet.Cells[1, i] = lv.Columns[i].Text;
                                }
                                // data part
                                for (int i = 0; i < lv.Items.Count; i++)
                                {
                                    for (int t = 1; t <= lv.Items[0].SubItems.Count - 1; t++)
                                    {
                                        xlSheet.Cells[2 + i, t] = lv.Items[i].SubItems[t].Text;
                                    }
                                }

                                range = xlSheet.Range[xlSheet.Cells[1, 1], xlSheet.Cells[1 + lv.Items.Count, lv.Items[0].SubItems.Count - 1]];
                                range.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignLeft;
                                range.Borders.LineStyle = 1;
                                range.Font.Size = 10;
                                range.NumberFormatLocal = "@";
                                range.WrapText = true;
                                range.Columns.AutoFit();

                                range = xlSheet.Range[xlSheet.Cells[2, 2], xlSheet.Cells[1 + lv.Items.Count, 3]];
                                range.NumberFormatLocal = "yyyy-mm-dd hh:mm";

                                if (xlSheet != null)
                                {
                                    xlSheet.SaveAs(dialog.FileName, missing, missing, missing, missing, missing, missing, missing, missing, missing);
                                }
                                xlApp.Quit();
                            }
                            catch (Exception ex)
                            {
                                Config.logWriter.writeErrorLog(ex);
                                xlApp.Quit();
                            }
                        }
                    }
                }
                else
                {
                    MessageBox.Show("No content !");
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        public static int returnSeconds(DateTime start, DateTime end)
        {
            try
            {
                TimeSpan t = end - start;
                return Convert.ToInt32(t.TotalSeconds);
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
                return 1;
            }
        }

        public static string getSubStr(string s, string splitSymbol, int start, int end)
        {
            string[] items = Regex.Split(s, splitSymbol);
            StringBuilder sb = new StringBuilder(items[start]);
            for (int i = start + 1; i <= end; i++)
            {
                sb.Append(splitSymbol).Append(items[i]);
            }
            return sb.ToString();
        }

        public static string updateSTR_ReplaceSubStr(string str, string splitSymbol,string pattern, string substitution)
        {
            try
            {
                string[] contents = Regex.Split(str, splitSymbol);
                StringBuilder sb = new StringBuilder();
                int mark = 0;
                if (contents[0] == pattern)
                {
                    sb.Append(substitution);
                }
                else
                {
                    sb.Append(contents[0]);
                }
                for (int i = 1; i < contents.Length; i++)
                {
                    if (contents[i] == pattern)
                    {
                        if (substitution != "")
                        {
                            sb.Append(splitSymbol).Append(substitution);
                        }
                        mark = i + 1;
                        break;
                    }
                    else
                    {
                        if (sb.Length>0)
                        {
                            sb.Append(splitSymbol).Append(contents[i]);
                        }
                        else
                        {
                            sb.Append(contents[i]);
                        }
                    }
                }
                if (mark != 0)
                {
                    for (int i = mark; i < contents.Length; i++)
                    {
                        sb.Append(splitSymbol).Append(contents[i]);
                    }
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
                return str;
            }
        }

        public static string updateSTR_AppendSubStr(string str, string splitSymbol, string subStr)
        {
            if (subStr != "")
            {
                if (str == "")
                {
                    str = subStr;
                }
                else
                {
                    str += splitSymbol + subStr;
                }
            }
            return str;
        }

        public static bool isSetRTBFont(RichTextBox box)
        {
            if (box.Multiline == true)
            {
                return true;
            }
            return false;
        }

        public static bool isSetTBFont(System.Windows.Forms.TextBox box)
        {
            if (box.Multiline == true)
            {
                return true;
            }
            return false;
        }

        public static void setFont(Control root, System.Drawing.Font font)
        {
            if (root.HasChildren)
            {
                foreach (Control item in root.Controls)
                {
                    if (item is ListView || item is DataGridView)
                    {
                        item.Font = font;
                    }
                    if (item is RichTextBox && isSetRTBFont(item as RichTextBox))
                    {
                        item.Font = font;
                    }
                    if (item is System.Windows.Forms.TextBox && isSetTBFont(item as System.Windows.Forms.TextBox))
                    {
                        item.Font = font;
                    }
                    setFont(item, font);
                }
            }
        }

        public static void clearComboBox(System.Windows.Forms.ComboBox box)
        {
            if (box.DropDownStyle == ComboBoxStyle.DropDownList)
            {
                box.SelectedIndex = -1;
            }
            else
            {
                box.Text = "";
            }
        }

        public static void clearDatagridView(DataGridView dgv)
        {
            dgv.Rows.Clear();
        }

        public static void clearContent(Control root)
        {
            foreach(Control box in root.Controls)
            {
                if (box is System.Windows.Forms.GroupBox)
                {
                    clearContent(box);
                }

                if (box is System.Windows.Forms.ComboBox)
                {
                    clearComboBox(box as System.Windows.Forms.ComboBox);
                }

                if (box is System.Windows.Forms.TextBox || box is System.Windows.Forms.RichTextBox)
                {
                    box.Text = "";
                }

                if (box is DataGridView)
                {
                    clearDatagridView(box as DataGridView);
                }
            }
        }

        public static Dictionary<string, string> initial_Products()
        {
            Dictionary<string, string> ProductCate_Pair = new Dictionary<string, string>();
            try
            {
                string cmd = "select * from Product";
                using (SqlConnection sqlConnection = SQL.dbConnect())
                {
                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                    {
                        string categoryDBName = Config.getValue("166");
                        string productDBName = Config.getValue("167");
                        using (SqlDataReader re = sqlcmd.ExecuteReader())
                        {
                            while (re.Read())
                            {
                                ProductCate_Pair.Add(re.GetValue(re.GetOrdinal(productDBName)).ToString().Trim(' '), re.GetValue(re.GetOrdinal(categoryDBName)).ToString().Trim(' '));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
            return ProductCate_Pair;
        }

        public static Dictionary<string, Dictionary<string, Dictionary<string, string>>>  initial_ProductMaster()
        {
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> ClientServerProductVersion = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
            try
            {
                string cmd = "select * from ProductMaster";
                using (SqlConnection sqlconnection = SQL.dbConnect())
                {
                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlconnection))
                    {
                        ClientServerProductVersion.Clear();
                        using (SqlDataReader re = sqlcmd.ExecuteReader())
                        {
                            while (re.Read())
                            {
                                string client = re.GetValue(re.GetOrdinal(Config.getValue("4"))).ToString().Trim(' ');
                                string server = re.GetValue(re.GetOrdinal(Config.getValue("160"))).ToString().Trim(' ');
                                string product = re.GetValue(re.GetOrdinal(Config.getValue("167"))).ToString().Trim(' ');
                                string version = re.GetValue(re.GetOrdinal(Config.getValue("169"))).ToString().Trim(' ');
                                if (ClientServerProductVersion.ContainsKey(client))
                                {
                                    if (ClientServerProductVersion[client].ContainsKey(server))
                                    {
                                        ClientServerProductVersion[client][server].Add(product, version);
                                    }
                                    else
                                    {
                                        Dictionary<string, string> productVersion = new Dictionary<string, string>();
                                        productVersion.Add(product, version);
                                        ClientServerProductVersion[client].Add(server, productVersion);
                                    }
                                }
                                else
                                {
                                    Dictionary<string, string> productVersion = new Dictionary<string, string>();
                                    productVersion.Add(product, version);
                                    Dictionary<string, Dictionary<string, string>> serverProductVersion = new Dictionary<string, Dictionary<string, string>>();
                                    serverProductVersion.Add(server, productVersion);
                                    ClientServerProductVersion.Add(client, serverProductVersion);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
            return ClientServerProductVersion;
        }
    }
}
