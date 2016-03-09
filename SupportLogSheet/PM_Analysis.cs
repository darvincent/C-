// 分析客户服务器上 所安装的所有 公司产品， 即每一个产品在使用的版本号下可能出现的问题
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SupportLogSheet
{
    public partial class PM_Analysis : Form
    {
        private SqlConnection sqlconnection = SQL.dbConnect();
        private Dictionary<string, List<ListViewItem>> hints = new Dictionary<string, List<ListViewItem>>();
        private bool SortType = true;
        private Dictionary<string, string> ProductCate_Pair;
        public PM_Analysis(List<ListViewItem> lvis, Dictionary<string, string> ProductCate_Pair)
        {
            InitializeComponent();
            utility.setFont(this, Config.Font_Content);
            this.ProductCate_Pair = ProductCate_Pair;
            initialProducts(lvis);
        }

        public void initialProducts(List<ListViewItem> lvis)
        {
            try
            {
                for (int i = 0; i < lvis.Count; i++)
                {
                    string product = lvis[i].SubItems[4].Text, version = lvis[i].SubItems[5].Text;
                    string cmd = new StringBuilder("select * from ProductHints where Product ='").Append(product).Append("' and (FromVersion<='").Append(version).Append("' or ToVersion>='").Append(version).Append("')").ToString();
                    List<ListViewItem> temp = SQL.genListLvi(cmd.ToString(), Config.UI_ProductHintsKeys, sqlconnection);
                    lvis[i].SubItems.Add(temp == null ? "0" : temp.Count.ToString());
                    hints.Add(product, temp);
                }
                LV_OP.initialListView(listView1, lvis,true);
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(listView1.SelectedItems.Count>0)
            {
                LV_OP.initialListView(listView2, hints[listView1.SelectedItems[0].SubItems[4].Text],true);
            }
        }

        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                ActProductHints editProductHints = new ActProductHints(listView2.SelectedItems[0],ProductCate_Pair);
                editProductHints.Show();
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            LV_OP.listviewSort(listView1, e, ref SortType);
        }

        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            LV_OP.listviewSort(listView2, e, ref SortType);
        }

        private void listView1_SizeChanged(object sender, EventArgs e)
        {
            LV_OP.listViewAutoSize(listView1);
        }

        private void listView2_SizeChanged(object sender, EventArgs e)
        {
            LV_OP.listViewAutoSize(listView2);
        }

    }
}
