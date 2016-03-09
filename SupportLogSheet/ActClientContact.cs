//增 删 改 客户的联系人
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

namespace SupportLogSheet
{
    public partial class ActClientContact : Form
    {
        private string Type;
        private string ExContact;
        public ActClientContact(Dictionary<string, List<ListViewItem>> CIMList)
        {
            InitializeComponent();
            Combo_OP.initialComboBox(comboBox1, CIMList.Keys.ToArray());
            this.Text = "AddContact";
            this.Type = "P1";
            button1.Text = "Add";
            button2.Text = "Clear";
            utility.setFont(this, Config.Font_Content);
        }
        public ActClientContact(ListViewItem lvi, bool IsSuperUser, Dictionary<string, List<ListViewItem>> CIMList)
        {
            InitializeComponent();
            Combo_OP.initialComboBox(comboBox1, CIMList.Keys.ToArray());
            this.Text = "EditContact";
            this.Type = "P2";
            comboBox1.Text = lvi.SubItems[1].Text;
            textBox1.Text = lvi.SubItems[2].Text;
            textBox2.Text = lvi.SubItems[3].Text;
            textBox3.Text = lvi.SubItems[4].Text;
            textBox4.Text = lvi.SubItems[5].Text;
            textBox5.Text = lvi.SubItems[6].Text;
            textBox6.Text = lvi.SubItems[7].Text;
            comboBox1.Enabled = false;
            if (!IsSuperUser)
            {
                this.Size = new System.Drawing.Size(305, 430);
                this.MaximumSize = new System.Drawing.Size(305, 430);
                this.MinimumSize = new System.Drawing.Size(305, 430);
                button1.Visible = false;
                button2.Visible = false;
            }
            ExContact = lvi.SubItems[2].Text;
            utility.setFont(this, Config.Font_Content);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text) || String.IsNullOrEmpty(comboBox1.Text))
            {
                MessageBox.Show("Please input Client and Contact!");
                return;
            }
            message msg = new message();
            msg.setKeyValuePair("4", comboBox1.Text);
            msg.setKeyValuePair("111", textBox1.Text);
            msg.setKeyValuePair("112", textBox2.Text);
            msg.setKeyValuePair("113", textBox3.Text);
            msg.setKeyValuePair("114", textBox4.Text);
            msg.setKeyValuePair("115", textBox5.Text);
            msg.setKeyValuePair("116", textBox6.Text);
            if (Type.Equals("P2"))
            {
                msg.setKeyValuePair("117", ExContact);
            }
            Config.SLS_Sock.socketMsg(Type, msg, this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Type.Equals("P1"))
            {
                utility.clearContent(this);
            }
            else
            {
                DialogResult result = MessageBox.Show("Delete ?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result.ToString().Equals( "OK"))
                {
                    message msg = new message();
                    msg.setKeyValuePair("4", comboBox1.Text);
                    msg.setKeyValuePair("111", textBox1.Text);
                    Config.SLS_Sock.socketMsg("P3", msg, this);
                }
            }
        }

    }
}
