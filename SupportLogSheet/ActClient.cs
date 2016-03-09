// 增 删 改 客户
// 客户就是一家Broker证券公司， 可以设一个level， 不同level的客户在其它地方会显示不同的颜色， 还可以和一个account manager 关联
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;

namespace SupportLogSheet
{
    public partial class ActClient : Form
    {
        private string Type;
        private string Ex_Client;
        //add Type=1
        public ActClient(string clientLevels, List<string> CaseProperty_AM)
        {
            InitializeComponent();
            Combo_OP.initialComboBox(comboBox1, clientLevels, ",");
            Combo_OP.initialComboBox(comboBox2, CaseProperty_AM);
            this.Type = "B1";
            button1.Text = "Add";
            button2.Text = "Clear";
            this.Text = "AddClient";
        }
        //edit Type=0
        public ActClient(ListViewItem lvi, string clientLevels, List<string> CaseProperty_AM)
        {
            InitializeComponent();
            Combo_OP.initialComboBox(comboBox1, clientLevels, ",");
            Combo_OP.initialComboBox(comboBox2, CaseProperty_AM);
            Ex_Client = lvi.SubItems[1].Text;
            textBox1.Text = lvi.SubItems[1].Text;
            comboBox1.Text = lvi.SubItems[2].Text;
            comboBox2.Text = lvi.SubItems[3].Text;
            this.Text = "EditClient";
            this.Type = "B2";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Type.Equals("B2"))
            {
                DialogResult result = MessageBox.Show("Delete ?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result.ToString().Equals("OK"))
                {
                    message msg = new message();
                    msg.setKeyValuePair("4", textBox1.Text);
                    Config.SLS_Sock.socketMsg("B3", msg, this);
                }
            }
            if (Type.Equals("B1"))
            {
                utility.clearContent(this);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Please input Client !");
                return;
            }
            if (!comboBox1.Items.Contains(comboBox1.Text))
            {
                MessageBox.Show("invalid client level index");
                return;
            }
            message msg = new message();
            msg.setKeyValuePair("4", textBox1.Text);
            msg.setKeyValuePair("132", comboBox1.Text);
            msg.setKeyValuePair("133", comboBox2.Text);
            if (Type.Equals("B2"))
            {
                msg.setKeyValuePair("135", Ex_Client);
            }
            Config.SLS_Sock.socketMsg(Type, msg, this);
        }
    }
}
