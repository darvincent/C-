// 超级用户 对 普通用户的 增删改
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

namespace SupportLogSheet
{
    public partial class ActUser_BySuperUser : Form
    {
        private string EXUserID;
        private Dictionary<string, string> UserIDNameMapping;
        public ActUser_BySuperUser(ListViewItem lvi, Dictionary<string, string> UserIDNameMapping)
        {
            InitializeComponent();
            TB_Name.Text = lvi.SubItems[1].Text;
            TB_ChiName.Text = lvi.SubItems[2].Text;
            TB_Email.Text = lvi.SubItems[3].Text;
            TB_UserID.Text = lvi.SubItems[5].Text;
            EXUserID = lvi.SubItems[5].Text;
            CB_SuperUser.Checked = lvi.SubItems[4].Text.Trim(' ') == "N" ? false : true;
            CB_Active.Checked = lvi.SubItems[6].Text.Trim(' ') == "N" ? false : true;
            CB_IsSupport.Checked = lvi.SubItems[7].Text.Trim(' ') == "N" ? false : true;
            this.UserIDNameMapping = UserIDNameMapping;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string name = TB_Name.Text.Trim();
            if (name.Equals("Admin") || name.Equals("Unassigned"))
            {
                MessageBox.Show("This user can't be deleted !");
                return;
            }
            DialogResult result = MessageBox.Show("Delete ?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
            if (result.ToString().Equals( "OK"))
            {
                message msg = new message();
                msg.setKeyValuePair("80", TB_Name.Text);
                Config.SLS_Sock.socketMsg("U3", msg, this);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string exID = TB_UserID.Text.Trim(' ');
            if (exID.Equals(""))
            {
                MessageBox.Show("Please assign an UserID for this user !");
                return;
            }
            if (!exID.Equals(EXUserID))
            {
                if (UserIDNameMapping.ContainsKey(exID))
                {
                    MessageBox.Show("This UserID has been used !");
                    return;
                }
            }
            string superUser = "";
            string active = "";
            string IsSupport = "";
            superUser = CB_SuperUser.Checked ? "Y" : "N";
            active = CB_Active.Checked ? "Y" : "N";
            IsSupport = CB_IsSupport.Checked ? "Y" : "N";
            message msg = new message();
            msg.setKeyValuePair("80", TB_Name.Text);
            msg.setKeyValuePair("81", TB_ChiName.Text);
            msg.setKeyValuePair("85", TB_Email.Text);
            msg.setKeyValuePair("87", TB_UserID.Text);
            msg.setKeyValuePair("86", superUser);
            msg.setKeyValuePair("88", active);
            msg.setKeyValuePair("90", IsSupport);
            Config.SLS_Sock.socketMsg("U1", msg, this);
        }

    }
}
