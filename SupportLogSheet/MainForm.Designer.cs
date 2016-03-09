using System.Windows.Forms;
using System;
using System.Threading;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;

namespace SupportLogSheet
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            this.quickInput = new System.Windows.Forms.Button();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.QI_AM = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.QI_description = new System.Windows.Forms.TextBox();
            this.QI_HappenTime = new System.Windows.Forms.RichTextBox();
            this.QI_IssueType = new SupportLogSheet.ComboNF();
            this.label29 = new System.Windows.Forms.Label();
            this.label32 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.QI_clear = new System.Windows.Forms.Button();
            this.QI_Phone = new SupportLogSheet.ComboNF();
            this.QI_CallPerson = new SupportLogSheet.ComboNF();
            this.QI_Product = new SupportLogSheet.ComboNF();
            this.QI_CaseType = new SupportLogSheet.ComboNF();
            this.QI_Client = new SupportLogSheet.ComboNF();
            this.QI_add = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.newCase = new System.Windows.Forms.Button();
            this.refresh = new System.Windows.Forms.Button();
            this.warning = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripComboBox1 = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.changeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.outstandingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.myTaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cIMToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.noticeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reminderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.caseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.myMarkedCasesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.caseAnalysisToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.productToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.productMasterToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.productHintsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customInfoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shortCutKeyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LabelCaseCount = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.tabControl2 = new System.Windows.Forms.TabControl();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.LV_outstanding = new SupportLogSheet.ListViewNF();
            this.Outstanding_Case = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Outstanding_CaseID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Outstanding_CreateTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Outstanding_HappenTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Outstanding_Client = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Outstanding_CallPerson = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Outstanding_ContactNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Outstanding_ReceivedThrough = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Outstanding_Description = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Outstanding_Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Outstanding_Incharge = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Outstanding_Status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Outstanding_Solution = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Outstanding_Priority = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Outstanding_IssueType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Outstanding_Product = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader17 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader18 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.TB_CaseID = new System.Windows.Forms.RichTextBox();
            this.label_SearchCount = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.Search_Multi = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.Search_export = new System.Windows.Forms.Button();
            this.Combo_Search_Subcate = new SupportLogSheet.ComboNF();
            this.LV_search = new SupportLogSheet.ListViewNF();
            this.Search_Case = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Search_CaseID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Search_CreateTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Search_HappenTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Search_Client = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Search_CallPerson = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Search_ContactNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Search_ReceivedThrough = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Search_Description = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Search_Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Search_Incharge = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Search_Status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Search_Solution = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Search_Priority = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Search_IssueType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Search_Product = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader19 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader20 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Search_clear = new System.Windows.Forms.Button();
            this.Search_search = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.Combo_Search_Cate = new SupportLogSheet.ComboNF();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.LV_mytask = new SupportLogSheet.ListViewNF();
            this.MyTask_Case = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyTask_CaseID = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyTask_CreateTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyTask_HappenTime = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyTask_Client = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyTask_CallPerson = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyTask_ContactNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyTask_ReceivedThrough = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyTask_Description = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyTask_Type = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyTask_Incharge = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyTask_Status = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyTask_Solution = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyTask_Priority = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyTask_IssueType = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.MyTask_Product = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader21 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader22 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.CIM_phone = new System.Windows.Forms.RichTextBox();
            this.CIM_add = new System.Windows.Forms.Button();
            this.CIM_search = new System.Windows.Forms.Button();
            this.label46 = new System.Windows.Forms.Label();
            this.CIM_client = new SupportLogSheet.ComboNF();
            this.LV_CIM = new SupportLogSheet.ListViewNF();
            this.columnHeader43 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader45 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader46 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader47 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader48 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader49 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader50 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader51 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label45 = new System.Windows.Forms.Label();
            this.tabPage10 = new System.Windows.Forms.TabPage();
            this.Notice = new SupportLogSheet.DataGridViewNF();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPage11 = new System.Windows.Forms.TabPage();
            this.checkBox_Popup = new System.Windows.Forms.CheckBox();
            this.Label_RemindCaseID = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.label10 = new System.Windows.Forms.Label();
            this.LV_Reminder_Expired = new SupportLogSheet.ListViewNF();
            this.columnHeader25 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader26 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader27 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader28 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader29 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader30 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader31 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader32 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader33 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader34 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader35 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader36 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader37 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader38 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader39 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader40 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader41 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader42 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Label_RemindTime = new System.Windows.Forms.Label();
            this.LV_Reminder = new SupportLogSheet.ListViewNF();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader14 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader15 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader16 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader23 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader24 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.linkLabel3 = new System.Windows.Forms.LinkLabel();
            this.CD_happenTime = new System.Windows.Forms.RichTextBox();
            this.CD_version = new System.Windows.Forms.RichTextBox();
            this.CD_solution = new System.Windows.Forms.TextBox();
            this.CD_description = new System.Windows.Forms.TextBox();
            this.CloseCase = new System.Windows.Forms.Button();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.CD_AM = new System.Windows.Forms.Label();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.CD_addCaseHints = new System.Windows.Forms.Button();
            this.CD_addFollowUp = new System.Windows.Forms.Button();
            this.label31 = new System.Windows.Forms.Label();
            this.CD_caseHints = new System.Windows.Forms.RichTextBox();
            this.CD_save = new System.Windows.Forms.Button();
            this.CD_through = new SupportLogSheet.ComboNF();
            this.label28 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label27 = new System.Windows.Forms.Label();
            this.label23 = new System.Windows.Forms.Label();
            this.CD_followUps = new SupportLogSheet.DataGridViewNF();
            this.Time = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Content = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label25 = new System.Windows.Forms.Label();
            this.label26 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label20 = new System.Windows.Forms.Label();
            this.CD_issueType = new SupportLogSheet.ComboNF();
            this.label21 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.CD_caseType = new SupportLogSheet.ComboNF();
            this.CD_callPerson = new SupportLogSheet.ComboNF();
            this.CD_phone = new SupportLogSheet.ComboNF();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.CD_server = new SupportLogSheet.ComboNF();
            this.CD_inchange = new SupportLogSheet.ComboNF();
            this.label12 = new System.Windows.Forms.Label();
            this.CD_product = new SupportLogSheet.ComboNF();
            this.label13 = new System.Windows.Forms.Label();
            this.CD_status = new SupportLogSheet.ComboNF();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.CD_client = new SupportLogSheet.ComboNF();
            this.CD_priority = new SupportLogSheet.ComboNF();
            this.LV_update = new SupportLogSheet.ListViewNF();
            this.columnHeader60 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader61 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader62 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader44 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tabPage1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tabControl2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.tabPage5.SuspendLayout();
            this.tabPage10.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Notice)).BeginInit();
            this.tabPage11.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CD_followUps)).BeginInit();
            this.SuspendLayout();
            // 
            // quickInput
            // 
            this.quickInput.BackColor = System.Drawing.SystemColors.Window;
            this.quickInput.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.quickInput.ForeColor = System.Drawing.SystemColors.WindowText;
            this.quickInput.Location = new System.Drawing.Point(14, 38);
            this.quickInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.quickInput.Name = "quickInput";
            this.quickInput.Size = new System.Drawing.Size(70, 23);
            this.quickInput.TabIndex = 10;
            this.quickInput.Text = "QucikInput";
            this.quickInput.UseVisualStyleBackColor = false;
            this.quickInput.Click += new System.EventHandler(this.Case_Click);
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.White;
            this.tabPage1.Controls.Add(this.QI_AM);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.QI_description);
            this.tabPage1.Controls.Add(this.QI_HappenTime);
            this.tabPage1.Controls.Add(this.QI_IssueType);
            this.tabPage1.Controls.Add(this.label29);
            this.tabPage1.Controls.Add(this.label32);
            this.tabPage1.Controls.Add(this.label6);
            this.tabPage1.Controls.Add(this.QI_clear);
            this.tabPage1.Controls.Add(this.QI_Phone);
            this.tabPage1.Controls.Add(this.QI_CallPerson);
            this.tabPage1.Controls.Add(this.QI_Product);
            this.tabPage1.Controls.Add(this.QI_CaseType);
            this.tabPage1.Controls.Add(this.QI_Client);
            this.tabPage1.Controls.Add(this.QI_add);
            this.tabPage1.Controls.Add(this.label5);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.ForeColor = System.Drawing.Color.Black;
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage1.Size = new System.Drawing.Size(213, 413);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Input";
            // 
            // QI_AM
            // 
            this.QI_AM.AutoSize = true;
            this.QI_AM.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.QI_AM.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.QI_AM.Location = new System.Drawing.Point(60, 47);
            this.QI_AM.Name = "QI_AM";
            this.QI_AM.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.QI_AM.Size = new System.Drawing.Size(0, 13);
            this.QI_AM.TabIndex = 87;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label11.Location = new System.Drawing.Point(6, 47);
            this.label11.Margin = new System.Windows.Forms.Padding(0);
            this.label11.Name = "label11";
            this.label11.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label11.Size = new System.Drawing.Size(22, 13);
            this.label11.TabIndex = 86;
            this.label11.Text = "AM";
            // 
            // QI_description
            // 
            this.QI_description.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.QI_description.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.QI_description.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.QI_description.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.QI_description.Location = new System.Drawing.Point(6, 264);
            this.QI_description.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.QI_description.Multiline = true;
            this.QI_description.Name = "QI_description";
            this.QI_description.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.QI_description.Size = new System.Drawing.Size(201, 108);
            this.QI_description.TabIndex = 8;
            this.QI_description.Enter += new System.EventHandler(this.QI_description_Enter);
            this.QI_description.Leave += new System.EventHandler(this.QI_description_Leave);
            // 
            // QI_HappenTime
            // 
            this.QI_HappenTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.QI_HappenTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.QI_HappenTime.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.QI_HappenTime.Location = new System.Drawing.Point(64, 212);
            this.QI_HappenTime.MaxLength = 40;
            this.QI_HappenTime.Multiline = false;
            this.QI_HappenTime.Name = "QI_HappenTime";
            this.QI_HappenTime.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.QI_HappenTime.Size = new System.Drawing.Size(142, 18);
            this.QI_HappenTime.TabIndex = 7;
            this.QI_HappenTime.Text = "";
            // 
            // QI_IssueType
            // 
            this.QI_IssueType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.QI_IssueType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.QI_IssueType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.QI_IssueType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.QI_IssueType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.QI_IssueType.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.QI_IssueType.FormattingEnabled = true;
            this.QI_IssueType.Location = new System.Drawing.Point(63, 183);
            this.QI_IssueType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.QI_IssueType.Name = "QI_IssueType";
            this.QI_IssueType.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.QI_IssueType.Size = new System.Drawing.Size(143, 21);
            this.QI_IssueType.TabIndex = 6;
            // 
            // label29
            // 
            this.label29.AutoSize = true;
            this.label29.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label29.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label29.Location = new System.Drawing.Point(6, 215);
            this.label29.Name = "label29";
            this.label29.Size = new System.Drawing.Size(55, 13);
            this.label29.TabIndex = 16;
            this.label29.Text = "HappenAt";
            // 
            // label32
            // 
            this.label32.AutoSize = true;
            this.label32.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label32.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label32.Location = new System.Drawing.Point(6, 187);
            this.label32.Name = "label32";
            this.label32.Size = new System.Drawing.Size(33, 13);
            this.label32.TabIndex = 15;
            this.label32.Text = "Issue";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label6.Location = new System.Drawing.Point(6, 244);
            this.label6.Margin = new System.Windows.Forms.Padding(0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "Description";
            // 
            // QI_clear
            // 
            this.QI_clear.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.QI_clear.BackColor = System.Drawing.Color.IndianRed;
            this.QI_clear.FlatAppearance.BorderColor = System.Drawing.Color.IndianRed;
            this.QI_clear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.QI_clear.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.QI_clear.Location = new System.Drawing.Point(41, 380);
            this.QI_clear.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.QI_clear.Name = "QI_clear";
            this.QI_clear.Size = new System.Drawing.Size(55, 23);
            this.QI_clear.TabIndex = 10;
            this.QI_clear.Text = "Clear";
            this.QI_clear.UseVisualStyleBackColor = false;
            this.QI_clear.Click += new System.EventHandler(this.button3_Click);
            // 
            // QI_Phone
            // 
            this.QI_Phone.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.QI_Phone.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.QI_Phone.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.QI_Phone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.QI_Phone.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.QI_Phone.ForeColor = System.Drawing.SystemColors.WindowText;
            this.QI_Phone.FormattingEnabled = true;
            this.QI_Phone.Location = new System.Drawing.Point(63, 99);
            this.QI_Phone.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.QI_Phone.Name = "QI_Phone";
            this.QI_Phone.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.QI_Phone.Size = new System.Drawing.Size(143, 21);
            this.QI_Phone.TabIndex = 3;
            // 
            // QI_CallPerson
            // 
            this.QI_CallPerson.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.QI_CallPerson.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.QI_CallPerson.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.QI_CallPerson.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.QI_CallPerson.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.QI_CallPerson.ForeColor = System.Drawing.SystemColors.WindowText;
            this.QI_CallPerson.FormattingEnabled = true;
            this.QI_CallPerson.Location = new System.Drawing.Point(63, 71);
            this.QI_CallPerson.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.QI_CallPerson.Name = "QI_CallPerson";
            this.QI_CallPerson.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.QI_CallPerson.Size = new System.Drawing.Size(143, 21);
            this.QI_CallPerson.TabIndex = 2;
            this.QI_CallPerson.TextChanged += new System.EventHandler(this.comboBox1_TextChanged);
            // 
            // QI_Product
            // 
            this.QI_Product.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.QI_Product.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.QI_Product.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.QI_Product.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.QI_Product.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.QI_Product.FormattingEnabled = true;
            this.QI_Product.Location = new System.Drawing.Point(63, 127);
            this.QI_Product.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.QI_Product.Name = "QI_Product";
            this.QI_Product.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.QI_Product.Size = new System.Drawing.Size(143, 21);
            this.QI_Product.TabIndex = 4;
            // 
            // QI_CaseType
            // 
            this.QI_CaseType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.QI_CaseType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.QI_CaseType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.QI_CaseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.QI_CaseType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.QI_CaseType.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.QI_CaseType.FormattingEnabled = true;
            this.QI_CaseType.Location = new System.Drawing.Point(63, 155);
            this.QI_CaseType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.QI_CaseType.Name = "QI_CaseType";
            this.QI_CaseType.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.QI_CaseType.Size = new System.Drawing.Size(143, 21);
            this.QI_CaseType.TabIndex = 5;
            // 
            // QI_Client
            // 
            this.QI_Client.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.QI_Client.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.QI_Client.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.QI_Client.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.QI_Client.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.QI_Client.ForeColor = System.Drawing.SystemColors.WindowText;
            this.QI_Client.FormattingEnabled = true;
            this.QI_Client.ItemHeight = 13;
            this.QI_Client.Location = new System.Drawing.Point(63, 15);
            this.QI_Client.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.QI_Client.Name = "QI_Client";
            this.QI_Client.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.QI_Client.Size = new System.Drawing.Size(143, 21);
            this.QI_Client.Sorted = true;
            this.QI_Client.TabIndex = 1;
            this.QI_Client.SelectedIndexChanged += new System.EventHandler(this.comboBox2_SelectedIndexChanged);
            this.QI_Client.TextChanged += new System.EventHandler(this.comboBox2_TextChanged);
            // 
            // QI_add
            // 
            this.QI_add.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.QI_add.BackColor = System.Drawing.Color.SeaGreen;
            this.QI_add.FlatAppearance.BorderColor = System.Drawing.Color.SeaGreen;
            this.QI_add.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.QI_add.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.QI_add.Location = new System.Drawing.Point(116, 380);
            this.QI_add.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.QI_add.Name = "QI_add";
            this.QI_add.Size = new System.Drawing.Size(55, 23);
            this.QI_add.TabIndex = 9;
            this.QI_add.Text = "Add";
            this.QI_add.UseVisualStyleBackColor = false;
            this.QI_add.Click += new System.EventHandler(this.button2_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label5.Location = new System.Drawing.Point(6, 159);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Type";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label4.Location = new System.Drawing.Point(6, 131);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(44, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Product";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label3.Location = new System.Drawing.Point(6, 103);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Phone";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label2.Location = new System.Drawing.Point(6, 75);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 7;
            this.label2.Text = "Caller";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(6, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(34, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Client";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(13, 75);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(221, 439);
            this.tabControl1.TabIndex = 9;
            // 
            // newCase
            // 
            this.newCase.BackColor = System.Drawing.Color.SeaGreen;
            this.newCase.FlatAppearance.BorderColor = System.Drawing.Color.SeaGreen;
            this.newCase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.newCase.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.newCase.Location = new System.Drawing.Point(107, 38);
            this.newCase.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.newCase.Name = "newCase";
            this.newCase.Size = new System.Drawing.Size(70, 23);
            this.newCase.TabIndex = 11;
            this.newCase.Text = "NewCase";
            this.newCase.UseVisualStyleBackColor = false;
            this.newCase.Click += new System.EventHandler(this.button4_Click);
            // 
            // refresh
            // 
            this.refresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.refresh.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.refresh.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.refresh.Location = new System.Drawing.Point(1104, 38);
            this.refresh.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.refresh.Name = "refresh";
            this.refresh.Size = new System.Drawing.Size(70, 23);
            this.refresh.TabIndex = 93;
            this.refresh.Text = "Refresh";
            this.refresh.UseVisualStyleBackColor = true;
            this.refresh.Click += new System.EventHandler(this.button5_Click);
            // 
            // warning
            // 
            this.warning.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.warning.AutoSize = true;
            this.warning.BackColor = System.Drawing.Color.Transparent;
            this.warning.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.warning.Font = new System.Drawing.Font("Tahoma", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.warning.ForeColor = System.Drawing.Color.SeaGreen;
            this.warning.Location = new System.Drawing.Point(235, 773);
            this.warning.Margin = new System.Windows.Forms.Padding(0);
            this.warning.Name = "warning";
            this.warning.Size = new System.Drawing.Size(109, 16);
            this.warning.TabIndex = 19;
            this.warning.Text = "Connected to SLS";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem5,
            this.toolStripMenuItem6,
            this.toolStripMenuItem8});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(142, 70);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(141, 22);
            this.toolStripMenuItem5.Text = "Copy CaseID";
            this.toolStripMenuItem5.Click += new System.EventHandler(this.toolStripMenuItem5_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripComboBox1,
            this.toolStripSeparator1,
            this.toolStripMenuItem7});
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(141, 22);
            this.toolStripMenuItem6.Text = "MarkTo";
            // 
            // toolStripComboBox1
            // 
            this.toolStripComboBox1.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.toolStripComboBox1.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.toolStripComboBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.toolStripComboBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.toolStripComboBox1.Margin = new System.Windows.Forms.Padding(2, 5, 2, 2);
            this.toolStripComboBox1.Name = "toolStripComboBox1";
            this.toolStripComboBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.toolStripComboBox1.Size = new System.Drawing.Size(121, 23);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(178, 6);
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.toolStripMenuItem7.ForeColor = System.Drawing.Color.SeaGreen;
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.toolStripMenuItem7.Size = new System.Drawing.Size(181, 24);
            this.toolStripMenuItem7.Text = "Add";
            this.toolStripMenuItem7.Click += new System.EventHandler(this.toolStripMenuItem7_Click);
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(141, 22);
            this.toolStripMenuItem8.Text = "Reminder";
            this.toolStripMenuItem8.Click += new System.EventHandler(this.toolStripMenuItem8_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.removeToolStripMenuItem,
            this.changeToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(118, 48);
            // 
            // removeToolStripMenuItem
            // 
            this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
            this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.removeToolStripMenuItem.Text = "Remove";
            this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
            // 
            // changeToolStripMenuItem
            // 
            this.changeToolStripMenuItem.Name = "changeToolStripMenuItem";
            this.changeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.changeToolStripMenuItem.Text = "Change";
            this.changeToolStripMenuItem.Click += new System.EventHandler(this.changeToolStripMenuItem_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem1,
            this.caseToolStripMenuItem,
            this.productToolStripMenuItem,
            this.settingToolStripMenuItem,
            this.toolStripMenuItem4,
            this.aboutToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.menuStrip1.Size = new System.Drawing.Size(1197, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.outstandingToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.myTaskToolStripMenuItem,
            this.cIMToolStripMenuItem,
            this.noticeToolStripMenuItem,
            this.reminderToolStripMenuItem});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.ShowShortcutKeys = false;
            this.toolStripMenuItem1.Size = new System.Drawing.Size(70, 20);
            this.toolStripMenuItem1.Text = "TabPages";
            // 
            // outstandingToolStripMenuItem
            // 
            this.outstandingToolStripMenuItem.Name = "outstandingToolStripMenuItem";
            this.outstandingToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.outstandingToolStripMenuItem.Text = "Outstanding";
            this.outstandingToolStripMenuItem.Click += new System.EventHandler(this.outstandingToolStripMenuItem_Click);
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.searchToolStripMenuItem.Text = "Search";
            this.searchToolStripMenuItem.Click += new System.EventHandler(this.searchToolStripMenuItem_Click);
            // 
            // myTaskToolStripMenuItem
            // 
            this.myTaskToolStripMenuItem.Name = "myTaskToolStripMenuItem";
            this.myTaskToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.myTaskToolStripMenuItem.Text = "MyTask";
            this.myTaskToolStripMenuItem.Click += new System.EventHandler(this.myTaskToolStripMenuItem_Click);
            // 
            // cIMToolStripMenuItem
            // 
            this.cIMToolStripMenuItem.Name = "cIMToolStripMenuItem";
            this.cIMToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.cIMToolStripMenuItem.Text = "CIM";
            this.cIMToolStripMenuItem.Click += new System.EventHandler(this.cIMToolStripMenuItem_Click);
            // 
            // noticeToolStripMenuItem
            // 
            this.noticeToolStripMenuItem.Name = "noticeToolStripMenuItem";
            this.noticeToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.noticeToolStripMenuItem.Text = "Notice";
            this.noticeToolStripMenuItem.Click += new System.EventHandler(this.noticeToolStripMenuItem_Click);
            // 
            // reminderToolStripMenuItem
            // 
            this.reminderToolStripMenuItem.Name = "reminderToolStripMenuItem";
            this.reminderToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.reminderToolStripMenuItem.Text = "Reminder";
            this.reminderToolStripMenuItem.Click += new System.EventHandler(this.reminderToolStripMenuItem_Click);
            // 
            // caseToolStripMenuItem
            // 
            this.caseToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.myMarkedCasesToolStripMenuItem,
            this.caseAnalysisToolStripMenuItem});
            this.caseToolStripMenuItem.Name = "caseToolStripMenuItem";
            this.caseToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.caseToolStripMenuItem.Text = "Case";
            // 
            // myMarkedCasesToolStripMenuItem
            // 
            this.myMarkedCasesToolStripMenuItem.Name = "myMarkedCasesToolStripMenuItem";
            this.myMarkedCasesToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.myMarkedCasesToolStripMenuItem.Text = "MyMarkedCases";
            this.myMarkedCasesToolStripMenuItem.Click += new System.EventHandler(this.myMarkedCasesToolStripMenuItem_Click);
            // 
            // caseAnalysisToolStripMenuItem
            // 
            this.caseAnalysisToolStripMenuItem.Name = "caseAnalysisToolStripMenuItem";
            this.caseAnalysisToolStripMenuItem.Size = new System.Drawing.Size(161, 22);
            this.caseAnalysisToolStripMenuItem.Text = "CaseAnalysis";
            this.caseAnalysisToolStripMenuItem.Click += new System.EventHandler(this.caseAnalysisToolStripMenuItem_Click);
            // 
            // productToolStripMenuItem
            // 
            this.productToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.productMasterToolStripMenuItem1,
            this.productHintsToolStripMenuItem});
            this.productToolStripMenuItem.Name = "productToolStripMenuItem";
            this.productToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.productToolStripMenuItem.Text = "Product";
            // 
            // productMasterToolStripMenuItem1
            // 
            this.productMasterToolStripMenuItem1.Name = "productMasterToolStripMenuItem1";
            this.productMasterToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.productMasterToolStripMenuItem1.Text = "ProductMaster";
            this.productMasterToolStripMenuItem1.Click += new System.EventHandler(this.productMasterToolStripMenuItem1_Click);
            // 
            // productHintsToolStripMenuItem
            // 
            this.productHintsToolStripMenuItem.Name = "productHintsToolStripMenuItem";
            this.productHintsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.productHintsToolStripMenuItem.Text = "ProductHints";
            this.productHintsToolStripMenuItem.Click += new System.EventHandler(this.productHintsToolStripMenuItem_Click);
            // 
            // settingToolStripMenuItem
            // 
            this.settingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customInfoToolStripMenuItem,
            this.shortCutKeyToolStripMenuItem});
            this.settingToolStripMenuItem.Name = "settingToolStripMenuItem";
            this.settingToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
            this.settingToolStripMenuItem.Text = "Setting";
            // 
            // customInfoToolStripMenuItem
            // 
            this.customInfoToolStripMenuItem.Name = "customInfoToolStripMenuItem";
            this.customInfoToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.customInfoToolStripMenuItem.Text = "Custom";
            this.customInfoToolStripMenuItem.Click += new System.EventHandler(this.customInfoToolStripMenuItem_Click);
            // 
            // shortCutKeyToolStripMenuItem
            // 
            this.shortCutKeyToolStripMenuItem.Name = "shortCutKeyToolStripMenuItem";
            this.shortCutKeyToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
            this.shortCutKeyToolStripMenuItem.Text = "ShortCutKey";
            this.shortCutKeyToolStripMenuItem.Click += new System.EventHandler(this.shortCutKeyToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.ShowShortcutKeys = false;
            this.toolStripMenuItem4.Size = new System.Drawing.Size(72, 20);
            this.toolStripMenuItem4.Text = "SuperUser";
            this.toolStripMenuItem4.Click += new System.EventHandler(this.toolStripMenuItem4_Click);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // LabelCaseCount
            // 
            this.LabelCaseCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LabelCaseCount.AutoSize = true;
            this.LabelCaseCount.BackColor = System.Drawing.Color.Transparent;
            this.LabelCaseCount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LabelCaseCount.Font = new System.Drawing.Font("Tahoma", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.LabelCaseCount.ForeColor = System.Drawing.Color.SeaGreen;
            this.LabelCaseCount.Location = new System.Drawing.Point(17, 773);
            this.LabelCaseCount.Margin = new System.Windows.Forms.Padding(0);
            this.LabelCaseCount.Name = "LabelCaseCount";
            this.LabelCaseCount.Size = new System.Drawing.Size(15, 16);
            this.LabelCaseCount.TabIndex = 95;
            this.LabelCaseCount.Text = "0";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(238, 75);
            this.splitContainer1.MinimumSize = new System.Drawing.Size(944, 695);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tabControl2);
            this.splitContainer1.Panel1.Margin = new System.Windows.Forms.Padding(1);
            this.splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(1);
            this.splitContainer1.Panel1MinSize = 370;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel2.Margin = new System.Windows.Forms.Padding(1);
            this.splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(1);
            this.splitContainer1.Panel2MinSize = 480;
            this.splitContainer1.Size = new System.Drawing.Size(946, 695);
            this.splitContainer1.SplitterDistance = 380;
            this.splitContainer1.SplitterWidth = 2;
            this.splitContainer1.TabIndex = 12;
            // 
            // tabControl2
            // 
            this.tabControl2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl2.Controls.Add(this.tabPage2);
            this.tabControl2.Controls.Add(this.tabPage3);
            this.tabControl2.Controls.Add(this.tabPage4);
            this.tabControl2.Controls.Add(this.tabPage5);
            this.tabControl2.Controls.Add(this.tabPage10);
            this.tabControl2.Controls.Add(this.tabPage11);
            this.tabControl2.Location = new System.Drawing.Point(2, 0);
            this.tabControl2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabControl2.MinimumSize = new System.Drawing.Size(366, 694);
            this.tabControl2.Multiline = true;
            this.tabControl2.Name = "tabControl2";
            this.tabControl2.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabControl2.SelectedIndex = 0;
            this.tabControl2.Size = new System.Drawing.Size(376, 694);
            this.tabControl2.TabIndex = 12;
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.LV_outstanding);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage2.Size = new System.Drawing.Size(368, 668);
            this.tabPage2.TabIndex = 0;
            this.tabPage2.Text = "Outstanding";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // LV_outstanding
            // 
            this.LV_outstanding.AllowColumnReorder = true;
            this.LV_outstanding.AllowDrop = true;
            this.LV_outstanding.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LV_outstanding.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.LV_outstanding.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LV_outstanding.CausesValidation = false;
            this.LV_outstanding.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Outstanding_Case,
            this.Outstanding_CaseID,
            this.Outstanding_CreateTime,
            this.Outstanding_HappenTime,
            this.Outstanding_Client,
            this.Outstanding_CallPerson,
            this.Outstanding_ContactNo,
            this.Outstanding_ReceivedThrough,
            this.Outstanding_Description,
            this.Outstanding_Type,
            this.Outstanding_Incharge,
            this.Outstanding_Status,
            this.Outstanding_Solution,
            this.Outstanding_Priority,
            this.Outstanding_IssueType,
            this.Outstanding_Product,
            this.columnHeader17,
            this.columnHeader18});
            this.LV_outstanding.ContextMenuStrip = this.contextMenuStrip1;
            this.LV_outstanding.FullRowSelect = true;
            this.LV_outstanding.GridLines = true;
            this.LV_outstanding.Location = new System.Drawing.Point(0, 0);
            this.LV_outstanding.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.LV_outstanding.MultiSelect = false;
            this.LV_outstanding.Name = "LV_outstanding";
            this.LV_outstanding.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LV_outstanding.Size = new System.Drawing.Size(368, 668);
            this.LV_outstanding.TabIndex = 12;
            this.LV_outstanding.UseCompatibleStateImageBehavior = false;
            this.LV_outstanding.View = System.Windows.Forms.View.Details;
            this.LV_outstanding.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.LV_outstanding.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.LV_outstanding_ItemSelectionChanged);
            this.LV_outstanding.DoubleClick += new System.EventHandler(this.listView1_DoubleClick);
            this.LV_outstanding.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView1_KeyDown);
            // 
            // Outstanding_Case
            // 
            this.Outstanding_Case.Text = "";
            this.Outstanding_Case.Width = 0;
            // 
            // Outstanding_CaseID
            // 
            this.Outstanding_CaseID.Text = "CaseID";
            this.Outstanding_CaseID.Width = 110;
            // 
            // Outstanding_CreateTime
            // 
            this.Outstanding_CreateTime.Text = "CreateTime";
            this.Outstanding_CreateTime.Width = 130;
            // 
            // Outstanding_HappenTime
            // 
            this.Outstanding_HappenTime.Text = "HappenAt";
            // 
            // Outstanding_Client
            // 
            this.Outstanding_Client.Text = "Client";
            this.Outstanding_Client.Width = 70;
            // 
            // Outstanding_CallPerson
            // 
            this.Outstanding_CallPerson.Text = "Caller";
            this.Outstanding_CallPerson.Width = 65;
            // 
            // Outstanding_ContactNo
            // 
            this.Outstanding_ContactNo.Text = "Phone";
            this.Outstanding_ContactNo.Width = 65;
            // 
            // Outstanding_ReceivedThrough
            // 
            this.Outstanding_ReceivedThrough.DisplayIndex = 13;
            this.Outstanding_ReceivedThrough.Text = "Through";
            this.Outstanding_ReceivedThrough.Width = 59;
            // 
            // Outstanding_Description
            // 
            this.Outstanding_Description.Text = "Description";
            this.Outstanding_Description.Width = 65;
            // 
            // Outstanding_Type
            // 
            this.Outstanding_Type.DisplayIndex = 7;
            this.Outstanding_Type.Text = "Type";
            this.Outstanding_Type.Width = 40;
            // 
            // Outstanding_Incharge
            // 
            this.Outstanding_Incharge.DisplayIndex = 9;
            this.Outstanding_Incharge.Text = "Incharge";
            this.Outstanding_Incharge.Width = 55;
            // 
            // Outstanding_Status
            // 
            this.Outstanding_Status.DisplayIndex = 10;
            this.Outstanding_Status.Text = "Status";
            this.Outstanding_Status.Width = 45;
            // 
            // Outstanding_Solution
            // 
            this.Outstanding_Solution.DisplayIndex = 11;
            this.Outstanding_Solution.Text = "Solution";
            // 
            // Outstanding_Priority
            // 
            this.Outstanding_Priority.DisplayIndex = 12;
            this.Outstanding_Priority.Text = "Priority";
            this.Outstanding_Priority.Width = 50;
            // 
            // Outstanding_IssueType
            // 
            this.Outstanding_IssueType.Text = "Issue";
            // 
            // Outstanding_Product
            // 
            this.Outstanding_Product.Text = "Product";
            // 
            // columnHeader17
            // 
            this.columnHeader17.Text = "Server";
            // 
            // columnHeader18
            // 
            this.columnHeader18.Text = "Version";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.Color.White;
            this.tabPage3.Controls.Add(this.TB_CaseID);
            this.tabPage3.Controls.Add(this.label_SearchCount);
            this.tabPage3.Controls.Add(this.label7);
            this.tabPage3.Controls.Add(this.Search_Multi);
            this.tabPage3.Controls.Add(this.label9);
            this.tabPage3.Controls.Add(this.Search_export);
            this.tabPage3.Controls.Add(this.Combo_Search_Subcate);
            this.tabPage3.Controls.Add(this.LV_search);
            this.tabPage3.Controls.Add(this.Search_clear);
            this.tabPage3.Controls.Add(this.Search_search);
            this.tabPage3.Controls.Add(this.label8);
            this.tabPage3.Controls.Add(this.Combo_Search_Cate);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage3.Size = new System.Drawing.Size(368, 668);
            this.tabPage3.TabIndex = 1;
            this.tabPage3.Text = "Search";
            // 
            // TB_CaseID
            // 
            this.TB_CaseID.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.TB_CaseID.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.TB_CaseID.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TB_CaseID.Location = new System.Drawing.Point(58, 15);
            this.TB_CaseID.MaxLength = 30;
            this.TB_CaseID.Multiline = false;
            this.TB_CaseID.Name = "TB_CaseID";
            this.TB_CaseID.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.TB_CaseID.Size = new System.Drawing.Size(126, 18);
            this.TB_CaseID.TabIndex = 14;
            this.TB_CaseID.Text = "";
            // 
            // label_SearchCount
            // 
            this.label_SearchCount.AutoSize = true;
            this.label_SearchCount.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label_SearchCount.Font = new System.Drawing.Font("Tahoma", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label_SearchCount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label_SearchCount.Location = new System.Drawing.Point(58, 105);
            this.label_SearchCount.Margin = new System.Windows.Forms.Padding(0);
            this.label_SearchCount.Name = "label_SearchCount";
            this.label_SearchCount.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label_SearchCount.Size = new System.Drawing.Size(56, 16);
            this.label_SearchCount.TabIndex = 81;
            this.label_SearchCount.Text = "matches";
            this.label_SearchCount.Visible = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label7.Location = new System.Drawing.Point(3, 82);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 13);
            this.label7.TabIndex = 80;
            this.label7.Text = "SubCate";
            // 
            // Search_Multi
            // 
            this.Search_Multi.BackColor = System.Drawing.Color.SeaGreen;
            this.Search_Multi.FlatAppearance.BorderColor = System.Drawing.Color.SeaGreen;
            this.Search_Multi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Search_Multi.ForeColor = System.Drawing.Color.White;
            this.Search_Multi.Location = new System.Drawing.Point(201, 77);
            this.Search_Multi.Name = "Search_Multi";
            this.Search_Multi.Size = new System.Drawing.Size(70, 23);
            this.Search_Multi.TabIndex = 18;
            this.Search_Multi.Text = "MultiFilter";
            this.Search_Multi.UseVisualStyleBackColor = false;
            this.Search_Multi.Click += new System.EventHandler(this.button10_Click);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label9.Location = new System.Drawing.Point(3, 49);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(52, 13);
            this.label9.TabIndex = 79;
            this.label9.Text = "Category";
            // 
            // Search_export
            // 
            this.Search_export.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Search_export.BackColor = System.Drawing.Color.White;
            this.Search_export.FlatAppearance.BorderColor = System.Drawing.SystemColors.WindowText;
            this.Search_export.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Search_export.ForeColor = System.Drawing.Color.Black;
            this.Search_export.Location = new System.Drawing.Point(313, 77);
            this.Search_export.Name = "Search_export";
            this.Search_export.Size = new System.Drawing.Size(49, 23);
            this.Search_export.TabIndex = 19;
            this.Search_export.Text = "Export";
            this.Search_export.UseVisualStyleBackColor = false;
            this.Search_export.Click += new System.EventHandler(this.button17_Click);
            // 
            // Combo_Search_Subcate
            // 
            this.Combo_Search_Subcate.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.Combo_Search_Subcate.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.Combo_Search_Subcate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.Combo_Search_Subcate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Combo_Search_Subcate.FormattingEnabled = true;
            this.Combo_Search_Subcate.Location = new System.Drawing.Point(58, 78);
            this.Combo_Search_Subcate.Name = "Combo_Search_Subcate";
            this.Combo_Search_Subcate.Size = new System.Drawing.Size(126, 21);
            this.Combo_Search_Subcate.Sorted = true;
            this.Combo_Search_Subcate.TabIndex = 16;
            this.Combo_Search_Subcate.TextChanged += new System.EventHandler(this.Combo_Search_Subcate_TextChanged);
            // 
            // LV_search
            // 
            this.LV_search.AllowColumnReorder = true;
            this.LV_search.AllowDrop = true;
            this.LV_search.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LV_search.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.LV_search.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LV_search.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.Search_Case,
            this.Search_CaseID,
            this.Search_CreateTime,
            this.Search_HappenTime,
            this.Search_Client,
            this.Search_CallPerson,
            this.Search_ContactNo,
            this.Search_ReceivedThrough,
            this.Search_Description,
            this.Search_Type,
            this.Search_Incharge,
            this.Search_Status,
            this.Search_Solution,
            this.Search_Priority,
            this.Search_IssueType,
            this.Search_Product,
            this.columnHeader19,
            this.columnHeader20});
            this.LV_search.ContextMenuStrip = this.contextMenuStrip1;
            this.LV_search.FullRowSelect = true;
            this.LV_search.GridLines = true;
            this.LV_search.Location = new System.Drawing.Point(0, 125);
            this.LV_search.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.LV_search.MultiSelect = false;
            this.LV_search.Name = "LV_search";
            this.LV_search.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LV_search.Size = new System.Drawing.Size(368, 543);
            this.LV_search.TabIndex = 20;
            this.LV_search.UseCompatibleStateImageBehavior = false;
            this.LV_search.View = System.Windows.Forms.View.Details;
            this.LV_search.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView2_ColumnClick);
            this.LV_search.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.LV_search_ItemSelectionChanged);
            this.LV_search.DoubleClick += new System.EventHandler(this.listView2_DoubleClick);
            this.LV_search.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView2_KeyDown);
            // 
            // Search_Case
            // 
            this.Search_Case.Text = "Case";
            this.Search_Case.Width = 0;
            // 
            // Search_CaseID
            // 
            this.Search_CaseID.Text = "CaseID";
            this.Search_CaseID.Width = 110;
            // 
            // Search_CreateTime
            // 
            this.Search_CreateTime.Text = "CreateTime";
            this.Search_CreateTime.Width = 130;
            // 
            // Search_HappenTime
            // 
            this.Search_HappenTime.Text = "HappenAt";
            // 
            // Search_Client
            // 
            this.Search_Client.Text = "Client";
            this.Search_Client.Width = 70;
            // 
            // Search_CallPerson
            // 
            this.Search_CallPerson.Text = "Caller";
            this.Search_CallPerson.Width = 65;
            // 
            // Search_ContactNo
            // 
            this.Search_ContactNo.Text = "Phone";
            this.Search_ContactNo.Width = 65;
            // 
            // Search_ReceivedThrough
            // 
            this.Search_ReceivedThrough.DisplayIndex = 13;
            this.Search_ReceivedThrough.Text = "Through";
            this.Search_ReceivedThrough.Width = 64;
            // 
            // Search_Description
            // 
            this.Search_Description.Text = "Description";
            this.Search_Description.Width = 65;
            // 
            // Search_Type
            // 
            this.Search_Type.DisplayIndex = 7;
            this.Search_Type.Text = "Type";
            this.Search_Type.Width = 40;
            // 
            // Search_Incharge
            // 
            this.Search_Incharge.DisplayIndex = 9;
            this.Search_Incharge.Text = "Incharge";
            this.Search_Incharge.Width = 55;
            // 
            // Search_Status
            // 
            this.Search_Status.DisplayIndex = 10;
            this.Search_Status.Text = "Status";
            this.Search_Status.Width = 45;
            // 
            // Search_Solution
            // 
            this.Search_Solution.DisplayIndex = 11;
            this.Search_Solution.Text = "Solution";
            // 
            // Search_Priority
            // 
            this.Search_Priority.DisplayIndex = 12;
            this.Search_Priority.Text = "Priority";
            this.Search_Priority.Width = 50;
            // 
            // Search_IssueType
            // 
            this.Search_IssueType.Text = "Issue";
            // 
            // Search_Product
            // 
            this.Search_Product.Text = "Product";
            // 
            // columnHeader19
            // 
            this.columnHeader19.Text = "Server";
            // 
            // columnHeader20
            // 
            this.columnHeader20.Text = "Version";
            // 
            // Search_clear
            // 
            this.Search_clear.BackColor = System.Drawing.Color.IndianRed;
            this.Search_clear.FlatAppearance.BorderColor = System.Drawing.Color.IndianRed;
            this.Search_clear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Search_clear.ForeColor = System.Drawing.SystemColors.MenuBar;
            this.Search_clear.Location = new System.Drawing.Point(201, 44);
            this.Search_clear.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Search_clear.Name = "Search_clear";
            this.Search_clear.Size = new System.Drawing.Size(70, 23);
            this.Search_clear.TabIndex = 17;
            this.Search_clear.Text = "Clear";
            this.Search_clear.UseVisualStyleBackColor = false;
            this.Search_clear.Click += new System.EventHandler(this.button7_Click);
            // 
            // Search_search
            // 
            this.Search_search.BackColor = System.Drawing.SystemColors.Window;
            this.Search_search.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Search_search.ForeColor = System.Drawing.SystemColors.WindowText;
            this.Search_search.Location = new System.Drawing.Point(201, 14);
            this.Search_search.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Search_search.Name = "Search_search";
            this.Search_search.Size = new System.Drawing.Size(70, 23);
            this.Search_search.TabIndex = 14;
            this.Search_search.Text = "Search";
            this.Search_search.UseVisualStyleBackColor = false;
            this.Search_search.Click += new System.EventHandler(this.button6_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label8.Location = new System.Drawing.Point(3, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(42, 13);
            this.label8.TabIndex = 11;
            this.label8.Text = "CaseID";
            // 
            // Combo_Search_Cate
            // 
            this.Combo_Search_Cate.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.Combo_Search_Cate.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.Combo_Search_Cate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.Combo_Search_Cate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Combo_Search_Cate.FormattingEnabled = true;
            this.Combo_Search_Cate.Location = new System.Drawing.Point(58, 45);
            this.Combo_Search_Cate.Name = "Combo_Search_Cate";
            this.Combo_Search_Cate.Size = new System.Drawing.Size(126, 21);
            this.Combo_Search_Cate.Sorted = true;
            this.Combo_Search_Cate.TabIndex = 15;
            this.Combo_Search_Cate.Text = "  Outstanding cases";
            this.Combo_Search_Cate.TextChanged += new System.EventHandler(this.Combo_Search_Cate_TextChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.LV_mytask);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tabPage4.Size = new System.Drawing.Size(368, 668);
            this.tabPage4.TabIndex = 2;
            this.tabPage4.Text = "MyTask";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // LV_mytask
            // 
            this.LV_mytask.AllowColumnReorder = true;
            this.LV_mytask.AllowDrop = true;
            this.LV_mytask.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LV_mytask.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.LV_mytask.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LV_mytask.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.MyTask_Case,
            this.MyTask_CaseID,
            this.MyTask_CreateTime,
            this.MyTask_HappenTime,
            this.MyTask_Client,
            this.MyTask_CallPerson,
            this.MyTask_ContactNo,
            this.MyTask_ReceivedThrough,
            this.MyTask_Description,
            this.MyTask_Type,
            this.MyTask_Incharge,
            this.MyTask_Status,
            this.MyTask_Solution,
            this.MyTask_Priority,
            this.MyTask_IssueType,
            this.MyTask_Product,
            this.columnHeader21,
            this.columnHeader22});
            this.LV_mytask.ContextMenuStrip = this.contextMenuStrip1;
            this.LV_mytask.FullRowSelect = true;
            this.LV_mytask.GridLines = true;
            this.LV_mytask.Location = new System.Drawing.Point(0, 0);
            this.LV_mytask.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.LV_mytask.MultiSelect = false;
            this.LV_mytask.Name = "LV_mytask";
            this.LV_mytask.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LV_mytask.Size = new System.Drawing.Size(368, 668);
            this.LV_mytask.TabIndex = 21;
            this.LV_mytask.UseCompatibleStateImageBehavior = false;
            this.LV_mytask.View = System.Windows.Forms.View.Details;
            this.LV_mytask.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView3_ColumnClick);
            this.LV_mytask.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.LV_mytask_ItemSelectionChanged);
            this.LV_mytask.DoubleClick += new System.EventHandler(this.listView3_DoubleClick);
            this.LV_mytask.KeyDown += new System.Windows.Forms.KeyEventHandler(this.listView3_KeyDown);
            // 
            // MyTask_Case
            // 
            this.MyTask_Case.Text = "Case";
            this.MyTask_Case.Width = 0;
            // 
            // MyTask_CaseID
            // 
            this.MyTask_CaseID.Text = "CaseID";
            this.MyTask_CaseID.Width = 125;
            // 
            // MyTask_CreateTime
            // 
            this.MyTask_CreateTime.Text = "CreateTime";
            this.MyTask_CreateTime.Width = 130;
            // 
            // MyTask_HappenTime
            // 
            this.MyTask_HappenTime.Text = "HappenAt";
            // 
            // MyTask_Client
            // 
            this.MyTask_Client.Text = "Client";
            this.MyTask_Client.Width = 70;
            // 
            // MyTask_CallPerson
            // 
            this.MyTask_CallPerson.Text = "Caller";
            this.MyTask_CallPerson.Width = 65;
            // 
            // MyTask_ContactNo
            // 
            this.MyTask_ContactNo.Text = "Phone";
            this.MyTask_ContactNo.Width = 65;
            // 
            // MyTask_ReceivedThrough
            // 
            this.MyTask_ReceivedThrough.DisplayIndex = 13;
            this.MyTask_ReceivedThrough.Text = "Through";
            this.MyTask_ReceivedThrough.Width = 64;
            // 
            // MyTask_Description
            // 
            this.MyTask_Description.Text = "Description";
            this.MyTask_Description.Width = 65;
            // 
            // MyTask_Type
            // 
            this.MyTask_Type.DisplayIndex = 7;
            this.MyTask_Type.Text = "Type";
            this.MyTask_Type.Width = 40;
            // 
            // MyTask_Incharge
            // 
            this.MyTask_Incharge.DisplayIndex = 9;
            this.MyTask_Incharge.Text = "Incharge";
            this.MyTask_Incharge.Width = 55;
            // 
            // MyTask_Status
            // 
            this.MyTask_Status.DisplayIndex = 10;
            this.MyTask_Status.Text = "Status";
            this.MyTask_Status.Width = 45;
            // 
            // MyTask_Solution
            // 
            this.MyTask_Solution.DisplayIndex = 11;
            this.MyTask_Solution.Text = "Solution";
            // 
            // MyTask_Priority
            // 
            this.MyTask_Priority.DisplayIndex = 12;
            this.MyTask_Priority.Text = "Priority";
            this.MyTask_Priority.Width = 50;
            // 
            // MyTask_IssueType
            // 
            this.MyTask_IssueType.Text = "Issue";
            // 
            // MyTask_Product
            // 
            this.MyTask_Product.Text = "Product";
            // 
            // columnHeader21
            // 
            this.columnHeader21.Text = "Server";
            // 
            // columnHeader22
            // 
            this.columnHeader22.Text = "Version";
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.CIM_phone);
            this.tabPage5.Controls.Add(this.CIM_add);
            this.tabPage5.Controls.Add(this.CIM_search);
            this.tabPage5.Controls.Add(this.label46);
            this.tabPage5.Controls.Add(this.CIM_client);
            this.tabPage5.Controls.Add(this.LV_CIM);
            this.tabPage5.Controls.Add(this.label45);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(368, 668);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "CIM";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // CIM_phone
            // 
            this.CIM_phone.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CIM_phone.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.CIM_phone.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CIM_phone.Location = new System.Drawing.Point(50, 48);
            this.CIM_phone.MaxLength = 40;
            this.CIM_phone.Multiline = false;
            this.CIM_phone.Name = "CIM_phone";
            this.CIM_phone.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.CIM_phone.Size = new System.Drawing.Size(137, 18);
            this.CIM_phone.TabIndex = 24;
            this.CIM_phone.Text = "";
            // 
            // CIM_add
            // 
            this.CIM_add.BackColor = System.Drawing.Color.SeaGreen;
            this.CIM_add.FlatAppearance.BorderColor = System.Drawing.Color.SeaGreen;
            this.CIM_add.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CIM_add.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.CIM_add.Location = new System.Drawing.Point(227, 14);
            this.CIM_add.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CIM_add.Name = "CIM_add";
            this.CIM_add.Size = new System.Drawing.Size(55, 23);
            this.CIM_add.TabIndex = 23;
            this.CIM_add.Text = "Add";
            this.CIM_add.UseVisualStyleBackColor = false;
            this.CIM_add.Visible = false;
            this.CIM_add.Click += new System.EventHandler(this.button14_Click);
            // 
            // CIM_search
            // 
            this.CIM_search.BackColor = System.Drawing.SystemColors.Window;
            this.CIM_search.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CIM_search.ForeColor = System.Drawing.SystemColors.WindowText;
            this.CIM_search.Location = new System.Drawing.Point(227, 47);
            this.CIM_search.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CIM_search.Name = "CIM_search";
            this.CIM_search.Size = new System.Drawing.Size(55, 23);
            this.CIM_search.TabIndex = 25;
            this.CIM_search.Text = "Search";
            this.CIM_search.UseVisualStyleBackColor = false;
            this.CIM_search.Click += new System.EventHandler(this.button12_Click);
            // 
            // label46
            // 
            this.label46.AutoSize = true;
            this.label46.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label46.Location = new System.Drawing.Point(6, 19);
            this.label46.Name = "label46";
            this.label46.Size = new System.Drawing.Size(34, 13);
            this.label46.TabIndex = 34;
            this.label46.Text = "Client";
            // 
            // CIM_client
            // 
            this.CIM_client.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CIM_client.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CIM_client.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CIM_client.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CIM_client.FormattingEnabled = true;
            this.CIM_client.Location = new System.Drawing.Point(50, 15);
            this.CIM_client.Name = "CIM_client";
            this.CIM_client.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CIM_client.Size = new System.Drawing.Size(137, 21);
            this.CIM_client.Sorted = true;
            this.CIM_client.TabIndex = 22;
            this.CIM_client.TextChanged += new System.EventHandler(this.CIM_client_TextChanged);
            // 
            // LV_CIM
            // 
            this.LV_CIM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LV_CIM.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.LV_CIM.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LV_CIM.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader43,
            this.columnHeader45,
            this.columnHeader46,
            this.columnHeader47,
            this.columnHeader48,
            this.columnHeader49,
            this.columnHeader50,
            this.columnHeader51});
            this.LV_CIM.FullRowSelect = true;
            this.LV_CIM.GridLines = true;
            this.LV_CIM.Location = new System.Drawing.Point(0, 94);
            this.LV_CIM.Name = "LV_CIM";
            this.LV_CIM.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LV_CIM.Size = new System.Drawing.Size(368, 574);
            this.LV_CIM.TabIndex = 26;
            this.LV_CIM.UseCompatibleStateImageBehavior = false;
            this.LV_CIM.View = System.Windows.Forms.View.Details;
            this.LV_CIM.SelectedIndexChanged += new System.EventHandler(this.LV_CIM_SelectedIndexChanged);
            this.LV_CIM.SizeChanged += new System.EventHandler(this.listView5_SizeChanged);
            this.LV_CIM.MouseClick += new System.Windows.Forms.MouseEventHandler(this.LV_CIM_MouseClick);
            this.LV_CIM.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.LV_CIM_MouseDoubleClick);
            // 
            // columnHeader43
            // 
            this.columnHeader43.Width = 0;
            // 
            // columnHeader45
            // 
            this.columnHeader45.Text = "Client";
            this.columnHeader45.Width = 70;
            // 
            // columnHeader46
            // 
            this.columnHeader46.Text = "Contact";
            this.columnHeader46.Width = 86;
            // 
            // columnHeader47
            // 
            this.columnHeader47.Text = "Title";
            this.columnHeader47.Width = 67;
            // 
            // columnHeader48
            // 
            this.columnHeader48.Text = "Phone";
            this.columnHeader48.Width = 125;
            // 
            // columnHeader49
            // 
            this.columnHeader49.Text = "Phone2";
            this.columnHeader49.Width = 122;
            // 
            // columnHeader50
            // 
            this.columnHeader50.Text = "Email";
            this.columnHeader50.Width = 161;
            // 
            // columnHeader51
            // 
            this.columnHeader51.Text = "Address";
            this.columnHeader51.Width = 259;
            // 
            // label45
            // 
            this.label45.AutoSize = true;
            this.label45.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label45.Location = new System.Drawing.Point(6, 48);
            this.label45.Name = "label45";
            this.label45.Size = new System.Drawing.Size(37, 13);
            this.label45.TabIndex = 33;
            this.label45.Text = "Phone";
            // 
            // tabPage10
            // 
            this.tabPage10.Controls.Add(this.Notice);
            this.tabPage10.ForeColor = System.Drawing.Color.Black;
            this.tabPage10.Location = new System.Drawing.Point(4, 22);
            this.tabPage10.Name = "tabPage10";
            this.tabPage10.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage10.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.tabPage10.Size = new System.Drawing.Size(368, 668);
            this.tabPage10.TabIndex = 5;
            this.tabPage10.Text = " Notice";
            this.tabPage10.UseVisualStyleBackColor = true;
            // 
            // Notice
            // 
            this.Notice.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(1);
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.DeepSkyBlue;
            this.Notice.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.Notice.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Notice.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.Notice.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.Notice.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.Notice.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.RaisedHorizontal;
            this.Notice.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.GrayText;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Notice.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.Notice.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Notice.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.Column1,
            this.dataGridViewTextBoxColumn2});
            this.Notice.Cursor = System.Windows.Forms.Cursors.WaitCursor;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.MenuBar;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Notice.DefaultCellStyle = dataGridViewCellStyle3;
            this.Notice.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.Notice.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Notice.Location = new System.Drawing.Point(2, 3);
            this.Notice.MultiSelect = false;
            this.Notice.Name = "Notice";
            this.Notice.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Notice.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Notice.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.Notice.RowHeadersVisible = false;
            this.Notice.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.Padding = new System.Windows.Forms.Padding(1);
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.Notice.RowsDefaultCellStyle = dataGridViewCellStyle5;
            this.Notice.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.Notice.Size = new System.Drawing.Size(363, 661);
            this.Notice.TabIndex = 67;
            this.Notice.Tag = "";
            this.Notice.UseWaitCursor = true;
            this.Notice.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.Notice_CellBeginEdit);
            this.Notice.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.Notice_CellEndEdit);
            this.Notice.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Notice_KeyDown);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.dataGridViewTextBoxColumn1.DividerWidth = 1;
            this.dataGridViewTextBoxColumn1.FillWeight = 40F;
            this.dataGridViewTextBoxColumn1.HeaderText = "Time";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn1.Width = 116;
            // 
            // Column1
            // 
            this.Column1.DividerWidth = 1;
            this.Column1.FillWeight = 40F;
            this.Column1.HeaderText = "Sender";
            this.Column1.Name = "Column1";
            this.Column1.Width = 45;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataGridViewTextBoxColumn2.DividerWidth = 1;
            this.dataGridViewTextBoxColumn2.FillWeight = 78.17259F;
            this.dataGridViewTextBoxColumn2.HeaderText = "Content";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            // 
            // tabPage11
            // 
            this.tabPage11.Controls.Add(this.checkBox_Popup);
            this.tabPage11.Controls.Add(this.Label_RemindCaseID);
            this.tabPage11.Controls.Add(this.button1);
            this.tabPage11.Controls.Add(this.label10);
            this.tabPage11.Controls.Add(this.LV_Reminder_Expired);
            this.tabPage11.Controls.Add(this.Label_RemindTime);
            this.tabPage11.Controls.Add(this.LV_Reminder);
            this.tabPage11.Location = new System.Drawing.Point(4, 22);
            this.tabPage11.Name = "tabPage11";
            this.tabPage11.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage11.Size = new System.Drawing.Size(368, 668);
            this.tabPage11.TabIndex = 6;
            this.tabPage11.Text = "Reminder";
            this.tabPage11.UseVisualStyleBackColor = true;
            // 
            // checkBox_Popup
            // 
            this.checkBox_Popup.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBox_Popup.AutoSize = true;
            this.checkBox_Popup.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkBox_Popup.Location = new System.Drawing.Point(313, 336);
            this.checkBox_Popup.Margin = new System.Windows.Forms.Padding(0);
            this.checkBox_Popup.Name = "checkBox_Popup";
            this.checkBox_Popup.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.checkBox_Popup.Size = new System.Drawing.Size(53, 17);
            this.checkBox_Popup.TabIndex = 92;
            this.checkBox_Popup.Text = "Popup";
            this.checkBox_Popup.UseVisualStyleBackColor = true;
            this.checkBox_Popup.CheckedChanged += new System.EventHandler(this.checkBox_Popup_CheckedChanged);
            // 
            // Label_RemindCaseID
            // 
            this.Label_RemindCaseID.AutoSize = true;
            this.Label_RemindCaseID.Font = new System.Drawing.Font("Tahoma", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Label_RemindCaseID.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Label_RemindCaseID.Location = new System.Drawing.Point(3, 320);
            this.Label_RemindCaseID.Name = "Label_RemindCaseID";
            this.Label_RemindCaseID.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label_RemindCaseID.Size = new System.Drawing.Size(0, 16);
            this.Label_RemindCaseID.TabIndex = 91;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.button1.Location = new System.Drawing.Point(312, 372);
            this.button1.Name = "button1";
            this.button1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.button1.Size = new System.Drawing.Size(50, 23);
            this.button1.TabIndex = 90;
            this.button1.Text = "Clear";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Tahoma", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label10.Location = new System.Drawing.Point(3, 375);
            this.label10.Name = "label10";
            this.label10.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label10.Size = new System.Drawing.Size(86, 16);
            this.label10.TabIndex = 32;
            this.label10.Text = "Expired Items";
            // 
            // LV_Reminder_Expired
            // 
            this.LV_Reminder_Expired.AllowColumnReorder = true;
            this.LV_Reminder_Expired.AllowDrop = true;
            this.LV_Reminder_Expired.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LV_Reminder_Expired.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.LV_Reminder_Expired.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LV_Reminder_Expired.CausesValidation = false;
            this.LV_Reminder_Expired.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader25,
            this.columnHeader26,
            this.columnHeader27,
            this.columnHeader28,
            this.columnHeader29,
            this.columnHeader30,
            this.columnHeader31,
            this.columnHeader32,
            this.columnHeader33,
            this.columnHeader34,
            this.columnHeader35,
            this.columnHeader36,
            this.columnHeader37,
            this.columnHeader38,
            this.columnHeader39,
            this.columnHeader40,
            this.columnHeader41,
            this.columnHeader42});
            this.LV_Reminder_Expired.ContextMenuStrip = this.contextMenuStrip2;
            this.LV_Reminder_Expired.FullRowSelect = true;
            this.LV_Reminder_Expired.GridLines = true;
            this.LV_Reminder_Expired.Location = new System.Drawing.Point(0, 400);
            this.LV_Reminder_Expired.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.LV_Reminder_Expired.MultiSelect = false;
            this.LV_Reminder_Expired.Name = "LV_Reminder_Expired";
            this.LV_Reminder_Expired.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LV_Reminder_Expired.Size = new System.Drawing.Size(368, 268);
            this.LV_Reminder_Expired.TabIndex = 31;
            this.LV_Reminder_Expired.UseCompatibleStateImageBehavior = false;
            this.LV_Reminder_Expired.View = System.Windows.Forms.View.Details;
            this.LV_Reminder_Expired.Click += new System.EventHandler(this.LV_Reminder_Expired_Click);
            this.LV_Reminder_Expired.DoubleClick += new System.EventHandler(this.LV_Reminder_Expired_DoubleClick);
            // 
            // columnHeader25
            // 
            this.columnHeader25.Text = "Case";
            this.columnHeader25.Width = 0;
            // 
            // columnHeader26
            // 
            this.columnHeader26.Text = "CaseID";
            this.columnHeader26.Width = 110;
            // 
            // columnHeader27
            // 
            this.columnHeader27.Text = "CreateTime";
            this.columnHeader27.Width = 130;
            // 
            // columnHeader28
            // 
            this.columnHeader28.Text = "HappenAt";
            // 
            // columnHeader29
            // 
            this.columnHeader29.Text = "Client";
            this.columnHeader29.Width = 70;
            // 
            // columnHeader30
            // 
            this.columnHeader30.Text = "Caller";
            this.columnHeader30.Width = 65;
            // 
            // columnHeader31
            // 
            this.columnHeader31.Text = "Phone";
            this.columnHeader31.Width = 65;
            // 
            // columnHeader32
            // 
            this.columnHeader32.DisplayIndex = 13;
            this.columnHeader32.Text = "Through";
            this.columnHeader32.Width = 64;
            // 
            // columnHeader33
            // 
            this.columnHeader33.Text = "Description";
            this.columnHeader33.Width = 65;
            // 
            // columnHeader34
            // 
            this.columnHeader34.DisplayIndex = 7;
            this.columnHeader34.Text = "Type";
            this.columnHeader34.Width = 40;
            // 
            // columnHeader35
            // 
            this.columnHeader35.DisplayIndex = 9;
            this.columnHeader35.Text = "Incharge";
            this.columnHeader35.Width = 55;
            // 
            // columnHeader36
            // 
            this.columnHeader36.DisplayIndex = 10;
            this.columnHeader36.Text = "Status";
            this.columnHeader36.Width = 45;
            // 
            // columnHeader37
            // 
            this.columnHeader37.DisplayIndex = 11;
            this.columnHeader37.Text = "Solution";
            // 
            // columnHeader38
            // 
            this.columnHeader38.DisplayIndex = 12;
            this.columnHeader38.Text = "Priority";
            this.columnHeader38.Width = 50;
            // 
            // columnHeader39
            // 
            this.columnHeader39.Text = "Issue";
            // 
            // columnHeader40
            // 
            this.columnHeader40.Text = "Product";
            // 
            // columnHeader41
            // 
            this.columnHeader41.Text = "Server";
            // 
            // columnHeader42
            // 
            this.columnHeader42.Text = "Version";
            // 
            // Label_RemindTime
            // 
            this.Label_RemindTime.AutoSize = true;
            this.Label_RemindTime.Font = new System.Drawing.Font("Tahoma", 9.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.Label_RemindTime.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.Label_RemindTime.Location = new System.Drawing.Point(3, 336);
            this.Label_RemindTime.Name = "Label_RemindTime";
            this.Label_RemindTime.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.Label_RemindTime.Size = new System.Drawing.Size(93, 16);
            this.Label_RemindTime.TabIndex = 30;
            this.Label_RemindTime.Text = "Remind Time: ";
            // 
            // LV_Reminder
            // 
            this.LV_Reminder.AllowColumnReorder = true;
            this.LV_Reminder.AllowDrop = true;
            this.LV_Reminder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LV_Reminder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.LV_Reminder.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.LV_Reminder.CausesValidation = false;
            this.LV_Reminder.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader12,
            this.columnHeader13,
            this.columnHeader14,
            this.columnHeader15,
            this.columnHeader16,
            this.columnHeader23,
            this.columnHeader24});
            this.LV_Reminder.ContextMenuStrip = this.contextMenuStrip2;
            this.LV_Reminder.FullRowSelect = true;
            this.LV_Reminder.GridLines = true;
            this.LV_Reminder.Location = new System.Drawing.Point(0, 0);
            this.LV_Reminder.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.LV_Reminder.MultiSelect = false;
            this.LV_Reminder.Name = "LV_Reminder";
            this.LV_Reminder.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LV_Reminder.Size = new System.Drawing.Size(368, 313);
            this.LV_Reminder.TabIndex = 29;
            this.LV_Reminder.UseCompatibleStateImageBehavior = false;
            this.LV_Reminder.View = System.Windows.Forms.View.Details;
            this.LV_Reminder.Click += new System.EventHandler(this.LV_Reminder_Click);
            this.LV_Reminder.DoubleClick += new System.EventHandler(this.listViewNF1_DoubleClick);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Case";
            this.columnHeader1.Width = 0;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "CaseID";
            this.columnHeader2.Width = 110;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "CreateTime";
            this.columnHeader3.Width = 130;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "HappenAt";
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Client";
            this.columnHeader5.Width = 70;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Caller";
            this.columnHeader6.Width = 65;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Phone";
            this.columnHeader7.Width = 65;
            // 
            // columnHeader8
            // 
            this.columnHeader8.DisplayIndex = 13;
            this.columnHeader8.Text = "Through";
            this.columnHeader8.Width = 64;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Description";
            this.columnHeader9.Width = 65;
            // 
            // columnHeader10
            // 
            this.columnHeader10.DisplayIndex = 7;
            this.columnHeader10.Text = "Type";
            this.columnHeader10.Width = 40;
            // 
            // columnHeader11
            // 
            this.columnHeader11.DisplayIndex = 9;
            this.columnHeader11.Text = "Incharge";
            this.columnHeader11.Width = 55;
            // 
            // columnHeader12
            // 
            this.columnHeader12.DisplayIndex = 10;
            this.columnHeader12.Text = "Status";
            this.columnHeader12.Width = 45;
            // 
            // columnHeader13
            // 
            this.columnHeader13.DisplayIndex = 11;
            this.columnHeader13.Text = "Solution";
            // 
            // columnHeader14
            // 
            this.columnHeader14.DisplayIndex = 12;
            this.columnHeader14.Text = "Priority";
            this.columnHeader14.Width = 50;
            // 
            // columnHeader15
            // 
            this.columnHeader15.Text = "Issue";
            // 
            // columnHeader16
            // 
            this.columnHeader16.Text = "Product";
            // 
            // columnHeader23
            // 
            this.columnHeader23.Text = "Server";
            // 
            // columnHeader24
            // 
            this.columnHeader24.Text = "Version";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.groupBox1.Controls.Add(this.linkLabel3);
            this.groupBox1.Controls.Add(this.CD_happenTime);
            this.groupBox1.Controls.Add(this.CD_version);
            this.groupBox1.Controls.Add(this.CD_solution);
            this.groupBox1.Controls.Add(this.CD_description);
            this.groupBox1.Controls.Add(this.CloseCase);
            this.groupBox1.Controls.Add(this.linkLabel1);
            this.groupBox1.Controls.Add(this.CD_AM);
            this.groupBox1.Controls.Add(this.linkLabel2);
            this.groupBox1.Controls.Add(this.CD_addCaseHints);
            this.groupBox1.Controls.Add(this.CD_addFollowUp);
            this.groupBox1.Controls.Add(this.label31);
            this.groupBox1.Controls.Add(this.CD_caseHints);
            this.groupBox1.Controls.Add(this.CD_save);
            this.groupBox1.Controls.Add(this.CD_through);
            this.groupBox1.Controls.Add(this.label28);
            this.groupBox1.Controls.Add(this.label24);
            this.groupBox1.Controls.Add(this.label27);
            this.groupBox1.Controls.Add(this.label23);
            this.groupBox1.Controls.Add(this.CD_followUps);
            this.groupBox1.Controls.Add(this.label25);
            this.groupBox1.Controls.Add(this.label26);
            this.groupBox1.Controls.Add(this.label19);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.CD_issueType);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.label22);
            this.groupBox1.Controls.Add(this.CD_caseType);
            this.groupBox1.Controls.Add(this.CD_callPerson);
            this.groupBox1.Controls.Add(this.CD_phone);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.CD_server);
            this.groupBox1.Controls.Add(this.CD_inchange);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.CD_product);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.CD_status);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.label18);
            this.groupBox1.Controls.Add(this.CD_client);
            this.groupBox1.Controls.Add(this.CD_priority);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.groupBox1.Location = new System.Drawing.Point(3, 1);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.groupBox1.Size = new System.Drawing.Size(563, 694);
            this.groupBox1.TabIndex = 33;
            this.groupBox1.TabStop = false;
            // 
            // linkLabel3
            // 
            this.linkLabel3.ActiveLinkColor = System.Drawing.Color.PowderBlue;
            this.linkLabel3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel3.AutoSize = true;
            this.linkLabel3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel3.ForeColor = System.Drawing.Color.SeaGreen;
            this.linkLabel3.LinkColor = System.Drawing.Color.SeaGreen;
            this.linkLabel3.Location = new System.Drawing.Point(367, 28);
            this.linkLabel3.Name = "linkLabel3";
            this.linkLabel3.Size = new System.Drawing.Size(53, 14);
            this.linkLabel3.TabIndex = 94;
            this.linkLabel3.TabStop = true;
            this.linkLabel3.Text = "Deadline";
            this.linkLabel3.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.linkLabel3.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel3_LinkClicked);
            // 
            // CD_happenTime
            // 
            this.CD_happenTime.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_happenTime.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.CD_happenTime.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CD_happenTime.Location = new System.Drawing.Point(70, 390);
            this.CD_happenTime.MaxLength = 40;
            this.CD_happenTime.Multiline = false;
            this.CD_happenTime.Name = "CD_happenTime";
            this.CD_happenTime.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.CD_happenTime.Size = new System.Drawing.Size(132, 18);
            this.CD_happenTime.TabIndex = 84;
            this.CD_happenTime.Text = "";
            // 
            // CD_version
            // 
            this.CD_version.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_version.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.CD_version.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CD_version.Location = new System.Drawing.Point(70, 149);
            this.CD_version.MaxLength = 20;
            this.CD_version.Multiline = false;
            this.CD_version.Name = "CD_version";
            this.CD_version.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
            this.CD_version.Size = new System.Drawing.Size(132, 18);
            this.CD_version.TabIndex = 74;
            this.CD_version.Text = "";
            // 
            // CD_solution
            // 
            this.CD_solution.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CD_solution.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_solution.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.CD_solution.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CD_solution.Location = new System.Drawing.Point(216, 515);
            this.CD_solution.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CD_solution.Multiline = true;
            this.CD_solution.Name = "CD_solution";
            this.CD_solution.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_solution.Size = new System.Drawing.Size(340, 170);
            this.CD_solution.TabIndex = 93;
            this.CD_solution.Enter += new System.EventHandler(this.CD_solution_Enter);
            this.CD_solution.Leave += new System.EventHandler(this.CD_solution_Leave);
            // 
            // CD_description
            // 
            this.CD_description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CD_description.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_description.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.CD_description.Font = new System.Drawing.Font("Microsoft YaHei", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.CD_description.Location = new System.Drawing.Point(216, 52);
            this.CD_description.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CD_description.Multiline = true;
            this.CD_description.Name = "CD_description";
            this.CD_description.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_description.Size = new System.Drawing.Size(340, 165);
            this.CD_description.TabIndex = 67;
            this.CD_description.Enter += new System.EventHandler(this.CD_description_Enter);
            this.CD_description.Leave += new System.EventHandler(this.CD_description_Leave);
            // 
            // CloseCase
            // 
            this.CloseCase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseCase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CloseCase.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.CloseCase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CloseCase.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CloseCase.ForeColor = System.Drawing.Color.Gray;
            this.CloseCase.Location = new System.Drawing.Point(483, 489);
            this.CloseCase.Name = "CloseCase";
            this.CloseCase.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CloseCase.Size = new System.Drawing.Size(73, 23);
            this.CloseCase.TabIndex = 68;
            this.CloseCase.Text = "CloseCase";
            this.CloseCase.UseVisualStyleBackColor = false;
            this.CloseCase.Click += new System.EventHandler(this.CloseCase_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.ActiveLinkColor = System.Drawing.Color.PowderBlue;
            this.linkLabel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.linkLabel1.ForeColor = System.Drawing.Color.SeaGreen;
            this.linkLabel1.LinkColor = System.Drawing.Color.SeaGreen;
            this.linkLabel1.Location = new System.Drawing.Point(422, 28);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(71, 14);
            this.linkLabel1.TabIndex = 70;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "CopyCaseID";
            this.linkLabel1.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // CD_AM
            // 
            this.CD_AM.AutoSize = true;
            this.CD_AM.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CD_AM.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.CD_AM.Location = new System.Drawing.Point(68, 26);
            this.CD_AM.Name = "CD_AM";
            this.CD_AM.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_AM.Size = new System.Drawing.Size(0, 16);
            this.CD_AM.TabIndex = 86;
            // 
            // linkLabel2
            // 
            this.linkLabel2.ActiveLinkColor = System.Drawing.Color.LightSeaGreen;
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.linkLabel2.ForeColor = System.Drawing.Color.Black;
            this.linkLabel2.LinkColor = System.Drawing.Color.SteelBlue;
            this.linkLabel2.Location = new System.Drawing.Point(69, 175);
            this.linkLabel2.Margin = new System.Windows.Forms.Padding(0);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(0, 14);
            this.linkLabel2.TabIndex = 75;
            this.linkLabel2.Click += new System.EventHandler(this.linkLabel2_Click);
            // 
            // CD_addCaseHints
            // 
            this.CD_addCaseHints.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_addCaseHints.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.CD_addCaseHints.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CD_addCaseHints.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CD_addCaseHints.ForeColor = System.Drawing.Color.Gray;
            this.CD_addCaseHints.Location = new System.Drawing.Point(157, 489);
            this.CD_addCaseHints.Name = "CD_addCaseHints";
            this.CD_addCaseHints.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_addCaseHints.Size = new System.Drawing.Size(45, 23);
            this.CD_addCaseHints.TabIndex = 86;
            this.CD_addCaseHints.Text = "Save";
            this.CD_addCaseHints.UseVisualStyleBackColor = false;
            this.CD_addCaseHints.Click += new System.EventHandler(this.button15_Click);
            // 
            // CD_addFollowUp
            // 
            this.CD_addFollowUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CD_addFollowUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_addFollowUp.FlatAppearance.BorderColor = System.Drawing.Color.Gainsboro;
            this.CD_addFollowUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CD_addFollowUp.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CD_addFollowUp.ForeColor = System.Drawing.Color.Gray;
            this.CD_addFollowUp.Location = new System.Drawing.Point(506, 233);
            this.CD_addFollowUp.Name = "CD_addFollowUp";
            this.CD_addFollowUp.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_addFollowUp.Size = new System.Drawing.Size(50, 23);
            this.CD_addFollowUp.TabIndex = 65;
            this.CD_addFollowUp.Text = "Input";
            this.CD_addFollowUp.UseVisualStyleBackColor = false;
            this.CD_addFollowUp.Click += new System.EventHandler(this.CD_addFollowUp_Click);
            // 
            // label31
            // 
            this.label31.AutoSize = true;
            this.label31.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label31.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label31.Location = new System.Drawing.Point(9, 26);
            this.label31.Margin = new System.Windows.Forms.Padding(0);
            this.label31.Name = "label31";
            this.label31.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label31.Size = new System.Drawing.Size(35, 16);
            this.label31.TabIndex = 85;
            this.label31.Text = "AM :";
            // 
            // CD_caseHints
            // 
            this.CD_caseHints.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.CD_caseHints.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_caseHints.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.CD_caseHints.ForeColor = System.Drawing.Color.Black;
            this.CD_caseHints.Location = new System.Drawing.Point(9, 515);
            this.CD_caseHints.Name = "CD_caseHints";
            this.CD_caseHints.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_caseHints.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
            this.CD_caseHints.Size = new System.Drawing.Size(193, 170);
            this.CD_caseHints.TabIndex = 85;
            this.CD_caseHints.Text = "";
            // 
            // CD_save
            // 
            this.CD_save.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CD_save.BackColor = System.Drawing.Color.SeaGreen;
            this.CD_save.FlatAppearance.BorderColor = System.Drawing.Color.SeaGreen;
            this.CD_save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CD_save.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CD_save.ForeColor = System.Drawing.SystemColors.ButtonFace;
            this.CD_save.Location = new System.Drawing.Point(501, 24);
            this.CD_save.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.CD_save.Name = "CD_save";
            this.CD_save.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_save.Size = new System.Drawing.Size(55, 23);
            this.CD_save.TabIndex = 92;
            this.CD_save.Text = "Save";
            this.CD_save.UseVisualStyleBackColor = false;
            this.CD_save.Click += new System.EventHandler(this.button8_Click);
            // 
            // CD_through
            // 
            this.CD_through.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CD_through.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CD_through.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_through.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CD_through.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CD_through.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CD_through.ForeColor = System.Drawing.Color.Black;
            this.CD_through.FormattingEnabled = true;
            this.CD_through.Location = new System.Drawing.Point(69, 356);
            this.CD_through.Name = "CD_through";
            this.CD_through.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_through.Size = new System.Drawing.Size(133, 21);
            this.CD_through.TabIndex = 83;
            // 
            // label28
            // 
            this.label28.AutoSize = true;
            this.label28.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label28.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label28.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label28.Location = new System.Drawing.Point(7, 492);
            this.label28.Name = "label28";
            this.label28.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label28.Size = new System.Drawing.Size(30, 16);
            this.label28.TabIndex = 79;
            this.label28.Text = "Hint";
            // 
            // label24
            // 
            this.label24.AutoSize = true;
            this.label24.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label24.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label24.Location = new System.Drawing.Point(7, 390);
            this.label24.Name = "label24";
            this.label24.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label24.Size = new System.Drawing.Size(55, 13);
            this.label24.TabIndex = 71;
            this.label24.Text = "HappenAt";
            // 
            // label27
            // 
            this.label27.AutoSize = true;
            this.label27.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label27.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label27.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label27.Location = new System.Drawing.Point(216, 237);
            this.label27.Name = "label27";
            this.label27.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label27.Size = new System.Drawing.Size(56, 14);
            this.label27.TabIndex = 77;
            this.label27.Text = "FollowUp";
            // 
            // label23
            // 
            this.label23.AutoSize = true;
            this.label23.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label23.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label23.Location = new System.Drawing.Point(7, 360);
            this.label23.Name = "label23";
            this.label23.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label23.Size = new System.Drawing.Size(47, 13);
            this.label23.TabIndex = 70;
            this.label23.Text = "Through";
            // 
            // CD_followUps
            // 
            this.CD_followUps.AllowUserToDeleteRows = false;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            dataGridViewCellStyle6.Padding = new System.Windows.Forms.Padding(1);
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.Color.DeepSkyBlue;
            this.CD_followUps.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle6;
            this.CD_followUps.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.CD_followUps.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.CD_followUps.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_followUps.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.CD_followUps.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.CD_followUps.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.GrayText;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.CD_followUps.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.CD_followUps.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.CD_followUps.ColumnHeadersVisible = false;
            this.CD_followUps.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Time,
            this.Content});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.MenuBar;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.CD_followUps.DefaultCellStyle = dataGridViewCellStyle8;
            this.CD_followUps.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.CD_followUps.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.CD_followUps.Location = new System.Drawing.Point(216, 260);
            this.CD_followUps.MultiSelect = false;
            this.CD_followUps.Name = "CD_followUps";
            this.CD_followUps.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_followUps.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle9.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            dataGridViewCellStyle9.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.CD_followUps.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.CD_followUps.RowHeadersVisible = false;
            this.CD_followUps.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle10.BackColor = System.Drawing.Color.WhiteSmoke;
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.Padding = new System.Windows.Forms.Padding(1);
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.CD_followUps.RowsDefaultCellStyle = dataGridViewCellStyle10;
            this.CD_followUps.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.CD_followUps.Size = new System.Drawing.Size(340, 214);
            this.CD_followUps.TabIndex = 66;
            this.CD_followUps.Tag = "";
            this.CD_followUps.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_CellBeginEdit);
            this.CD_followUps.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.CD_followUps.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
            // 
            // Time
            // 
            this.Time.DividerWidth = 1;
            this.Time.FillWeight = 40F;
            this.Time.HeaderText = "Time";
            this.Time.Name = "Time";
            this.Time.Width = 80;
            // 
            // Content
            // 
            this.Content.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Content.FillWeight = 78.17259F;
            this.Content.HeaderText = "Content";
            this.Content.Name = "Content";
            // 
            // label25
            // 
            this.label25.AutoSize = true;
            this.label25.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label25.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label25.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label25.Location = new System.Drawing.Point(216, 493);
            this.label25.Name = "label25";
            this.label25.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label25.Size = new System.Drawing.Size(51, 14);
            this.label25.TabIndex = 75;
            this.label25.Text = "Solution";
            // 
            // label26
            // 
            this.label26.AutoSize = true;
            this.label26.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label26.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label26.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label26.Location = new System.Drawing.Point(216, 28);
            this.label26.Name = "label26";
            this.label26.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label26.Size = new System.Drawing.Size(67, 14);
            this.label26.TabIndex = 74;
            this.label26.Text = "Description";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label19.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label19.Location = new System.Drawing.Point(7, 425);
            this.label19.Name = "label19";
            this.label19.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label19.Size = new System.Drawing.Size(34, 13);
            this.label19.TabIndex = 64;
            this.label19.Text = "Caller";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label20.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label20.Location = new System.Drawing.Point(7, 457);
            this.label20.Name = "label20";
            this.label20.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label20.Size = new System.Drawing.Size(37, 13);
            this.label20.TabIndex = 65;
            this.label20.Text = "Phone";
            // 
            // CD_issueType
            // 
            this.CD_issueType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CD_issueType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CD_issueType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_issueType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CD_issueType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CD_issueType.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CD_issueType.ForeColor = System.Drawing.Color.Black;
            this.CD_issueType.FormattingEnabled = true;
            this.CD_issueType.Location = new System.Drawing.Point(69, 292);
            this.CD_issueType.Name = "CD_issueType";
            this.CD_issueType.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_issueType.Size = new System.Drawing.Size(133, 21);
            this.CD_issueType.TabIndex = 79;
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label21.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label21.Location = new System.Drawing.Point(7, 328);
            this.label21.Name = "label21";
            this.label21.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label21.Size = new System.Drawing.Size(31, 13);
            this.label21.TabIndex = 66;
            this.label21.Text = "Type";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label22.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label22.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label22.Location = new System.Drawing.Point(7, 296);
            this.label22.Name = "label22";
            this.label22.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label22.Size = new System.Drawing.Size(33, 13);
            this.label22.TabIndex = 67;
            this.label22.Text = "Issue";
            // 
            // CD_caseType
            // 
            this.CD_caseType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CD_caseType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CD_caseType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_caseType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CD_caseType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CD_caseType.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CD_caseType.ForeColor = System.Drawing.Color.Black;
            this.CD_caseType.FormattingEnabled = true;
            this.CD_caseType.Location = new System.Drawing.Point(69, 324);
            this.CD_caseType.Name = "CD_caseType";
            this.CD_caseType.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_caseType.Size = new System.Drawing.Size(133, 21);
            this.CD_caseType.TabIndex = 80;
            // 
            // CD_callPerson
            // 
            this.CD_callPerson.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CD_callPerson.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CD_callPerson.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_callPerson.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CD_callPerson.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CD_callPerson.ForeColor = System.Drawing.Color.Black;
            this.CD_callPerson.FormattingEnabled = true;
            this.CD_callPerson.Location = new System.Drawing.Point(69, 421);
            this.CD_callPerson.Name = "CD_callPerson";
            this.CD_callPerson.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_callPerson.Size = new System.Drawing.Size(133, 21);
            this.CD_callPerson.Sorted = true;
            this.CD_callPerson.TabIndex = 81;
            this.CD_callPerson.TextChanged += new System.EventHandler(this.comboBox7_TextChanged);
            // 
            // CD_phone
            // 
            this.CD_phone.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CD_phone.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CD_phone.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_phone.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CD_phone.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CD_phone.ForeColor = System.Drawing.Color.Black;
            this.CD_phone.FormattingEnabled = true;
            this.CD_phone.Location = new System.Drawing.Point(69, 453);
            this.CD_phone.Name = "CD_phone";
            this.CD_phone.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_phone.Size = new System.Drawing.Size(133, 21);
            this.CD_phone.Sorted = true;
            this.CD_phone.TabIndex = 82;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label17.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label17.Location = new System.Drawing.Point(7, 149);
            this.label17.Name = "label17";
            this.label17.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label17.Size = new System.Drawing.Size(42, 13);
            this.label17.TabIndex = 59;
            this.label17.Text = "Version";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label16.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label16.Location = new System.Drawing.Point(7, 87);
            this.label16.Name = "label16";
            this.label16.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label16.Size = new System.Drawing.Size(39, 13);
            this.label16.TabIndex = 57;
            this.label16.Text = "Server";
            // 
            // CD_server
            // 
            this.CD_server.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CD_server.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CD_server.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_server.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CD_server.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CD_server.ForeColor = System.Drawing.Color.Black;
            this.CD_server.FormattingEnabled = true;
            this.CD_server.Location = new System.Drawing.Point(69, 83);
            this.CD_server.Name = "CD_server";
            this.CD_server.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_server.Size = new System.Drawing.Size(133, 21);
            this.CD_server.TabIndex = 72;
            this.CD_server.TextChanged += new System.EventHandler(this.comboBox11_TextChanged);
            // 
            // CD_inchange
            // 
            this.CD_inchange.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CD_inchange.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CD_inchange.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_inchange.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CD_inchange.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CD_inchange.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CD_inchange.ForeColor = System.Drawing.Color.Black;
            this.CD_inchange.FormattingEnabled = true;
            this.CD_inchange.Location = new System.Drawing.Point(69, 196);
            this.CD_inchange.Name = "CD_inchange";
            this.CD_inchange.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_inchange.Size = new System.Drawing.Size(133, 21);
            this.CD_inchange.TabIndex = 76;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label12.Location = new System.Drawing.Point(7, 264);
            this.label12.Name = "label12";
            this.label12.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label12.Size = new System.Drawing.Size(38, 13);
            this.label12.TabIndex = 53;
            this.label12.Text = "Status";
            // 
            // CD_product
            // 
            this.CD_product.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CD_product.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CD_product.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_product.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CD_product.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CD_product.ForeColor = System.Drawing.Color.Black;
            this.CD_product.FormattingEnabled = true;
            this.CD_product.Location = new System.Drawing.Point(69, 114);
            this.CD_product.Name = "CD_product";
            this.CD_product.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_product.Size = new System.Drawing.Size(133, 21);
            this.CD_product.TabIndex = 73;
            this.CD_product.TextChanged += new System.EventHandler(this.comboBox10_TextChanged);
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label13.Location = new System.Drawing.Point(8, 56);
            this.label13.Name = "label13";
            this.label13.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label13.Size = new System.Drawing.Size(34, 13);
            this.label13.TabIndex = 51;
            this.label13.Text = "Client";
            // 
            // CD_status
            // 
            this.CD_status.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CD_status.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CD_status.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_status.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CD_status.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CD_status.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CD_status.ForeColor = System.Drawing.Color.Black;
            this.CD_status.FormattingEnabled = true;
            this.CD_status.Location = new System.Drawing.Point(69, 260);
            this.CD_status.Name = "CD_status";
            this.CD_status.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_status.Size = new System.Drawing.Size(133, 21);
            this.CD_status.TabIndex = 78;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label15.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label15.Location = new System.Drawing.Point(7, 118);
            this.label15.Name = "label15";
            this.label15.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label15.Size = new System.Drawing.Size(44, 13);
            this.label15.TabIndex = 55;
            this.label15.Text = "Product";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.label14.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label14.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label14.Location = new System.Drawing.Point(7, 200);
            this.label14.Name = "label14";
            this.label14.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label14.Size = new System.Drawing.Size(50, 13);
            this.label14.TabIndex = 52;
            this.label14.Text = "Incharge";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label18.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label18.Location = new System.Drawing.Point(7, 232);
            this.label18.Name = "label18";
            this.label18.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.label18.Size = new System.Drawing.Size(41, 13);
            this.label18.TabIndex = 54;
            this.label18.Text = "Priority";
            // 
            // CD_client
            // 
            this.CD_client.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CD_client.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CD_client.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_client.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CD_client.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CD_client.ForeColor = System.Drawing.Color.Black;
            this.CD_client.FormattingEnabled = true;
            this.CD_client.Location = new System.Drawing.Point(69, 52);
            this.CD_client.Name = "CD_client";
            this.CD_client.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_client.Size = new System.Drawing.Size(133, 21);
            this.CD_client.Sorted = true;
            this.CD_client.TabIndex = 71;
            this.CD_client.TextChanged += new System.EventHandler(this.comboBox1_TextChanged_1);
            // 
            // CD_priority
            // 
            this.CD_priority.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.CD_priority.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.CD_priority.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.CD_priority.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CD_priority.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CD_priority.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CD_priority.ForeColor = System.Drawing.Color.Black;
            this.CD_priority.FormattingEnabled = true;
            this.CD_priority.Location = new System.Drawing.Point(69, 228);
            this.CD_priority.Name = "CD_priority";
            this.CD_priority.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.CD_priority.Size = new System.Drawing.Size(133, 21);
            this.CD_priority.TabIndex = 77;
            // 
            // LV_update
            // 
            this.LV_update.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LV_update.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(244)))), ((int)(((byte)(232)))));
            this.LV_update.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LV_update.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader60,
            this.columnHeader61,
            this.columnHeader62,
            this.columnHeader44});
            this.LV_update.FullRowSelect = true;
            this.LV_update.GridLines = true;
            this.LV_update.Location = new System.Drawing.Point(13, 520);
            this.LV_update.MultiSelect = false;
            this.LV_update.Name = "LV_update";
            this.LV_update.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.LV_update.Size = new System.Drawing.Size(219, 248);
            this.LV_update.TabIndex = 32;
            this.LV_update.UseCompatibleStateImageBehavior = false;
            this.LV_update.View = System.Windows.Forms.View.Details;
            this.LV_update.DoubleClick += new System.EventHandler(this.LV_update_DoubleClick);
            // 
            // columnHeader60
            // 
            this.columnHeader60.Width = 0;
            // 
            // columnHeader61
            // 
            this.columnHeader61.Text = "User";
            // 
            // columnHeader62
            // 
            this.columnHeader62.DisplayIndex = 3;
            this.columnHeader62.Text = "Handling";
            this.columnHeader62.Width = 114;
            // 
            // columnHeader44
            // 
            this.columnHeader44.DisplayIndex = 2;
            this.columnHeader44.Text = "Total";
            this.columnHeader44.Width = 43;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(1197, 792);
            this.Controls.Add(this.LabelCaseCount);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.LV_update);
            this.Controls.Add(this.warning);
            this.Controls.Add(this.refresh);
            this.Controls.Add(this.newCase);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.quickInput);
            this.Controls.Add(this.menuStrip1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.WindowText;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SupportLogSheet";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyDown);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.tabControl2.ResumeLayout(false);
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.tabPage10.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.Notice)).EndInit();
            this.tabPage11.ResumeLayout(false);
            this.tabPage11.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CD_followUps)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button quickInput;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button QI_add;
        private System.Windows.Forms.Button QI_clear;
        private System.Windows.Forms.Button newCase;
        private Button refresh;
        private ComboNF QI_Client;
        private ComboNF QI_CaseType;
        private ComboNF QI_Product;
        private TabPage tabPage4;
        private ListViewNF LV_mytask;
        private ColumnHeader MyTask_Case;
        private ColumnHeader MyTask_CaseID;
        private ColumnHeader MyTask_CreateTime;
        private ColumnHeader MyTask_Client;
        private ColumnHeader MyTask_CallPerson;
        private ColumnHeader MyTask_ContactNo;
        private ColumnHeader MyTask_ReceivedThrough;
        private ColumnHeader MyTask_Description;
        private ColumnHeader MyTask_Type;
        private ColumnHeader MyTask_Incharge;
        private ColumnHeader MyTask_Status;
        private ColumnHeader MyTask_Solution;
        private ColumnHeader MyTask_Priority;
        private TabPage tabPage3;
        private ListViewNF LV_search;
        private ColumnHeader Search_Case;
        private ColumnHeader Search_CaseID;
        private ColumnHeader Search_CreateTime;
        private ColumnHeader Search_Client;
        private ColumnHeader Search_CallPerson;
        private ColumnHeader Search_ContactNo;
        private ColumnHeader Search_ReceivedThrough;
        private ColumnHeader Search_Description;
        private ColumnHeader Search_Type;
        private ColumnHeader Search_Incharge;
        private ColumnHeader Search_Status;
        private ColumnHeader Search_Solution;
        private ColumnHeader Search_Priority;
        private Button Search_Multi;
        private Button Search_clear;
        private Button Search_search;
        private Label label8;
        private TabPage tabPage2;
        private ListViewNF LV_outstanding;
        private ColumnHeader Outstanding_Case;
        private ColumnHeader Outstanding_CaseID;
        private ColumnHeader Outstanding_CreateTime;
        private ColumnHeader Outstanding_Client;
        private ColumnHeader Outstanding_CallPerson;
        private ColumnHeader Outstanding_ContactNo;
        private ColumnHeader Outstanding_ReceivedThrough;
        private ColumnHeader Outstanding_Description;
        private ColumnHeader Outstanding_Type;
        private ColumnHeader Outstanding_Incharge;
        private ColumnHeader Outstanding_Status;
        private ColumnHeader Outstanding_Solution;
        private ColumnHeader Outstanding_Priority;
        private TabControl tabControl2;
        private TabPage tabPage5;
        private ListViewNF LV_CIM;
        private ColumnHeader columnHeader43;
        private ColumnHeader columnHeader45;
        private ColumnHeader columnHeader46;
        private ColumnHeader columnHeader47;
        private ColumnHeader columnHeader48;
        private ColumnHeader columnHeader49;
        private ColumnHeader columnHeader50;
        private ColumnHeader columnHeader51;
        private ComboNF QI_CallPerson;
        private ComboNF QI_Phone;
        private Button CIM_search;
        private ComboNF CIM_client;
        private Label label45;
        private Label label46;
        private Button CIM_add;
        private ColumnHeader MyTask_HappenTime;
        private ColumnHeader Search_HappenTime;
        private ColumnHeader Outstanding_HappenTime;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem outstandingToolStripMenuItem;
        private ToolStripMenuItem searchToolStripMenuItem;
        private ToolStripMenuItem myTaskToolStripMenuItem;
        private ToolStripMenuItem cIMToolStripMenuItem;
        private ToolStripMenuItem toolStripMenuItem4;
        private Label warning;
        private ListViewNF LV_update;
        private ColumnHeader columnHeader60;
        private ColumnHeader columnHeader61;
        private ColumnHeader columnHeader62;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private TabPage tabPage10;
        private Button Search_export;
        private ToolStripMenuItem noticeToolStripMenuItem;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem toolStripMenuItem5;
        private ToolStripMenuItem toolStripMenuItem6;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem toolStripMenuItem7;
        private ToolStripComboBox toolStripComboBox1;
        private ToolStripMenuItem toolStripMenuItem8;
        private TabPage tabPage11;
        private ListViewNF LV_Reminder;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader2;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader6;
        private ColumnHeader columnHeader7;
        private ColumnHeader columnHeader8;
        private ColumnHeader columnHeader9;
        private ColumnHeader columnHeader10;
        private ColumnHeader columnHeader11;
        private ColumnHeader columnHeader12;
        private ColumnHeader columnHeader13;
        private ColumnHeader columnHeader14;
        private Label Label_RemindTime;
        private ContextMenuStrip contextMenuStrip2;
        private ToolStripMenuItem removeToolStripMenuItem;
        private ToolStripMenuItem changeToolStripMenuItem;
        private ToolStripMenuItem reminderToolStripMenuItem;
        private ColumnHeader MyTask_IssueType;
        private ColumnHeader Search_IssueType;
        private ColumnHeader Outstanding_IssueType;
        private ColumnHeader columnHeader15;
        private ColumnHeader MyTask_Product;
        private ColumnHeader Search_Product;
        private ColumnHeader Outstanding_Product;
        private ColumnHeader columnHeader16;
        private ColumnHeader columnHeader21;
        private ColumnHeader columnHeader22;
        private ColumnHeader columnHeader19;
        private ColumnHeader columnHeader20;
        private ColumnHeader columnHeader17;
        private ColumnHeader columnHeader18;
        private ColumnHeader columnHeader23;
        private ColumnHeader columnHeader24;
        private ToolStripMenuItem productToolStripMenuItem;
        private ToolStripMenuItem productMasterToolStripMenuItem1;
        private ToolStripMenuItem productHintsToolStripMenuItem;
        private ToolStripMenuItem caseToolStripMenuItem;
        private ToolStripMenuItem myMarkedCasesToolStripMenuItem;
        private ToolStripMenuItem caseAnalysisToolStripMenuItem;
        private ToolStripMenuItem settingToolStripMenuItem;
        private ToolStripMenuItem customInfoToolStripMenuItem;
        private ToolStripMenuItem shortCutKeyToolStripMenuItem;
        private Label label6;
        private GroupBox groupBox1;
        private Label label17;
        private Label label16;
        private ComboNF CD_server;
        private ComboNF CD_inchange;
        private Label label12;
        private ComboNF CD_product;
        private Label label13;
        private ComboNF CD_status;
        private Label label15;
        private Label label14;
        private Label label18;
        private ComboNF CD_client;
        private ComboNF CD_priority;
        private Label label19;
        private Label label20;
        private ComboNF CD_issueType;
        private Label label21;
        private Label label22;
        private ComboNF CD_caseType;
        private ComboNF CD_callPerson;
        private ComboNF CD_phone;
        private Label label23;
        private ComboNF CD_through;
        private Label label24;
        private Label label25;
        private Label label26;
        private Label label27;
        private SplitContainer splitContainer1;
        private RichTextBox CD_caseHints;
        private Label label28;
        private Button CD_save;
        private Button CD_addFollowUp;
        private Label CD_AM;
        private Label label31;
        private Button CD_addCaseHints;
        private LinkLabel linkLabel2;
        private ComboNF QI_IssueType;
        private Label label29;
        private Label label32;
        private LinkLabel linkLabel1;
        private Button CloseCase;
        private Label label7;
        private ComboNF Combo_Search_Subcate;
        private Label label9;
        private ComboNF Combo_Search_Cate;
        private Label label10;
        private ListViewNF LV_Reminder_Expired;
        private ColumnHeader columnHeader25;
        private ColumnHeader columnHeader26;
        private ColumnHeader columnHeader27;
        private ColumnHeader columnHeader28;
        private ColumnHeader columnHeader29;
        private ColumnHeader columnHeader30;
        private ColumnHeader columnHeader31;
        private ColumnHeader columnHeader32;
        private ColumnHeader columnHeader33;
        private ColumnHeader columnHeader34;
        private ColumnHeader columnHeader35;
        private ColumnHeader columnHeader36;
        private ColumnHeader columnHeader37;
        private ColumnHeader columnHeader38;
        private ColumnHeader columnHeader39;
        private ColumnHeader columnHeader40;
        private ColumnHeader columnHeader41;
        private ColumnHeader columnHeader42;
        private Button button1;
        private Label Label_RemindCaseID;
        private Label label_SearchCount;
        private ColumnHeader columnHeader44;
        private TextBox CD_description;
        private TextBox CD_solution;
        private Label LabelCaseCount;
        private CheckBox checkBox_Popup;
        private DataGridViewNF CD_followUps;
        private RichTextBox QI_HappenTime;
        private RichTextBox TB_CaseID;
        private RichTextBox CIM_phone;
        private RichTextBox CD_happenTime;
        private RichTextBox CD_version;
        private DataGridViewTextBoxColumn Time;
        private DataGridViewTextBoxColumn Content;
        private TextBox QI_description;
        private LinkLabel linkLabel3;
        private Label QI_AM;
        private Label label11;
        private DataGridViewNF Notice;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;

        public System.Windows.Forms.ToolStripComboBox ToolStripComboBox1
        {
            get { return toolStripComboBox1; }
            set { toolStripComboBox1 = value; }
        }

        public System.Windows.Forms.Label Warning
        {
            get { return warning; }
            set { warning = value; }
        }
    }

}

