// 客户服务器安装的公司产品 信息集合
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Collections;
using System.Threading;
using System.Net.Sockets;
using System.Net;
using System.Text.RegularExpressions;

namespace SupportLogSheet
{
    public partial class ProductMaster : Form
    {
        private MyInfo myInfo;
        private bool IsSupport = true;
        private bool SortType = true;
        private SqlConnection sqlConnection;
        public Dictionary<string, ArrayList> Dic_clientServers = new Dictionary<string, ArrayList>();

        private List<string> ClientList;
        private List<string> UserList;
        private Dictionary<string, string> ProductCate_Pair;
        private Dictionary<string, Dictionary<string, Dictionary<string, string>>> ClientServerProductVersion;

        private MutiFilterProduct Form_MutiFilter;
        public bool IsInstanceMutiFilter = false;
        private BackgroundWorker BW_ProductMaster;

        public ProductMaster(List<string> ClientList, List<string> UserList, Dictionary<string, string> ProductCate_Pair, Dictionary<string, Dictionary<string, Dictionary<string, string>>> ClientServerProductVersion)
        {
            InitializeComponent();
            sqlConnection = SQL.dbConnect();
            initialClientServer();
            this.ClientList = ClientList;
            this.UserList = UserList;
            this.ProductCate_Pair = ProductCate_Pair;
            this.ClientServerProductVersion = ClientServerProductVersion;
        }

        // for ProductMaster mode, only load this form
        public ProductMaster(int i)
        {
            IsSupport = false;
            myInfo = new MyInfo(LoginForm.MyID, LoginForm.LoginUser, "", "", LoginForm.LoginUser, Config.Customization_INI.readValue("COMMON", "Location"));

            BW_ProductMaster = new BackgroundWorker();
            BW_ProductMaster.DoWork += new DoWorkEventHandler(DoWork_ProductMaster);

            sqlConnection = SQL.dbConnect();

            initialClientServer();
            ProductCate_Pair = utility.initial_Products();
            ClientList = initialClients();
            UserList = initialUsers();
            ClientServerProductVersion = utility.initial_ProductMaster();

            InitializeComponent();
        }

        public List<Msg_OP> handleMsg(List<Msg_OP> msg_Ops)
        {
            int count = msg_Ops.Count;
            List<Msg_OP> toRemove = new List<Msg_OP>();
            for (int i = 0; i < count; i++)
            {
                if (Config.PM_SeparateType.Contains(msg_Ops[i].msgType))
                {
                    if (!BW_ProductMaster.IsBusy)
                    {
                        BW_ProductMaster.RunWorkerAsync(msg_Ops[i]);
                        toRemove.Add(msg_Ops[i]);
                    }
                }
                else
                {
                    toRemove.Add(msg_Ops[i]);
                }
            }
            return toRemove;
        }

        public Dictionary<string, string> getProductCate_Pair()
        {
            return ProductCate_Pair;
        }

