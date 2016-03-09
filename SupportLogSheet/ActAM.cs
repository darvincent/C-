// account manager 操作类， 用于增，删，改
// account manager 嘛， 特殊公司里面的职位，名曰 客户经理， 负责客户滴
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
    public partial class ActAM : Form
    {
        private string ActionType;
        private string ExAMName;
        public ActAM() // 0 for new 
        {
            InitializeComponent();
            this.ActionType = "A1";
            button2.Text = "Clear";
        }
        public ActAM(ListViewItem lvi) // 1 for add
        {
            InitializeComponent();
            this.ActionType = "A2";
            textBox1.Text = lvi.SubItems[1].Text;
            textBox2.Text = lvi.SubItems[2].Text;
            textBox3.Text = lvi.SubItems[3].Text;
            ExAMName = lvi.SubItems[1].Text.Trim(' ');
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(textBox1.Text))
            {
                MessageBox.Show("Please input AccountManager name !");
                return;
            }
            message msg = new message();
            msg.setKeyValuePair("150", textBox1.Text);
            msg.setKeyValuePair("151", textBox2.Text);
            msg.setKeyValuePair("152", textBox3.Text);
            if (ActionType.Equals("A2")) 
            {
                msg.setKeyValuePair("153", ExAMName);
            }
            Config.SLS_Sock.socketMsg(ActionType, msg, this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ActionType.Equals("A1"))
            {
                utility.clearContent(this);
            }
            else
            {
                message msg = new message();
                msg.setKeyValuePair("150", textBox1.Text);
                Config.SLS_Sock.socketMsg("A3", msg, this);
            }
        }
    }
}
