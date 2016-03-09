// 编辑 收藏case的笔记
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.Office.Interop.Word;
using System.Threading;

namespace SupportLogSheet
{
    public partial class MarkedCaseContent : Form
    {
        public string Category;
        public string CaseID;
        public string FilePath;
        public MarkedCases myMarkedCase;
        public MarkedCaseContent(string category,string caseID)
        {
            try
            {
                this.Category = category;
                this.CaseID = caseID;
                this.FilePath = new StringBuilder("./").Append(Category).Append("/").Append(CaseID).Append("_CaseRemark.doc").ToString();
                InitializeComponent();
                utility.setFont(this, Config.Font_Content);
                if (!Directory.Exists("./" + Category))
                {
                    Directory.CreateDirectory("./" + Category);
                }
                if (File.Exists(FilePath))
                {
                    Microsoft.Office.Interop.Word._Application DOCApp = new Microsoft.Office.Interop.Word.Application();
                    Microsoft.Office.Interop.Word._Document doc = new Microsoft.Office.Interop.Word.Document();
                    Object Nothing = System.Reflection.Missing.Value;
                    doc = DOCApp.Documents.Add(ref Nothing, ref Nothing, ref Nothing, ref Nothing);
                    richTextBox3.LoadFile(FilePath);
                    doc.Close();
                    DOCApp.Quit();
                }
            }
            catch (IOException ex)
            {
                MessageBox.Show("The Remark document is closing");
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                richTextBox3.SaveFile(@FilePath);
                Microsoft.Office.Interop.Word._Application DOCApp = new Microsoft.Office.Interop.Word.Application();
                Microsoft.Office.Interop.Word._Document doc = new Document();
                Object Nothing = System.Reflection.Missing.Value;
                doc = DOCApp.Documents.Add(ref Nothing, ref Nothing, ref Nothing, ref Nothing);
                doc.Close();
                DOCApp.Quit();
                MessageBox.Show("Change Saved!");
            }
            catch (IOException ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Remove ?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (result.ToString().Equals("OK"))
            {
                if (myMarkedCase.Category_LVs[Category].SelectedItems.Count > 0)
                {
                    myMarkedCase.Category_LVs[Category].SelectedItems[0].Remove();
                    myMarkedCase.mf.MarkedCateCases[Category].Remove(CaseID);
                    message msg = new message();
                    msg.setKeyValuePair("1", CaseID);
                    msg.setKeyValuePair("210", Category);
                    msg.setKeyValuePair("212", FilePath);
                    Msg_OP msg_Op = new Msg_OP("IO5", new message[] { msg });
                    Config.SLS_Sock.inMsgQueue_add(msg_Op);
                    this.Dispose();
                }
            }
        }
    }
}
