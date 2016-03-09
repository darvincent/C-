////增删改 部署给客户服务器的， 我公司某个软件的更新信息
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SupportLogSheet
{
    public partial class ActClientProduct_UpdateInfo : Form
    {
        private string client;
        private string server;
        private string product;
        private string exVersion;
        private Dictionary<string, string> ProductCate_Pair;
        public ActClientProduct_UpdateInfo(ListViewItem lvi,List<string> UserList, Dictionary<string, string> ProductCate_Pair)
        {
            InitializeComponent();
            this.ProductCate_Pair = ProductCate_Pair;
            Combo_OP.initialComboBox(comboBox1, UserList);
            client = lvi.SubItems[1].Text.Trim(' ');
            server = lvi.SubItems[2].Text.Trim(' ');
            product = lvi.SubItems[3].Text.Trim(' ');
            exVersion = lvi.SubItems[4].Text.Trim(' ');
            this.Text = new StringBuilder(client).Append(" , ").Append(server).Append(" server").ToString();
            textBox1.Text = lvi.SubItems[4].Text.Trim(' ');
            textBox2.Text = lvi.SubItems[5].Text.Trim(' ');
            comboBox1.Text = lvi.SubItems[6].Text.Trim(' ');
            textBox4.Text = lvi.SubItems[7].Text.Trim(' ');
            TB_description.Text = lvi.SubItems[8].Text.Trim(' ');
            utility.setFont(this, Config.Font_Content);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            message msg = new message();
            msg.setKeyValuePair("4", client);
            msg.setKeyValuePair("160", server);
            msg.setKeyValuePair("166", ProductCate_Pair[textBox1.Text.Trim(' ')]);
            msg.setKeyValuePair("167", textBox1.Text);
            msg.setKeyValuePair("169", textBox2.Text);
            msg.setKeyValuePair("171", comboBox1.Text);
            msg.setKeyValuePair("170", textBox4.Text);
            msg.setKeyValuePair("172", TB_description.Text);
            Config.SLS_Sock.socketMsg("PM7", msg, this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            message msg = new message();
            msg.setKeyValuePair("4", client);
            msg.setKeyValuePair("160", server);
            msg.setKeyValuePair("167", textBox1.Text);
            msg.setKeyValuePair("169", textBox2.Text);
            Config.SLS_Sock.socketMsg("PM8", msg, this);
        }

        private void TB_description_Leave(object sender, EventArgs e)
        {
            TB_description.ScrollBars = ScrollBars.None;
        }

        private void TB_description_Enter(object sender, EventArgs e)
        {
            TB_description.ScrollBars = ScrollBars.Vertical;
        }

    }
}
