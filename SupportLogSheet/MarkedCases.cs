// 收藏的case 窗口
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Data.SqlClient;
using System.Threading;

namespace SupportLogSheet
{
    public partial class MarkedCases : Form
    {
        public Dictionary<string, ListView> Category_LVs;
        public bool SortType = false;
        public MarkedCaseOverView Form_OverView;
        public bool BOOL_isInstanceOverView =false;
        public MainForm mf;
        public MarkedCases(int i)  
        {
            // for instance in markedCaseOverView
            InitializeComponent();
        }

        public MarkedCases(MainForm mf)
        {
            InitializeComponent();
            this.mf = mf;
            Category_LVs = new Dictionary<string, ListView>();
            Combo_OP.initialToolComboBox(toolStripComboBox1, mf.MarkedCateCases.Keys.ToList());
            initialMyMarkedCase(initial_MarkedCase());
        }

        public void initialMyMarkedCase(Dictionary<string,List<ListViewItem>> cases)
        {
            foreach (string cate in cases.Keys)
            {
                initialTabPage(cate, cases[cate]);
            }
        }

        public Dictionary<string,List<ListViewItem>> initial_MarkedCase()
        {
            Dictionary<string, List<ListViewItem>> cases = new Dictionary<string, List<ListViewItem>>();
            using (SqlConnection sqlConnection = SQL.dbConnect())
            {               
                try
                {
                    foreach(string category in mf.MarkedCateCases.Keys)
                    {
                        List<ListViewItem> lvis = new List<ListViewItem>();
                         if (mf.MarkedCateCases[category].Count > 0)
                         {
                             StringBuilder cmd = new StringBuilder("select * from SupportLogSheet where CaseID in ('");
                             for (int t = 0; t < mf.MarkedCateCases[category].Count - 1; t++)
                             {
                                 cmd.Append(mf.MarkedCateCases[category][t]).Append("','");
                             }
                             cmd.Append(mf.MarkedCateCases[category].Last()).Append("')");
                             lvis =  SQL.genListLvi(cmd.ToString(), Config.UI_CaseKeys, sqlConnection); 
                         }
                         cases.Add(category,lvis);
                    }
                }
                catch (Exception ex)
                {
                    Config.logWriter.writeErrorLog(ex);
                }
            }
            return cases;
        }

        //public void ClearEvent(Control control, string eventname)
        //{
        //    if (control == null) return;
        //    if (string.IsNullOrEmpty(eventname)) return;

        //    BindingFlags mPropertyFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic;
        //    BindingFlags mFieldFlags = BindingFlags.Static | BindingFlags.NonPublic;
        //    type controlType = typeof(System.Windows.Forms.Control);
        //    PropertyInfo propertyInfo = controlType.GetProperty("Events", mPropertyFlags);
        //    EventHandlerList eventHandlerList = (EventHandlerList)propertyInfo.GetValue(control, null);
        //    FieldInfo fieldInfo = (typeof(Control)).GetField("Event" + eventname, mFieldFlags);
        //    Delegate d = eventHandlerList[fieldInfo.GetValue(control)];

        //    if (d == null) return;
        //    EventInfo eventInfo = controlType.GetEvent(eventname);
        //    foreach (Delegate dx in d.GetInvocationList())
        //    {
        //        eventInfo.RemoveEventHandler(control, dx);
        //    }
        //}

        public void updateCateList()
        {
            Combo_OP.initialToolComboBox(toolStripComboBox1, mf.MarkedCateCases.Keys.ToList());
            Combo_OP.initialToolComboBox(mf.ToolStripComboBox1, mf.MarkedCateCases.Keys.ToList());
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            message msg=UI_deleteCatogory(tabControl1.SelectedIndex,new message());
            Msg_OP msg_Op = new Msg_OP("IO3", new message[] { msg });
           Config.SLS_Sock.inMsgQueue_add(msg_Op);
        }

        public void tabStyle(TabPage tab)
        {
            tab.BackColor = System.Drawing.Color.White;
            tab.ContextMenuStrip = this.contextMenuStrip1;
            tab.Location = new System.Drawing.Point(4, 22);
            tab.Padding = new System.Windows.Forms.Padding(3);
            tab.Size = new System.Drawing.Size(752, 432);
        }

