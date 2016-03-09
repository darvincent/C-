// deploy 公司产品给客户的服务器
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using System.Data.SqlClient;

namespace SupportLogSheet
{
    public partial class DeployProduct : Form
    {
        private Dictionary<string, ArrayList> Dic_clientServers;
        private Dictionary<string, string> ProductCate_Pair;
        public DeployProduct(Dictionary<string, ArrayList> clientServers,List<string> ClientList,Dictionary<string, string> ProductCate_Pair)
        {
            InitializeComponent();
            utility.setFont(this, Config.Font_Content);
            Dic_clientServers = clientServers;
            Combo_OP.initialComboBox(comboBox1, ClientList);
            Combo_OP.initialComboBox(comboBox3, ProductCate_Pair.Keys.ToArray());
            this.ProductCate_Pair = ProductCate_Pair;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string client = comboBox1.Text.Trim(' '), server = comboBox2.Text.Trim(' '), product = comboBox3.Text.Trim(' '), version = textBox1.Text.Trim();
            if (!utility.isCorrectVersionFormat(product, version))
            {
                MessageBox.Show("Version format of this product, must be 4 intergers seperated with 3 dots");
            }
            if (!ProductCate_Pair.ContainsKey(product))
            {
                MessageBox.Show("Product is not in product list, please add it first in SuperUser form!");
            }
            else
            {
                message msg = new message();
                msg.setKeyValuePair("4", client);
                msg.setKeyValuePair("160", server);
                msg.setKeyValuePair("166", ProductCate_Pair[product]);
                msg.setKeyValuePair("167", product);
                msg.setKeyValuePair("169", version);
                msg.setKeyValuePair("172", TB_description.Text);
                Config.SLS_Sock.socketMsg("PM4", msg, this);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                Combo_OP.initialComboBox(comboBox2, Dic_clientServers[comboBox1.Text.Trim(' ')].ToArray());
            }
            catch (Exception ex)
            {
                comboBox2.Text = "";
                comboBox2.Items.Clear();
                Config.logWriter.writeErrorLog(ex);
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

    }
}
