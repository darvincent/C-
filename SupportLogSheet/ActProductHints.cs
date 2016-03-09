// 增删改 公司的程序的某个版本区间 出现的问题， 以及相关问题的解决方案
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
    public partial class ActProductHints : Form
    {
        private string Type;
        //add Type =0
        public ActProductHints(Dictionary<string, string> ProductCate_Pair)
        {
            InitializeComponent();
            Combo_OP.initialComboBox(CB_Product, ProductCate_Pair.Keys.ToArray());
            CB_Type.Items.AddRange(Config.CategoryType2);
            this.Text = "AddProductHints";
            this.Type = "PH1";
            button1.Text = "Add";
            button2.Text = "Clear";
            TB_ID.Enabled = false;
            utility.setFont(this, Config.Font_Content);
        }

        //edit Type =1
        public ActProductHints(ListViewItem lvi,Dictionary<string, string> ProductCate_Pair)
        {
            InitializeComponent();
            Combo_OP.initialComboBox(CB_Product, ProductCate_Pair.Keys.ToArray());
            CB_Type.Items.AddRange(Config.CategoryType2);
            this.Text = "EditProductHints";
            this.Type = "PH2";

            TB_ID.Text = lvi.SubItems[1].Text;
            CB_Product.Text = lvi.SubItems[2].Text;
            CB_Type.Text = lvi.SubItems[3].Text;
            RB_Description.Text = lvi.SubItems[4].Text;
            TB_FromVersion.Text = lvi.SubItems[5].Text;
            TB_ToVersion.Text = lvi.SubItems[6].Text;
            RB_Details.Text = lvi.SubItems[7].Text;
            utility.setFont(this, Config.Font_Content);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if ((!utility.isCorrectVersionFormat(CB_Product.Text,TB_FromVersion.Text) || !utility.isCorrectVersionFormat(CB_Product.Text,TB_ToVersion.Text)))
            {
                MessageBox.Show("Version format of this product, must be 4 intergers seperated with 3 dots");
                return;
            }
            else
            {
                try
                {
                    string fromVersion = TB_FromVersion.Text.Trim(' '), toVersion = TB_ToVersion.Text.Trim(' ');
                    if (!fromVersion.Equals("") && !toVersion.Equals("") && !utility.comPareVersion(fromVersion, toVersion))
                    {
                        MessageBox.Show("FromVersion should be smaller than ToVersion!");
                    }
                    else
                    {
                        message msg = new message();
                        msg.setKeyValuePair("167", CB_Product.Text);
                        if (Type.Equals("PH2"))
                        {
                            msg.setKeyValuePair("200", TB_ID.Text);
                        }
                        msg.setKeyValuePair("201", CB_Type.Text);
                        msg.setKeyValuePair("202", RB_Description.Text);
                        msg.setKeyValuePair("203", TB_FromVersion.Text);
                        msg.setKeyValuePair("204", TB_ToVersion.Text);
                        msg.setKeyValuePair("205", RB_Details.Text);
                        Config.SLS_Sock.socketMsg(Type, msg, this);
                    }
                }
                catch (Exception ex)
                {
                    Config.logWriter.writeErrorLog(ex);
                }
            }
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (Type.Equals("PH1"))
            {
                utility.clearContent(this);
            }
            else
            {
                message msg = new message();
                msg.setKeyValuePair("200", TB_ID.Text);
                Config.SLS_Sock.socketMsg("PH3", msg, this);
            }
        }

        private void TB_description_Enter(object sender, EventArgs e)
        {
            RB_Details.ScrollBars = ScrollBars.Vertical;
        }

        private void TB_description_Leave(object sender, EventArgs e)
        {
            RB_Details.ScrollBars = ScrollBars.None;
        }
    }
}
