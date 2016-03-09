// 筛选case 窗口
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace SupportLogSheet
{
    public partial class MultiFilter : Form
    {
        private Dictionary<string, List<ListViewItem>> CIMList;
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> ClientServerProductVersion;

        public MultiFilter(
            Dictionary<string, string> CaseProperty, 
            Dictionary<string, List<ListViewItem>> CIMList,
            List<string> ClientList,
            List<string> UserList, 
            Dictionary<string, string> ProductCate_Pair,
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> ClientServerProductVersion)
        {
            InitializeComponent();
            dateTimePicker_From.Value = DateTime.Now.AddMonths(-1);
            dateTimePicker_To.Value = DateTime.Now;
            Combo_OP.initialComboBox(comboBox_Client, ClientList);
            Combo_OP.initialComboBox(comboBox_Status, CaseProperty["Status"].ToString(), ",");
            Combo_OP.initialComboBox(comboBox_Priority, CaseProperty["Priority"].ToString(), ",");
            Combo_OP.initialComboBox(comboBox_CaseType, CaseProperty["Type"].ToString(), ",");
            Combo_OP.initialComboBox(comboBox_Through, CaseProperty["Through"].ToString(), ",");
            Combo_OP.initialComboBox(comboBox_Incharge, UserList);
            Combo_OP.initialComboBox(comboBox_IssueType, CaseProperty["Issue"].ToString(), ",");
            Combo_OP.initialComboBox(comboBox_Product, ProductCate_Pair.Keys.ToArray());
            this.CIMList = CIMList;
            this.ClientServerProductVersion = ClientServerProductVersion;
            utility.setFont(this, Config.Font_Content);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            message msg = new message();
            msg.setKeyValuePair("4", makeCondition(comboBox_Client,checkBox_Client,"4"));
            msg.setKeyValuePair("5", makeCondition(comboBox_CallPerson,checkBox_CallPerson,"5"));
            msg.setKeyValuePair("6", makeCondition(comboBox_ContactNo, checkBox_ContactNo, "6"));
            msg.setKeyValuePair("7", makeCondition(comboBox_Through, checkBox_Through, "7"));
            msg.setKeyValuePair("8", makeCondition(richTextBox_Description, checkBox_Description, "8"));
            msg.setKeyValuePair("9", makeCondition(comboBox_CaseType, checkBox_CaseType, "9"));
            msg.setKeyValuePair("10", makeCondition(comboBox_Incharge, checkBox_Incharge, "10"));
            msg.setKeyValuePair("11", makeCondition(comboBox_Status, checkBox_Status, "11"));
            msg.setKeyValuePair("12", makeCondition(richTextBox_Solution, checkBox_solution, "12"));
            msg.setKeyValuePair("13", makeCondition(comboBox_Priority, checkBox_Priority, "13"));
            msg.setKeyValuePair("14", makeCondition(comboBox_IssueType, checkBox_IssueType, "14"));
            msg.setKeyValuePair("18", makeCondition(dateTimePicker_From,dateTimePicker_To,checkBox_allTime,"2"));
            msg.setKeyValuePair("160", makeCondition(comboBox_Server, checkBox_Server, "160"));
            msg.setKeyValuePair("167", makeCondition(comboBox_Product, checkBox_Product, "167"));
            msg.setKeyValuePair("169", makeCondition(textBox_Version, checkBox_Version, "169"));
            Msg_OP msg_Op = new Msg_OP("C5", new message[] { msg });
            Config.SLS_Sock.inMsgQueue_add(msg_Op);
            this.Dispose();
        }

        private string makeCondition(ComboBox combo, CheckBox notEqual,string dbColumnKey)
        {
            string content = combo.Text.Trim();
            if (content.Equals(""))
            {
                return "";
            }
            if (notEqual == null)
            {
                return new StringBuilder(Config.getValue(dbColumnKey)).Append(" = '").Append(content).Append("'").ToString();
            }
            if (notEqual.Checked)
            {
                return new StringBuilder(Config.getValue(dbColumnKey)).Append(" != '").Append(content).Append("'").ToString();
            }
            else
            {
                return new StringBuilder(Config.getValue(dbColumnKey)).Append(" = '").Append(content).Append("'").ToString();
            }
        }

        private string makeCondition(TextBox tb, CheckBox notEqual, string dbColumnKey)
        {
            string content = tb.Text.Trim();
            if (content.Equals(""))
            {
                return "";
            }
            if (notEqual == null)
            {
                return new StringBuilder(Config.getValue(dbColumnKey)).Append(" = '").Append(content).Append("'").ToString();
            }
            if (notEqual.Checked)
            {
                return new StringBuilder(Config.getValue(dbColumnKey)).Append(" != '").Append(content).Append("'").ToString();
            }
            else
            {
                return new StringBuilder(Config.getValue(dbColumnKey)).Append(" = '").Append(content).Append("'").ToString();
            }
        }

        private string makeCondition(RichTextBox rtb, CheckBox notLike, string dbColumnKey)
        {
            string content = rtb.Text.Trim();
            if (content.Equals(""))
            {
                return "";
            }
            if (notLike == null)
            {
                return new StringBuilder(Config.getValue(dbColumnKey)).Append(" like '%").Append(content).Append("%'").ToString();
            }
            if (notLike.Checked)
            {
                return new StringBuilder(Config.getValue(dbColumnKey)).Append(" not like '%").Append(content).Append("%'").ToString();
            }
            else
            {
                return new StringBuilder(Config.getValue(dbColumnKey)).Append(" like '%").Append(content).Append("%'").ToString();
            }
        }

        private string makeCondition(DateTimePicker from, DateTimePicker to, CheckBox isAllTime, string TimeKey)
        {
            if (isAllTime.Checked)
            {
                return "";
            }
            else
            {
                string DBColumnName = Config.getValue(TimeKey);
                StringBuilder sb = new StringBuilder(DBColumnName);
                sb.Append(" >= '").Append(from.Value.ToString("yyyy-MM-dd HH:mm:ss")).Append("' and ").Append(DBColumnName).Append(" <= '").Append(to.Value.ToString("yyyy-MM-dd HH:mm:ss")).Append("'");
                return sb.ToString();
            }
        }

        private void comboBox7_TextChanged(object sender, EventArgs e)
        {
            utility.loadContactPhone(comboBox_Client, comboBox_CallPerson, comboBox_ContactNo,CIMList);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string client = comboBox_Client.Text.Trim(' ');
                utility.loadClientContact(comboBox_Client, comboBox_CallPerson, CIMList);
                if (ClientServerProductVersion.ContainsKey(client))
                {
                    comboBox_Server.Items.Clear();
                    comboBox_Server.Items.AddRange(ClientServerProductVersion[client].Keys.ToArray());
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            foreach (Control box in this.Controls)
            {
                if (box is ComboBox || box is TextBox || box is RichTextBox)
                {
                    box.Text = "";
                }
                if (box is GroupBox)
                {
                    foreach (Control subbox in box.Controls)
                    {
                        if (subbox is ComboBox || subbox is TextBox || subbox is RichTextBox)
                        {
                            subbox.Text = "";
                        }
                    }
                    foreach (CheckBox cb in box.Controls)
                    {
                        cb.Checked = false;
                    }
                }
            }
        }

        private void comboBox10_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string client = comboBox_Client.Text.Trim(' ');
                if (client != "")
                {
                    if (ClientServerProductVersion.ContainsKey(client))
                    {
                        string server = comboBox_Server.Text.Trim(' ');
                        if (ClientServerProductVersion[client].ContainsKey(server))
                        {
                            string product = comboBox_Product.Text.Trim(' ');
                            if (ClientServerProductVersion[client][server].ContainsKey(product))
                            {
                                textBox_Version.Text = ClientServerProductVersion[client][server][product];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void comboBox11_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string client = comboBox_Client.Text.Trim(' ');
                if (client != "")
                {
                    if (ClientServerProductVersion.ContainsKey(client))
                    {
                        string server = comboBox_Server.Text.Trim(' ');
                        if (ClientServerProductVersion[client].ContainsKey(server))
                        {
                            comboBox_Product.Items.Clear();
                            comboBox_Product.Items.AddRange(ClientServerProductVersion[client][server].Keys.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }
    }
}