        public void buttonStyle(Button button)
        {
            button.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            button.Font = new System.Drawing.Font("Tahoma", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            button.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            button.ForeColor = System.Drawing.Color.OliveDrab;
            button.Location = new System.Drawing.Point(1105, 3);
            button.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            button.Size = new System.Drawing.Size(68, 39);
            button.Text = "Save";
            button.UseVisualStyleBackColor = false;
            //button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        }

        public void richTextBoxStyle(RichTextBox richTextBox,string category)
        {
            richTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            richTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            richTextBox.Font = Config.Font_Content;
            richTextBox.Location = new System.Drawing.Point(1, 3);
            richTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            richTextBox.Multiline = true;
            richTextBox.RightToLeft = System.Windows.Forms.RightToLeft.No;
            richTextBox.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            richTextBox.Size = new System.Drawing.Size(1097, 40);
            richTextBox.Text = " Input description of this category";
            //richTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            string description = mf.getCateDescription(category);
           if (!description.Equals(""))
            {
                richTextBox.Text = description;
            }
            richTextBox.Click += delegate(object o, EventArgs e)
            {
                if (richTextBox.Text == " Input description of this category")
                {
                    richTextBox.Clear();
                }
            };
        }

        public void listViewStyle(ListViewNF lv,string category)
        {
            lv.Name = category;
            ColumnHeader[] columns = new ColumnHeader[Config.UI_CaseKeys.Length + 1];
            columns[0] = new ColumnHeader();
            columns[0].Text = "Case";
            columns[0].Width = 0;
            string[] DBColumnNames = Config.getValues(Config.UI_CaseKeys);
            for (int i = 0; i < DBColumnNames.Length; i++)
            {
                columns[i + 1] = new ColumnHeader();
                columns[i + 1].Text = DBColumnNames[i];
            }
            lv.Columns.AddRange(columns);
            LV_OP.readColumnOrder(lv, Config.Customization_INI, "Outstanding_Order");
            LV_OP.readColumnWidth(lv, Config.Customization_INI, "Outstanding_Width");
            lv.AllowColumnReorder = true;
            //lv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            //| System.Windows.Forms.AnchorStyles.Left)
            //| System.Windows.Forms.AnchorStyles.Right)));
            lv.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            lv.Font = Config.Font_Content;
            lv.BorderStyle = System.Windows.Forms.BorderStyle.None;
            lv.CausesValidation = false;         
            lv.ContextMenuStrip = this.contextMenuStrip1;
            lv.FullRowSelect = true;
            lv.GridLines = true;
            lv.HideSelection = false;
            lv.Location = new System.Drawing.Point(0, 50);
            lv.Margin = new System.Windows.Forms.Padding(0, 0, 0, 0);
            lv.MultiSelect = false;
            lv.RightToLeft = System.Windows.Forms.RightToLeft.No;
            lv.Size = new System.Drawing.Size(1180, 495);
            lv.UseCompatibleStateImageBehavior = false;
            lv.View = System.Windows.Forms.View.Details;
            lv.Scrollable = true;
            lv.ContextMenuStrip = contextMenuStrip2;
            lv.SelectedIndexChanged += delegate(object o, EventArgs e)
            {
                if (lv.SelectedItems.Count > 0)
                {
                    if (LV_OP.IsNewOrEdited(lv.SelectedItems[0]))
                    {
                        lv.BeginUpdate();
                        LV_OP.changeLviBackColor(lv.SelectedItems[0], Config.UI_CaseKeys);
                        lv.EndUpdate();
                    }
                }
            };
            lv.DoubleClick += delegate(object o, EventArgs e)
            {
                if (lv.SelectedItems.Count > 0)
                {
                    MarkedCaseContent markedCaseContent = new MarkedCaseContent(category, lv.SelectedItems[0].SubItems[1].Text);
                    markedCaseContent.RichTextBox1.Text = lv.SelectedItems[0].SubItems[8].Text;
                    markedCaseContent.RichTextBox2.Text = lv.SelectedItems[0].SubItems[12].Text;
                    markedCaseContent.myMarkedCase = this;
                    markedCaseContent.Show();
                }
            };
            lv.ColumnClick += delegate(object o, ColumnClickEventArgs e)
            {
                if (e.Column != 0)
                {
                    lv.ListViewItemSorter = new ListViewItemsComparer(e.Column, SortType);
                    lv.Sort();
                    SortType = !SortType;
                }
            };            
        }

        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                TabPage newTab = new TabPage();
                newTab.Text = toolStripTextBox1.Text.Trim(' ');
                tabStyle(newTab);
                tabControl1.Controls.Add(newTab);
                contextMenuStrip1.Hide();
            }
        }

