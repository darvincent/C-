// 统计分析历史case
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading;

namespace SupportLogSheet
{
    public partial class CaseAnalysis : Form
    {
        private SqlConnection sqlconnection;
        private Dictionary<string, List<ListViewItem>> cases;
        private delegate void Del_showEditCaseForm(ListViewItem lvi);
        private bool SortType1 = true;
        private bool SortType2 = true;
        private Dictionary<string, string> CaseProperty;
        private Dictionary<string, List<ListViewItem>> CIMList;
        private Dictionary<string, string> ClientAM;
        private Dictionary<string, string> AMPhone;
        private List<string> ClientList;
        private List<string> UserList;
        private Dictionary<string, string> ProductCate_Pair;
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> ClientServerProductVersion;
        private MyInfo myInfo;
        public CaseAnalysis(
            MyInfo myInfo,
            Dictionary<string, string> CaseProperty, 
            Dictionary<string, List<ListViewItem>> CIMList,
            List<string> ClientList,
            List<string> UserList,
            Dictionary<string, string> ProductCate_Pair,
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> ClientServerProductVersion,
            Dictionary<string, string> ClientAM,
            Dictionary<string, string> AMPhone)
        {
            InitializeComponent();
            this.myInfo = myInfo;
            LV_OP.readColumnOrder(listView1, Config.Customization_INI, "AnalysisCategory_Order");
            LV_OP.readColumnOrder(listViewNF1, Config.Customization_INI, "AnalysisCases_Order");
            LV_OP.readColumnWidth(listView1, Config.Customization_INI, "AnalysisCategory_Width");
            LV_OP.readColumnWidth(listViewNF1, Config.Customization_INI, "AnalysisCases_Width");
            comboBox1.Items.AddRange(new string[] { "Client", "Incharge", "Product", "Priority", "Issue", "type", "Status" });
            sqlconnection = SQL.dbConnect();
            this.CaseProperty = CaseProperty;
            this.CIMList = CIMList;
            this.ClientList = ClientList;
            this.UserList = UserList;
            this.ProductCate_Pair = ProductCate_Pair;
            this.ClientServerProductVersion = ClientServerProductVersion;
            this.ClientAM = ClientAM;
            this.AMPhone = AMPhone;
            utility.setFont(this, Config.Font_Content);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            cases = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comboBox1.Text != "")
                {
                    string category = comboBox1.Text.Trim(' ');
                    string startDate = dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss").Trim(' ');
                    string endDate = dateTimePicker2.Value.ToString("yyyy-MM-dd HH:mm:ss").Trim(' ');
                    string cmd = new StringBuilder("select * from ").Append(Config.DBName_SupportLogSheet).Append(" where CreateTime >='").Append(startDate).Append("' and CreateTime<='").Append(endDate).Append("'").Append(Config.Msg_Separator2).Append( category).ToString();
                    try
                    {
                        ThreadPool.QueueUserWorkItem(analyze, cmd);
                    }
                    catch
                    {
                        Thread th = new Thread(new ParameterizedThreadStart(analyze));
                        th.Start(cmd);
                    }
                }
                else
                {
                    MessageBox.Show("Please input category !");
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void analyze(object obj)
        {
            try
            {
                if (cases == null)
                {
                    cases = new Dictionary<string, List<ListViewItem>>();
                }
                else
                {
                    cases.Clear();
                }
                int index = 0;
                string[] temp = Regex.Split(obj.ToString(), Config.Msg_Separator2);
                string cmd = temp[0], category = temp[1];
                SqlCommand sqlcmd = new SqlCommand(cmd, sqlconnection);
                string[] DBColumnNames = Config.getValues(Config.UI_CaseAnalysisKeys);
                for (int i = 0; i < DBColumnNames.Length; i++)
                {
                    if (DBColumnNames[i] == category)
                    {
                        index = i;
                        break;
                    }
                }
                using (SqlDataReader re = sqlcmd.ExecuteReader())
                {
                    message msg = new message();
                    while (re.Read())
                    {
                        for (int i = 0; i < DBColumnNames.Length; i++)
                        {
                            msg.setKeyValuePair(Config.UI_CaseAnalysisKeys[i], re.GetValue(re.GetOrdinal(DBColumnNames[i])).ToString().Trim(' '));
                        }
                        category = msg.getValueFromPairs(Config.UI_CaseAnalysisKeys[index]) == "" ? "Undefined" : msg.getValueFromPairs(Config.UI_CaseAnalysisKeys[index]);
                        if (cases.ContainsKey(category))
                        {
                            cases[category].Add(LV_OP.getLVI(msg,Config.UI_CaseAnalysisKeys));
                        }
                        else
                        { 
                            cases.Add(category,new List<ListViewItem>{LV_OP.getLVI(msg,Config.UI_CaseAnalysisKeys)});
                        }
                    }
                }
                List<ListViewItem> results = new List<ListViewItem>();
                foreach (string key in cases.Keys)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.SubItems.Add(key);
                    lvi.SubItems.Add(cases[key].Count.ToString());
                    results.Add(lvi);
                }
                this.Invoke(new LV_OP.Del_initialListView(LV_OP.initialListView), new object[] { listView1, results,true });
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listView1.SelectedItems.Count > 0)
            {
                LV_OP.initialListView(listViewNF1, cases[listView1.SelectedItems[0].SubItems[1].Text], true);
            }
        }

        private void listViewNF1_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                if (listViewNF1.SelectedItems.Count > 0)
                {
                    string caseID = listViewNF1.SelectedItems[0].SubItems[1].Text;
                    string cmd = new StringBuilder("select * from SupportLogSheet where CaseID='").Append(caseID).Append("'").ToString();
                    try
                    {
                        ThreadPool.QueueUserWorkItem(showCase, cmd);
                    }
                    catch
                    {
                        Thread th = new Thread(new ParameterizedThreadStart(showCase));
                        th.Start(cmd);
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void showCase(object obj)
        {
            try
            {
                this.Invoke(new Del_showEditCaseForm(showEditCaseForm), new object[] { SQL.genListLvi(obj.ToString(), Config.UI_CaseKeys, sqlconnection)[0] });
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void showEditCaseForm(ListViewItem lvi)
        {
            ActCase f = new ActCase(myInfo,lvi,CaseProperty,ClientList,UserList,ProductCate_Pair,ClientServerProductVersion,CIMList,ClientAM,AMPhone);
            f.Show();
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            LV_OP.listviewSort(listView1, e,ref SortType1);
        }

        private void listViewNF1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            LV_OP.listviewSort(listViewNF1, e, ref SortType2);
        }

        private void CaseAnalysis_FormClosed(object sender, FormClosedEventArgs e)
        {
            LV_OP.writeColumnOrder(listView1, Config.Customization_INI, "AnalysisCategory_Order");
            LV_OP.writeColumnOrder(listViewNF1, Config.Customization_INI, "AnalysisCases_Order");
            LV_OP.writeColumnWidth(listView1, Config.Customization_INI, "AnalysisCategory_Width");
            LV_OP.writeColumnWidth(listViewNF1, Config.Customization_INI, "AnalysisCases_Width");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Statistic_Graphic graph = new Statistic_Graphic(CaseProperty,ClientList,UserList,ProductCate_Pair);
            graph.Show();
        }

        private void listView1_SizeChanged(object sender, EventArgs e)
        {
            LV_OP.listViewAutoSize(listView1);
        }

        private void listViewNF1_SizeChanged(object sender, EventArgs e)
        {
            LV_OP.listViewAutoSize(listViewNF1);
        }
    }
}
