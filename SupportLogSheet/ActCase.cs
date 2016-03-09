// 用于增 删 改 case的
// case 嘛， 就是特殊公司支持部门收到客户的问题而创建的， 名曰 case。。

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Threading;

namespace SupportLogSheet
{

    public partial class ActCase : Form
    {
        private string ActionType;
        private message msg = new message();
        private int NewCaseFormCounter;
        private bool BOOL_ManualClose = true;
        private List<ListViewItem> ProductHints_Result;
        private int editCaseEventCount = 0;
        private ProductHints Form_ProductHints;
        private Dictionary<string, List<ListViewItem>> CIMList;
        private Dictionary<string, string> ClientAM;
        private Dictionary<string, string> AMPhone;
        private Dictionary<string, string> ProductCate_Pair;
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> ClientServerProductVersion;
        private MyInfo myInfo;
        public ActCase(
            MyInfo myInfo,
            int NewCaseFormCounter,
            Dictionary<string, string> CaseProperty,
            List<string> ClientList,
            List<string> UserList, 
            Dictionary<string, string> ProductCate_Pair,
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> ClientServerProductVersion,
            Dictionary<string, List<ListViewItem>> CIMList, 
            Dictionary<string, string> ClientAM, 
            Dictionary<string, string> AMPhone)
        {
            ActionType = "C1";
            InitializeComponent();
            this.myInfo = myInfo;
            this.NewCaseFormCounter = NewCaseFormCounter;
            this.Text = "New Case";
            button2.Text = "Clear";
            button2.Width = 55;
            button1.Text = "Add";
            linkLabel1.Visible = false;
            this.CIMList = CIMList;
            this.ClientAM = ClientAM;
            this.AMPhone = AMPhone;
            this.ClientServerProductVersion = ClientServerProductVersion;
            this.ProductCate_Pair = ProductCate_Pair;
            initial(CaseProperty,ClientList,UserList,ProductCate_Pair);
        }
        public ActCase(
            MyInfo myinfo,
            ListViewItem lvi,
            Dictionary<string, string> CaseProperty,
            List<string> ClientList,
            List<string> UserList, 
            Dictionary<string, string> ProductCate_Pair,
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> ClientServerProductVersion,
            Dictionary<string, List<ListViewItem>> CIMList, 
            Dictionary<string, string> ClientAM, 
            Dictionary<string, string> AMPhone)
        {
            ActionType = "C2";
            InitializeComponent();
            this.myInfo = myinfo;
            initial(CaseProperty, ClientList, UserList, ProductCate_Pair);
            this.CIMList = CIMList;
            this.ClientAM = ClientAM;
            this.AMPhone = AMPhone;
            this.ClientServerProductVersion = ClientServerProductVersion;
            this.ProductCate_Pair = ProductCate_Pair;
            msg.setKeyValuePair("1", lvi.SubItems[1].Text);
            msg.setKeyValuePair("2", lvi.SubItems[2].Text);
            textBox1.Text = lvi.SubItems[3].Text;
            comboBox1.Text = lvi.SubItems[4].Text;
            comboBox7.Text = lvi.SubItems[5].Text;
            comboBox8.Text = lvi.SubItems[6].Text;
            comboBox5.Text = lvi.SubItems[7].Text;
            TB_description.Text = lvi.SubItems[8].Text;
            comboBox6.Text = lvi.SubItems[9].Text;
            comboBox3.Text = lvi.SubItems[10].Text;
            comboBox4.Text = lvi.SubItems[11].Text;
            TB_solution.Text = lvi.SubItems[12].Text;
            comboBox2.Text = lvi.SubItems[13].Text;
            comboBox9.Text = lvi.SubItems[14].Text;
            comboBox10.Text = lvi.SubItems[15].Text;
            comboBox11.Text = lvi.SubItems[16].Text;
            textBox2.Text = lvi.SubItems[17].Text;
            label13.Text = utility.getAMInfo(lvi.SubItems[4].Text,ClientAM,AMPhone);
            if (myInfo.MyID == "7")
            {
                button3.Visible = true;
            }
            this.Text = lvi.SubItems[1].Text;
        }