        private void toolStripTextBox1_Click(object sender, EventArgs e)
        {
            if (toolStripTextBox1.Text == "Input and enter")
            {
                toolStripTextBox1.Clear();
            }
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (toolStripTextBox2.Text.Trim(' ') != "")
            {
                message msg=UI_editCategory(toolStripTextBox2.Text.Trim(' '), tabControl1.SelectedTab.Text.Trim(' '), tabControl1.SelectedIndex,new message());
                Msg_OP msg_Op = new Msg_OP("IO2", new message[] { msg });
               Config.SLS_Sock.inMsgQueue_add(msg_Op);
            }
            else
            { 
                MessageBox.Show("Please input category name!");
            }
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (toolStripTextBox1.Text.Trim(' ') != "")
            {
                message msg=UI_addCate(toolStripTextBox1.Text.Trim(' '), new message());
                Msg_OP msg_OP=new Msg_OP("IO1",new message[]{msg});
               Config.SLS_Sock.inMsgQueue_add(msg_OP);
            }
            else
            {
                MessageBox.Show("Please input category name!");
            }
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (!BOOL_isInstanceOverView)
            {
                Form_OverView = new MarkedCaseOverView(this);
                BOOL_isInstanceOverView = true;
                Form_OverView.Show();
            }
            else
            {
                Form_OverView.WindowState = FormWindowState.Normal;
                Form_OverView.Focus();
            }
        }

