//主窗口

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
using System.Collections;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace SupportLogSheet
{
    public partial class MainForm : Form
    {
        private MyInfo myInfo;
        private MarkedCase_OP MarkedCases_OP = new MarkedCase_OP(AppDomain.CurrentDomain.BaseDirectory + "MyMarkedCases.xml");
        private Reminder_OP Reminder_OP = new Reminder_OP(AppDomain.CurrentDomain.BaseDirectory + "Reminder.xml");
        // Form
        private ProductHints Form_ProductHints;
        private SuperUser Form_Superuser;
        private CaseAnalysis Form_Analysis;
        private ProductMaster Form_ProductMaster;
        private Custom Form_Custom;
        private MarkedCases Form_MyMarkedCases;
        private ShortCutKey Form_ShortCutKey;

        private List<ListViewItem> ListLVI_ClientList_SuperUser = new List<ListViewItem>();
        private List<ListViewItem> ListLVI_AM = new List<ListViewItem>();
        private List<ListViewItem> ListLVI_UserFile = new List<ListViewItem>();

        private Dictionary<string, ListViewItem> OutstandingCases = new Dictionary<string, ListViewItem>();
        private Dictionary<string, ListViewItem> MyCases = new Dictionary<string, ListViewItem>();

        private  Dictionary<string, string> ProductCate_Pair = new Dictionary<string, string>();
        private  List<string> ClientList = new List<string>();
        private  List<string> UserList = new List<string>();
        private  Dictionary<string, Dictionary<string, Dictionary<string, string>>> ClientServerProductVersion = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();

        private Dictionary<string, List<ListViewItem>> CIMList = new Dictionary<string, List<ListViewItem>>();
        private Dictionary<string, string> CaseProperty = new Dictionary<string, string>();
        private Dictionary<string, string> ClientAM = new Dictionary<string, string>();
        private Dictionary<string, string> AMPhone = new Dictionary<string, string>();
        private Dictionary<string, string> UserIDNameMapping = new Dictionary<string, string>();
        public static Dictionary<string, List<string>> ClientLevelDetail = new Dictionary<string, List<string>>();
        private Dictionary<string, int> outstandingCaseOfUser = new Dictionary<string, int>();

        private bool IsSuperUser = false;
        private bool SortType_Outstanding = true;
        private bool SortType_Search = true;
        private bool SortType_Mytask = true;
        private bool IsPopupReminder = false;
        private bool IsBeenKicked = false;
        //
        private string SuperUsers = "";
        public Dictionary<string, List<string>> MarkedCateCases = new Dictionary<string, List<string>>();
        private List<string> UserUpdate_HandleCases = new List<string>();
        private List<string> CaseProperty_AM = new List<string>();
        public Dictionary<string, string> ShortCutKeyIni = new Dictionary<string, string>();
        //
        private int OutstandingCount = 0;
        private int MyTaskCount = 0;
        private int NewCaseFormCounter = 0;// this index is to specify which new case form is
        private int ReminderCounter = 0;

        private BackgroundWorker BW_LowFrequency;
        private BackgroundWorker BW_Case;
        private BackgroundWorker BW_HighFrequency;
        private BackgroundWorker BW_IO;

        private delegate void flashWindowCallback(IntPtr handle, bool bInvert);
        private delegate void del_showWarning(string msg);
        private Dictionary<TabPage, EventHandler> QuickAdd = new Dictionary<TabPage, EventHandler>();
        private Dictionary<TabPage, EventHandler> QuickClear = new Dictionary<TabPage, EventHandler>();
        private Dictionary<TabPage, EventHandler> CopyCIMInfo = new Dictionary<TabPage, EventHandler>();

        private message tempCaseMsg = new message();
        private List<ListViewItem> ProductHints_Result = new List<ListViewItem>();

        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        private static extern void FlashWindow(IntPtr handle, bool bInvert);

        public MainForm()
        {
            BW_HighFrequency = new BackgroundWorker();
            BW_HighFrequency.DoWork += new DoWorkEventHandler(DoWork_HighFrequency);

            BW_Case = new BackgroundWorker();
            BW_Case.DoWork += new DoWorkEventHandler(DoWork_Case);

            BW_LowFrequency = new BackgroundWorker();
            BW_LowFrequency.DoWork += new DoWorkEventHandler(DoWork_LowFrequency);

            BW_IO = new BackgroundWorker();
            BW_IO.DoWork += new DoWorkEventHandler(DoWork_IO);

            InitializeComponent();
            initial();
        }

        private void initial()
        {
            myInfo = new MyInfo(LoginForm.MyID, LoginForm.LoginUser, "", "", LoginForm.LoginUser, Config.Customization_INI.readValue("COMMON", "Location"));
            initial_CaseProperty();
            readIni();
            loadClients_SuperUser();
            IsSuperUser = initial_Users();
            initial_CIM();
            initial_AM();
            ProductCate_Pair = utility.initial_Products();
            ClientServerProductVersion = utility.initial_ProductMaster();
            initialMarkedCases();
            Reminder_OP.loadConfig();
        }

        private void readIni()
        {
            LV_OP.initialClientLevelColor(CaseProperty["ClientLevel"]);
            LV_OP.initialPriorityColor(CaseProperty["Priority"]);
            initialShortCutKey();
            LV_OP.Color_NewItem = utility.getColor(Config.Customization_INI.readValue("NotifyColor", "NewCase"));
            LV_OP.Color_EditItem = utility.getColor(Config.Customization_INI.readValue("NotifyColor", "EditCase"));
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Text = myInfo.MyName;
            if (Config.Customization_INI.readValue("COMMON", "IsPopupReminder") == "Y")
            {
                IsPopupReminder = true;
            }
            checkBox_Popup.Checked = IsPopupReminder;
            string width = Config.Customization_INI.readValue("COMMON", "SplitWidth");
            if (!width.Equals(""))
            {
                splitContainer1.SplitterDistance = Int32.Parse(width);
            }
            //outstanding
            LV_OP.readColumnOrder(LV_outstanding, Config.Customization_INI, "Outstanding_Order");
            LV_OP.readColumnWidth(LV_outstanding, Config.Customization_INI, "Outstanding_Width");
            LV_OP.initialListView(LV_outstanding, initial_Outstanding(), true);
            //search
            LV_OP.readColumnOrder(LV_search, Config.Customization_INI, "Search_Order");
            LV_OP.readColumnWidth(LV_search, Config.Customization_INI, "Search_Width");
            //Mytask 
            LV_OP.readColumnOrder(LV_mytask, Config.Customization_INI, "MyTask_Order");
            LV_OP.readColumnWidth(LV_mytask, Config.Customization_INI, "MyTask_Width");
            LV_OP.initialListView(LV_mytask, initial_MyTask(), true);
            //CIM
            LV_OP.initialListView(LV_CIM, CIMList, true);
            //Reminder
            LV_OP.readColumnOrder(LV_Reminder, Config.Customization_INI, "Reminder_Order");
            LV_OP.readColumnWidth(LV_Reminder, Config.Customization_INI, "Reminder_Width");
            LV_OP.readColumnOrder(LV_Reminder_Expired, Config.Customization_INI, "Reminder_Order");
            LV_OP.readColumnWidth(LV_Reminder_Expired, Config.Customization_INI, "Reminder_Width");
            LV_OP.initialListView(LV_Reminder, Reminder_OP.getNoExpiredCases(), true);
            LV_OP.initialListView(LV_Reminder_Expired, Reminder_OP.getExpiredCases(), true);
            //Notices
            DGV_Op.initialDGV(Notice, Config.UI_NoticeKeys, initial_Notices());
            //UserUpdate
            initialUserUpdate();

            utility.setFont(this, Config.Font_Content);

            CD_followUps.Font = Config.Font_Content;
            if (IsSuperUser)
            {
                CIM_add.Visible = true;
            }
            Combo_OP.initialComboBox(QI_CaseType, CaseProperty["Type"], ",");
            Combo_OP.initialComboBox(QI_IssueType, CaseProperty["Issue"], ",");
            Combo_OP.initialComboBox(QI_Product, ProductCate_Pair.Keys.ToArray());
            Combo_OP.initialComboBox(QI_Client, ClientList);

            Combo_OP.initialComboBox(Combo_Search_Cate, Config.CategoryType1);
            Combo_OP.initialComboBox(CIM_client, CIMList.Keys.ToArray());

            Combo_OP.initialComboBox(CD_client, ClientList);
            Combo_OP.initialComboBox(CD_status, CaseProperty["Status"], ",");
            Combo_OP.initialComboBox(CD_priority, CaseProperty["Priority"], ",");
            Combo_OP.initialComboBox(CD_caseType, CaseProperty["Type"], ",");
            Combo_OP.initialComboBox(CD_inchange, UserList);
            Combo_OP.initialComboBox(CD_issueType, CaseProperty["Issue"], ",");
            Combo_OP.initialComboBox(CD_through, CaseProperty["Through"], ",");
            Combo_OP.initialComboBox(CD_product, ProductCate_Pair.Keys.ToArray());
            Combo_OP.initialToolComboBox(toolStripComboBox1, MarkedCateCases.Keys.ToList());

            EventHandler copyInfo = delegate(object s, EventArgs k)
            {
                ListViewItem lvi = (ListViewItem)s;
                string client = LV_OP.getColumnValue(lvi, Config.UI_ClientContactKeys, "4");
                if (QI_Client.Text.Trim().Equals(client))
                {
                    string contact = LV_OP.getColumnValue(lvi, Config.UI_ClientContactKeys, "111");
                    string phone1 = LV_OP.getColumnValue(lvi, Config.UI_ClientContactKeys, "113");
                    string phone2 = LV_OP.getColumnValue(lvi, Config.UI_ClientContactKeys, "114");
                    QI_CallPerson.BeginUpdate();
                    QI_CallPerson.Text = contact;
                    QI_CallPerson.EndUpdate();
                    QI_Phone.BeginUpdate();
                    if (!phone1.Equals(""))
                    {
                        QI_Phone.Text = phone1;
                    }
                    else
                    {
                        QI_Phone.Text = phone2;
                    }
                    QI_Phone.EndUpdate();
                }
            };
            CopyCIMInfo.Add(tabPage1, copyInfo);
            QuickAdd.Add(tabPage1, new EventHandler(button2_Click));
            QuickClear.Add(tabPage1, new EventHandler(button3_Click));
            CD_followUps.Columns[0].ReadOnly = true;

            Config.logWriter.writeLog(myInfo.MyName + " login successfully!");


            if (!Config.SLS_Sock.establish(new SLSocket.Del_handleMsg(handleMsg), myInfo))
            {
                MessageBox.Show("Disconnected to SLS, you can only search informations");
                warning.Text = "Disconnected to SLS, you can only search informations";
                warning.ForeColor = Color.IndianRed;
            }
            UI_updateTotalOutstandingCase();
        }

        private void initialUserUpdate()
        {
            int index = Config.getIndex(Config.UI_CaseKeys, "10")+1;
            for (int i = 0; i < UserList.Count; i++)
            {
                outstandingCaseOfUser.Add(UserList[i], 0);
            }
            if (index > 0)
            {
                foreach (ListViewItem lvi in OutstandingCases.Values)
                {
                    string name = lvi.SubItems[index].Text;
                    try
                    {
                        if (outstandingCaseOfUser.ContainsKey(name))
                        {
                            outstandingCaseOfUser[name]++;
                        }
                    }
                    catch { }
                }
            }
            UI_initialUserUpdate();
        }

        private void UI_initialUserUpdate()
        {
            LV_update.BeginUpdate();
            LV_update.Items.Clear();
            foreach (var user in outstandingCaseOfUser)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.SubItems.Add(user.Key);
                lvi.SubItems.Add("");
                lvi.SubItems.Add(user.Value.ToString());
                LV_update.Items.Add(lvi);
            }
            LV_update.EndUpdate();
        }

        private void initialShortCutKey()
        {
            for (int i = 0; i < Config.shortCutKeys.Length; i++)
            {
                ShortCutKeyIni.Add(Config.shortCutKeys[i], Config.Customization_INI.readValue("ShortCutKey", Config.shortCutKeys[i]));
            }
        }

        private void saveShortcutKey()
        {
            foreach (string value in ShortCutKeyIni.Keys)
            {
                Config.Customization_INI.writeValue("ShortCutKey", value, ShortCutKeyIni[value]);
            }
        }

        private void initialMarkedCases()
        {
            MarkedCateCases = MarkedCases_OP.getCateCases();
        }

        public List<Msg_OP> handleMsg(List<Msg_OP> msg_Ops)
        {
            int count = msg_Ops.Count;
            List<Msg_OP> toRemove = new List<Msg_OP>();
            for (int i = 0; i < count; i++)
            {
                if (Config.highFrequencyType.Contains(msg_Ops[i].msgType))
                {
                    if (!BW_HighFrequency.IsBusy)
                    {
                        BW_HighFrequency.RunWorkerAsync(msg_Ops[i]);
                        toRemove.Add(msg_Ops[i]);
                    }
                }
                else if (Config.caseType.Contains(msg_Ops[i].msgType))
                {
                    if (!BW_Case.IsBusy)
                    {
                        BW_Case.RunWorkerAsync(msg_Ops[i]);
                        toRemove.Add(msg_Ops[i]);
                    }
                }
                else if (Config.lowFrequencyType.Contains(msg_Ops[i].msgType))
                {
                    if (!BW_LowFrequency.IsBusy)
                    {
                        BW_LowFrequency.RunWorkerAsync(msg_Ops[i]);
                        toRemove.Add(msg_Ops[i]);
                    }
                }
                else if (Config.IOType.Contains(msg_Ops[i].msgType))
                {
                    if (!BW_IO.IsBusy)
                    {
                        BW_IO.RunWorkerAsync(msg_Ops[i]);
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

        public string getCateDescription(string category)
        {
            return MarkedCases_OP.getCateDescription(category);
        }

        public Label cloneLabel(Label y)
        {
            Label x = new Label();
            x.AutoSize = y.AutoSize;
            x.Font = y.Font;
            x.Location = y.Location;
            x.Size = y.Size;
            x.TabIndex = y.TabIndex;
            x.Text = y.Text;
            x.ForeColor = y.ForeColor;
            x.Anchor = y.Anchor;
            return x;
        }

        public TextBox cloneTextBox(TextBox y)
        {
            TextBox x = new TextBox();
            x.BackColor = y.BackColor;
            x.Font = y.Font;
            x.Location = y.Location;
            x.Margin = y.Margin;
            x.RightToLeft = y.RightToLeft;
            x.Size = y.Size;
            x.TabIndex = y.TabIndex;
            x.BorderStyle = y.BorderStyle;
            x.Multiline = y.Multiline;
            x.Anchor = y.Anchor;
            return x;
        }

        public RichTextBox cloneRichTextBox(RichTextBox y)
        {
            RichTextBox x = new RichTextBox();
            x.Anchor = y.Anchor;
            x.BackColor = y.BackColor;
            x.BorderStyle = y.BorderStyle;
            x.Location = y.Location;
            x.Margin = y.Margin;
            x.Multiline = y.Multiline;
            x.RightToLeft = y.RightToLeft;
            x.ScrollBars = y.ScrollBars;
            x.Size = y.Size;
            x.TabIndex = y.TabIndex;
            x.Font = y.Font;
            return x;
        }

        public ComboBox cloneComboBox( ComboBox y)
        {
            ComboBox x = new ComboBox();
            x.AutoCompleteMode = y.AutoCompleteMode;
            x.AutoCompleteSource = y.AutoCompleteSource;
            x.BackColor = y.BackColor;
            x.ForeColor = y.ForeColor;
            x.FormattingEnabled = y.FormattingEnabled;
            x.Location = y.Location;
            x.Margin = y.Margin;
            x.RightToLeft = y.RightToLeft;
            x.Size = y.Size;
            x.TabIndex = y.TabIndex;
            x.Font = y.Font;
            x.FlatStyle = y.FlatStyle;
            x.Anchor = y.Anchor;
            return x;
        }

        public Button cloneButton( Button y)
        {
            Button x = new Button();
            x.Anchor = y.Anchor;
            x.BackColor = y.BackColor;
            x.FlatStyle = y.FlatStyle;
            x.Font = y.Font;
            x.ForeColor = y.ForeColor;
            x.Location = y.Location;
            x.Margin = y.Margin;
            x.Size = y.Size;
            x.TabIndex = y.TabIndex;
            x.Text = y.Text;
            x.UseVisualStyleBackColor = y.UseVisualStyleBackColor;
            return x;
        }

        public TabControl cloneTabControl(TabControl y)
        {
            TabControl x = new TabControl();
            x.Location = y.Location;
            x.Size = y.Size;
            x.TabIndex = y.TabIndex;
            x.Font = y.Font;
            x.Anchor = y.Anchor;
            return x;
        }

        public TabPage cloneTabPage(TabPage y)
        {
            TabPage x = new TabPage();
            x.Location = y.Location;
            x.Size = y.Size;
            x.TabIndex = y.TabIndex;
            x.Padding = y.Padding;
            x.Text = "New";
            x.UseVisualStyleBackColor = y.UseVisualStyleBackColor;
            x.BackColor = y.BackColor;
            x.Font = y.Font;
            x.Anchor = y.Anchor;
            return x;
        }

        public ListView cloneListView(ListView y)
        {
            ListView x = new ListView();
            x.BackColor = y.BackColor;
            x.BorderStyle = y.BorderStyle;
            x.FullRowSelect = y.FullRowSelect;
            x.GridLines = y.GridLines;
            x.HideSelection = y.HideSelection;
            x.Location = y.Location;
            x.Size = y.Size;
            x.TabIndex = y.TabIndex;
            x.UseCompatibleStateImageBehavior = y.UseCompatibleStateImageBehavior;
            x.View = y.View;
            x.Font = y.Font;
            x.Anchor = y.Anchor;
            return x;
        }

        public ColumnHeader cloneColumnHeader( ColumnHeader y)
        {
            ColumnHeader x = new ColumnHeader();
            x.Text = y.Text;
            x.Width = y.Width;
            return x;
        }

        private void Case_Click(object sender, EventArgs e)
        {
            Button add = cloneButton(this.QI_add);
            Button close = cloneButton(this.QI_clear);
            Label label1 = cloneLabel(this.label1);
            Label label2 = cloneLabel(this.label2);
            Label label3 = cloneLabel(this.label3);
            Label label4 = cloneLabel(this.label4);
            Label label5 = cloneLabel(this.label5);
            Label label6 = cloneLabel(this.label6);
            Label label7 = cloneLabel(this.label29);
            Label label8 = cloneLabel(this.label32);
            RichTextBox happenTime = cloneRichTextBox(QI_HappenTime);
            TextBox description = cloneTextBox(QI_description);
            ComboBox Combo_client = cloneComboBox(QI_Client);
            ComboBox Combo_callperson = cloneComboBox(QI_CallPerson);
            ComboBox Combo_phone = cloneComboBox(QI_Phone);
            ComboBox Combo_product = cloneComboBox(QI_Product);
            ComboBox Combo_caseType = cloneComboBox(QI_CaseType);
            ComboBox Combo_issueType = cloneComboBox(QI_IssueType);

            TabPage newPage = cloneTabPage(this.tabPage1);

            tabControl1.Controls.Add(newPage);
            tabControl1.SelectedTab = newPage;

            EventHandler copyInfo = delegate(object s, EventArgs k)
            {
                ListViewItem lvi = (ListViewItem)s;
                string client = LV_OP.getColumnValue(lvi, Config.UI_ClientContactKeys, "4");
                if (Combo_client.Text.Trim().Equals(client))
                {
                    string contact = LV_OP.getColumnValue(lvi, Config.UI_ClientContactKeys, "111");
                    string phone1 = LV_OP.getColumnValue(lvi, Config.UI_ClientContactKeys, "113");
                    string phone2 = LV_OP.getColumnValue(lvi, Config.UI_ClientContactKeys, "114");
                    Combo_callperson.BeginUpdate();
                    Combo_callperson.Text = contact;
                    Combo_callperson.EndUpdate();
                    Combo_phone.BeginUpdate();
                    if (!phone1.Equals(""))
                    {
                        Combo_phone.Text = phone1;
                    }
                    else
                    {
                        Combo_phone.Text = phone2;
                    }
                    Combo_phone.EndUpdate();
                }
            };

            CopyCIMInfo.Add(newPage, copyInfo);

            EventHandler closeEv= delegate(object s, EventArgs k)
            {
                message msg = new message();
                msg.setKeyValuePair("17", "T" + tabControl1.SelectedIndex);
                msg.setKeyValuePair("80", myInfo.MyName);
                if (Config.SLS_Sock.socketMsg("R2", msg, newPage))
                {
                    tabControl1.SelectedIndex = tabControl1.SelectedIndex - 1 < 0 ? tabControl1.TabCount - 1 : tabControl1.SelectedIndex - 1;
                    QuickClear.Remove(newPage);
                    QuickAdd.Remove(newPage);
                }
            };
            close.Click += closeEv;
            QuickClear.Add(newPage, closeEv);

            EventHandler enterDescription = delegate(object s, EventArgs k)
            {
                description.ScrollBars = ScrollBars.Vertical;
            };
            description.Enter += enterDescription;

            EventHandler leaveDescription = delegate(object s, EventArgs k)
            {
                description.ScrollBars = ScrollBars.None;
            };
            description.Leave += leaveDescription;

            EventHandler addEv= delegate(object s, EventArgs k)
            {
                if (!Combo_client.Items.Contains(Combo_client.Text))
                {
                    MessageBox.Show("Input client is not in clientlist !");
                }
                else
                {
                    message msg = new message();
                    msg.setKeyValuePair("1", "");
                    if (!happenTime.Text.Trim(' ').Equals(""))
                    {
                        msg.setKeyValuePair("3", happenTime.Text);
                    }
                    msg.setKeyValuePair("4", Combo_client.Text);
                    msg.setKeyValuePair("5", Combo_callperson.Text);
                    msg.setKeyValuePair("6", Combo_phone.Text);
                    msg.setKeyValuePair("8", description.Text);
                    msg.setKeyValuePair("9", Combo_caseType.Text);
                    msg.setKeyValuePair("10", "Unassigned");
                    msg.setKeyValuePair("11", "Follow");
                    msg.setKeyValuePair("14", Combo_issueType.Text);
                    msg.setKeyValuePair("167", Combo_product.Text);
                    msg.setKeyValuePair("17", "T" + tabControl1.SelectedIndex);
                    if (Config.SLS_Sock.socketMsg("C1", msg, newPage))
                    {
                        QuickClear.Remove(newPage);
                        QuickAdd.Remove(newPage);
                    }
                }
            };
            add.Click += addEv;
            QuickAdd.Add(newPage, addEv);

            Combo_OP.initialComboBox(Combo_client, ClientList);
            Combo_OP.initialComboBox(Combo_caseType, CaseProperty["Type"].ToString(), ",");
            Combo_OP.initialComboBox(Combo_issueType, CaseProperty["Issue"].ToString(), ",");
            Combo_OP.initialComboBox(Combo_product, ProductCate_Pair.Keys.ToArray());

            Combo_callperson.TextChanged += delegate(object s, EventArgs k)
            {
                utility.loadContactPhone(Combo_client, Combo_callperson, Combo_phone, CIMList);
            };

            Combo_client.SelectedIndexChanged += delegate(object s, EventArgs k)
            {
                string clientName = Combo_client.Text.Trim(' ');
                if (Combo_client.Items.Contains(clientName))
                {
                    tabControl2.SelectedIndex = 3;
                    CIM_client.Text = clientName;
                    //locateClientInCIM(clientName);
                    utility.loadClientContact(Combo_client, Combo_callperson, CIMList);
                    if (!Combo_Search_Cate.Text.Trim().Equals("Client") || !Combo_Search_Subcate.Text.Trim().Equals(clientName))
                    {
                        Combo_Search_Cate.Text = "Client";
                        Combo_Search_Subcate.Text = clientName;
                    }
                    else
                    {
                        searchOutstandingCateCases("Client", clientName);
                    }
                    message msg = new message();
                    msg.setKeyValuePair("1", clientName);
                    msg.setKeyValuePair("80", myInfo.MyName);
                    msg.setKeyValuePair("17", "T" + tabControl1.SelectedIndex);
                    Config.SLS_Sock.socketMsg("R1", msg, null);
                    Combo_client.Focus();
                }
            };

            Combo_client.TextChanged += delegate(object s, EventArgs k)
            {
                if (Combo_client.Text.Trim(' ').Equals(""))
                {
                    newPage.Text = "Input";
                }
                else
                {
                    newPage.Text = Combo_client.Text.Trim(' ');
                }
            };

            newPage.Controls.Add(Combo_client);
            newPage.Controls.Add(Combo_callperson);
            newPage.Controls.Add(Combo_product);
            newPage.Controls.Add(Combo_phone);
            newPage.Controls.Add(Combo_issueType);
            newPage.Controls.Add(Combo_caseType);
            newPage.Controls.Add(happenTime);
            newPage.Controls.Add(description);
            newPage.Controls.Add(label1);
            newPage.Controls.Add(label2);
            newPage.Controls.Add(label3);
            newPage.Controls.Add(label4);
            newPage.Controls.Add(label5);
            newPage.Controls.Add(label6);
            newPage.Controls.Add(label7);
            newPage.Controls.Add(label8);
            newPage.Controls.Add(add);
            newPage.Controls.Add(close);

            Combo_client.Focus();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string client = QI_Client.Text.Trim();
            if (!QI_Client.Items.Contains(client))
            {
                MessageBox.Show("Input client is not in client list !");
            }
            else
            {
                message msg = new message();
                msg.setKeyValuePair("1", "");
                if (!QI_HappenTime.Text.Trim(' ').Equals(""))
                {
                    msg.setKeyValuePair("3", QI_HappenTime.Text);
                }
                msg.setKeyValuePair("4", QI_Client.Text);
                msg.setKeyValuePair("5", QI_CallPerson.Text);
                msg.setKeyValuePair("6", QI_Phone.Text);
                msg.setKeyValuePair("8", QI_description.Text);
                msg.setKeyValuePair("9", QI_CaseType.Text);
                msg.setKeyValuePair("10", "Unassigned");
                msg.setKeyValuePair("11", "Follow");
                msg.setKeyValuePair("14", QI_IssueType.Text);
                msg.setKeyValuePair("167", QI_Product.Text);
                msg.setKeyValuePair("17", "T" + tabControl1.SelectedIndex);
                if (Config.SLS_Sock.socketMsg("C1", msg, null))
                {
                    utility.clearContent(tabPage1);
                    tabPage1.Text = "Input";
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            ActCase NewCase = new ActCase(myInfo,NewCaseFormCounter,CaseProperty, ClientList, UserList, ProductCate_Pair, ClientServerProductVersion, CIMList, ClientAM, AMPhone);
            Interlocked.Increment(ref NewCaseFormCounter);
            NewCase.Show();
        }

        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (LV_outstanding.SelectedItems.Count > 0)
            {
                ActCase EditCase = new ActCase(myInfo,LV_outstanding.SelectedItems[0], CaseProperty, ClientList, UserList, ProductCate_Pair, ClientServerProductVersion,CIMList, ClientAM, AMPhone);
                EditCase.Show();
            }  
        }

        private void button3_Click(object sender, EventArgs e)
        {
            message msg = new message();
            msg.setKeyValuePair("17", "T0");
            msg.setKeyValuePair("80", myInfo.MyName);
            if (Config.SLS_Sock.socketMsg("R2", msg, null))
            {
                utility.clearContent(tabPage1);
                tabPage1.Text = "Input";
            }
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string client = QI_Client.Text.Trim(' ');        
            if (QI_Client.Items.Contains(client))
            {
                QI_AM.Text = utility.getAMInfo(client, ClientAM, AMPhone);
                utility.loadClientContact(QI_Client, QI_CallPerson, CIMList);
                CIM_client.Text = client;
                //locateClientInCIM(client);       
                tabControl2.SelectedIndex = 3;
                if (!Combo_Search_Cate.Text.Trim().Equals("Client") || !Combo_Search_Subcate.Text.Trim().Equals(client))
                {
                    Combo_Search_Cate.Text = "Client";
                    Combo_Search_Subcate.Text = client;
                }
                else
                {
                    searchOutstandingCateCases("Client", client);
                }
                message msg = new message();
                msg.setKeyValuePair("1", client);
                msg.setKeyValuePair("80", myInfo.MyName);
                msg.setKeyValuePair("17", "T0");
                Config.SLS_Sock.socketMsg("R1", msg, null);
            }
            QI_Client.Focus();
        }

        private void listView1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (LV_outstanding.SelectedItems.Count > 0)
                {
                    ActCase EditCase = new ActCase(myInfo,LV_outstanding.SelectedItems[0], CaseProperty, ClientList, UserList, ProductCate_Pair, ClientServerProductVersion,CIMList, ClientAM, AMPhone);
                    EditCase.Show();
                }
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (TB_CaseID.Text != "")
            {
                message msg = new message();
                msg.setKeyValuePair("1", TB_CaseID.Text);
                Msg_OP msg_Op = new Msg_OP("C4", new message[] { msg });
               Config.SLS_Sock.inMsgQueue_add(msg_Op);
            }
            else
            {
                MessageBox.Show("Please input CaseID !");
            }
        }

        private void listView2_DoubleClick(object sender, EventArgs e)
        {
            if (LV_search.SelectedItems.Count > 0)
            {
                ActCase EditCase = new ActCase(myInfo,LV_search.SelectedItems[0], CaseProperty,ClientList, UserList, ProductCate_Pair, ClientServerProductVersion, CIMList, ClientAM, AMPhone);
                EditCase.Show();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            LV_search.Items.Clear();
            TB_CaseID.Clear();
            Combo_Search_Cate.Text = "";
            Combo_Search_Subcate.Text = "";
            label_SearchCount.Hide();
        }

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.Control)
                {
                    string key = e.KeyCode.ToString();
                    foreach (string SCName in ShortCutKeyIni.Keys)
                    {
                        if (ShortCutKeyIni[SCName].Equals(key))
                        {
                            switch (SCName)
                            {
                                case "QI_New":
                                    {
                                        Case_Click(null, null);
                                        break;
                                    }
                                case "QI_Add":
                                    {
                                        QuickAdd[tabControl1.SelectedTab](null, null);
                                        break;
                                    }
                                case "QI_Clear":
                                    {
                                        QuickClear[tabControl1.SelectedTab].Invoke(null, null);
                                        break;
                                    }
                                case "QI_SwitchLeft":
                                    {
                                        tabControl1.SelectedIndex = tabControl1.SelectedIndex - 1 < 0 ? tabControl1.TabCount - 1 : tabControl1.SelectedIndex - 1;
                                        break;
                                    }
                                case "QI_SwitchRight":
                                    {
                                        tabControl1.SelectedIndex = tabControl1.SelectedIndex + 1 < tabControl1.TabCount ? tabControl1.SelectedIndex + 1 : 0;
                                        break;
                                    }
                                case "Case_New":
                                    {
                                        button4_Click(null, null);
                                        break;
                                    }
                                case "Case_Refresh":
                                    {
                                        button5_Click(null, null);
                                        break;
                                    }
                                case "Case_Close":
                                    {
                                        CloseCase_Click(null, null);
                                        break;
                                    }
                                case "Case_Multifilter":
                                    {
                                        button10_Click(null, null);
                                        break;
                                    }
                                case "Tab_Outstanding":
                                    {
                                        tabControl2.SelectedIndex = 0;
                                        break;
                                    }
                                case "Tab_Search":
                                    {
                                        tabControl2.SelectedIndex = 1;
                                        break;
                                    }
                                case "Tab_MyTask":
                                    {
                                        tabControl2.SelectedIndex = 2;
                                        break;
                                    }
                                case "Tab_CIM":
                                    {
                                        tabControl2.SelectedIndex = 3;
                                        break;
                                    }
                                case "Tab_Notice":
                                    {
                                        tabControl2.SelectedIndex = 4;
                                        break;
                                    }
                                case "Tab_Reminder":
                                    {
                                        tabControl2.SelectedIndex = 5;
                                        break;
                                    }
                                default: break;
                            }
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }
        
        private void listView3_DoubleClick(object sender, EventArgs e)
        {
            if (LV_mytask.SelectedItems.Count > 0)
            {
                ActCase EditCase = new ActCase(myInfo,LV_mytask.SelectedItems[0], CaseProperty, ClientList, UserList, ProductCate_Pair, ClientServerProductVersion,CIMList, ClientAM, AMPhone);
                EditCase.Show();
            }
        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != 0)
            {
                LV_outstanding.ListViewItemSorter = new ListViewItemsComparer(e.Column, SortType_Outstanding);
                LV_outstanding.Sort();
                SortType_Outstanding = !SortType_Outstanding;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            MultiFilter multiFilter = new MultiFilter(CaseProperty,CIMList,ClientList,UserList,ProductCate_Pair,ClientServerProductVersion);
            multiFilter.Show();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            UI_initialCase();
            MyTaskCount = 0;
            OutstandingCount = 0;
            tabPage2.Text = "Outstanding";
            tabPage4.Text = "MyTask";
        }

        private void listView2_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != 0)
            {
                LV_search.ListViewItemSorter = new ListViewItemsComparer(e.Column, SortType_Search);
                LV_search.Sort();
                SortType_Search = !SortType_Search;
            }
        }

        private void listView3_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (e.Column != 0)
            {
                LV_mytask.ListViewItemSorter = new ListViewItemsComparer(e.Column, SortType_Mytask);
                LV_mytask.Sort();
                SortType_Mytask = !SortType_Mytask;
            }
        }

        private void listView2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (LV_search.SelectedItems.Count > 0)
                {
                    ActCase EditCase = new ActCase(myInfo,LV_search.SelectedItems[0], CaseProperty,ClientList, UserList, ProductCate_Pair, ClientServerProductVersion, CIMList, ClientAM, AMPhone);
                    EditCase.Show();
                }
            }
        }

        private void listView3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (LV_mytask.SelectedItems.Count > 0)
                {
                    ActCase EditCase = new ActCase(myInfo,LV_mytask.SelectedItems[0], CaseProperty,ClientList, UserList, ProductCate_Pair, ClientServerProductVersion, CIMList, ClientAM, AMPhone);
                    EditCase.Show();
                }
            }
        }

        private void textBox7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                button6_Click(null, null);
            }
        }

        private void initialCaseContent(ListViewItem lvi)
        {
            try
            {
                tempCaseMsg.setKeyValuePair("1", lvi.SubItems[1].Text);
                tempCaseMsg.setKeyValuePair("2", lvi.SubItems[2].Text);
                groupBox1.Text = lvi.SubItems[1].Text;
                CD_happenTime.Text = lvi.SubItems[3].Text;
                CD_client.Text = lvi.SubItems[4].Text;
                CD_callPerson.Text = lvi.SubItems[5].Text;
                CD_phone.Text = lvi.SubItems[6].Text;
                if (lvi.SubItems[7].Text.Equals(""))
                {
                    CD_through.SelectedIndex = -1;
                }
                else
                {
                    CD_through.Text = lvi.SubItems[7].Text;
                }
                CD_description.Text = lvi.SubItems[8].Text;
                if (lvi.SubItems[9].Text.Equals(""))
                {
                    CD_caseType.SelectedIndex = -1;
                }
                else
                {
                    CD_caseType.Text = lvi.SubItems[9].Text;
                }
                CD_inchange.Text = lvi.SubItems[10].Text;
                CD_status.Text = lvi.SubItems[11].Text;
                CD_solution.Text = lvi.SubItems[12].Text;
                if (lvi.SubItems[13].Text.Equals(""))
                {
                    CD_priority.SelectedIndex = -1;
                }
                else
                {
                    CD_priority.Text = lvi.SubItems[13].Text;
                }
                if (lvi.SubItems[14].Text.Equals(""))
                {
                    CD_issueType.SelectedIndex = -1;
                }
                else
                {
                    CD_issueType.Text = lvi.SubItems[14].Text;
                }
                CD_product.Text = lvi.SubItems[15].Text;
                CD_server.Text = lvi.SubItems[16].Text;
                CD_version.Text = lvi.SubItems[17].Text;
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void clickCase(ListView lv, ListViewItem lvi, bool ifUpdate, ref int caseCount, TabPage tab, string tabName)
        {
            if (LV_OP.IsNewOrEdited(lvi))
            {
                if (ifUpdate)
                {
                    utility.minusToUnNegative(ref caseCount);
                    tab.Text = tabName + utility.getPositiveNo(caseCount);
                }
                lv.BeginUpdate();
                LV_OP.changeLviBackColor(lvi, Config.UI_CaseKeys);
                lv.EndUpdate();
            }
            initialCaseContent(lvi);
            int index = Config.getIndex(Config.UI_CaseKeys, "1");
            if (index >= 0)
            {
                string caseID = lvi.SubItems[++index].Text;
                message msg = new message();
                msg.setKeyValuePair("1", caseID);
                CD_followUps.Rows.Clear();
                Config.SLS_Sock.socketMsg("F0", msg, null);
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            ShortCutKey shortCutKey = new ShortCutKey(this);
            shortCutKey.Show();
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            utility.loadContactPhone(QI_Client, QI_CallPerson, QI_Phone, CIMList);
        }

        private void locateClientInCIM(string client)
        {
            LV_CIM.BeginUpdate();
            if (CIMList.Keys.Contains(client))
            {
                LV_CIM.TopItem = CIMList[client][0];
            }
            else
            {
                LV_CIM.TopItem = LV_CIM.Items[0];
            }
            LV_CIM.EndUpdate();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            string phone = CIM_phone.Text.Trim(' ');
            int index1 = LV_OP.getLviIndex(LV_CIM, Config.UI_ClientContactKeys, "113", phone);
            if (index1 >= 0)
            {
                LV_CIM.TopItem = LV_CIM.Items[index1];
                LV_CIM.Items[index1].BackColor = LV_OP.Color_EditItem;
            }
            else
            {
                int index2 = LV_OP.getLviIndex(LV_CIM, Config.UI_ClientContactKeys, "114", phone);
                if (index2 >= 0)
                {
                    LV_CIM.TopItem = LV_CIM.Items[index2];
                    LV_CIM.Items[index2].BackColor = LV_OP.Color_EditItem;
                }
                else
                {
                    MessageBox.Show("Phone No doesn't exist in CIM !");
                }
            }    
        }

        private void button14_Click(object sender, EventArgs e)
        {
            ActClientContact AddContact = new ActClientContact(CIMList);
            AddContact.Show();
        }

        private void toolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (IsSuperUser)
            {
                if (Form_Superuser == null)
                {
                    Form_Superuser = new SuperUser(ListLVI_ClientList_SuperUser,ListLVI_UserFile,ListLVI_AM,CaseProperty,UserIDNameMapping,CaseProperty_AM);
                    Form_Superuser.Show();
                }
                else if (Form_Superuser.IsDisposed)
                {
                    Form_Superuser = new SuperUser(ListLVI_ClientList_SuperUser, ListLVI_UserFile, ListLVI_AM,CaseProperty,UserIDNameMapping,CaseProperty_AM);
                    Form_Superuser.Show();
                }
                else
                {
                    Form_Superuser.WindowState = FormWindowState.Normal;
                    Form_Superuser.Focus();
                }
            }
            else
            {
                MessageBox.Show("You are not SuperUser, for SuperUser functions, please ask " + SuperUsers);
            }
        }

        private void outstandingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedTab = tabPage2;
        }

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedTab = tabPage3;
        }

        private void myTaskToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedTab = tabPage4;
        }

        private void cIMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedTab = tabPage5;
        }

        private void UI_addClientContact(Msg_OP msg_Op)
        {
            string client = msg_Op.getValueFromPairs("4"),contact = msg_Op.getValueFromPairs("111");
            ListViewItem lvi = LV_OP.getLVIs(msg_Op,Config.UI_ClientContactKeys)[0];
            if (CIMList.Keys.Contains(client))
            {                
                int index = Config.getIndex(Config.UI_ClientContactKeys, "111");
                if (index >= 0)
                {
                    index++;
                    int offset = 0;
                    for (offset = 0; offset < CIMList[client].Count; offset++)
                    {
                        if (CIMList[client][offset].SubItems[index].Text.CompareTo(contact) > 0)
                        {      
                            break;
                        }
                    } 
                    for (int i = 0; i < LV_CIM.Items.Count; i++)
                    {               
                        if (ReferenceEquals(LV_CIM.Items[i] , CIMList[client][0]))
                        {
                            LV_OP.insertOneListviewItem(LV_CIM, offset+i, lvi);                     
                            break;
                        }
                    }
                    CIMList[client].Insert(offset, lvi);   
                }
            }
            else
            {
                LV_OP.insertOneListviewItem(LV_CIM, 0, lvi);
                CIMList.Add(client, new List<ListViewItem>() { lvi });
                Combo_OP.initialComboBox(CIM_client, CIMList.Keys.ToArray());
            }
        }

        private void UI_editClientContact(Msg_OP msg_Op)
        {
            string client = msg_Op.getValueFromPairs("4");
            LV_OP.updateOneListviewItem(CIMList[client], Config.UI_ClientContactKeys, Config.UI_Index_ClientContactKeys, Config.UI_IndexValue_ClientContactKeys, msg_Op.getMsgs()[0]);
        }

        private void UI_deleteClientContact(Msg_OP msg_Op)
        {
            string client = msg_Op.getValueFromPairs("4");
            if (CIMList.ContainsKey(client))
            {
                ListViewItem lvi = LV_OP.findListviewItem(CIMList[client], Config.UI_ClientContactKeys, Config.UI_Index_ClientContactKeys, msg_Op.getMsgs()[0]);
                if (CIMList[client].Contains(lvi))
                {
                    CIMList[client].Remove(lvi);
                }
                if (LV_CIM.Items.Contains(lvi))
                {
                    LV_CIM.Items.Remove(lvi);
                }
                if (CIMList[client].Count == 0)
                {
                    CIMList.Remove(client);
                    Combo_OP.initialComboBox(CIM_client, CIMList.Keys.ToArray());
                }
            }
        }

        private void addCase(Msg_OP msg_Op)
        {
            if (msg_Op.getValueFromPairs("10").Equals(myInfo.MyName))
            {
                MyTaskCount = Interlocked.Increment(ref MyTaskCount);
            }
            OutstandingCount = Interlocked.Increment(ref OutstandingCount);
            this.Invoke(new Msg_OP.Del_MsgOp(UI_addCase), new object[] { msg_Op });
        }

        private void UI_addCase(Msg_OP msg_Op)
        {
            UI_updateHandlingCaseCount(msg_Op);
            string status = msg_Op.getValueFromPairs("11");
            string caseID = msg_Op.getValueFromPairs("1");
            ListViewItem lvi1 = LV_OP.getLVIs(msg_Op, Config.UI_CaseKeys)[0];
            LV_OP.insertOneListviewItem(LV_outstanding, 0, lvi1);
            OutstandingCases.Add(caseID, lvi1);
            string Incharge = msg_Op.getValueFromPairs("10");
            tabPage2.Text = "Outstanding" + utility.getPositiveNo(OutstandingCount);
            if (Incharge.Equals(myInfo.MyName))
            {
                ListViewItem lvi2 = LV_OP.getLVIs(msg_Op, Config.UI_CaseKeys)[0];
                LV_OP.insertOneListviewItem(LV_mytask, 0, lvi2);
                MyCases.Add(caseID, lvi2);
                tabPage4.Text = "MyTask" + utility.getPositiveNo(MyTaskCount);
            }
            UI_updateTotalOutstandingCase();
            flashWindow();
        }

        private void UI_updateHandlingCaseCount(Msg_OP msg_Op)
        {
            string caseID = msg_Op.getValueFromPairs("1");
            string Incharge = msg_Op.getValueFromPairs("10");
            string exIncharge = "";
            string status = msg_Op.getValueFromPairs("11");
            string exStatus = "";
            ListViewItem caseLvi;
            int statusIndex = Config.getIndex(Config.UI_CaseKeys, "11") + 1;
            int inchargeIndex = Config.getIndex(Config.UI_CaseKeys, "10") + 1;
            if (OutstandingCases.ContainsKey(caseID))
            {
                caseLvi = OutstandingCases[caseID];
                if (statusIndex > 0 && inchargeIndex > 0)
                {
                    exIncharge = caseLvi.SubItems[inchargeIndex].Text;
                    exStatus = caseLvi.SubItems[statusIndex].Text;
                }
            }
            if (!exStatus.Equals("Closed"))
            {
                if (outstandingCaseOfUser.ContainsKey(exIncharge))
                {
                    int temp = outstandingCaseOfUser[exIncharge];
                    outstandingCaseOfUser[exIncharge] = utility.minusToUnNegative(ref temp);
                }
            }
            if (!status.Equals("Closed"))
            {
                if (outstandingCaseOfUser.ContainsKey(Incharge))
                {
                    int temp = outstandingCaseOfUser[Incharge];
                    outstandingCaseOfUser[Incharge] = Interlocked.Increment(ref temp);
                }
            }
            int nameIndex = Config.getIndex(Config.UI_UserUpdateKeys, "80") + 1;
            int countIndex = Config.getIndex(Config.UI_UserUpdateKeys, "19") + 1;
            if (nameIndex > 0 && countIndex > 0)
            {
                LV_update.BeginUpdate();
                foreach (ListViewItem lvi in LV_update.Items)
                {
                    if (lvi.SubItems[nameIndex].Text.Equals(Incharge))
                    {
                        lvi.SubItems[countIndex].Text = outstandingCaseOfUser[Incharge].ToString();
                    }
                    if (lvi.SubItems[nameIndex].Text.Equals(exIncharge))
                    {
                        lvi.SubItems[countIndex].Text = outstandingCaseOfUser[exIncharge].ToString();
                    }
                }
                LV_update.EndUpdate();
            }
        }

        private void UI_updateTotalOutstandingCase()
        {
            int count = 0;
            foreach (int i in outstandingCaseOfUser.Values)
            {
                count += i;
            }
            LabelCaseCount.Text = count + " outstanding cases";
        }

        private void editCase(Msg_OP msg_Op)
        {
            msg_Op.buildMsgs();
            this.Invoke(new Msg_OP.Del_MsgOp(UI_editCase), new object[] { msg_Op });
        }

        private void UI_editCase(Msg_OP msg_Op)
        {
            UI_updateHandlingCaseCount(msg_Op);
            editCase_Outstanding(msg_Op);
            editCase_MyTask(msg_Op);
            editCase_Search(msg_Op);
            editCase_Reminder(msg_Op);
            if (groupBox1.Text.Equals(msg_Op.getValueFromPairs("1")))
            {
                initialCaseContent(LV_OP.getLVIs(msg_Op, Config.UI_CaseKeys)[0]);
            }
            UI_updateTotalOutstandingCase();
            flashWindow();
        }

        private void editCase_Outstanding(Msg_OP msg_Op)
        {
            string caseID = msg_Op.getValueFromPairs("1");
            if (OutstandingCases.Keys.Contains(caseID))
            {
                if (!LV_OP.IsNewOrEdited(OutstandingCases[caseID]))
                {
                    OutstandingCount = Interlocked.Increment(ref OutstandingCount);
                }
                ListViewItem lvi = LV_OP.getLVIs(msg_Op, Config.UI_CaseKeys)[0];
                LV_OP.replaceOneListviewItem(LV_outstanding, OutstandingCases[caseID], lvi);
                OutstandingCases[caseID] = lvi;
            }
            else
            {
                OutstandingCount = Interlocked.Increment(ref OutstandingCount);
                ListViewItem lvi = LV_OP.getLVIs(msg_Op,Config.UI_CaseKeys)[0];
                LV_OP.insertOneListviewItem(LV_outstanding, 0, lvi);
                OutstandingCases.Add(caseID, lvi);
            }
            tabPage2.Text = "Outstanding" + utility.getPositiveNo(OutstandingCount);
            Config.logWriter.writeLog("update case " + caseID + " in outstanding done!");
        }

        private void editCase_MyTask(Msg_OP msg_Op)
        {
            try
            {
                string caseID = msg_Op.getValueFromPairs("1");
                if (msg_Op.getValueFromPairs("10").Equals(myInfo.MyName))
                {
                    if (MyCases.Keys.Contains(caseID))
                    {
                        // The edit case is my case, and exist in MyTask listview
                        if (msg_Op.getValueFromPairs("11").Equals("Closed"))
                        {
                            if (LV_OP.IsNewOrEdited(MyCases[caseID]))
                            {
                                utility.minusToUnNegative(ref MyTaskCount);
                            }
                            LV_OP.removeOneListviewItem(LV_mytask, MyCases[caseID]);
                            MyCases.Remove(caseID);
                        }
                        else
                        {
                            if (!LV_OP.IsNewOrEdited(MyCases[caseID]))
                            {
                                MyTaskCount = Interlocked.Increment(ref MyTaskCount);
                            }
                            ListViewItem lvi = LV_OP.getLVIs(msg_Op,Config.UI_CaseKeys)[0];
                            LV_OP.replaceOneListviewItem(LV_mytask, MyCases[caseID], lvi);
                            MyCases[caseID] = lvi;
                        }
                    }
                    else
                    {
                        // The eidt case is my case, but not in MyTask listview, should be a search case, then edit it
                        MyTaskCount = Interlocked.Increment(ref MyTaskCount);
                        ListViewItem lvi = LV_OP.getLVIs(msg_Op,Config.UI_CaseKeys)[0];
                        LV_OP.insertOneListviewItem(LV_mytask, 0, lvi);
                        MyCases.Add(caseID,lvi);
                    }
                }
                else
                {
                    if(MyCases.Keys.Contains(caseID))
                    {
                         // The edit case is used to be in MyTask listview, but been edited, now is not my case, should be removed
                        if (LV_OP.IsNewOrEdited(MyCases[caseID]))
                        {
                            utility.minusToUnNegative(ref MyTaskCount);
                        }
                        LV_OP.removeOneListviewItem(LV_mytask, MyCases[caseID]);
                    }
                }
                tabPage4.Text = "MyTask" + utility.getPositiveNo(MyTaskCount);
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void editCase_Search(Msg_OP msg_Op)
        {
            string caseID = msg_Op.getValueFromPairs("1");
            LV_OP.replaceOneListviewItem(LV_search, Config.UI_CaseKeys, Config.UI_Index_CaseKeys, msg_Op.getMsgs()[0]);
        }

        private void editCase_Reminder(Msg_OP msg_Op)
        {
            LV_OP.updateOneListviewItem(LV_Reminder, Config.UI_CaseKeys,  new string[] { "1" }, new string[] { "1" }, msg_Op.getMsgs()[0]);
            LV_OP.updateOneListviewItem(LV_Reminder_Expired, Config.UI_CaseKeys, new string[] { "1" }, new string[] { "1" }, msg_Op.getMsgs()[0]);
        }

        private void deleteCase(Msg_OP msg_Op)
        {
            string caseID = msg_Op.getValueFromPairs("1");
            Reminder_OP.removeReminder(caseID);
            this.Invoke(new Msg_OP.Del_MsgOp(UI_deleteCase), new object[] { msg_Op });
        }

        private void UI_deleteCase(Msg_OP msg_Op)
        {
            string caseID = msg_Op.getValueFromPairs("1");
            if (caseID.Equals(groupBox1.Text))
            {
                utility.clearContent(groupBox1);
            }
            UI_updateHandlingCaseCount(msg_Op);

            if (OutstandingCases.Keys.Contains(caseID))
            {
                if (LV_OP.IsNewOrEdited(OutstandingCases[caseID]))
                {
                    utility.minusToUnNegative(ref OutstandingCount);
                }
                LV_OP.removeOneListviewItem(LV_outstanding, OutstandingCases[caseID]);
                OutstandingCases.Remove(caseID);
            }
            if (MyCases.Keys.Contains(caseID))
            {
                if (LV_OP.IsNewOrEdited(MyCases[caseID]))
                {
                    utility.minusToUnNegative(ref MyTaskCount);
                }
                LV_OP.removeOneListviewItem(LV_mytask, MyCases[caseID]);
                MyCases.Remove(caseID);
            }
            message msg = msg_Op.getMsgs()[0];
            LV_OP.removeOneListviewItem(LV_Reminder, Config.UI_CaseKeys, Config.UI_Index_CaseKeys, msg);
            LV_OP.removeOneListviewItem(LV_Reminder_Expired, Config.UI_CaseKeys, Config.UI_Index_CaseKeys, msg);
            LV_OP.removeOneListviewItem(LV_search, Config.UI_CaseKeys, Config.UI_Index_CaseKeys, msg);

            tabPage2.Text = "Outstanding" + utility.getPositiveNo(OutstandingCount);
            tabPage4.Text = "MyTask" + utility.getPositiveNo(MyTaskCount);
            UI_updateTotalOutstandingCase();
        }

        private void UI_loadFollowUp(Msg_OP msg_Op)
        {
            if (msg_Op.sentFrom == myInfo.MyID)
            {
                string[] followUps = Regex.Split(msg_Op.getMsgContent(), Config.Msg_Separator3);
                if (followUps.Length > 0)
                {
                    message firstMsg = new message(followUps[0]);
                    string caseID = firstMsg.getValueFromPairs("1");
                    if (!caseID.Equals(groupBox1.Text))
                    {
                        return;
                    }
                    else
                    {
                        //CD_followUps.Rows.Clear();
                        CD_followUps.Rows.Add(firstMsg.getValuesFromKeys(Config.UI_CaseFollowUpKeys));
                    }

                    for (int i = 1; i < followUps.Length; i++)
                    {
                        message msg = new message(followUps[i]);
                        CD_followUps.Rows.Insert(0, msg.getValuesFromKeys(Config.UI_CaseFollowUpKeys));
                    }
                }
            }
        }

        private void UI_addFollowUp(Msg_OP msg_Op)
        {
            if (msg_Op.sentFrom == myInfo.MyID)
            {
                string[] followUps = Regex.Split(msg_Op.getMsgContent(), Config.Msg_Separator3);
                if (followUps.Length > 0)
                {
                    message firstMsg = new message(followUps[0]);
                    string caseID = firstMsg.getValueFromPairs("1");
                    if (!caseID.Equals(groupBox1.Text))
                    {
                        return;
                    }
                    else
                    {
                        CD_followUps.Rows.Clear();
                        CD_followUps.Rows.Add(firstMsg.getValuesFromKeys(Config.UI_CaseFollowUpKeys));
                    }

                    for (int i = 1; i < followUps.Length; i++)
                    {
                        message msg = new message(followUps[i]);
                        CD_followUps.Rows.Insert(0, msg.getValuesFromKeys(Config.UI_CaseFollowUpKeys));
                    }
                    if (!LV_OP.IsNewOrEdited(OutstandingCases[caseID]))
                    {
                        Interlocked.Increment(ref OutstandingCount);
                        tabPage2.Text = "Outstanding" + utility.getPositiveNo(OutstandingCount);
                        ListViewItem toReplace = LV_OP.cloneLVI(OutstandingCases[caseID]);
                        LV_OP.replaceOneListviewItem(LV_outstanding, OutstandingCases[caseID], toReplace);
                        OutstandingCases[caseID] = toReplace;
                        flashWindow();
                    }
                }
            }
        }

        private void UI_loadCaseHint(Msg_OP msg_Op)
        {
            string caseID = msg_Op.getValueFromPairs("1"), caseHint = msg_Op.getValueFromPairs("73");
            if (caseID.Equals(groupBox1.Text))
            {
                CD_caseHints.Text = caseHint;
            }
        }

        private void UI_editCaseHint(Msg_OP msg_Op)
        {
            string caseID = msg_Op.getValueFromPairs("1");
            if (caseID.Equals(groupBox1.Text))
            {
                CD_caseHints.Text = msg_Op.getValueFromPairs("73");
            }
            if (MyCases.ContainsKey(caseID))
            {
                if (!LV_OP.IsNewOrEdited(MyCases[caseID]))
                {
                    MyTaskCount = Interlocked.Increment(ref MyTaskCount);
                    tabPage4.Text = "MyTask" + utility.getPositiveNo(MyTaskCount);
                    ListViewItem temp = LV_OP.cloneLVI(MyCases[caseID]);
                    LV_OP.replaceOneListviewItem(LV_mytask, MyCases[caseID], temp);
                    MyCases[caseID] = temp;
                    flashWindow();
                }
            }
        }

        private void UI_editProduct(Msg_OP msg_Op)
        {
            if (Form_Superuser != null && !Form_Superuser.IsDisposed)
            {
                LV_OP.updateOneListviewItem(Form_Superuser.ListViewNF2, Config.UI_ProductKeys, Config.UI_Index_ProductKeys, Config.UI_IndexValue_ProductKeys, msg_Op.getMsgs()[0]);
            }
            if (Form_ProductMaster != null && !Form_ProductMaster.IsDisposed)
            {
                Combo_OP.initialComboBox(Form_ProductMaster.ComboBox2, ProductCate_Pair.Keys.ToArray());
            }
        }

        private void UI_deleteProduct(Msg_OP msg_Op)
        {
            if (Form_Superuser != null && !Form_Superuser.IsDisposed)
            {
                LV_OP.removeOneListviewItem(Form_Superuser.ListViewNF2, Config.UI_ProductKeys, new string[] { "166", "167" }, msg_Op.getMsgs()[0]);
            }
            if (Form_ProductMaster!=null && !Form_ProductMaster.IsDisposed)
            {
                Combo_OP.initialComboBox(Form_ProductMaster.ComboBox2, ProductCate_Pair.Keys.ToArray());
            }
        }

        private void loadNotice(Msg_OP msg_Op)
        {
            if (msg_Op.sentFrom == myInfo.MyID)
            {
                string[] followUps = Regex.Split(msg_Op.getMsgContent(), Config.Msg_Separator3);
                if (followUps.Length > 0)
                {
                    message firstMsg = new message(followUps[0]);
                    string caseID = firstMsg.getValueFromPairs("1");
                    if (!caseID.Equals(groupBox1.Text))
                    {
                        return;
                    }
                    else
                    {
                        //CD_followUps.Rows.Clear();
                        CD_followUps.Rows.Add(firstMsg.getValuesFromKeys(Config.UI_CaseFollowUpKeys));
                    }

                    for (int i = 1; i < followUps.Length; i++)
                    {
                        message msg = new message(followUps[i]);
                        CD_followUps.Rows.Insert(0, msg.getValuesFromKeys(Config.UI_CaseFollowUpKeys));
                    }
                }
            }
        }

        private void changePassword(Msg_OP msg_Op)
        {
            if (Form_Custom !=null && !Form_Custom.IsDisposed)
            {
                Form_Custom.TextBox4.Clear();
                Form_Custom.TextBox5.Clear();
                Form_Custom.TextBox6.Clear();
                MessageBox.Show("Change password successfully !");
            }
        }

        private void showWarning(string s)
        {
            MessageBox.Show(s);
        }

        private void editUser(Msg_OP msg_Op)
        {
            try
            {
                string name = msg_Op.getValueFromPairs("80"), isSetToSuperUser = msg_Op.getValueFromPairs("86");
                if (name == myInfo.MyName)
                {
                    myInfo.MyID = msg_Op.getValueFromPairs("87");
                    if (isSetToSuperUser == "N" && IsSuperUser)
                    {
                        if (IsSuperUser && Form_Superuser!=null && !Form_Superuser.IsDisposed)
                        {
                            Form_Superuser.Dispose();
                            this.Invoke(new del_showWarning(showWarning), "Your SuperUser right has been removed !");
                        }
                        IsSuperUser = false;
                    }
                    if (isSetToSuperUser == "Y" && !IsSuperUser)
                    {
                        this.Invoke(new del_showWarning(showWarning), "You have been granted SuperUser right !");
                        IsSuperUser = true;
                    }
                }
                if (isSetToSuperUser == "N")
                {
                    SuperUsers = Regex.Replace(SuperUsers, name, "");
                }
                if (isSetToSuperUser == "Y")
                {
                    SuperUsers = Regex.Replace(SuperUsers, name, "");
                    SuperUsers += " " + name;
                }
                int index = LV_OP.getLviIndex(ListLVI_UserFile, Config.UI_UserFileKeys, "80", name);
                for (int i = 0; i < ListLVI_UserFile.Count; i++)
                {
                    if (ListLVI_UserFile[i].SubItems[1].Text == name)
                    {
                        ListLVI_UserFile[i]= LV_OP.getLVIs(msg_Op,Config.UI_UserFileKeys)[0];
                        break;
                    }
                }
                if (Form_Superuser != null && !Form_Superuser.IsDisposed)
                {
                    this.Invoke(new LV_OP.Del_initialListView(LV_OP.initialListView), new object[] { Form_Superuser.ListView2, ListLVI_UserFile, true });
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void deleteUser(Msg_OP msg_Op)
        {
            //
            string deletedUser = msg_Op.getValueFromPairs("80");
            UserList.Remove(deletedUser);
            for (int i = 0; i < ListLVI_UserFile.Count; i++)
            {
                if (ListLVI_UserFile[i].SubItems[1].Text == deletedUser)
                {
                    ListLVI_UserFile.RemoveAt(i);
                    break;
                }
            }
            if (Form_Superuser != null && !Form_Superuser.IsDisposed)
            {
                this.Invoke(new Msg_OP.Del_MsgOp(UI_deleteUser),new object[]{msg_Op});
            }
        }

        private void UI_deleteUser(Msg_OP msg_Op)
        {
            LV_OP.initialListView(Form_Superuser.ListView2, ListLVI_UserFile, true);
            string name = msg_Op.getValueFromPairs("80");
            for (int i = 0; i < LV_update.Items.Count; i++)
            {
                if (LV_update.Items[i].SubItems[1].Text.Equals(name))
                {
                    LV_update.Items.RemoveAt(i);
                    break;
                }
            }
        }

        private void handleClient()
        {
            loadClients_SuperUser();
            this.Invoke(new utility.Del_NoArgs(UI_handleClient));
        }

        private void UI_handleClient()
        {
            if (Form_Superuser != null && !Form_Superuser.IsDisposed)
            {
                LV_OP.initialListView(Form_Superuser.ListView1, ListLVI_ClientList_SuperUser, true);
            }
            Combo_OP.initialComboBox(QI_Client, ClientList);
            Combo_OP.initialComboBox(CD_client, ClientList);
        }

        private void UI_updateCaseProperty(Msg_OP msg_Op)
        {
            switch (msg_Op.getValueFromPairs("100"))
            {
                case "ClientLevel":
                    {
                        LV_OP.initialClientLevelColor(CaseProperty["ClientLevel"]);
                        loadClients_SuperUser();
                        break;
                    }
                case "Priority":
                    {
                        Combo_OP.initialComboBox(QI_CaseType, CaseProperty["Priority"], ",");
                        break;
                    }
                case "Through":
                    {
                        Combo_OP.initialComboBox(CD_through, CaseProperty["Through"], ",");
                        break;
                    }
                case "Type":
                    {
                        Combo_OP.initialComboBox(QI_CaseType, CaseProperty["Type"], ",");
                        Combo_OP.initialComboBox(CD_caseType, CaseProperty["Type"], ",");
                        break;
                    }
                case "Status":
                    {
                        Combo_OP.initialComboBox(CD_status, CaseProperty["Status"], ",");
                        break;
                    }
                case "Issue":
                    {
                        Combo_OP.initialComboBox(QI_IssueType, CaseProperty["Issue"], ",");
                        Combo_OP.initialComboBox(CD_issueType, CaseProperty["Issue"], ",");
                        break;
                    }
                default: break;
            }
        }

        private void addAM()
        {
            initial_AM();
            if (Form_Superuser != null && !Form_Superuser.IsDisposed)
            {
                this.Invoke(new LV_OP.Del_initialListView(LV_OP.initialListView), new object[] { Form_Superuser.ListViewNF1, ListLVI_AM, true });
            }
        }

        private void editDeleteAM()
        {
            initial_AM();
            loadClients_SuperUser();
            if (Form_Superuser != null && !Form_Superuser.IsDisposed)
            {
                this.Invoke(new utility.Del_NoArgs(UI_editDeleteAM));
            }
        }

        private void UI_editDeleteAM()
        {
            LV_OP.initialListView(Form_Superuser.ListViewNF1, ListLVI_AM, true);
            LV_OP.initialListView(Form_Superuser.ListView1, ListLVI_ClientList_SuperUser, true);
        }

        private void login(Msg_OP msg_Op)
        {
            msg_Op.buildMsgs();
            this.Invoke(new Msg_OP.Del_MsgOp(UI_login), new object[] { msg_Op });
        }

        private void UI_login(Msg_OP msg_Op)
        {
            int nameIndex = Config.getIndex(Config.UI_UserUpdateKeys, "80") + 1;
            int handlingIndex = Config.getIndex(Config.UI_UserUpdateKeys, "17") + 1;
            if (nameIndex > 0 && handlingIndex > 0)
            {
                LV_update.BeginUpdate();
                foreach (message msg in msg_Op.getMsgs())
                {
                    for (int i = 0; i < LV_update.Items.Count; i++)
                    {
                        if (LV_update.Items[i].SubItems[nameIndex].Text.Equals(msg.getValueFromPairs("80")))
                        {
                            LV_update.Items[i].ForeColor = Color.SeaGreen;
                            LV_update.Items[i].SubItems[handlingIndex].Text = msg.getValueFromPairs("17");
                            break;
                        }
                    }
                }
                LV_update.EndUpdate();
            }
        }

        private void UI_DisconnectToSLS(Msg_OP msg_Op)
        {
            flashWindow();
            warning.Text = "Disconnected to SLS, you can only search informations";
            warning.ForeColor = Color.IndianRed;
            MessageBox.Show("Disconnected to SLS");
        }

        private void UI_ReconnectToSLS(Msg_OP msg_Op)
        {
            warning.Text = "Connected to SLS";
            warning.ForeColor = Color.SeaGreen;
        }
        private void logout(Msg_OP msg_Op)
        {    
            if (msg_Op.getValueFromPairs("80") != myInfo.MyName)
            {
                this.Invoke(new Msg_OP.Del_MsgOp(UI_logout), new object[] { msg_Op });
            }
        }

        private void UI_logout(Msg_OP msg_Op)
        {
            int nameIndex = Config.getIndex(Config.UI_UserUpdateKeys, "80") + 1;
            int handlingIndex = Config.getIndex(Config.UI_UserUpdateKeys, "17") + 1;
            if (nameIndex > 0 && handlingIndex > 0)
            {
                LV_update.BeginUpdate();
                for (int i = 0; i < LV_update.Items.Count; i++)
                {
                    if (LV_update.Items[i].SubItems[nameIndex].Text.Equals(msg_Op.getValueFromPairs("80")))
                    {
                        LV_update.Items[i].ForeColor = Color.Black;
                        LV_update.Items[i].SubItems[handlingIndex].Text = "";
                        break;
                    }
                }
                LV_update.EndUpdate();
            }
        }

        private void userUpdate(Msg_OP msg_Op) 
        {  
            msg_Op.buildMsgs();
            this.Invoke(new Msg_OP.Del_MsgOp(UI_userUpdate), new object[] { msg_Op });
        }

        private void UI_userUpdate(Msg_OP msg_Op)
        {
            int nameIndex = Config.getIndex(Config.UI_UserUpdateKeys, "80") + 1;
            int handlingIndex = Config.getIndex(Config.UI_UserUpdateKeys, "17") + 1;
            if (nameIndex > 0 && handlingIndex > 0)
            {
                LV_update.BeginUpdate();
                for (int i = 0; i < LV_update.Items.Count; i++)
                {
                    if (LV_OP.getColumnValue(LV_update.Items[i], Config.UI_UserUpdateKeys, "80").Equals(msg_Op.getValueFromPairs("80")))
                    {
                        message msg = msg_Op.getMsgs()[0];
                        LV_update.Items[i].SubItems[nameIndex].Text = msg.getValueFromPairs("80");
                        LV_update.Items[i].SubItems[handlingIndex].Text = msg.getValueFromPairs("17");
                        break;
                    }
                }
                LV_update.EndUpdate();
            }
        }

        private void editMyInfo(Msg_OP msg_Op)
        {
            string exUserName = msg_Op.getValueFromPairs("89"), UserName = msg_Op.getValueFromPairs("80");
            if (myInfo.MyName.Equals(exUserName))
            {
                myInfo.MyName = UserName;
                myInfo.MyChiName = msg_Op.getValueFromPairs("81");
                myInfo.MyEmail = msg_Op.getValueFromPairs("85");
                myInfo.MyExName = exUserName;
            }
            if (UserList.Contains(exUserName))
            {
                UserList.Remove(exUserName);
                UserList.Add(UserName);
            }
            LV_OP.updateOneListviewItem(ListLVI_UserFile, Config.UI_UserFileKeys, Config.UI_Index_UserFileKeys, Config.UI_IndexValue_UserFileKeys, msg_Op.getMsgs()[0]);
            this.Invoke(new Msg_OP.Del_MsgOp(UI_editUserList), new object[] { msg_Op });
        }

        private void UI_editUserList(Msg_OP msg_Op)
        {
            try
            {
                string exUserName = msg_Op.getValueFromPairs("89"), UserName = msg_Op.getValueFromPairs("80");           
                if (myInfo.MyName == UserName)
                {
                    this.Text = UserName;
                    this.Text = myInfo.MyName;
                    if (Form_Custom != null && !Form_Custom.IsDisposed)
                    {
                        Form_Custom.TextBox1.Text = myInfo.MyName;
                        Form_Custom.TextBox2.Text = myInfo.MyChiName;
                        Form_Custom.TextBox3.Text = myInfo.MyEmail;
                    }
                }
                if (!exUserName.Equals(UserName))
                {
                    UI_initialCase();
                    Combo_OP.initialComboBox(CD_inchange, UserList);
                    if (Combo_Search_Cate.Text.Equals("Incharge"))
                    {
                        Combo_OP.initialComboBox(Combo_Search_Subcate, UserList);
                    }
                    for (int i = 0; i < LV_update.Items.Count; i++)
                    {
                        if (LV_update.Items[i].SubItems[1].Text.Equals(exUserName))
                        {
                            LV_update.Items[i].SubItems[1].Text = UserName;
                            break;
                        }
                    }
                }
                if (Form_Superuser != null && !Form_Superuser.IsDisposed)
                {
                    LV_OP.initialListView(Form_Superuser.ListView2, ListLVI_UserFile, true);
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void upgradeNotice(Msg_OP msg_Op)
        {
            this.MouseEnter += delegate(object o, EventArgs ea)
            {   
                string path = msg_Op.getValueFromPairs("61");
                Config.Customization_INI.writeValue("COMMON", "PackagePath", path);
                DialogResult result = MessageBox.Show("A new version available !" , "", MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
                if (result.ToString() == "OK")
                {
                    Config.SLS_Sock.close();
                    openUpgradeProgram();
                    Application.Exit();
                }
            };
        }

        private void openUpgradeProgram()
        {
            try
            {
                Process myProcess = new Process();
                string fileName = @"SLSUpgrade.exe";
                ProcessStartInfo myProcessStartInfo = new ProcessStartInfo(fileName, "upgrading");
                myProcess.StartInfo = myProcessStartInfo;
                myProcess.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void UI_ReminderTimeUp(Msg_OP msg_Op)
        {
            string caseID = msg_Op.getValueFromPairs("1");
            int index = LV_OP.getLviIndex(LV_Reminder, Config.UI_CaseKeys, "1", caseID);
            if (index >= 0)
            {
                Interlocked.Increment(ref ReminderCounter);
                ListViewItem lvi = LV_Reminder.Items[index];
                LV_OP.removeOneListviewItem(LV_Reminder, lvi);
                LV_OP.insertOneListviewItem(LV_Reminder_Expired, 0, lvi);
                tabPage11.Text = "Reminder" + utility.getPositiveNo(ReminderCounter);
                if (IsPopupReminder)
                {
                    MessageBox.Show(caseID + "'s reminder is time up!!!!");
                }
                flashWindow();
            }
        }

        private void setReminder(Msg_OP msg_Op)
        {
            if (Reminder_OP.addReminder(msg_Op))
            {
                this.Invoke(new Msg_OP.Del_MsgOp(UI_setReminder), new object[] { msg_Op });
            }
        }

        private void UI_setReminder(Msg_OP msg_Op)
        {
            ListViewItem lvi =LV_OP.getLVIs(msg_Op, Config.UI_CaseKeys)[0];
            LV_OP.removeOneListviewItem(LV_Reminder_Expired, Config.UI_CaseKeys, new string[] { "1" }, msg_Op.getMsgs()[0]);
            LV_OP.removeOneListviewItem(LV_Reminder, Config.UI_CaseKeys, new string[] { "1" }, msg_Op.getMsgs()[0]);
            LV_OP.insertOneListviewItem(LV_Reminder, 0, lvi);
        }

        private void UI_handleError(Msg_OP msg_Op)
        {
            IsBeenKicked = true;
            MessageBox.Show(msg_Op.getValueFromPairs("75"));
        }

        private void UI_addProduct(Msg_OP msg_Op)
        {
            if (IsSuperUser)
            {
                LV_OP.insertOneListviewItem(Form_Superuser.ListViewNF2, 0, LV_OP.getLVIs(msg_Op,Config.UI_ProductKeys)[0]);
            }
            if (Form_ProductMaster!=null && !Form_ProductMaster.IsDisposed)
            {
                Combo_OP.initialComboBox(Form_ProductMaster.ComboBox2, ProductCate_Pair.Keys.ToArray());
            }
        }

        private void UI_initialCIM()
        {
            LV_OP.initialListView(LV_CIM, CIMList, true);
            Combo_OP.initialComboBox(CIM_client, CIMList.Keys.ToArray());
        }

        private void UI_initialCase()
        {
            LV_OP.initialListView(LV_outstanding, initial_Outstanding(),true);
            LV_OP.initialListView(LV_mytask, initial_MyTask(), true);
            Reminder_OP.refresh();
            LV_OP.initialListView(LV_Reminder, Reminder_OP.getNoExpiredCases(), true);
            LV_OP.initialListView(LV_Reminder_Expired, Reminder_OP.getExpiredCases(), true);
        }

        private void DoWork_LowFrequency(object sender, DoWorkEventArgs e)
        {
            Msg_OP msg_Op = (Msg_OP)e.Argument;
            try
            {
                switch (msg_Op.msgType)
                {
                    case "U1":  //edit a user by superUser
                        {
                            editUser(msg_Op);
                            break;
                        }
                    case "U2":  // edit myInfo
                        {
                            editMyInfo(msg_Op);
                            break;
                        }
                    case "U3":  //delete a User by superUser
                        {
                            deleteUser(msg_Op);
                            break;
                        }
                    case "U4":  // change password by user
                        {  
                            this.Invoke(new Msg_OP.Del_MsgOp(changePassword), new object[] { msg_Op });
                            break;
                        }
                    case "B1":   // add a client 
                        {
                            handleClient();
                            break;
                        }
                    case "B2": // edit a client
                        {
                            handleClient();
                            break;
                        }
                    case "B3": // delete a client
                        {
                            handleClient();
                            break;
                        }
                    case "A1": // add AccountManager
                        {
                            addAM();
                            break;
                        }
                    case "A2": // edit AM
                        {
                            editDeleteAM();
                            break;
                        }
                    case "A3": // delete AM
                        {
                            editDeleteAM();
                            break;
                        }
                    case "S": //Update CaseProperty
                        {
                            string category = msg_Op.getValueFromPairs("100"), properties = msg_Op.getValueFromPairs("101");
                            CaseProperty[category] = properties;
                            if (category.Equals("Priority"))
                            {
                                LV_OP.initialPriorityColor(CaseProperty["Priority"]);
                            }
                            this.Invoke(new Msg_OP.Del_MsgOp(UI_updateCaseProperty), new object[] { msg_Op });
                            break;
                        }
                    case "F1": // new a follow up
                        {
                            msg_Op.buildMsgs();
                            this.Invoke(new Msg_OP.Del_MsgOp(UI_addFollowUp), new object[] { msg_Op });
                            break;
                        }
                    case "F2": // edit a follow up
                        {
                            msg_Op.buildMsgs();
                            this.Invoke(new Msg_OP.Del_MsgOp(UI_loadFollowUp), new object[] { msg_Op });
                            break;
                        }
                    case "F3": // delete a follow up
                        {
                            msg_Op.buildMsgs();
                            this.Invoke(new Msg_OP.Del_MsgOp(UI_loadFollowUp), new object[] { msg_Op });
                            break;
                        }
                    case "H1": // add a caseHint
                        {
                            msg_Op.buildMsgs();
                            this.Invoke(new Msg_OP.Del_MsgOp(UI_editCaseHint), new object[] { msg_Op });
                            break;
                        }
                    case "L1":  // Login
                        {
                            login(msg_Op);
                            break;
                        }
                    case "L0": // logOut
                        {
                            logout(msg_Op);
                            break;
                        }
                    case "L2": // Disconnected to SLS
                        {
                            this.Invoke(new Msg_OP.Del_MsgOp(UI_DisconnectToSLS), new object[] { msg_Op });
                            break;
                        }
                    case "L3": // re-connected to SLS
                        {
                            this.Invoke(new Msg_OP.Del_MsgOp(UI_ReconnectToSLS), new object[] { msg_Op });
                            break;
                        }
                    case "R1": // Add handling case 
                        {
                            UI_userUpdate(msg_Op);
                            break;
                        }
                    case "R2": // Remove handling case 
                        {
                            UI_userUpdate(msg_Op);
                            break;
                        }
                    case "N0":   // common notice
                        {
                            this.Invoke(new Msg_OP.Del_MsgOp(loadNotice), new object[] { msg_Op });
                            break;
                        }
                    case "N1": // upgrade notice
                        {
                            upgradeNotice(msg_Op);
                            break;
                        }
                    case "N2": // add a reminder
                        {
                            setReminder(msg_Op);
                            break;
                        }
                    case "N3"://reminder time up
                        {
                            msg_Op.buildMsgs();
                            this.Invoke(new Msg_OP.Del_MsgOp(UI_ReminderTimeUp), new object[] { msg_Op });
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
                            break;
                        }
                    case "PM1": // add a server
                        {
                            if (Form_ProductMaster!=null && !Form_ProductMaster.IsDisposed)
                            {
                                Form_ProductMaster.addServer(msg_Op);
                            }
                            break;
                        }
                    case "PM2":  // edit a server
                        {
                            if (Form_ProductMaster!=null && !Form_ProductMaster.IsDisposed)
                            {
                                Form_ProductMaster.editServer(msg_Op);
                            }
                            break;
                        }
                    case "PM3": // delete a server
                        {
                            if (Form_ProductMaster!=null && !Form_ProductMaster.IsDisposed)
                            {
                                Form_ProductMaster.deleteServer(msg_Op);
                            }
                            break;
                        }
                    case "PM4": // deploy product to a client's server
                        {
                            if (Form_ProductMaster!=null && !Form_ProductMaster.IsDisposed)
                            {
                                Form_ProductMaster.addProductToClientServer(msg_Op);
                            }
                            break;
                        }
                    case "PM5": // edit product deploy info of a client's server
                        {
                            if (Form_ProductMaster!=null && !Form_ProductMaster.IsDisposed)
                            { 
                                Form_ProductMaster.editProductOfClientServer(msg_Op);
                            }
                            break;
                        }
                    case "PM6": // delete product deploy info of a client's server
                        {
                            if (Form_ProductMaster!=null && !Form_ProductMaster.IsDisposed)
                            {
                                Form_ProductMaster.deleteProductOfClientServer(msg_Op);
                            }
                            break;
                        }
                    case "PM7": // edit product deploy update Info
                        {
                            if (Form_ProductMaster!=null && !Form_ProductMaster.IsDisposed)
                            {
                                msg_Op.buildMsgs();
                                this.Invoke(new Msg_OP.Del_MsgOp(Form_ProductMaster.UI_editProductUpdate), new object[] { msg_Op });
                            }
                            break;
                        }
                    case "PM8": // delete product deploy update Info
                        {
                            if (Form_ProductMaster!=null && !Form_ProductMaster.IsDisposed)
                            {
                                msg_Op.buildMsgs();
                                this.Invoke(new Msg_OP.Del_MsgOp(Form_ProductMaster.UI_deleteProductUpdate), new object[] { msg_Op });
                            }
                            break;
                        }
                    case "PM9": // add a product
                        {
                            ProductCate_Pair.Add(msg_Op.getValueFromPairs("167"), msg_Op.getValueFromPairs("166"));
                            this.Invoke(new Msg_OP.Del_MsgOp(UI_addProduct), new object[] { msg_Op });

                            break;
                        }
                    case "PM10": // edit a product
                        {                   
                            ProductCate_Pair.Remove(msg_Op.getValueFromPairs("176"));
                            ProductCate_Pair.Add(msg_Op.getValueFromPairs("167"), msg_Op.getValueFromPairs("166"));
                            this.Invoke(new Msg_OP.Del_MsgOp(UI_editProduct), new object[] { msg_Op });
                            break;
                        }
                    case "PM11": //delete a product
                        {
                            ProductCate_Pair.Remove(msg_Op.getValueFromPairs("167"));
                            this.Invoke(new Msg_OP.Del_MsgOp(UI_deleteProduct), new object[] { msg_Op });
                            break;
                        }
                    case "PH1":  //add productHint
                        {
                            if (Form_ProductHints !=null && !Form_ProductHints.IsDisposed)
                            {
                                msg_Op.buildMsgs();
                                this.Invoke(new Msg_OP.Del_MsgOp(Form_ProductHints.UI_addProductHints), new object[] { msg_Op });
                            }
                            break;
                        }
                    case "PH2": //edit productHint
                        {
                           if (Form_ProductHints !=null && !Form_ProductHints.IsDisposed)
                            {
                                msg_Op.buildMsgs();
                                this.Invoke(new Msg_OP.Del_MsgOp(Form_ProductHints.UI_editProductHints), new object[] { msg_Op });
                            }
                            break;
                        }
                    case "PH3":  //delete productHint
                        {
                           if (Form_ProductHints !=null && !Form_ProductHints.IsDisposed)
                            {
                                msg_Op.buildMsgs();
                                this.Invoke(new Msg_OP.Del_MsgOp(Form_ProductHints.UI_deleteProductHints), new object[] { msg_Op });
                            }
                            break;
                        }
                    case "P0":  // initial CIM
                        {
                            initial_CIM();
                            this.Invoke(new utility.Del_NoArgs(UI_initialCIM));
                            break;
                        }
                    case "P1":  // add CIM info
                        {
                            msg_Op.buildMsgs();
                            this.Invoke(new Msg_OP.Del_MsgOp(UI_addClientContact), new object[] { msg_Op });
                            break;
                        }
                    case "P2":  // edit CIM info
                        {
                            msg_Op.buildMsgs();
                            this.Invoke(new Msg_OP.Del_MsgOp(UI_editClientContact), new object[] { msg_Op });
                            break;
                        }
                    case "P3":  //delete CIM info
                        {
                            msg_Op.buildMsgs();
                            this.Invoke(new Msg_OP.Del_MsgOp(UI_deleteClientContact), new object[] { msg_Op });                
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

        private void DoWork_Case(object sender, DoWorkEventArgs e)
        {
            Msg_OP msg_Op = (Msg_OP)e.Argument;
            try
            {
                switch (msg_Op.msgType)
                {
                    case "C0":  // initial
                        {
                            this.Invoke(new utility.Del_NoArgs(UI_initialCase));
                            break;
                        }
                    case "C1":  // add
                        {         
                            addCase(msg_Op);      
                            break;
                        }
                    case "C2":  // edit
                        {
                            editCase(msg_Op);
                            break;
                        }
                    case "C3":  //delete
                        {
                            deleteCase(msg_Op);
                            break;
                        }
                    case "C4":  // serach By one CaseID
                        {
                            this.Invoke(new LV_OP.Del_initialListView(LV_OP.initialListView), new object[] { LV_search, searchCaseByCaseID(msg_Op.getValueFromPairs("1")), true });
                            break;
                        }
                    case "C5":  // multiFilter
                        {
                            this.Invoke(new utility.Del_NoArgs(multiFilter_Begin));
                            multiFilter(msg_Op.getMsgs()[0]);
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

        private void DoWork_HighFrequency(object sender, DoWorkEventArgs e)
        {
            Msg_OP msg_Op = (Msg_OP)e.Argument;
            try
            {
                switch (msg_Op.msgType)
                {
                    case "F0": // Load a follow up of a case
                        {
                            msg_Op.buildMsgs();
                            this.Invoke(new Msg_OP.Del_MsgOp(UI_loadFollowUp), new object[] { msg_Op });
                            break;
                        }
                    case "H0":
                        {
                            msg_Op.buildMsgs();
                            this.Invoke(new Msg_OP.Del_MsgOp(UI_loadCaseHint), new object[] { msg_Op });
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

        private void DoWork_IO(object sender, DoWorkEventArgs e)
        {
            Msg_OP msg_Op = (Msg_OP)e.Argument;
            try
            {
                switch (msg_Op.msgType)
                {
                    case "IO1":  
                        {
                            MarkedCases_OP.addCategory(msg_Op.getMsgs()[0]);
                            break;
                        }
                    case "IO2":  
                        {
                            MarkedCases_OP.editCategory(msg_Op.getMsgs()[0]);
                            break;
                        }
                    case "IO3":  
                        {
                            MarkedCases_OP.removeCategory(msg_Op.getMsgs()[0]);
                            break;
                        }
                    case "IO4":  
                        {
                            MarkedCases_OP.addCase(msg_Op.getMsgs()[0]);
                            break;
                        }
                    case "IO5":  
                        {
                            MarkedCases_OP.removeCase(msg_Op.getMsgs()[0]);
                            break;
                        }
                    case "IO6":  
                        {
                            MarkedCases_OP.moveCaseBetweenCates(msg_Op.getMsgs()[0]);
                            break;
                        }
                    case "IO7":
                        {
                            MarkedCases_OP.editCateDescription(msg_Op.getMsgs()[0]);
                            break;
                        }
                    case "IO8":
                        {
                            saveShortcutKey();
                            break;
                        }
                    case "IO9":
                        {
                            Config.logWriter.flushToFile(msg_Op);
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

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            saveData();
            if (IsBeenKicked)
            {
                Config.SLS_Sock.close();
            }
            else
            {
                Config.SLS_Sock.exit();
            }
        }

        private void saveData()
        {
            LV_OP.writeColumnOrder(LV_outstanding, Config.Customization_INI, "Outstanding_Order");
            LV_OP.writeColumnOrder(LV_search, Config.Customization_INI, "Search_Order");
            LV_OP.writeColumnOrder(LV_mytask, Config.Customization_INI, "MyTask_Order");
            LV_OP.writeColumnOrder(LV_Reminder, Config.Customization_INI, "Reminder_Order");
            LV_OP.writeColumnWidth(LV_outstanding, Config.Customization_INI, "Outstanding_Width");
            LV_OP.writeColumnWidth(LV_search, Config.Customization_INI, "Search_Width");
            LV_OP.writeColumnWidth(LV_mytask, Config.Customization_INI, "MyTask_Width");
            LV_OP.writeColumnWidth(LV_Reminder, Config.Customization_INI, "Reminder_Width");

            Config.Customization_INI.writeValue("COMMON", "IsPopupReminder", IsPopupReminder == true ? "Y" : "N");
            Config.Customization_INI.writeValue("COMMON", "SplitWidth", splitContainer1.SplitterDistance.ToString());
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(Config.ZB);
        }

        private void button17_Click(object sender, EventArgs e)
        {
            utility.exportToExcel(LV_search, "HistoryCases");
        }

        private void noticeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedIndex = 4;
        }

        private void toolStripMenuItem5_Click(object sender, EventArgs e)
        {
            try
            {
                Clipboard.Clear();
                if (tabControl2.SelectedIndex == 0)
                {
                    if (LV_outstanding.SelectedItems.Count > 0)
                    {
                        MessageBox.Show("Copied CaseID: " + LV_outstanding.SelectedItems[0].SubItems[1].Text);
                        Clipboard.SetText(LV_outstanding.SelectedItems[0].SubItems[1].Text);
                    }
                }
                if (tabControl2.SelectedIndex == 1)
                {
                    if (LV_search.SelectedItems.Count > 0)
                    {
                        MessageBox.Show("Copied CaseID: " + LV_search.SelectedItems[0].SubItems[1].Text);
                        Clipboard.SetText(LV_search.SelectedItems[0].SubItems[1].Text);
                    }
                }
                if (tabControl2.SelectedIndex == 2)
                {
                    if (LV_mytask.SelectedItems.Count > 0)
                    {
                        MessageBox.Show("Copied CaseID: " + LV_mytask.SelectedItems[0].SubItems[1].Text);
                        Clipboard.SetText(LV_mytask.SelectedItems[0].SubItems[1].Text);
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void toolStripMenuItem7_Click(object sender, EventArgs e)
        {
            string category = toolStripComboBox1.Text.Trim(' ');
            toolStripComboBox1.Text = "";
            if (category.Equals(""))
            {
                MessageBox.Show("Please input category name");
                return;
            }
            ListViewItem lvi = new ListViewItem();
            if (tabControl2.SelectedIndex == 0)
            {
                if (LV_outstanding.SelectedItems.Count > 0)
                {
                    lvi = (ListViewItem)LV_outstanding.SelectedItems[0].Clone();
                }
            }
            if (tabControl2.SelectedIndex == 1)
            {
                if (LV_search.SelectedItems.Count > 0)
                {
                    lvi = (ListViewItem)LV_search.SelectedItems[0].Clone();
                }
            }
            if (tabControl2.SelectedIndex == 2)
            {
                if (LV_mytask.SelectedItems.Count > 0)
                {
                    lvi = (ListViewItem)LV_mytask.SelectedItems[0].Clone();
                }
            }
            string caseID = LV_OP.getColumnValue(lvi, Config.UI_CaseKeys, "1");

            if (MarkedCateCases.Keys.Contains(category))
            {
                if (MarkedCateCases[category].Contains(caseID))
                {
                    MessageBox.Show("This case already marked to category: " + category);
                    return;
                }

                if (Form_MyMarkedCases!=null && !Form_MyMarkedCases.IsDisposed)
                {
                    Msg_OP msg_Op = new Msg_OP("IO4", new message[] { Form_MyMarkedCases.UI_addCase(category, lvi, new message()) });
                   Config.SLS_Sock.inMsgQueue_add(msg_Op);
                }
                else
                {
                    MarkedCateCases[category].Add(caseID);
                    message msg = new message();
                    msg.setKeyValuePair("1", caseID);
                    msg.setKeyValuePair("210", category);
                    Msg_OP msg_Op = new Msg_OP("IO4", new message[] { msg });
                   Config.SLS_Sock.inMsgQueue_add(msg_Op);
                }
            }
            else
            {
                if (Form_MyMarkedCases != null && !Form_MyMarkedCases.IsDisposed)
                {
                    Msg_OP msg_Op = new Msg_OP("IO4", new message[] { Form_MyMarkedCases.UI_addCase(category, lvi, new message()) });
                   Config.SLS_Sock.inMsgQueue_add(msg_Op);
                }
                else
                {
                    MarkedCateCases.Add(category, new List<string>() { caseID });
                    Combo_OP.initialToolComboBox(toolStripComboBox1, MarkedCateCases.Keys.ToList());
                    message msg = new message();
                    msg.setKeyValuePair("1", caseID);
                    msg.setKeyValuePair("210", category);
                    Msg_OP msg_Op = new Msg_OP("IO4", new message[] { msg });
                   Config.SLS_Sock.inMsgQueue_add(msg_Op);
                }
            }     
        }

        private void toolStripMenuItem8_Click(object sender, EventArgs e)
        {
            if (tabControl2.SelectedIndex == 0)
            {
                if (LV_outstanding.SelectedItems.Count > 0)
                {
                    Reminder reminder = new Reminder(LV_outstanding.SelectedItems[0]);
                    reminder.Show();
                    return;
                }
            }
            if (tabControl2.SelectedIndex == 1)
            {
                if (LV_search.SelectedItems.Count > 0)
                {
                    Reminder reminder = new Reminder(LV_search.SelectedItems[0]);
                    reminder.Show();
                    return;
                }
            }
            if (tabControl2.SelectedIndex == 2)
            {
                if (LV_mytask.SelectedItems.Count > 0)
                {
                    Reminder reminder = new Reminder(LV_mytask.SelectedItems[0]);
                    reminder.Show();
                    return;
                }
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string caseID = "";
            if (LV_Reminder.SelectedItems.Count > 0)
            {
                caseID = LV_OP.getColumnValue(LV_Reminder.SelectedItems[0], Config.UI_CaseKeys, "1");      
                LV_OP.removeOneListviewItem(LV_Reminder, LV_Reminder.SelectedItems[0]);       
            }
            if (LV_Reminder_Expired.SelectedItems.Count > 0)
            {
                caseID = LV_OP.getColumnValue(LV_Reminder_Expired.SelectedItems[0], Config.UI_CaseKeys, "1");
                LV_OP.removeOneListviewItem(LV_Reminder_Expired, LV_Reminder_Expired.SelectedItems[0]);
            }
            ThreadPool.QueueUserWorkItem(new WaitCallback(Reminder_OP.Th_removeReminder), caseID);
        }

        private void changeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ListViewItem lvi;
            if (LV_Reminder.SelectedItems.Count > 0)
            {
                lvi = LV_Reminder.SelectedItems[0];
            }
            else if (LV_Reminder_Expired.SelectedItems.Count > 0)
            {
                lvi = LV_Reminder_Expired.SelectedItems[0];
            }
            else
            {
                return;
            }
            Reminder reminder = new Reminder(lvi);
            reminder.Show();
        }

        private void listViewNF1_DoubleClick(object sender, EventArgs e)
        {
            if (LV_Reminder.SelectedItems.Count > 0)
            {
                ActCase EditCase = new ActCase(myInfo,LV_Reminder.SelectedItems[0], CaseProperty,ClientList, UserList, ProductCate_Pair, ClientServerProductVersion, CIMList, ClientAM, AMPhone);
                EditCase.Show();
            }  
        }

        private void flashWindow()
        {
            try
            {
                if (this.InvokeRequired)
                {
                    flashWindowCallback d = new flashWindowCallback(FlashWindow);
                    this.Invoke(d, new object[] { this.Handle, true });
                }
                else
                {
                    FlashWindow(this.Handle, true);
                }
            }
            catch { }
        }

        private void richTextBox3_Click(object sender, EventArgs e)
        {
            tabPage10.Text = "Notice";
        }

        private void reminderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tabControl2.SelectedIndex = 5;
        }

        private void comboBox2_TextChanged(object sender, EventArgs e)
        {         
            if (QI_Client.Text.Trim(' ').Equals(""))
            {
                tabPage1.Text = "Input";
            }
            else
            {
                tabPage1.Text = QI_Client.Text;
            }
        }

        public List<message> initial_Notices()
        {
            string cmd = "select * from Notice where Receiver='" + myInfo.MyName + "'";
            using (SqlConnection sqlConnection = SQL.dbConnect()) {
                return SQL.queryResults(cmd, Config.UI_NoticeKeys, sqlConnection);
            }
        }

        public  List<ListViewItem> initial_Outstanding()
        {
            string cmd = "select * from SupportLogSheet where Status != 'Closed'";
            using (SqlConnection sqlConnection = SQL.dbConnect())
            {
                OutstandingCases = SQL.genCaseDic(cmd, Config.UI_CaseKeys, sqlConnection);
                return OutstandingCases.Values.ToList();
            }
        }

        public  List<ListViewItem> initial_MyTask()
        {
            string cmd = new StringBuilder("select * from SupportLogSheet where Status != 'Closed' and Incharge = '").Append(myInfo.MyName).Append("'").ToString();
            using (SqlConnection sqlConnection = SQL.dbConnect())
            {
                MyCases= SQL.genCaseDic(cmd, Config.UI_CaseKeys, sqlConnection);
                return MyCases.Values.ToList();
            }
        }

        private  List<ListViewItem> searchCaseByCaseID(string caseID)
        {
            string cmd = new StringBuilder("select * from SupportLogSheet where CaseID='").Append(caseID).Append("'").ToString();
            using (SqlConnection sqlConnection = SQL.dbConnect())
            {
                return SQL.genListLvi(cmd, Config.UI_CaseKeys, sqlConnection);
            }
        }

        private void multiFilter_Begin()
        {
            tabControl2.SelectedIndex = 1;
            label_SearchCount.Text = "searching...";
            label_SearchCount.Show();
        }

        private void multiFilter(message msg)
        {
            string sql = "";
            StringBuilder sb = new StringBuilder("select * from SupportLogSheet where ");
            List<string> values = msg.getExistingValues();
            foreach (string value in values)
            {
                if (!value.Equals(""))
                {
                    sb.Append(value).Append(" and ");
                }
            }
            if (sb.ToString().Equals("select * from SupportLogSheet where "))
            {
                sql = "select * from SupportLogSheet";
            }
            else
            {
                sql = Regex.Replace(sb.ToString(), "and $", "");
            }
            using (SqlConnection sqlConnection = SQL.dbConnect())
            {
                this.Invoke(new LV_OP.Del_initialContent(UI_MultiFilter), new object[] { SQL.genListLvi(sql, Config.UI_CaseKeys, sqlConnection) });
            }
        }

        private void UI_MultiFilter(List<ListViewItem> contents)
        {
            int count = contents.Count;
            LV_OP.initialListView(LV_search, contents, true);
            if (count == 0)
            {
                MessageBox.Show("No record"); ;
                label_SearchCount.Hide();
            }
            else
            {
                label_SearchCount.Text = count + " records";
            }
        }

        private void loadClients_SuperUser()
        {
            using (SqlConnection sqlConnection = SQL.dbConnect())
            {
                string cmd = "select * from ClientList";
                SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection);
                ListLVI_ClientList_SuperUser.Clear();
                ClientLevelDetail.Clear();
                ClientAM.Clear();
                ClientList.Clear();
                string[] levels = Regex.Split(CaseProperty["ClientLevel"].ToString(), ",");
                for (int i = 0; i < levels.Length; i++)
                {
                    ClientLevelDetail.Add(levels[i], new List<string>());
                }
                using (SqlDataReader re = sqlcmd.ExecuteReader())
                {
                    string[] DBColumnNames = Config.getValues(Config.UI_ClientListKeys);
                    while (re.Read())
                    {
                        try
                        {
                            ListViewItem lvi = new ListViewItem();
                            lvi.BackColor = LV_OP.Color_Missing;
                            for (int i = 0; i < DBColumnNames.Length; i++)
                            {
                                lvi.SubItems.Add(re.GetValue(re.GetOrdinal(DBColumnNames[i])).ToString().Trim(' '));
                            }
                            string client = LV_OP.getColumnValue(lvi, Config.UI_ClientListKeys, "4");
                            string AM = LV_OP.getColumnValue(lvi, Config.UI_ClientListKeys, "133");
                            string level = LV_OP.getColumnValue(lvi, Config.UI_ClientListKeys, "132");
                            if (LV_OP.ClientLevelColorDetail.ContainsKey(level))
                            {
                                lvi.ForeColor = LV_OP.ClientLevelColorDetail[level];
                                ClientLevelDetail[level].Add(client);
                            }
                            ClientAM.Add(client, AM);
                            ClientList.Add(client);
                            ListLVI_ClientList_SuperUser.Add(lvi);
                        }
                        catch (Exception ex)
                        {
                            Config.logWriter.writeErrorLog(ex);
                        }
                    }
                }
            }
        }

        private void initial_CaseProperty()
        { 
            try
            {
                using (SqlConnection sqlConnection = SQL.dbConnect())
                {
                    string cmd = "select * from CaseProperty";
                    SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection);
                    CaseProperty.Clear();
                    for (int i = 0; i < Config.CaseProperty_Category.Length; i++)
                    {
                        CaseProperty.Add(Config.CaseProperty_Category[i], "");
                    }
                    using (SqlDataReader re = sqlcmd.ExecuteReader())
                    {
                        string[] DBColumnNames = Config.getValues(Config.UI_CasePropertyKeys);
                        string[] values = new string[DBColumnNames.Length];
                        while (re.Read())
                        {
                            for (int i = 0; i < DBColumnNames.Length; i++)
                            {
                                values[i] = re.GetValue(re.GetOrdinal(DBColumnNames[i])).ToString().Trim(' ');
                            }
                            CaseProperty[values[0]] = utility.updateSTR_AppendSubStr(CaseProperty[values[0]].ToString(), ",", values[1]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void initial_AM()
        {
            try
            {
                using (SqlConnection sqlConnection = SQL.dbConnect())
                {
                    string cmd = "select * from AccountManager";
                    SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection);
                    using (SqlDataReader re = sqlcmd.ExecuteReader())
                    {
                        ListLVI_AM.Clear();
                        AMPhone.Clear();
                        CaseProperty_AM.Clear();
                        message msg = new message();
                        string[] DBColumnNames = Config.getValues(Config.UI_AccountManagerKeys);
                        while (re.Read())
                        {
                            for (int i = 0; i < DBColumnNames.Length; i++)
                            {
                                msg.setKeyValuePair(Config.UI_AccountManagerKeys[i], re.GetValue(re.GetOrdinal(DBColumnNames[i])).ToString().Trim(' '));
                            }
                            ListLVI_AM.Add(LV_OP.getLVI(msg,Config.UI_AccountManagerKeys));
                            AMPhone.Add(msg.getValueFromPairs("150"), utility.updateSTR_AppendSubStr(msg.getValueFromPairs("151"), ",", msg.getValueFromPairs("152")));
                            CaseProperty_AM.Add(msg.getValueFromPairs("150"));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        public bool initial_Users()
        {
            try
            {
                using (SqlConnection sqlConnection = SQL.dbConnect())
                {
                    bool isSuperUser = false;
                    string cmd = "select * from UserFile";
                    SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection);
                    using (SqlDataReader re = sqlcmd.ExecuteReader())
                    {
                        SuperUsers = "";
                        ListLVI_UserFile.Clear();
                        UserIDNameMapping.Clear();
                        message msg = new message();
                        string[] DBColumnNames = Config.getValues(Config.UI_UserFileKeys);
                        string username = "";
                        while (re.Read())
                        {
                            for (int i = 0; i < DBColumnNames.Length; i++)
                            {
                                msg.setKeyValuePair(Config.UI_UserFileKeys[i], re.GetValue(re.GetOrdinal(DBColumnNames[i])).ToString().Trim(' '));
                            }
                            ListLVI_UserFile.Add(LV_OP.getLVI(msg,Config.UI_UserFileKeys));
                            username = msg.getValueFromPairs("80");
                            UserList.Add(username);
                            UserIDNameMapping.Add(msg.getValueFromPairs("87"), username);
                            if (msg.getValueFromPairs("86").Equals("Y"))
                            {
                                SuperUsers += " " + username;
                                if (username.ToUpper() == myInfo.MyName.ToUpper())
                                {
                                    isSuperUser = true;
                                }
                            }
                            if (username.ToUpper() == myInfo.MyName.ToUpper())
                            {
                                myInfo.MyID = msg.getValueFromPairs("87");
                                myInfo.MyEmail = msg.getValueFromPairs("85");
                                myInfo.MyChiName = msg.getValueFromPairs("81");
                            }
                        }
                    }
                    return isSuperUser;
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
            return false;
        }

        private void initial_CIM()
        {
            try
            {
                using (SqlConnection sqlConnection = SQL.dbConnect())
                {
                    string cmd = "select * from ClientContact";
                    SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection);
                    using (SqlDataReader re = sqlcmd.ExecuteReader())
                    {
                        string client = "";
                        CIMList.Clear();
                        message msg = new message();
                        string[] DBColumnNames = Config.getValues(Config.UI_ClientContactKeys);
                        while (re.Read())
                        {                    
                            for (int i = 0; i < DBColumnNames.Length; i++)
                            {
                                msg.setKeyValuePair(Config.UI_ClientContactKeys[i], re.GetValue(re.GetOrdinal(DBColumnNames[i])).ToString().Trim(' '));
                            }
                            client = msg.getValueFromPairs("4");
                            if (!CIMList.Keys.Contains(client))
                            {
                                CIMList.Add(client, new List<ListViewItem>() { LV_OP.getLVI(msg,Config.UI_ClientContactKeys) });
                            }
                            else
                            {
                                CIMList[client].Add(LV_OP.getLVI(msg,Config.UI_ClientContactKeys));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
                MessageBox.Show("load CIM data ERROR");
            }
        }

        private void productMasterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (Form_ProductMaster == null)
            {
                Form_ProductMaster = new ProductMaster(ClientList,UserList,ProductCate_Pair,ClientServerProductVersion);
                Form_ProductMaster.Show();
            }
            else if (Form_ProductMaster.IsDisposed)
            {
                Form_ProductMaster = new ProductMaster(ClientList, UserList, ProductCate_Pair, ClientServerProductVersion);
                Form_ProductMaster.Show();
            }
            else
            {
                Form_ProductMaster.WindowState = FormWindowState.Normal;
                Form_ProductMaster.Focus();
            }
        }

        private void productHintsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Form_ProductHints == null)
            {
                Form_ProductHints = new ProductHints(ProductCate_Pair);
                Form_ProductHints.Show();
            }
            else if (Form_ProductHints.IsDisposed)
            {
                Form_ProductHints = new ProductHints(ProductCate_Pair);
                Form_ProductHints.Show();
            }
            else
            {
                Form_ProductHints.WindowState = FormWindowState.Normal;
                Form_ProductHints.Focus();
            }
        }

        private void myMarkedCasesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Form_MyMarkedCases == null)
            {
                Form_MyMarkedCases = new MarkedCases(this);
                Form_MyMarkedCases.Show();
            }
            else if (Form_MyMarkedCases.IsDisposed)
            {
                Form_MyMarkedCases = new MarkedCases(this);
                Form_MyMarkedCases.Show();
            }
            else
            {
                Form_MyMarkedCases.WindowState = FormWindowState.Normal;
                Form_MyMarkedCases.Focus();
            }
        }

        private void caseAnalysisToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Form_Analysis == null)
            {
                Form_Analysis = new CaseAnalysis(myInfo,CaseProperty,CIMList,ClientList,UserList,ProductCate_Pair,ClientServerProductVersion,ClientAM,AMPhone);
                Form_Analysis.Show();
            }
            else if (Form_Analysis.IsDisposed)
            {
                Form_Analysis = new CaseAnalysis(myInfo,CaseProperty, CIMList, ClientList, UserList, ProductCate_Pair, ClientServerProductVersion,ClientAM, AMPhone);
                Form_Analysis.Show();
            }
            else
            {
                Form_Analysis.WindowState = FormWindowState.Normal;
                Form_Analysis.Focus();
            }    
        }

        private void shortCutKeyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if ( Form_ShortCutKey == null)
            {
                Form_ShortCutKey = new ShortCutKey(this);
                Form_ShortCutKey.Show();
            }
            else if (Form_ShortCutKey.IsDisposed)
            {
                Form_ShortCutKey = new ShortCutKey(this);
                Form_ShortCutKey.Show();
            }
            else
            {
                Form_ShortCutKey.WindowState = FormWindowState.Normal;
                Form_ShortCutKey.Focus();
            }
        }

        private  void getFontObjs(Control root, Custom Form_Custom)
        {
            if (Form_Custom != null && !Form_Custom.IsDisposed)
            {
                if (root.HasChildren)
                {
                    foreach (Control item in root.Controls)
                    {
                        if (item is ListView || item is DataGridView)
                        {
                            Form_Custom.addControl(item);
                        }
                        if (item is RichTextBox && utility.isSetRTBFont(item as RichTextBox))
                        {
                            Form_Custom.addControl(item);
                        }
                        if (item is System.Windows.Forms.TextBox && utility.isSetTBFont(item as System.Windows.Forms.TextBox))
                        {
                            Form_Custom.addControl(item);
                        }
                        getFontObjs(item, Form_Custom);
                    }
                }
            }
        }

        private void customInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Form_Custom == null)
            {
                Form_Custom = new Custom(myInfo,CaseProperty);
                getFontObjs(this, Form_Custom);
                Form_Custom.Show();
            }
            else if (Form_Custom.IsDisposed)
            {
                Form_Custom = new Custom(myInfo,CaseProperty);
                getFontObjs(this, Form_Custom);
                Form_Custom.Show();
            }
            else
            {
                Form_Custom.WindowState = FormWindowState.Normal;
                Form_Custom.Focus();
            }
        }

        private void listView5_SizeChanged(object sender, EventArgs e)
        {
            LV_OP.listViewAutoSize(LV_CIM);
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            CD_followUps.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightBlue;
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (groupBox1.Text.Equals(""))
                {
                    MessageBox.Show("Please select a case");
                    CD_followUps.Rows.Clear();
                    return;
                }
                if (CD_followUps.Rows[e.RowIndex].Cells[1].Value == null)
                {
                    return;
                }
                else
                {
                    message msg = new message();
                    msg.setKeyValuePair("1", groupBox1.Text);
                    msg.setKeyValuePair("52", CD_followUps.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                    string type = "";
                    if (CD_followUps.Rows[e.RowIndex].Cells[0].Value == null)
                    {
                        type = "F1";
                    }
                    else
                    {
                        msg.setKeyValuePair("50", CD_followUps.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value.ToString());
                        type = "F2";
                    }
                    Config.SLS_Sock.socketMsg(type,msg, null);
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            string client = CD_client.Text.Trim();
            if (groupBox1.Text.Trim().Equals(""))
            {
                MessageBox.Show("For editing case only, please select a case first");
                return;
            }
            if (!CD_client.Items.Contains(client))
            {
                MessageBox.Show("Input client is not in client list !");
                return;
            }
            else if (String.IsNullOrEmpty(CD_inchange.Text))
            {
                MessageBox.Show("please input incharge person !");
                return;
            }
            else if (!utility.isCorrectVersionFormat(CD_product.Text, CD_version.Text))
            {
                MessageBox.Show("Version format of this product, must be 4 intergers seperated with 3 dots");
                return;
            }
            else
            {
                if (CD_status.Text.Trim(' ').Equals("Closed") && CD_solution.Text.Trim(' ').Equals(""))
                {
                    MessageBox.Show("Please input solution!");
                    return;
                }
                else
                {
                    tempCaseMsg.setKeyValuePair("3", CD_happenTime.Text);
                    tempCaseMsg.setKeyValuePair("4", CD_client.Text);
                    tempCaseMsg.setKeyValuePair("5", CD_callPerson.Text);
                    tempCaseMsg.setKeyValuePair("6", CD_phone.Text);
                    tempCaseMsg.setKeyValuePair("7", CD_through.Text);
                    tempCaseMsg.setKeyValuePair("8", CD_description.Text);
                    tempCaseMsg.setKeyValuePair("9", CD_caseType.Text);
                    tempCaseMsg.setKeyValuePair("10", CD_inchange.Text);
                    tempCaseMsg.setKeyValuePair("11", CD_status.Text);
                    tempCaseMsg.setKeyValuePair("12", CD_solution.Text);
                    tempCaseMsg.setKeyValuePair("13", CD_priority.Text);
                    tempCaseMsg.setKeyValuePair("14", CD_issueType.Text);
                    tempCaseMsg.setKeyValuePair("167", CD_product.Text);
                    tempCaseMsg.setKeyValuePair("160", CD_server.Text);
                    tempCaseMsg.setKeyValuePair("169", CD_version.Text);
                    Config.SLS_Sock.socketMsg("C2", tempCaseMsg, null);
                }
            }
        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (!groupBox1.Text.Equals(""))
            {
                message msg = new message();
                msg.setKeyValuePair("1", groupBox1.Text);
                msg.setKeyValuePair("73", CD_caseHints.Text);
                Config.SLS_Sock.socketMsg("H1", msg, null);
            }
        }

        private void comboBox10_TextChanged(object sender, EventArgs e)
        {
            try
            {
                linkLabel2.Text = "";
                string client = CD_client.Text.Trim(' '), product = "", version = "";
                if (!client.Equals(""))
                {
                    if (ClientServerProductVersion.ContainsKey(client))
                    {
                        string server = CD_server.Text.Trim(' ');
                        if (ClientServerProductVersion[client].ContainsKey(server))
                        {
                            product = CD_product.Text.Trim(' ');
                            if (ClientServerProductVersion[client][server].ContainsKey(product))
                            {
                                CD_version.Text = ClientServerProductVersion[client][server][product];
                                version = CD_version.Text;
                            }
                        }
                    }
                }
                if (!product.Equals("") && !version.Equals(""))
                {
                    linkLabel2.Text = "Analyzing...";
                    StringBuilder cmd = new StringBuilder("select * from ProductHints where Product ='").Append(product).Append("' and (FromVersion<='").Append(version).Append("' or ToVersion>='").Append(version).Append("')");
                    ThreadPool.QueueUserWorkItem(new WaitCallback(setProducHint), cmd);
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void setProducHint(object cmd)
        {
            ProductHints_Result = LV_OP.realtimeProductHint(cmd);
            if (ProductHints_Result == null)
            {
                linkLabel2.Invoke(new utility.Del_setText(utility.setText), new object[] { linkLabel2, "No ProductHints match" });
            }
            else
            {
                linkLabel2.Invoke(new utility.Del_setText(utility.setText), new object[] { linkLabel2, ProductHints_Result.Count.ToString() + " ProductHints" });
            }
        }

        private void linkLabel2_Click(object sender, EventArgs e)
        {
            if (Form_ProductHints == null)
            {
                Form_ProductHints = new ProductHints(ProductHints_Result,ProductCate_Pair);
                Form_ProductHints.Show();
            }
            else if (Form_ProductHints.IsDisposed)
            {
                Form_ProductHints = new ProductHints(ProductHints_Result,ProductCate_Pair);
                Form_ProductHints.Show();
            }
            else
            {
                Form_ProductHints.WindowState = FormWindowState.Normal;
                Form_ProductHints.Focus();
                LV_OP.initialListView(Form_ProductHints.ListView1, ProductHints_Result, true);
            }
        }

        private void comboBox1_TextChanged_1(object sender, EventArgs e)
        {
            string client = CD_client.Text.Trim(' ');
            utility.loadClientContact(CD_client, CD_callPerson, CIMList);
            CD_server.Items.Clear();
            if (ClientServerProductVersion.ContainsKey(client))
            {
                CD_server.Items.AddRange(ClientServerProductVersion[client].Keys.ToArray());
            }
            CD_AM.Text = utility.getAMInfo(client,ClientAM,AMPhone);
        }

        private void comboBox7_TextChanged(object sender, EventArgs e)
        {
            utility.loadContactPhone(CD_client, CD_callPerson, CD_phone, CIMList);
        }

        private void comboBox11_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string client = CD_client.Text.Trim(' ');
                if (client != "")
                {
                    if (ClientServerProductVersion.ContainsKey(client))
                    {
                        string server = CD_server.Text.Trim(' ');
                        if (ClientServerProductVersion[client].ContainsKey(server))
                        {
                            CD_product.Items.Clear();
                            CD_product.Items.AddRange(ClientServerProductVersion[client][server].Keys.ToArray());
                        }
                    }
                }
                if (CD_product.Text != "")
                {
                    comboBox10_TextChanged(sender,e);
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
                
            }
        }

        private void dataGridView1_KeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                if (e.KeyCode == Keys.Delete && CD_followUps.SelectedCells.Count > 0)
                {
                    int rowIndex = CD_followUps.SelectedCells[0].RowIndex;
                    if (rowIndex == CD_followUps.Rows.Count-2)
                    {
                        MessageBox.Show("Can't delete the first follow up.");
                        return;
                    }
                    else if (rowIndex == CD_followUps.Rows.Count - 1)
                    {
                        return;
                    }
                    else if (groupBox1.Text.Equals("") || CD_followUps.Rows[rowIndex].Cells[0].Value == null)
                    {
                        return;
                    }
                    else
                    {
                        CD_followUps.AllowUserToDeleteRows = true;
                        message msg = new message();
                        msg.setKeyValuePair("1", groupBox1.Text);
                        msg.setKeyValuePair("50", CD_followUps.Rows[rowIndex].Cells[0].Value.ToString());
                        Config.SLS_Sock.socketMsg("F3", msg, null);
                        CD_followUps.AllowUserToDeleteRows = false;
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Clipboard.Clear();
                if (!groupBox1.Text.Equals(""))
                {
                    Clipboard.SetText(groupBox1.Text);
                    MessageBox.Show("Copied CaseID: " + groupBox1.Text);
                }
                else
                {
                    MessageBox.Show("Please select a case first !");
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void CD_addFollowUp_Click(object sender, EventArgs e)
        {
            CD_followUps.Rows[CD_followUps.Rows.Count - 1].Cells[1].Selected = true;
            this.KeyDown+=new KeyEventHandler(MainForm_KeyDown);
        }

        private void LV_CIM_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (LV_CIM.SelectedItems.Count > 0)
            {
                LV_OP.changeLVIBackColor(LV_CIM.SelectedItems[0], Config.UI_ClientContactKeys);
            }
        }

        private void CloseCase_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(CD_client.Text) || String.IsNullOrEmpty(CD_inchange.Text))
            {
                MessageBox.Show("Please input Client and Incharge person!");
            }
            else if (!utility.isCorrectVersionFormat(CD_product.Text, CD_version.Text))
            {
                MessageBox.Show("Version format of this product, must be 4 intergers seperated with 3 dots");
            }
            else
            {
                if (CD_solution.Text.Trim(' ').Equals(""))
                {
                    MessageBox.Show("Please input solution!");
                }
                else
                {
                    tempCaseMsg.setKeyValuePair("3", CD_happenTime.Text);
                    tempCaseMsg.setKeyValuePair("4", CD_client.Text);
                    tempCaseMsg.setKeyValuePair("5", CD_callPerson.Text);
                    tempCaseMsg.setKeyValuePair("6", CD_phone.Text);
                    tempCaseMsg.setKeyValuePair("7", CD_through.Text);
                    tempCaseMsg.setKeyValuePair("8", CD_description.Text);
                    tempCaseMsg.setKeyValuePair("9", CD_caseType.Text);
                    tempCaseMsg.setKeyValuePair("10", CD_inchange.Text);
                    tempCaseMsg.setKeyValuePair("11", "Closed");
                    tempCaseMsg.setKeyValuePair("12", CD_solution.Text);
                    tempCaseMsg.setKeyValuePair("13", CD_priority.Text);
                    tempCaseMsg.setKeyValuePair("14", CD_issueType.Text);
                    tempCaseMsg.setKeyValuePair("167", CD_product.Text);
                    tempCaseMsg.setKeyValuePair("160", CD_server.Text);
                    tempCaseMsg.setKeyValuePair("169", CD_version.Text);
                    Config.SLS_Sock.socketMsg("C2", tempCaseMsg, null);
                }
            }
        }

        private void Combo_Search_Cate_TextChanged(object sender, EventArgs e)
        {
            Combo_Search_Subcate.Text = "";
            Combo_Search_Subcate.Items.Clear();
            switch (Combo_Search_Cate.Text.Trim(' '))
            {
                case "Client":
                    {
                        Combo_OP.initialComboBox(Combo_Search_Subcate, ClientList);
                        break;
                    }
                case "Incharge":
                    {
                        Combo_OP.initialComboBox(Combo_Search_Subcate, UserList);
                        break;
                    }
                case "Product":
                    {
                        Combo_OP.initialComboBox(Combo_Search_Subcate, ProductCate_Pair.Keys.ToArray());
                        break;
                    }
                case "Priority":
                    {
                        Combo_OP.initialComboBox(Combo_Search_Subcate, CaseProperty["Priority"], ",");
                        break;
                    }
                case "Issue":
                    {
                        Combo_OP.initialComboBox(Combo_Search_Subcate, CaseProperty["Issue"], ",");
                        break;
                    }
                case "Type":
                    {
                        Combo_OP.initialComboBox(Combo_Search_Subcate, CaseProperty["Type"], ",");
                        break;
                    }
                case "Status":
                    {
                        Combo_OP.initialComboBox(Combo_Search_Subcate, CaseProperty["Status"], ",");
                        break;
                    }
                default:
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string caseIDs = LV_OP.getColumnValues(LV_Reminder_Expired, Config.UI_CaseKeys, "1");
            LV_OP.clearListView(LV_Reminder_Expired);
            Label_RemindTime.Text = "Remind Time: ";
            ThreadPool.QueueUserWorkItem(new WaitCallback(Reminder_OP.removeReminders), caseIDs);
        }

        private void LV_outstanding_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                clickCase(LV_outstanding,e.Item, true, ref OutstandingCount, tabPage2, "Outstanding");
            }
        }

        private void LV_search_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                int i = -1;
                clickCase(LV_search,e.Item, false, ref  i, null, null);
            }
        }

        private void LV_mytask_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            if (e.IsSelected)
            {
                clickCase(LV_mytask, e.Item, true, ref  MyTaskCount, tabPage4, "MyTask");
            }
        }

        private void LV_Reminder_Expired_DoubleClick(object sender, EventArgs e)
        {
            if (LV_Reminder_Expired.SelectedItems.Count > 0)
            {
                ActCase EditCase = new ActCase(myInfo, LV_Reminder_Expired.SelectedItems[0], CaseProperty, ClientList, UserList, ProductCate_Pair, ClientServerProductVersion, CIMList, ClientAM, AMPhone);
                EditCase.Show();
            }  
        }

        private void LV_CIM_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && LV_CIM.SelectedItems.Count>0)
            {
                if (CopyCIMInfo.ContainsKey(tabControl1.SelectedTab))
                {
                    CopyCIMInfo[tabControl1.SelectedTab](LV_CIM.SelectedItems[0], null);
                }
            }
        }

        private void LV_CIM_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && LV_CIM.SelectedItems.Count > 0 )
            {
                ActClientContact EditContact = new ActClientContact(LV_CIM.SelectedItems[0], IsSuperUser,CIMList);
                EditContact.Show();
            }
        }

        private void LV_update_DoubleClick(object sender, EventArgs e)
        {
            if (LV_update.SelectedItems.Count > 0)
            {
                int userIndex = Config.getIndex(Config.UI_UserUpdateKeys, "80") + 1;
                if (userIndex > 0)
                {
                    tabControl2.SelectedIndex = 1;
                    string name =  LV_update.SelectedItems[0].SubItems[userIndex].Text;
                    if (!Combo_Search_Cate.Text.Trim().Equals("Incharge") || !Combo_Search_Subcate.Text.Trim().Equals(name))
                    {
                        Combo_Search_Cate.Text = "Incharge";
                        Combo_Search_Subcate.Text = name;
                    }
                    else
                    {
                        searchOutstandingCateCases("Incharge", name);
                    }
                }
            }
        }

        private void LV_Reminder_Click(object sender, EventArgs e)
        {
            Label_RemindTime.Text = "Remind Time:";
            if (LV_Reminder.SelectedItems.Count>0)
            {
                clickCase(LV_Reminder, LV_Reminder.SelectedItems[0], false, ref ReminderCounter, tabPage11, "Reminder");
                Label_RemindTime.Text = "Remind Time: " + Reminder_OP.getRemindTime(LV_OP.getColumnValue(LV_Reminder.SelectedItems[0], Config.UI_CaseKeys, "1"));
            }
        }

        private void LV_Reminder_Expired_Click(object sender, EventArgs e)
        {
            Label_RemindTime.Text = "Remind Time:";
            if (LV_Reminder_Expired.SelectedItems.Count>0)
            {
                clickCase(LV_Reminder_Expired, LV_Reminder_Expired.SelectedItems[0], true, ref ReminderCounter, tabPage11, "Reminder");
                Label_RemindTime.Text = "Remind Time: " + Reminder_OP.getRemindTime(LV_OP.getColumnValue(LV_Reminder_Expired.SelectedItems[0], Config.UI_CaseKeys, "1"));
            }
        }

        private void searchOutstandingCateCases(string cate, string subCate)
        {
            label_SearchCount.Hide();   
            List<ListViewItem> items = new List<ListViewItem>();
            if (Config.CategoryType1.Contains(cate) && Combo_Search_Subcate.Items.Contains(subCate))
            {
                string[] colums = Config.getValues(Config.UI_CaseKeys);
                int statusIndex = Config.getIndex(Config.UI_CaseKeys, "11") + 1;
                int catIndex = 0;
                for (int i = 0; i < colums.Length; i++)
                {
                    if (colums[i].Equals(cate))
                    {
                        catIndex = i + 1;
                        break;
                    }
                }
                if (catIndex > 0 && statusIndex > 0)
                {
                    for (int i = 0; i < LV_outstanding.Items.Count; i++)
                    {
                        if (LV_outstanding.Items[i].SubItems[catIndex].Text.Equals(subCate) && !LV_outstanding.Items[i].SubItems[statusIndex].Text.Equals("Closed"))
                        {
                            items.Add((ListViewItem)LV_outstanding.Items[i].Clone());
                        }
                    }
                }
            }
            LV_OP.initialListView(LV_search, items, true);
            label_SearchCount.Text = items.Count + " records";
            label_SearchCount.Show();
        }

        private void Combo_Search_Subcate_TextChanged(object sender, EventArgs e)
        {
            searchOutstandingCateCases(Combo_Search_Cate.Text.Trim(' '), Combo_Search_Subcate.Text.Trim(' '));
        }

        private void checkBox_Popup_CheckedChanged(object sender, EventArgs e)
        {
            IsPopupReminder = checkBox_Popup.Checked;
        }

        private void CD_solution_Enter(object sender, EventArgs e)
        {
            CD_solution.ScrollBars = ScrollBars.Vertical;
        }

        private void CD_solution_Leave(object sender, EventArgs e)
        {
            CD_solution.ScrollBars = ScrollBars.None;
        }

        private void CD_description_Enter(object sender, EventArgs e)
        {
            CD_description.ScrollBars = ScrollBars.Vertical;
        }

        private void CD_description_Leave(object sender, EventArgs e)
        {
            CD_description.ScrollBars = ScrollBars.None;
        }

        private void QI_description_Enter(object sender, EventArgs e)
        {
            QI_description.ScrollBars = ScrollBars.Vertical;
        }

        private void QI_description_Leave(object sender, EventArgs e)
        {
            QI_description.ScrollBars = ScrollBars.None;
        }

        private void CIM_client_TextChanged(object sender, EventArgs e)
        {
            locateClientInCIM(CIM_client.Text.Trim());
        }

        private void linkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string caseID = groupBox1.Text;
            Reminder reminder = new Reminder(OutstandingCases[caseID]);
            reminder.Show();
        }

        private void Notice_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            Notice.Rows[e.RowIndex].Cells[e.ColumnIndex].Style.BackColor = Color.LightBlue;
        }

        private void Notice_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (groupBox1.Text.Equals(""))
                {
                    MessageBox.Show("Please select a case");
                    CD_followUps.Rows.Clear();
                    return;
                }
                if (CD_followUps.Rows[e.RowIndex].Cells[1].Value == null)
                {
                    return;
                }
                else
                {
                    message msg = new message();
                    msg.setKeyValuePair("1", groupBox1.Text);
                    msg.setKeyValuePair("52", CD_followUps.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString());
                    string type = "";
                    if (CD_followUps.Rows[e.RowIndex].Cells[0].Value == null)
                    {
                        type = "F1";
                    }
                    else
                    {
                        msg.setKeyValuePair("50", CD_followUps.Rows[e.RowIndex].Cells[e.ColumnIndex - 1].Value.ToString());
                        type = "F2";
                    }
                    Config.SLS_Sock.socketMsg(type, msg, null);
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void Notice_KeyDown(object sender, KeyEventArgs e)
        {

        }

    }
}