        public void DoWork_ProductMaster(object sender, DoWorkEventArgs e)
        {
            Msg_OP msg_Op = (Msg_OP)e.Argument;
            try
            {
                switch (msg_Op.msgType)
                {
                    case "PM1": // add a server
                        {
                            addServer(msg_Op);
                            break;
                        }
                    case "PM2":  // edit a server
                        {
                            editServer(msg_Op);
                            break;
                        }
                    case "PM3": // delete a server
                        {
                            deleteServer(msg_Op);
                            break;
                        }
                    case "PM4": // deploy product to a client's server
                        {
                            addProductToClientServer(msg_Op);
                            break;
                        }
                    case "PM5": // edit product deploy info of a client's server
                        {
                            editProductOfClientServer(msg_Op);
                            break;
                        }
                    case "PM6": // delete product deploy info of a client's server
                        {
                            deleteProductOfClientServer(msg_Op);
                            break;
                        }
                    case "PM7": // edit product deploy update Info
                        {
                            msg_Op.buildMsgs();
                            Invoke(new Msg_OP.Del_MsgOp(UI_editProductUpdate), new object[] { msg_Op });
                            break;
                        }
                    case "PM8": // delete product deploy update Info
                        {
                            msg_Op.buildMsgs();
                            Invoke(new Msg_OP.Del_MsgOp(UI_deleteProductUpdate), new object[] { msg_Op });
                            break;
                        }
                    case "PM9": // add a product
                        {
                            ProductCate_Pair.Add(msg_Op.getValueFromPairs("167"), msg_Op.getValueFromPairs("166"));
                            Invoke(new Combo_OP.Del_updateComboBoxNoSplit(Combo_OP.initialComboBox), new object[] { ComboBox2, ProductCate_Pair.Keys.ToArray() });
                            break;
                        }
                    case "PM10": // edit a product
                        {
                            ProductCate_Pair.Remove(msg_Op.getValueFromPairs("176"));
                            ProductCate_Pair.Add(msg_Op.getValueFromPairs("167"), msg_Op.getValueFromPairs("166"));
                            Invoke(new Combo_OP.Del_updateComboBoxNoSplit(Combo_OP.initialComboBox), new object[] { ComboBox2, ProductCate_Pair.Keys.ToArray() });
                            break;
                        }
                    case "PM11": //delete a product
                        {
                            ProductCate_Pair.Remove(msg_Op.getValueFromPairs("167"));
                            Invoke(new Combo_OP.Del_updateComboBoxNoSplit(Combo_OP.initialComboBox), new object[] { ComboBox2, ProductCate_Pair.Keys.ToArray() });
                            break;
                        }
                    case "IO9": // log writing
                        {
                            Config.logWriter.flushToFile(msg_Op);
                            break;
                        }
                    case "N1": // upgrade notice
                        {
                            editNotice(msg_Op);
                            break;
                        }
                    case "ERROR":
                        {
                            this.Invoke(new Msg_OP.Del_MsgOp(UI_handleError), new object[] { msg_Op });
                            break;
                        }
                    case "Kicked":
                        {
                            this.Invoke(new Msg_OP.Del_MsgOp(UI_handleError), new object[] { msg_Op });
                            Application.Exit();
                            break;
                        }
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void UI_handleError(Msg_OP msg_Op)
        {
            MessageBox.Show(msg_Op.getValueFromPairs("75"));
        }

        private void editNotice(Msg_OP msg_Op)
        {
            this.MouseEnter += delegate(object o, EventArgs ea)
            {
                string path = msg_Op.getValueFromPairs("61");
                DialogResult result = MessageBox.Show("New version available,please get from " + path, "", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                if (result.ToString() == "OK")
                {
                    MessageBox.Show("Copied new version program path");
                    Clipboard.Clear();
                    Clipboard.SetText(path);
                    Application.Exit();
                }
            };
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ActClientServer addServer = new ActClientServer(ClientList);
            addServer.Show();
        }

        private void ProductMaster_FormClosed(object sender, FormClosedEventArgs e)
        {
            try
            {
                sqlConnection.Close();        
                if (!IsSupport)
                {
                    Config.SLS_Sock.exit();
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string client = comboBox1.Text.Trim();
            if (comboBox1.Items.Contains(client))
            {
                string cmd = new StringBuilder("select * from ClientServer where Client='").Append(client).Append("'").ToString();
                listView3.Items.Clear();
                listView4.Items.Clear();
                ThreadPool.QueueUserWorkItem(SearchClientServer, cmd);
            }    
        }

        public void SearchClientServer(object cmd)
        {
            Invoke(new LV_OP.Del_initialListView(LV_OP.initialListView), new object[] { listView1,SQL.genListLvi(cmd.ToString(), Config.UI_ClientServerKeys, sqlConnection),true });
        }

        public void initialClientServer()
        {
            try
            {
                Dic_clientServers.Clear();
                string cmd = "select * from ClientServer";
                SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection);
                using (SqlDataReader re = sqlcmd.ExecuteReader())
                {
                    string[] DBColumnNames = Config.getValues(Config.ClientServerKeys);
                    message msg = new message();
                    while (re.Read())
                    {
                        for (int i = 0; i < DBColumnNames.Length; i++)
                        {
                            msg.setKeyValuePair(Config.ClientServerKeys[i], re.GetString(re.GetOrdinal(DBColumnNames[i])).Trim(' '));
                        }
                        string client = msg.getValueFromPairs("4"), server = msg.getValueFromPairs("160");
                        if (Dic_clientServers.ContainsKey(client))
                        {
                            Dic_clientServers[client].Add(server);
                        }
                        else
                        {
                            Dic_clientServers.Add(client, new ArrayList { server });
                        }
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
            ActClientServer editServer = new ActClientServer(comboBox1.Text.Trim(' '), listView1.SelectedItems[0].SubItems[1].Text.Trim(),ClientList);
            editServer.Show();
        }

        public void initialProducts(object cmd)
        {
            Invoke(new LV_OP.Del_initialListView(LV_OP.initialListView), new object[] { listView3, SQL.genListLvi(cmd.ToString(), Config.UI_ProductMasterKeys, sqlConnection) ,true});
        }

        private void button3_Click(object sender, EventArgs e)
        {      
            DeployProduct deployProduct = new DeployProduct(Dic_clientServers,ClientList,ProductCate_Pair);
            if (listView3.Items.Count > 0)
            {
                deployProduct.ComboBox1.Text = LV_OP.getColumnValue(listView3.Items[0], Config.UI_ProductMasterKeys, "4");
                deployProduct.ComboBox2.Text = LV_OP.getColumnValue(listView3.Items[0], Config.UI_ProductMasterKeys, "160");
            }
            deployProduct.Show();
        }

        public void initialProduct_updates(object cmd)
        {
            Invoke(new LV_OP.Del_initialListView(LV_OP.initialListView), new object[] { listView4, SQL.genListLvi(cmd.ToString(), Config.UI_ProductMaster_UpdateKeys, sqlConnection) ,true});
        }

        private void listView3_DoubleClick(object sender, EventArgs e)
        {
            if (listView3.SelectedItems.Count > 0)
            {
                ActClientProductInfo edp = new ActClientProductInfo(listView3.SelectedItems[0],ProductCate_Pair);
                edp.Show();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!utility.isCorrectVersionFormat(comboBox2.Text,textBox1.Text) || !utility.isCorrectVersionFormat(comboBox2.Text,textBox2.Text))
            {
                MessageBox.Show("Wrong version format, must be 4 intergers seperated with 3 dots");
            }
            else
            {
                string product = comboBox2.Text.Trim(' ');
                string beginVersion = textBox1.Text.Trim(' ');
                string endVersion = textBox2.Text.Trim(' ');
                StringBuilder cmd = new StringBuilder("select * from ProductMaster where Product not in('OSKey','Windows OS') and Product = '");
                cmd.Append(product).Append("'").ToString();
                if (beginVersion != "")
                {
                    if (checkBox1.Checked)
                    {
                        cmd.Append(" and dbo.f_IP2Int(Version) >=  dbo.f_IP2Int('").Append(beginVersion).Append("')");
                    }
                    else
                    {
                        cmd.Append(" and dbo.f_IP2Int(Version) > dbo.f_IP2Int('").Append(beginVersion).Append("')");
                    }
                }
                if (endVersion != "")
                {
                    if (checkBox1.Checked)
                    {
                        cmd.Append(" and dbo.f_IP2Int(Version) <= dbo.f_IP2Int('").Append(endVersion).Append("')");
                    }
                    else
                    {
                        cmd.Append(" and dbo.f_IP2Int(Version) < dbo.f_IP2Int('").Append(endVersion).Append("')");
                    }
                }
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
        }

        public void FilterProduct(object cmd)
        {
            List<ListViewItem> lvis = new List<ListViewItem>();
            try
            {
                SqlCommand sqlcmd = new SqlCommand(cmd.ToString(), sqlConnection);
                using (SqlDataReader re = sqlcmd.ExecuteReader())
                {
                    string[] DBColumnNames = Config.getValues(Config.ProductMasterKeys);
                    message msg = new message();
                    while (re.Read())
                    {
                        for (int i = 0; i < DBColumnNames.Length; i++)
                        {
                            msg.setKeyValuePair(Config.ProductMasterKeys[i], re.GetString(re.GetOrdinal(DBColumnNames[i])).Trim(' '));
                        }
                        lvis.Add(LV_OP.getLVI(msg,Config.UI_FilterProductVersionsKeys));
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
            Invoke(new LV_OP.Del_initialListView(LV_OP.initialListView), new object[] { listView2, lvis,true });
        }

        private void button4_Click(object sender, EventArgs e)
        {
            comboBox2.Text = "";
            textBox1.Text = "";
            textBox2.Text = "";
            listView2.Items.Clear();
        }

        private void listView4_DoubleClick(object sender, EventArgs e)
        {
            if (listView4.SelectedItems.Count > 0)
            {
                ActClientProduct_UpdateInfo form = new ActClientProduct_UpdateInfo(listView4.SelectedItems[0],UserList,ProductCate_Pair);
                form.Show();
            }
        }

        private void listView3_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            LV_OP.listviewSort(listView3, e, ref SortType);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (listView3.Items.Count > 0)
            {
                List<ListViewItem> lvis = new List<ListViewItem>();
                for(int i =0;i<listView3.Items.Count;i++)
                {
                    lvis.Add((ListViewItem)listView3.Items[i].Clone());
                }
                PM_Analysis pma = new PM_Analysis(lvis,ProductCate_Pair);
                pma.Show();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (!IsInstanceMutiFilter)
            {
                Form_MutiFilter = new MutiFilterProduct(this);
                IsInstanceMutiFilter = true;
                Form_MutiFilter.Show();
            }
            else
            {
                Form_MutiFilter.WindowState = FormWindowState.Normal;
                Form_MutiFilter.Focus();
            }
        }

        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            LV_OP.listviewSort(listView2, e, ref SortType);
        }

        public void addServer(Msg_OP msg_Op)
        {
            string client = msg_Op.getValueFromPairs("4"), server = msg_Op.getValueFromPairs("160");
            if (Dic_clientServers.ContainsKey(client))
            {
                Dic_clientServers[client].Add(server);
            }
            Invoke(new Msg_OP.Del_MsgOp(UI_addServer), new object[] { msg_Op });
        }

        public void UI_addServer(Msg_OP msg_Op)
        {
            if (ComboBox1.Text.Trim() == msg_Op.getValueFromPairs("4"))
            {
                LV_OP.insertOneListviewItem(listView1, 0, LV_OP.getLVIs(msg_Op,Config.UI_ClientServerKeys)[0]);
            }
        }

        public void editServer(Msg_OP msg_Op)
        {
            string client = msg_Op.getValueFromPairs("4"), server = msg_Op.getValueFromPairs("160"), exServer = msg_Op.getValueFromPairs("173");
            if (Dic_clientServers.ContainsKey(client))
            {
                if (Dic_clientServers[client].Contains(exServer))
                {
                    Dic_clientServers[client].Remove(exServer);
                }
                Dic_clientServers[client].Add(server);
            }
            Invoke(new Msg_OP.Del_MsgOp(UI_editServer), new object[] { msg_Op });
        }

        public void UI_editServer(Msg_OP msg_Op)
        {
            if (ComboBox1.Text.Trim().Equals( msg_Op.getValueFromPairs("4")))
            {
                string exServer  = msg_Op.getValueFromPairs("173");
                for (int i = 0; i < ListView1.Items.Count; i++)
                {
                    if (LV_OP.getColumnValue(listView1.Items[i], Config.UI_ClientServerKeys, "160").Equals(exServer))
                    {
                        LV_OP.replaceOneListviewItem(listView1, ListView1.Items[i], LV_OP.getLVIs(msg_Op,Config.UI_ClientServerKeys)[0]);
                        break;
                    }
                }
            }
        }

        public void deleteServer(Msg_OP msg_Op)
        {
            string client = msg_Op.getValueFromPairs("4"), server = msg_Op.getValueFromPairs("160");
            if (Dic_clientServers.ContainsKey(client))
            {
                if (Dic_clientServers[client].Contains(server))
                {
                    Dic_clientServers[client].Remove(server);
                }
            }
            Invoke(new Msg_OP.Del_MsgOp(UI_deleteServer), new object[] { msg_Op });
        }

        public void UI_deleteServer(Msg_OP msg_Op)
        {
            if (ComboBox1.Text.Trim().Equals( msg_Op.getValueFromPairs("4")))
            {
                string server = msg_Op.getValueFromPairs("160");
                int index1 = LV_OP.getLviIndex(listView1, Config.UI_ClientServerKeys, "160", server);
                if (index1 >= 0)
                {
                    LV_OP.removeOneListviewItem(listView1, listView1.Items[index1]);
                }
                int index2 = LV_OP.getLviIndex(listView3, Config.UI_ProductMasterKeys, "160", server);
                if (index2 >= 0)
                {
                    LV_OP.removeOneListviewItem(listView3, listView3.Items[index2]);
                }
                int index3 = LV_OP.getLviIndex(listView4, Config.UI_ProductMaster_UpdateKeys, "160", server);
                if (index3 >= 0)
                {
                    LV_OP.removeOneListviewItem(listView4, listView4.Items[index3]);
                }
            }
        }

        public void addProductToClientServer(Msg_OP msg_Op)
        {
            string client = msg_Op.getValueFromPairs("4"),server = msg_Op.getValueFromPairs("160");
            string product = msg_Op.getValueFromPairs("167"), version = msg_Op.getValueFromPairs("169");
            if (ClientServerProductVersion.ContainsKey(client))
            {
                if (ClientServerProductVersion[client].ContainsKey(server))
                {
                    ClientServerProductVersion[client][server].Add(product, version);
                }
                else
                {
                    Dictionary<string, string> productVersion = new Dictionary<string, string>();
                    productVersion.Add(product, version);
                    ClientServerProductVersion[client].Add(server, productVersion);
                }
            }
            else
            {
                Dictionary<string, string> productVersion = new Dictionary<string, string>();
                productVersion.Add(product, version);
                Dictionary<string, Dictionary<string, string>> serverProductVersion = new Dictionary<string, Dictionary<string, string>>();
                serverProductVersion.Add(server, productVersion);
                ClientServerProductVersion.Add(client, serverProductVersion);
            }
            Invoke(new Msg_OP.Del_MsgOp(UI_addProductToClientServer), new object[] { msg_Op });
        }

        public void UI_addProductToClientServer(Msg_OP msg_Op)
        {
            if (ComboBox1.Text.Trim(' ') == msg_Op.getValueFromPairs("4") && ListView1.SelectedItems[0].SubItems[1].Text == msg_Op.getValueFromPairs("160"))
            {
                LV_OP.insertOneListviewItem(listView3, 0, LV_OP.getLVIs(msg_Op,Config.UI_ProductMasterKeys)[0]);
            }
        }

        public void editProductOfClientServer(Msg_OP msg_Op)
        {
            string client = msg_Op.getValueFromPairs("4"), server = msg_Op.getValueFromPairs("160");
            string product = msg_Op.getValueFromPairs("167"), version = msg_Op.getValueFromPairs("169");
            ClientServerProductVersion[client][server][product] = version;
            Invoke(new Msg_OP.Del_MsgOp(UI_editProductOfClientServer), new object[] { msg_Op });
        }

        public void UI_editProductOfClientServer(Msg_OP msg_Op)
        {
            string client = msg_Op.getValueFromPairs("4");
            string server = msg_Op.getValueFromPairs("160");
            string product = msg_Op.getValueFromPairs("167");
            if (listView3.Items.Count > 0)
            {
                if (LV_OP.getColumnValue(listView3.Items[0], Config.UI_ProductMasterKeys, "4") != client)
                {
                    return;
                }
                else if (LV_OP.getColumnValue(listView3.Items[0], Config.UI_ProductMasterKeys, "160") != server)
                {
                    return;
                }
                else
                {
                    for (int i = 0; i < ListView3.Items.Count; i++)
                    {
                        if (LV_OP.getColumnValue(listView3.Items[i], Config.UI_ProductMasterKeys, "167") == product)
                        {
                            LV_OP.replaceOneListviewItem(listView3, ListView3.Items[i], LV_OP.getLVIs(msg_Op,Config.UI_ProductMasterKeys)[0]);
                            break;
                        }
                    }
                }
            }
        }

        public void deleteProductOfClientServer(Msg_OP msg_Op)
        {
            string client = msg_Op.getValueFromPairs("4");
            string server = msg_Op.getValueFromPairs("160");
            string product = msg_Op.getValueFromPairs("167");
            ClientServerProductVersion[client][server].Remove(product);
            if (ClientServerProductVersion[client][server].Count == 0)
            {
                ClientServerProductVersion[client].Remove(server);
                if (ClientServerProductVersion[client].Count == 0)
                {
                    ClientServerProductVersion.Remove(client);
                }
            }
            Invoke(new Msg_OP.Del_MsgOp(UI_deleteProductOfClientServer), new object[] { msg_Op });
        }

        public void UI_deleteProductOfClientServer(Msg_OP msg_Op)
        {
            string client = msg_Op.getValueFromPairs("4");
            string server = msg_Op.getValueFromPairs("160");
            string product = msg_Op.getValueFromPairs("167");
            if (listView3.Items.Count > 0)
            {
                if (LV_OP.getColumnValue(listView3.Items[0], Config.UI_ProductMasterKeys, "4") != client)
                {
                    return ;
                }
                else if (LV_OP.getColumnValue(listView3.Items[0], Config.UI_ProductMasterKeys, "160") != server)
                {
                    return;
                }
                else
                {
                    for (int i = 0; i < ListView3.Items.Count; i++)
                    {
                        if (LV_OP.getColumnValue(listView3.Items[i], Config.UI_ProductMasterKeys, "167") == product)
                        {
                            LV_OP.removeOneListviewItem(listView3, ListView3.Items[i]);
                            break;
                        }
                    }
                }
            }
        }

        public void UI_editProductUpdate(Msg_OP msg_Op)
        {
            string client = msg_Op.getValueFromPairs("4"), server = msg_Op.getValueFromPairs("160");
            string product = msg_Op.getValueFromPairs("167"), version = msg_Op.getValueFromPairs("169");
            if (listView4.Items.Count > 0)
            {
                if (LV_OP.getColumnValue(listView4.Items[0], Config.UI_ProductMaster_UpdateKeys, "4") != client)
                {
                    return;
                }
                else if (LV_OP.getColumnValue(listView4.Items[0], Config.UI_ProductMaster_UpdateKeys, "160") != server)
                {
                    return;
                }
                else if (LV_OP.getColumnValue(listView4.Items[0], Config.UI_ProductMaster_UpdateKeys, "167") != product)
                {
                    return;
                }
                else
                {
                    for (int i = 0; i < ListView4.Items.Count; i++)
                    {
                        if (LV_OP.getColumnValue(listView4.Items[i], Config.UI_ProductMaster_UpdateKeys, "169") == version)
                        {
                            LV_OP.replaceOneListviewItem(listView4, ListView4.Items[i], LV_OP.getLVIs(msg_Op,Config.UI_ProductMaster_UpdateKeys)[0]);
                            break;
                        }
                    }
                }
            }
        }

        public void UI_deleteProductUpdate(Msg_OP msg_Op)
        {
            string client = msg_Op.getValueFromPairs("4"), server = msg_Op.getValueFromPairs("160");
            string product = msg_Op.getValueFromPairs("167"), version = msg_Op.getValueFromPairs("169");
            if (listView4.Items.Count > 0)
            {
                if (LV_OP.getColumnValue(listView4.Items[0], Config.UI_ProductMaster_UpdateKeys, "4") != client)
                {
                    return;
                }
                else if (LV_OP.getColumnValue(listView4.Items[0], Config.UI_ProductMaster_UpdateKeys, "160") != server)
                {
                    return;
                }
                else if (LV_OP.getColumnValue(listView4.Items[0], Config.UI_ProductMaster_UpdateKeys, "167") != product)
                {
                    return;
                }
                else
                {
                    for (int i = 0; i < ListView4.Items.Count; i++)
                    {
                        if (LV_OP.getColumnValue(listView4.Items[i], Config.UI_ProductMaster_UpdateKeys, "169") == version)
                        {
                            LV_OP.removeOneListviewItem(listView4, ListView4.Items[i]);
                            break;
                        }
                    }
                }
            }
        }

        public List<string>  initialClients()
        {
            List<string> clientList = new List<string>();
            try
            {
                string cmd = "select Client from ClientList";
                SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection);
                using (SqlDataReader re = sqlcmd.ExecuteReader())
                {
                    while (re.Read())
                    {
                        clientList.Add(re[0].ToString().Trim(' '));
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
            return clientList;
        }

        public List<string> initialUsers()
        {
            List<string> userList = new List<string>();
            try
            {
                string cmd = "select UserName from UserFile";
                SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection);
                using (SqlDataReader re = sqlcmd.ExecuteReader())
                {
                    while (re.Read())
                    {
                        userList.Add(re[0].ToString().Trim(' '));
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
            return userList;
        }

        private void listView1_SizeChanged(object sender, EventArgs e)
        {
            LV_OP.listViewAutoSize(listView1);
        }

        private void listView2_SizeChanged(object sender, EventArgs e)
        {
            LV_OP.listViewAutoSize(listView2);
        }

        private void listView3_SizeChanged(object sender, EventArgs e)
        {
            LV_OP.listViewAutoSize(listView3);
        }

        private void listView4_SizeChanged(object sender, EventArgs e)
        {
            LV_OP.listViewAutoSize(listView4);
        }

        private void listView1_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            try
            {
                string client = comboBox1.Text.Trim(' ');
                if (client != "")
                {
                    if (e.IsSelected)
                    {
                        string server = LV_OP.getColumnValue(e.Item, Config.UI_ClientServerKeys, "160");
                        string cmd = SQL.selectSqlCmd(Config.DBName_ProductMaster, null, new string[] { "4", "160" }, new string[] { client, server });
                        LV_OP.changeLVIBackColor(e.Item, Config.UI_ClientServerKeys);
                        listView4.Items.Clear();
                        ThreadPool.QueueUserWorkItem(new WaitCallback(initialProducts), cmd);
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void listView3_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                LV_OP.changeLVIBackColor(e.Item, Config.UI_ProductMasterKeys);
                string client = LV_OP.getColumnValue(e.Item, Config.UI_ProductMasterKeys, "4");
                string server = LV_OP.getColumnValue(e.Item, Config.UI_ProductMasterKeys, "160");
                string product = LV_OP.getColumnValue(e.Item, Config.UI_ProductMasterKeys, "167");
                string cmd = SQL.selectSqlCmd(Config.DBName_ProductMaster_Update, null, new string[] { "4", "160", "167" }, new string[] { client, server, product });
                ThreadPool.QueueUserWorkItem(new WaitCallback(initialProduct_updates), cmd);
            }
        }

        private void listView4_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                LV_OP.changeLVIBackColor(e.Item, Config.UI_ProductMaster_UpdateKeys);
            }
        }

        private void ProductMaster_Load(object sender, EventArgs e)
        {
            utility.setFont(this, Config.Font_Content);
            Combo_OP.initialComboBox(comboBox1, ClientList);
            comboBox2.Items.AddRange(ProductCate_Pair.Keys.ToArray());

            Config.logWriter.writeLog("Login successfully.");
            if (!IsSupport)
            {
                if (!Config.SLS_Sock.establish(new SLSocket.Del_handleMsg(handleMsg), myInfo))
                {
                    MessageBox.Show("Can't connect to SLS, you can only search informations");
                }
            }
        }

    }
}
