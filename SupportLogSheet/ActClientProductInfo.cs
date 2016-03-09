//增删改 部署给客户服务器de， 我公司某个软件的信息
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
    public partial class ActClientProductInfo : Form
    {
        private string ExVersion;
        private Dictionary<string, string> ProductCate_Pair;
        public ActClientProductInfo(ListViewItem lvi, Dictionary<string, string> ProductCate_Pair)
        {
            InitializeComponent();
            textBox1.Text = lvi.SubItems[1].Text;
            textBox2.Text = lvi.SubItems[2].Text;
            textBox3.Text = lvi.SubItems[4].Text;
            ExVersion = lvi.SubItems[5].Text;
            textBox4.Text = ExVersion;
            this.ProductCate_Pair = ProductCate_Pair;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string product = textBox3.Text.Trim(' '), version = textBox4.Text;
            if (!utility.isCorrectVersionFormat(product,version))
            {
                MessageBox.Show("Wrong version format, must be 4 intergers seperated with 3 dots");
            }
             if(!ProductCate_Pair.ContainsKey(product)) 
             {
                 MessageBox.Show("Product is not in product list, please add it first in SuperUser form!");
             }
            else
            {       
                message msg = new message();
                msg.setKeyValuePair("4", textBox1.Text);
                msg.setKeyValuePair("160", textBox2.Text);
                msg.setKeyValuePair("166", ProductCate_Pair[product]);
                msg.setKeyValuePair("167", product);
                msg.setKeyValuePair("169", version);
                msg.setKeyValuePair("174", ExVersion);
                Config.SLS_Sock.socketMsg("PM5", msg, this);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            message msg = new message();
            msg.setKeyValuePair("4", textBox1.Text);
            msg.setKeyValuePair("160", textBox2.Text);
            msg.setKeyValuePair("167", textBox3.Text);
            Config.SLS_Sock.socketMsg("PM6", msg, this);
        }

    }
}
