// 快捷键设置
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SupportLogSheet
{
    public partial class ShortCutKey : Form
    {
        private Dictionary<string, TextBox> shortCutKeyMapping = new Dictionary<string, TextBox>();
        private MainForm mf;
        private bool isInitialDone = false;
        public ShortCutKey(MainForm mf)
        {
            this.mf = mf;
            InitializeComponent();
            TextBox[] boxes = { TX_QI_NewOne, TB_QI_Add, TB_QI_Clear, TB_QI_SwitchLeft, TB_QI_SwitchRight, TB_NewCase, TB_Refresh, TB_CloseCase, TB_Multifilter, TB_Tab_Outstanding, TB_Tab_Search, TB_Tab_Mytask, TB_Tab_CIM, TB_Tab_Notice, TB_Tab_Reminder };
            for (int i = 0; i < Config.shortCutKeys.Length; i++)
            {
                shortCutKeyMapping.Add(Config.shortCutKeys[i], boxes[i]);
                boxes[i].Text = mf.ShortCutKeyIni[Config.shortCutKeys[i]];
            }
            isInitialDone = true;
        }

        private void Button_Save_Click(object sender, EventArgs e)
        {
            mf.ShortCutKeyIni.Clear();
            for (int i = 0; i < Config.shortCutKeys.Length; i++)
            {
                mf.ShortCutKeyIni.Add(Config.shortCutKeys[i], shortCutKeyMapping[Config.shortCutKeys[i]].Text);
            }
            Msg_OP msg_Op = new Msg_OP("IO8", null);
            Config.SLS_Sock.inMsgQueue_add(msg_Op);
             this.Dispose();
        }

        public string input_shortcut(string s)
        {
            if (isInitialDone)
            {
                try
                {
                    string value = s.Substring(0, 1).ToUpper();
                    if (mf.ShortCutKeyIni.Values.Contains(value))
                    {
                        MessageBox.Show(value + " already been set");
                        return "";
                    }
                    return value;
                }
                catch
                {
                    return "";
                }
            }
            return s;
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            TX_QI_NewOne.Text = input_shortcut(TX_QI_NewOne.Text);
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            TB_QI_Add.Text = input_shortcut(TB_QI_Add.Text);
        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {
            TB_QI_Clear.Text = input_shortcut(TB_QI_Clear.Text);
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            TB_QI_SwitchLeft.Text = input_shortcut(TB_QI_SwitchLeft.Text);
        }

        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            TB_QI_SwitchRight.Text = input_shortcut(TB_QI_SwitchRight.Text);
        }

        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            TB_NewCase.Text = input_shortcut(TB_NewCase.Text);
        }
        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            TB_Refresh.Text = input_shortcut(TB_Refresh.Text);
        }

        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            TB_Tab_Outstanding.Text = input_shortcut(TB_Tab_Outstanding.Text);
        }

        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            TB_Tab_Search.Text = input_shortcut(TB_Tab_Search.Text);
        }
        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            TB_Tab_Mytask.Text = input_shortcut(TB_Tab_Mytask.Text);
        }

        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            TB_Tab_CIM.Text = input_shortcut(TB_Tab_CIM.Text);
        }

        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            TB_Tab_Notice.Text = input_shortcut(TB_Tab_Notice.Text);
        }
        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            TB_Tab_Reminder.Text = input_shortcut(TB_Tab_Reminder.Text);
        }

        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            TB_CloseCase.Text = input_shortcut(TB_CloseCase.Text);
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            TB_Multifilter.Text = input_shortcut(TB_Multifilter.Text);
        }

    }
}
