// 超级用户 管理界面
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Threading;

namespace SupportLogSheet
{
    public partial class SuperUser : Form
    {
        private bool SortType = true;
        private Dictionary<string, string> CaseProperty;
        private Dictionary<string, string> UserIDNameMapping;
        private List<string> CaseProperty_AM;
        public SuperUser(
            List<ListViewItem> clientlist,
            List<ListViewItem> userist,
            List<ListViewItem> AMlist, 
            Dictionary<string, string> CaseProperty, 
            Dictionary<string, string> UserIDNameMapping,
            List<string> CaseProperty_AM)
        {
            InitializeComponent();
            utility.setFont(this, Config.Font_Content);
            LV_OP.initialListView(listView1, clientlist, true);
            LV_OP.initialListView(listView2, userist, true);
            LV_OP.initialListView(listViewNF1, AMlist,true);
            comboBox1.Items.AddRange(Config.CaseProperty_Category);
            this.CaseProperty = CaseProperty;
            this.UserIDNameMapping = UserIDNameMapping;
            this.CaseProperty_AM = CaseProperty_AM;
        }

        public void initialProducts(object obj)
        {
            string cmd = "select * from Product";
            List<ListViewItem> lvis = new List<ListViewItem>();
            try
            {
                using (SqlConnection sqlConnection = SQL.dbConnect())
                {
                    using (SqlCommand sqlcmd = new SqlCommand(cmd.ToString(), sqlConnection))
                    {
                        using (SqlDataReader re = sqlcmd.ExecuteReader())
                        {
                            string[] DBColumnNames = Config.getValues(Config.ProductKeys);
                            message msg = new message();
                            while (re.Read())
                            {
                                for (int i = 0; i < DBColumnNames.Length; i++)
                                {
                                    msg.setKeyValuePair(Config.ProductKeys[i], re.GetString(re.GetOrdinal(DBColumnNames[i])).Trim(' '));
                                }
                                lvis.Add(LV_OP.getLVI(msg, Config.UI_ProductKeys));
                            }
                        }
                        this.Invoke(new LV_OP.Del_initialListView(LV_OP.initialListView), new object[] { listViewNF2, lvis, true });
                    }
                }
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
                ActClient buildMsg_EditClient = new ActClient(listView1.SelectedItems[0], CaseProperty["ClientLevel"],CaseProperty_AM);
                buildMsg_EditClient.Show();
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
            {
                ActClient buildMsg_AddClient = new ActClient(CaseProperty["ClientLevel"],CaseProperty_AM);
                buildMsg_AddClient.Show();
            }
            if (tabControl1.SelectedIndex == 3)
            {
                ActAM buildMsg_AddAM = new ActAM();
                buildMsg_AddAM.Show();
            }
            if (tabControl1.SelectedTab == tabPage5)
            {
                ActProduct f = new ActProduct(CaseProperty["ProductCate"]);
                f.Show();
            }
        }

        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            if (listView2.SelectedItems.Count > 0)
            {
                ActUser_BySuperUser buildMsg_EditUser = new ActUser_BySuperUser(listView2.SelectedItems[0],UserIDNameMapping);

                buildMsg_EditUser.Show();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage1)
            {
                button1.Visible = true;
                label9.Text = "Tips";
            }
           if (tabControl1.SelectedTab == tabPage4)
            {
                button1.Visible = true;
                label9.Text = "Tips";
            }
            if (tabControl1.SelectedTab == tabPage2)
            {
                button1.Visible = false;
                label9.Text = "Tips";
            }
            if (tabControl1.SelectedTab == tabPage5)
            {
                button1.Visible = true;
                label9.Text = "Tips";
            }
            if (tabControl1.SelectedTab == tabPage3)
            {
                button1.Visible = false;
                label9.Text = "Tips";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            comboBox1.Text = comboBox1.Text.Trim(' ');
            int i = 0;
            for (i = 0; i < comboBox1.Items.Count; i++)
            {
                if (comboBox1.Text.ToUpper() == comboBox1.Items[i].ToString().ToUpper())
                {
                    message msg = new message();
                    msg.setKeyValuePair("100", comboBox1.Text);
                    msg.setKeyValuePair("101", textBox1.Text);
                    Config.SLS_Sock.socketMsg("S", msg, null);
                    break;
                }
            }
            if (i == comboBox1.Items.Count)
            {
                MessageBox.Show("Please input correct category !");
            }
        }

        private void label9_Click(object sender, EventArgs e)
        {
            if (tabControl1.SelectedTab == tabPage1)
            {
                label9.Text = "Click Add button to add a client, double click a client to edit/delete.";
            }
            if (tabControl1.SelectedTab == tabPage2)
            {
                label9.Text = "Double click a user in UserList to edit it.\r\nYou can delete this user, change UserID for this user and set this user to SuperUser.";
            }
            if (tabControl1.SelectedTab == tabPage3)
            {
                label9.Text = "You can edit property types, and those types are splited by \",\" .";
            }
            if (tabControl1.SelectedTab == tabPage4)
            {
                label9.Text = "Click Add button to add an AM, double click an AM to edit/delete.";
            }
            if (tabControl1.SelectedTab == tabPage5)
            {
                label9.Text = "Click Add button to add a Product, double click a product to edit/delete.";
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != 0)
            {
                listView1.ListViewItemSorter = new ListViewItemsComparer(e.Column, SortType);
                listView1.Sort();
                SortType = !SortType;
            }
        }

        private void listViewNF1_DoubleClick(object sender, EventArgs e)
        {
            if (listViewNF1.SelectedItems.Count > 0)
            {
                ActAM editDeleteAM = new ActAM(listViewNF1.SelectedItems[0]);
                editDeleteAM.Show();
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            string category = comboBox1.Text.Trim(' ');
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                if (category.ToUpper() == comboBox1.Items[i].ToString().ToUpper())
                {
                    textBox1.Text = CaseProperty[category];
                    break;
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            comboBox1.Text = "";
            textBox1.Clear();
        }

        private void listViewNF2_DoubleClick(object sender, EventArgs e)
        {
            if (listViewNF2.SelectedItems.Count > 0)
            {
                ActProduct editProduct = new ActProduct(listViewNF2.SelectedItems[0], CaseProperty["ProductCate"]);
                editProduct.Show();
            }
        }

        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            LV_OP.listviewSort(listView2, e,ref SortType);
        }

        private void listViewNF1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            LV_OP.listviewSort(listViewNF1, e, ref SortType);
        }

        private void listViewNF2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            LV_OP.listviewSort(listViewNF2, e, ref SortType);
        }

        private void SuperUser_Load(object sender, EventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(initialProducts));
        }   
    }
}
