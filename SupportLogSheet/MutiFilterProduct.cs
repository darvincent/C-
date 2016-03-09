// 通过 版本信息筛选公司产品 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Threading;

namespace SupportLogSheet
{
    public partial class MutiFilterProduct : Form
    {
        private SqlConnection sqlConnection;
        private ProductMaster PM;
        public MutiFilterProduct(ProductMaster PM)
        {
            InitializeComponent();
            utility.setFont(this, Config.Font_Content);
            comboBox1.Items.AddRange(PM.getProductCate_Pair().Keys.ToArray());
            comboBox2.Items.AddRange(PM.getProductCate_Pair().Keys.ToArray());
            comboBox3.Items.AddRange(PM.getProductCate_Pair().Keys.ToArray());
            sqlConnection = SQL.dbConnect();
            this.PM = PM;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                StringBuilder cmd =new StringBuilder("select distinct p1.Client,p1.Server from ProductMaster p1 ");
                StringBuilder condition = new StringBuilder(" where");
                List<string> products = new List<string>();
                List<string> beginVersions = new List<string>();
                List<string> endVersions = new List<string>();
                List<bool> isEquals = new List<bool>();
                if (!comboBox1.Text.Trim(' ').Equals( ""))
                {
                    if (!utility.isCorrectVersionFormat(comboBox1.Text, textBox1.Text) || !utility.isCorrectVersionFormat(comboBox1.Text, textBox2.Text))
                    {
                        MessageBox.Show("Version format of this product, must be 4 intergers seperated with 3 dots");
                    }
                    else
                    {
                        products.Add(comboBox1.Text.Trim(' '));
                        beginVersions.Add(textBox1.Text.Trim(' '));
                        endVersions.Add(textBox2.Text.Trim(' '));
                        isEquals.Add(checkBox1.Checked);
                    }
                }
                if (!comboBox2.Text.Trim(' ').Equals(""))
                {
                    if (!utility.isCorrectVersionFormat(comboBox2.Text, textBox3.Text) || !utility.isCorrectVersionFormat(comboBox2.Text, textBox4.Text))
                    {
                        MessageBox.Show("Version format of this product, must be 4 intergers seperated with 3 dots");
                    }
                    else
                    {
                        products.Add(comboBox2.Text.Trim(' '));
                        beginVersions.Add(textBox3.Text.Trim(' '));
                        endVersions.Add(textBox4.Text.Trim(' '));
                        isEquals.Add(checkBox2.Checked);
                    }
                }
                if (!comboBox3.Text.Trim(' ').Equals( ""))
                {
                    if (!utility.isCorrectVersionFormat(comboBox3.Text, textBox5.Text) || !utility.isCorrectVersionFormat(comboBox3.Text, textBox6.Text))
                    {
                        MessageBox.Show("Version format of this product, must be 4 intergers seperated with 3 dots");
                    }
                    else
                    {
                        products.Add(comboBox3.Text.Trim(' '));
                        beginVersions.Add(textBox5.Text.Trim(' '));
                        endVersions.Add(textBox6.Text.Trim(' '));
                        isEquals.Add(checkBox3.Checked);
                    }
                }

                if (products.Count > 0)
                {
                    condition.Append(" p1.Product = '").Append(products[0]).Append("'");
                    if (isEquals[0])
                    {
                        if (!beginVersions[0].Equals(""))
                        {
                            condition.Append(" and dbo.f_IP2Int(p1.Version) >=  dbo.f_IP2Int('").Append(beginVersions[0]).Append("')");
                        }
                        if (!endVersions[0] .Equals( ""))
                        {
                            condition.Append(" and dbo.f_IP2Int(p1.Version) <=  dbo.f_IP2Int('").Append(endVersions[0]).Append("')");
                        }
                    }
                    else
                    {
                        if (!beginVersions[0] .Equals( ""))
                        {
                            condition.Append(" and dbo.f_IP2Int(p1.Version) > dbo.f_IP2Int('").Append(beginVersions[0]).Append("')");
                        }
                        if (!endVersions[0].Equals(""))
                        {
                            condition.Append(" and dbo.f_IP2Int(p1.Version) <  dbo.f_IP2Int('").Append(endVersions[0]).Append("')");
                        }
                    }
                    for (int i = 1; i < products.Count; i++)
                    {
                        string dbName = "p" + (i + 1).ToString();
                        cmd.Append("inner join ProductMaster ").Append(dbName).Append(" on p1.Client=").Append(dbName).Append(".Client and p1.Server=").Append(dbName).Append(".Server");
                        condition.Append(" and ").Append(dbName).Append(".Product = '").Append(products[i]).Append("'");
                        if (isEquals[i])
                        {
                            if (!beginVersions[0].Equals(""))
                            {
                                condition.Append(" and dbo.f_IP2Int(").Append(dbName).Append(".Version) >=  dbo.f_IP2Int('").Append(beginVersions[i]).Append("')");
                            }
                            if (!endVersions[0].Equals(""))
                            {
                                condition.Append(" and dbo.f_IP2Int(").Append(dbName).Append(".Version) <=  dbo.f_IP2Int('").Append(endVersions[i]).Append("')");
                            }
                        }
                        else
                        {
                            if (!beginVersions[i].Equals(""))
                            {
                                condition.Append(" and dbo.f_IP2Int(").Append(dbName).Append(".Version) > dbo.f_IP2Int('").Append(beginVersions[i]).Append("')");
                            }
                            if (!endVersions[i].Equals(""))
                            {
                                condition.Append(" and dbo.f_IP2Int(").Append(dbName).Append(".Version) < dbo.f_IP2Int('").Append(endVersions[i]).Append("')");
                            }
                        }
                    }
                }
                else
                {
                    condition.Clear();
                }

                cmd.Append(condition.ToString());
                try
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(FilterProduct), cmd);
                }
                catch (Exception ex)
                {
                    Thread th = new Thread(new ParameterizedThreadStart(FilterProduct));
                    th.Start(cmd);
                    Config.logWriter.writeErrorLog(ex);
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        public void FilterProduct(object cmd)
        {
            try
            {
                List<ListViewItem> lvis = new List<ListViewItem>();
                SqlCommand sqlcmd = new SqlCommand(cmd.ToString(), sqlConnection);
                using (SqlDataReader re = sqlcmd.ExecuteReader())
                {
                    string[] DBColumnNames = Config.getValues(Config.UI_MultiFilterProductKeys);
                    message msg = new message();
                    while (re.Read())
                    {
                        for (int i = 0; i < DBColumnNames.Length; i++)
                        {
                            msg.setKeyValuePair(Config.UI_MultiFilterProductKeys[i], re.GetString(re.GetOrdinal(DBColumnNames[i])).Trim(' '));
                        }
                        lvis.Add(LV_OP.getLVI(msg,Config.UI_MultiFilterProductKeys));
                    }
                }
                listView1.Invoke(new LV_OP.Del_initialListView(LV_OP.initialListView), new object[] { listView1, lvis ,true});
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }   
        }

        private void button4_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (Control obj in this.Controls)
                {
                    if (obj is ComboBox || obj is TextBox)
                    {
                        obj.Text = "";
                    }
                    if (obj is ListView)
                    {
                        (obj as ListView).BeginUpdate();
                        (obj as ListView).Items.Clear();
                        (obj as ListView).EndUpdate();
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
             textBox1.Clear();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            string cmd = new StringBuilder("select * from ").Append(Config.DBName_ProductMaster).Append(" where Client='").Append(listView1.SelectedItems[0].SubItems[1].Text + "' and Server='" + listView1.SelectedItems[0].SubItems[2].Text).Append("'").ToString();
            PM.initialProducts(cmd);
        }

        private void MutiFilterProduct_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                PM.IsInstanceMutiFilter = false;
                sqlConnection.Close();
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }
    }
}
