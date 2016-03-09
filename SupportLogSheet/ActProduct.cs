// 增删改 公司的产品 信息（UI端，服务端，第三方接口）
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
    public partial class ActProduct : Form
    {
        private string Type;
        private string ExCate;
        private string ExProduct;
        // add Type =0
        public ActProduct(string productCates)
        {
            InitializeComponent();
            Combo_OP.initialComboBox(comboBox1, productCates, ",");
            this.Type = "PM9";
            this.Text = "AddProduct";
            button1.Text = "Add";
            button2.Text = "Clear";
            utility.setFont(this, Config.Font_Content);
        }
        // edit Type =1
        public ActProduct(ListViewItem lvi, string productCates)
        {
            InitializeComponent();
            Combo_OP.initialComboBox(comboBox1, productCates, ",");
            this.Text = "EditProduct";
            this.Type = "PM10";
            ExCate = lvi.SubItems[1].Text;
            ExProduct = lvi.SubItems[2].Text;
            comboBox1.Text = ExCate;
            textBox1.Text = ExProduct;
            richTextBox1.Text = lvi.SubItems[3].Text;
            utility.setFont(this, Config.Font_Content);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!comboBox1.Items.Contains(comboBox1.Text.Trim()))
            {
                MessageBox.Show("invalid product category");
                return;
            }
            if (textBox1.Text.Trim().Equals(""))
            {
                MessageBox.Show("invalid product name");
                return;
            }
            message msg = new message();
            msg.setKeyValuePair("166", comboBox1.Text);
            msg.setKeyValuePair("167", textBox1.Text);
            msg.setKeyValuePair("168", richTextBox1.Text);
            if (Type.Equals("PM10"))
            {
                msg.setKeyValuePair("175", ExCate);
                msg.setKeyValuePair("176", ExProduct);
            }
            Config.SLS_Sock.socketMsg(Type, msg, this);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Type.Equals("PM9"))
            {
                utility.clearContent(this);
            }
            else
            {
                DialogResult result = MessageBox.Show("Delete ?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result.ToString() == "OK")
                {
                    if (true)
                    {
                        message msg = new message();
                        msg.setKeyValuePair("166", comboBox1.Text);
                        msg.setKeyValuePair("167", textBox1.Text);
                        Config.SLS_Sock.socketMsg("PM11", msg, this);
                    }
                }
            }
        }

    }
}