        private void removeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Category_LVs.ContainsKey(tabControl1.SelectedTab.Text))
            {
                if (Category_LVs[tabControl1.SelectedTab.Text].SelectedItems.Count > 0)
                {
                    message msg = UI_removeCase(tabControl1.SelectedTab.Text, Category_LVs[tabControl1.SelectedTab.Text].SelectedItems[0], new message());
                    Msg_OP msg_OP = new Msg_OP("IO5", new message[] { msg });
                    Config.SLS_Sock.inMsgQueue_add(msg_OP);
                }
            }
        }

        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            try
            {
                if (toolStripComboBox1.Text != "")
                {
                    string from_Cate = tabControl1.SelectedTab.Text;
                    string to_Cate = toolStripComboBox1.Text.Trim(' ');
                    if (Category_LVs[from_Cate].SelectedItems.Count > 0)
                    {
                        message msg = UI_moveCaseBetweenCates(from_Cate, to_Cate, Category_LVs[from_Cate].SelectedItems[0], new message());
                        Msg_OP msg_OP = new Msg_OP("IO6", new message[] { msg });
                       Config.SLS_Sock.inMsgQueue_add(msg_OP);
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        public void initialTabPage(string category, List<ListViewItem> cases)
        {
            TabPage tab = new TabPage();
            tab.Text = category;
            tabStyle(tab);
            ListViewNF lv = new ListViewNF();
            RichTextBox richTextBox = new RichTextBox();
            Button button = new Button();
            tabControl1.Controls.Add(tab);
            tab.Controls.Add(button);
            tab.Controls.Add(richTextBox);
            tab.Controls.Add(lv);
         
            listViewStyle(lv,category);
            Category_LVs.Add(category, lv);

            LV_OP.initialListView(lv, cases, true);
  
            richTextBoxStyle(richTextBox,category);

            buttonStyle(button);            
            button.Click += delegate(object o, EventArgs e)
            {
                if (richTextBox.Text != " Input description of this category")
                {
                    message msg = new message();
                    msg.setKeyValuePair("210", category);
                    msg.setKeyValuePair("212", richTextBox.Text);
                    Msg_OP msg_OP = new Msg_OP("IO7", new message[] { msg });
                   Config.SLS_Sock.inMsgQueue_add(msg_OP);
                }
            };
        }

        public message UI_removeCase(string category, ListViewItem lvi,message msg)
        {         
            string caseID = LV_OP.getColumnValue(Category_LVs[category].SelectedItems[0], Config.UI_CaseKeys, "1");
            Category_LVs[category].BeginUpdate();
            Category_LVs[category].Items.Remove(lvi);
            Category_LVs[category].EndUpdate();
            mf.MarkedCateCases[category].Remove(caseID);
            msg.setKeyValuePair("1", caseID);
            msg.setKeyValuePair("210", category);
            msg.setKeyValuePair("212", new StringBuilder("./").Append(category).Append("/").Append(caseID).Append("_CaseRemark.doc").ToString());
            return msg;
        }

        public message UI_moveCaseBetweenCates(string category, string newCategoy, ListViewItem lvi, message msg)
        {
            toolStripComboBox1.Text = "";
            msg = UI_removeCase(category, lvi, msg);
            string caseID = msg.getValueFromPairs("1");
            int index = Config.getIndex(Config.UI_CaseKeys, "1");
            if (index >= 0)
            {
                index++;
                foreach (ListViewItem lvi_temp in Category_LVs[newCategoy].Items)
                {
                    if (caseID.Equals(lvi_temp.SubItems[index].Text))
                    {
                        msg.setKeyValuePair("210", category);
                        msg.setKeyValuePair("211", newCategoy);
                        return msg;
                    }
                }
                msg = UI_addCase(newCategoy, lvi, msg);
                msg.setKeyValuePair("210", category);
                msg.setKeyValuePair("211", newCategoy);
            }
            return msg;
        }

        public message UI_editCategory(string newCategory, string category, int tabIndex, message msg)
        {
            tabControl1.TabPages[tabIndex].Text = newCategory;
            Category_LVs.Add(newCategory, Category_LVs[category]);
            Category_LVs.Remove(category);

            if (mf.MarkedCateCases.ContainsKey(category))
            {
                List<string> cases = mf.MarkedCateCases[category];
                mf.MarkedCateCases.Remove(category);
                mf.MarkedCateCases.Add(newCategory, cases);
            }
            toolStripTextBox2.Clear();
            updateCategories();
            msg.setKeyValuePair("210", category);
            msg.setKeyValuePair("211", newCategory);
            return msg;
        }

        public message UI_deleteCatogory(int index,message msg)
        {
            DialogResult result = MessageBox.Show("Delete ?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (result.ToString() == "OK")
            {
                string category = tabControl1.TabPages[index].Text;
                Category_LVs.Remove(category);
                tabControl1.TabPages[index].Dispose();
                mf.MarkedCateCases.Remove(category);
                updateCategories();
                msg.setKeyValuePair("210", category);
                return msg;
            }
            return msg;
        }

        public message UI_addCase(string category, ListViewItem lvi,message msg)
        {          
            string caseID = LV_OP.getColumnValue(lvi, Config.UI_CaseKeys, "1");
            if (!mf.MarkedCateCases.ContainsKey(category))
            {
                msg = UI_addCate(category,msg);
            }
            else
            {        
                msg.setKeyValuePair("210", category);
            }
            msg.setKeyValuePair("1", caseID);
            mf.MarkedCateCases[category].Add(caseID);
            LV_OP.insertOneListviewItem(Category_LVs[category], 0, lvi);
            return msg;
        }

        public message UI_addCate(string category,message msg)
        {
            initialTabPage(category, null);
            mf.MarkedCateCases.Add(category, new List<string>());
            updateCategories();
            msg.setKeyValuePair("210", category);
            return msg;
        }

        private void copyCaseIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                for (int i = 0; i < tabControl1.TabPages.Count; i++)
                {
                    foreach (Control t in tabControl1.TabPages[i].Controls)
                    {
                        if (t is ListView)
                        {
                            Clipboard.Clear();
                            if ((t as ListView).SelectedItems.Count > 0)
                            {
                                MessageBox.Show("Copied CaseID: " + (t as ListView).SelectedItems[0].SubItems[1].Text);
                                Clipboard.SetText((t as ListView).SelectedItems[0].SubItems[1].Text);
                                return;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void updateCategories()
        {
            Combo_OP.initialToolComboBox(toolStripComboBox1, mf.MarkedCateCases.Keys.ToList());
            Combo_OP.initialToolComboBox(mf.ToolStripComboBox1, mf.MarkedCateCases.Keys.ToList());
        }

        private void MarkedCases_Click(object sender, EventArgs e)
        {
            Label_Hint.Hide();
        }

    }
}
 