        public void initial(Dictionary<string, string> CaseProperty,List<string> ClientList, List<string> UserList,Dictionary<string, string> ProductCate_Pair )
        {
            Combo_OP.initialComboBox(comboBox1, ClientList);
            Combo_OP.initialComboBox(comboBox4, CaseProperty["Status"].ToString(), ",");
            Combo_OP.initialComboBox(comboBox2, CaseProperty["Priority"].ToString(), ",");
            Combo_OP.initialComboBox(comboBox6, CaseProperty["Type"].ToString(), ",");
            Combo_OP.initialComboBox(comboBox5, CaseProperty["Through"].ToString(), ",");
            Combo_OP.initialComboBox(comboBox3, UserList);
            Combo_OP.initialComboBox(comboBox9, CaseProperty["Issue"].ToString(), ",");
            Combo_OP.initialComboBox(comboBox10, ProductCate_Pair.Keys.ToArray());
            utility.setFont(this, Config.Font_Content);
        }

        public void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.Text.Trim().Equals("") || comboBox3.Text.Trim().Equals(""))
            {
                MessageBox.Show("Please input Client and Incharge person!");
                return;
            }
            else if (!utility.isCorrectVersionFormat(comboBox10.Text, textBox2.Text))
            {
                MessageBox.Show("Version format of this product, must be 4 intergers seperated with 3 dots");
                return;
            }
            else if (comboBox4.Text.Trim().Equals("Closed") && TB_solution.Text.Trim().Equals(""))
            {
                MessageBox.Show("Please input solution");
                return;
            }
            msg.setKeyValuePair("4", comboBox1.Text);
            msg.setKeyValuePair("5", comboBox7.Text);
            msg.setKeyValuePair("6", comboBox8.Text);
            msg.setKeyValuePair("7", comboBox5.Text);
            msg.setKeyValuePair("8", TB_description.Text);
            msg.setKeyValuePair("9", comboBox6.Text);
            msg.setKeyValuePair("10", comboBox3.Text);
            msg.setKeyValuePair("12", TB_solution.Text);
            msg.setKeyValuePair("13", comboBox2.Text);
            msg.setKeyValuePair("14", comboBox9.Text);
            msg.setKeyValuePair("167", comboBox10.Text);
            msg.setKeyValuePair("160", comboBox11.Text);
            msg.setKeyValuePair("169", textBox2.Text);
            switch (this.ActionType)
            {
                //  edit case
                case "C2":
                    {
                        msg.setKeyValuePair("3", textBox1.Text);
                        msg.setKeyValuePair("11", comboBox4.Text);
                        break;
                    }
                // new case
                case "C1":
                    {
                        msg.setKeyValuePair("1", "");
                        if (!textBox1.Text.Trim(' ').Equals(""))
                        {
                            msg.setKeyValuePair("3", textBox1.Text);
                        }
                        if (comboBox4.Text.Trim(' ').Equals(""))
                        {
                            msg.setKeyValuePair("11", "Follow");
                        }
                        else
                        {
                            msg.setKeyValuePair("11", comboBox4.Text);
                        }
                        msg.setKeyValuePair("17", "N" + NewCaseFormCounter);
                        break;
                    }
                default:
                    break;
            }
            Config.SLS_Sock.socketMsg(ActionType, msg, this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ActionType.Equals("C2"))
            {
                if (comboBox1.Text.Trim().Equals("") || comboBox3.Text.Trim().Equals(""))
                {
                    MessageBox.Show("Please input Client and Incharge person!");
                }
                else if (!utility.isCorrectVersionFormat(comboBox10.Text, textBox2.Text))
                {
                    MessageBox.Show("Version format of this product, must be 4 intergers seperated with 3 dots");
                }
                else
                {
                    if (TB_solution.Text.Trim().Equals(""))
                    {
                        MessageBox.Show("Please input solution!");
                        return;
                    }
                    else
                    {
                        msg.setKeyValuePair("3", textBox1.Text);
                        msg.setKeyValuePair("4", comboBox1.Text);
                        msg.setKeyValuePair("5", comboBox7.Text);
                        msg.setKeyValuePair("6", comboBox8.Text);
                        msg.setKeyValuePair("7", comboBox5.Text);
                        msg.setKeyValuePair("8", TB_description.Text);
                        msg.setKeyValuePair("9", comboBox6.Text);
                        msg.setKeyValuePair("10", comboBox3.Text);
                        msg.setKeyValuePair("11", "Closed");
                        msg.setKeyValuePair("12", TB_solution.Text);
                        msg.setKeyValuePair("13", comboBox2.Text);
                        msg.setKeyValuePair("14", comboBox9.Text);
                        msg.setKeyValuePair("167", comboBox10.Text);
                        msg.setKeyValuePair("160", comboBox11.Text);
                        msg.setKeyValuePair("169", textBox2.Text);
                        // "C2" for Edit case
                        Config.SLS_Sock.socketMsg("C2", msg, this);
                    }
                }
            }
            else
            {
                message tempMsg = new message();
                tempMsg.setKeyValuePair("80", myInfo.MyName);
                tempMsg.setKeyValuePair("17", "N" + NewCaseFormCounter);
                Config.SLS_Sock.socketMsg("R2", tempMsg, null);
                utility.clearContent(this);
            }
        }

        private void comboBox7_TextChanged(object sender, EventArgs e)
        {
            utility.loadContactPhone(comboBox1, comboBox7, comboBox8,CIMList);
        }

        private void NewAndEditCaseForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                if (ActionType.Equals("C1"))
                {
                    if (BOOL_ManualClose)
                    {
                        message tempMsg = new message();
                        tempMsg.setKeyValuePair("1", comboBox1.Text.Trim(' '));
                        tempMsg.setKeyValuePair("80", myInfo.MyName);
                        tempMsg.setKeyValuePair("17", "N" + NewCaseFormCounter);
                        Config.SLS_Sock.socketMsg("R2", tempMsg, null);
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void linkLabel1_Click(object sender, EventArgs e)
        {
            try
            {
                if (ActionType.Equals("C2"))
                {
                    Clipboard.Clear();
                    Clipboard.SetText(msg.getValueFromPairs("1"));
                    MessageBox.Show("Copied STR_CaseID: " + msg.getValueFromPairs("1"));  
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string client = comboBox1.Text.Trim(' ');
                utility.loadClientContact(comboBox1, comboBox7, CIMList);
                comboBox11.Items.Clear();
                if (ClientServerProductVersion.ContainsKey(client))
                {
                    comboBox11.Items.AddRange(ClientServerProductVersion[client].Keys.ToArray());
                }
                label13.Text = utility.getAMInfo(client,ClientAM,AMPhone);

                if (ActionType.Equals("C1"))
                {
                    if (comboBox1.Items.Contains(client))
                    {
                        message tempMsg = new message();
                        tempMsg.setKeyValuePair("1", client);
                        tempMsg.setKeyValuePair("17", "N" + NewCaseFormCounter);
                        tempMsg.setKeyValuePair("80", myInfo.MyName);
                        Config.SLS_Sock.socketMsg("R1", tempMsg, null);
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
                string client = comboBox1.Text.Trim(' ');
                if (!client.Equals(""))
                {
                    if (ClientServerProductVersion.ContainsKey(client))
                    {
                        string server = comboBox11.Text.Trim(' ');
                        if (ClientServerProductVersion[client].ContainsKey(server))
                        {
                            comboBox10.Items.Clear();
                            comboBox10.Items.AddRange(ClientServerProductVersion[client][server].Keys.ToArray());
                        }
                    }
                }
                if (!comboBox10.Text.Equals(""))
                {
                    comboBox10_TextChanged(sender, e);
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void comboBox10_TextChanged(object sender, EventArgs e)
        {
            try
            {
                linkLabel2.Text = "";
                string client = comboBox1.Text.Trim(' '), product = "", version = "";
                if (client != "")
                {
                    if (ClientServerProductVersion.ContainsKey(client))
                    {
                        string server = comboBox11.Text.Trim(' ');
                        if (ClientServerProductVersion[client].ContainsKey(server))
                        {
                            product = comboBox10.Text.Trim(' ');
                            if (ClientServerProductVersion[client][server].ContainsKey(product))
                            {
                                textBox2.Text = ClientServerProductVersion[client][server][product];
                                version = textBox2.Text;
                            }
                        }
                    }
                }
                if (product != "" && version != "")
                {
                    linkLabel2.Text = "Analyzing...";
                    StringBuilder cmd = new StringBuilder("select * from ProductHints where Product ='").Append(product).Append("' and (FromVersion<='").Append(version).Append("' or ToVersion>='").Append(version).Append("')");
                    ThreadPool.QueueUserWorkItem(new WaitCallback(realtimeProductHint), cmd);
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        public void  realtimeProductHint(object cmd)
        {
            try
            {
                if (ActionType.Equals("C2"))
                {
                    if (editCaseEventCount == 0)
                    {
                        editCaseEventCount++;
                        return;
                    }
                }
                ProductHints_Result = SQL.genListLvi(cmd.ToString(), Config.UI_ProductHintsKeys, SQL.dbConnect());
                if (ProductHints_Result == null)
                {
                    linkLabel2.Invoke(new utility.Del_setText(utility.setText), new object[] { linkLabel2, "No ProductHints match" });
                }
                else
                {
                    linkLabel2.Invoke(new utility.Del_setText(utility.setText), new object[] { linkLabel2, ProductHints_Result.Count.ToString() + " ProductHints" });
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (Form_ProductHints == null)
            {
                Form_ProductHints = new ProductHints(ProductHints_Result,ProductCate_Pair);
                Form_ProductHints.Show();
            }
            else if (Form_ProductHints.IsDisposed)
            {
                Form_ProductHints = new ProductHints(ProductHints_Result, ProductCate_Pair);
                Form_ProductHints.Show();
            }
            else
            {
                Form_ProductHints.WindowState = FormWindowState.Normal;
                Form_ProductHints.Focus();
                LV_OP.initialListView(Form_ProductHints.ListView1, ProductHints_Result, true);
            }
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            linkLabel2.Text = "";
            string product = comboBox10.Text.Trim(' '), version = textBox2.Text.Trim(' ');
            if (product != "" && version != "")
            {
                linkLabel2.Text = "Analyzing...";
                string cmd = "";
                try
                {
                    cmd = new StringBuilder("select * from ProductHints where Product ='").Append(product).Append("' and (FromVersion<='").Append(version).Append("' or ToVersion>='").Append(version).Append("')").ToString();
                    ThreadPool.QueueUserWorkItem(new WaitCallback(realtimeProductHint), cmd);
                }
                catch (Exception ex)
                {
                    Thread th = new Thread(new ParameterizedThreadStart(realtimeProductHint));
                    th.Start(cmd);
                    Config.logWriter.writeErrorLog(ex);
                }
            }
        }

        private void ActCase_Load(object sender, EventArgs e)
        {
            string product = comboBox10.Text.Trim(' '), version = textBox2.Text.Trim(' ');
            if (product != "" && version != "")
            {
                linkLabel2.Text = "Analyzing...";
                string cmd = "";
                try
                {
                    cmd = new StringBuilder("select * from ProductHints where Product ='").Append(product).Append("' and (FromVersion<='").Append(version).Append("' or ToVersion>='").Append(version).Append("')").ToString();
                    ThreadPool.QueueUserWorkItem(new WaitCallback(realtimeProductHint), cmd);
                }
                catch (Exception ex)
                {
                    Thread th = new Thread(new ParameterizedThreadStart(realtimeProductHint));
                    th.Start(cmd);
                    Config.logWriter.writeErrorLog(ex);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Delete ?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (result.ToString() == "OK")
            {
                // "C3" for Delete case
                message del_msg = new message();
                del_msg.setKeyValuePair("1", msg.getValueFromPairs("1"));
                Config.SLS_Sock.socketMsg("C3", del_msg, this);
            }
        }

        private void TB_description_Enter(object sender, EventArgs e)
        {
            TB_description.ScrollBars = ScrollBars.Vertical;
        }

        private void TB_description_Leave(object sender, EventArgs e)
        {
            TB_description.ScrollBars = ScrollBars.None;
        }

        private void TB_solution_Enter(object sender, EventArgs e)
        {
            TB_solution.ScrollBars = ScrollBars.Vertical;
        }

        private void TB_solution_Leave(object sender, EventArgs e)
        {
            TB_solution.ScrollBars = ScrollBars.None;
        }
    }
}
