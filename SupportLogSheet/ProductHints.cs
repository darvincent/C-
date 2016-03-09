// 聚集公司所有产品在特定版本区间 出现的问题
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace SupportLogSheet
{
    public partial class ProductHints : Form
    {
        private SqlConnection sqlconnection = SQL.dbConnect();
        private bool sortType = true;
        private int initialType;
        private List<ListViewItem> lvis;
        private Dictionary<string, string> ProductCate_Pair;
        //realtime load productHints
        public ProductHints(List<ListViewItem> lvis, Dictionary<string, string> ProductCate_Pair)
        {
            initialType = 1;
            this.lvis = lvis;
            InitializeComponent();
            utility.setFont(this, Config.Font_Content);
            this.ProductCate_Pair = ProductCate_Pair;
        }
        // initial
        public ProductHints(Dictionary<string, string> ProductCate_Pair)
        {
            initialType = 0;
            InitializeComponent();
            utility.setFont(this, Config.Font_Content);
            this.ProductCate_Pair = ProductCate_Pair;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ActProductHints aph = new ActProductHints(ProductCate_Pair);
            aph.Show();
        }

        public void initialProductHints()
        {
            string cmd = "select * from ProductHints";
            try
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(FilterProductHint), cmd);
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                ActProductHints editProductHints = new ActProductHints(listView1.SelectedItems[0],ProductCate_Pair);
                editProductHints.Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            initialProductHints();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string product = comboBox1.Text.Trim(' ');
            if (product != "")
            {
                string cmd = new StringBuilder("select * from ProductHints where Product = '").Append(product).Append("'").ToString();
                if (!utility.isCorrectVersionFormat(product,textBox1.Text) || !utility.isCorrectVersionFormat(product,textBox2.Text) )
                {
                    MessageBox.Show("Version format of this product, must be 4 intergers seperated with 3 dots");
                    return;
                }
                else
                {
                    string fromVersion = textBox1.Text.Trim(' ');
                    string toVersion = textBox2.Text.Trim(' ');
                    if (fromVersion == "")
                    {
                        fromVersion = "0.0.0.0";
                    }
                    if (toVersion == "")
                    {
                        toVersion = "9999.999.999.999";
                    }
                    if (checkBox1.Checked)
                    {
                        cmd += SQL.getVersionFilterSQL(fromVersion, toVersion, true);
                    }
                    else
                    {
                        cmd += SQL.getVersionFilterSQL(fromVersion, toVersion, false);
                    }
                }
                Config.logWriter.writeLog(cmd);
                ThreadPool.QueueUserWorkItem(new WaitCallback(FilterProductHint), cmd);
            }
        }

        public void FilterProductHint(object cmd)
        {
            List<ListViewItem> lvis = new List<ListViewItem>();
            SqlCommand sqlcmd = new SqlCommand(cmd.ToString(), sqlconnection);
            using (SqlDataReader re = sqlcmd.ExecuteReader())
            {
                string[] DBColumnNames = Config.getValues(Config.UI_ProductHintsKeys);
                message msg = new message();
                try
                {
                    while (re.Read())
                    {
                        for (int i = 0; i < DBColumnNames.Length; i++)
                        {
                            msg.setKeyValuePair(Config.UI_ProductHintsKeys[i], re.GetString(re.GetOrdinal(DBColumnNames[i])).Trim(' '));
                        }
                        lvis.Add(LV_OP.getLVI(msg, Config.UI_ProductHintsKeys));
                    }
                }
                catch (Exception ex)
                {
                    Config.logWriter.writeErrorLog(ex);
                }
            }
            listView1.Invoke(new LV_OP.Del_initialListView(LV_OP.initialListView), new object[] { listView1, lvis, true });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            comboBox1.Text = "";
            textBox1.Clear();
            textBox2.Clear();
            initialProductHints();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            LV_OP.listviewSort(listView1, e, ref sortType);
        }

        private void ProductHints_Load(object sender, EventArgs e)
        {
            if (initialType == 0)
            {
                initialProductHints();
            }
            if (initialType == 1)
            {
                LV_OP.initialListView(listView1, lvis,true);
            }
            comboBox1.Items.AddRange(ProductCate_Pair.Keys.ToArray());
            comboBox2.Items.AddRange(Config.CategoryType2);
        }

        public void UI_addProductHints(Msg_OP msg_Op)
        {
            LV_OP.insertOneListviewItem(listView1, 0, LV_OP.getLVIs(msg_Op,Config.UI_ProductHintsKeys)[0]);
        }

        public void UI_editProductHints(Msg_OP msg_Op)
        {
            LV_OP.updateOneListviewItem(listView1, Config.UI_ProductHintsKeys, Config.UI_Index_ProductHintsKeys, null, msg_Op.getMsgs()[0]);
        }

        public void UI_deleteProductHints(Msg_OP msg_Op)
        {
            LV_OP.removeOneListviewItem(listView1, Config.UI_ProductHintsKeys, Config.UI_Index_ProductHintsKeys, msg_Op.getMsgs()[0]);
        }

        private void listView1_SizeChanged(object sender, EventArgs e)
        {
            LV_OP.listViewAutoSize(listView1);
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                LV_OP.changeLVIBackColor(listView1.SelectedItems[0], Config.UI_ProductHintsKeys);
            }
        }

    }
}
