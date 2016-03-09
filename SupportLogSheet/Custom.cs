// 编辑用户个人信息
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
using System.Runtime.InteropServices;

namespace SupportLogSheet
{
    public partial class Custom : Form
    {
        private bool IsClientLevelOpened = false;
        private bool IsCasePriorityOpened = false;
        private bool IsClientLevelInitialDone = false;
        private bool IsCasePriorityInitialDone = false;
        private bool IsNotifyOpened = false;
        private bool IsNotifyInitialDone = false;
        private bool IsColorOpened = false;
        private Dictionary<string, string> CaseProperty;
        private List<Control> FontControls = new List<Control>();
        private MyInfo myInfo;

        public Custom(MyInfo myInfo,Dictionary<string, string> CaseProperty)
        {
            InitializeComponent();
            this.CaseProperty = CaseProperty;          
            textBox1.Text = myInfo.MyName;
            textBox2.Text = myInfo.MyChiName;
            textBox3.Text = myInfo.MyEmail;
            this.myInfo = myInfo;
            FontConverter fc = new FontConverter();
            label2.Font = (Font)fc.ConvertFrom(Config.Customization_INI.readValue("Font", "Content"));
        }

        public void addControl(Control c)
        {
            FontControls.Add(c);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Trim(' ').Equals("") || textBox2.Text.Trim(' ').Equals("") || textBox3.Text.Trim(' ').Equals(""))
            {
                MessageBox.Show("Please input all the informations !");
                return;
            }
            message msg = new message();
            msg.setKeyValuePair("80", textBox1.Text);
            msg.setKeyValuePair("81", textBox2.Text);
            msg.setKeyValuePair("85", textBox3.Text);
            msg.setKeyValuePair("89", myInfo.MyName);
            Config.SLS_Sock.socketMsg("U2", msg, null);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bool canBeChange = true;
            if (utility.checkLogin(myInfo.MyName, textBox4.Text.Trim(' ')) == -2)
            {
                canBeChange = false;
                MessageBox.Show("Wrong Password ! ");
            }
            if (!textBox5.Text.Trim(' ').Equals(textBox6.Text.Trim(' ')))
            {
                canBeChange = false;
                MessageBox.Show("Confirm password is not the same as New Password ! ");
            }
            if (canBeChange)
            {
                string salt = Guid.NewGuid().ToString();
                string password = textBox6.Text.Trim(' ');
                byte[] passwordAndSaltBytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = new System.Security.Cryptography.SHA256Managed().ComputeHash(passwordAndSaltBytes);
                string hashString = Convert.ToBase64String(hashBytes);
                message msg = new message();
                msg.setKeyValuePair("80", myInfo.MyName);
                msg.setKeyValuePair("82", password);
                msg.setKeyValuePair("83", hashString);
                msg.setKeyValuePair("84", salt);
                Config.SLS_Sock.socketMsg("U4", msg, null);
            } 
        }

        private void label1_Click(object sender, EventArgs e)
        {
            if (!IsClientLevelOpened)
            {
                if (!IsClientLevelInitialDone)
                {
                    string[] clientLevels = Regex.Split(CaseProperty["ClientLevel"].ToString(), ",");
                    panel1.Size = new System.Drawing.Size(75, clientLevels.Length * 22 + 4);
                    panel1.Location = new System.Drawing.Point(label1.Left + label1.Width, label1.Top);
                    for (int i = 0; i < clientLevels.Length; i++)
                    {
                        LinkLabel linkLable = new LinkLabel();
                        linkLable.Text = "ClientLevel " + clientLevels[i];
                        linkLable.LinkColor = LV_OP.ClientLevelColorDetail[clientLevels[i]];
                        linkLable.VisitedLinkColor = Color.DarkOrange;
                        linkLable.Left = 3;
                        linkLable.Top = 3 + i * 22;
                        panel1.Controls.Add(linkLable);
                    }
                    foreach (LinkLabel label in panel1.Controls)
                    {
                        label.Click += delegate(object s, EventArgs k)
                        {
                            for (int i = 0; i < panel1.Controls.Count; i++)
                            {
                                if (s == panel1.Controls[i])
                                {
                                    ColorDialog ColorDlg = new ColorDialog();
                                    ColorDlg.ShowHelp = true;
                                    if (ColorDlg.ShowDialog(this) == DialogResult.OK)
                                    {
                                        (s as LinkLabel ).LinkColor = ColorDlg.Color;
                                        LV_OP.ClientLevelColorDetail[clientLevels[i]] = ColorDlg.Color;
                                        string[] getIniName = Regex.Split((s as LinkLabel).Text, " ");
                                        Config.Customization_INI.writeValue("ClientLevelColor", getIniName[1].Trim(' '), ColorDlg.Color.ToString());
                                        break;
                                    }
                                }
                            }
                        };
                    }
                }
                IsClientLevelOpened = true;                      
                label3.Text = "CasePriority>";
                label5.Text = "Notify>";
                label1.Text = "ClientLevel<";
                panel3.Hide();
                panel2.Hide();
                panel1.Show();
                //panel1.BringToFront();       
            }
            else
            {
                IsClientLevelOpened = false;
                label1.Text = "ClientLevel>";
                panel1.Hide();
            }
            IsClientLevelInitialDone = true;
        }

        private void label2_Click(object sender, EventArgs e)
        {
            try
            {
                FontConverter fc = new FontConverter();
                FontDialog FontDlg = new FontDialog();
                FontDlg.ShowHelp = true;
                if (FontDlg.ShowDialog(this) == DialogResult.OK)
                {
                    label2.Font = FontDlg.Font;
                    Config.Customization_INI.writeValue("Font", "Content", fc.ConvertToString(FontDlg.Font));
                    Config.Font_Content = FontDlg.Font;

                    foreach (Control c in FontControls)
                    {
                        if (c != null)
                        {
                            c.Font = Config.Font_Content;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void SetLabelFont(TabControl tab,Font font)
        {
            foreach (TabPage page in tab.TabPages)
            {
                foreach (Control control in page.Controls)
                {
                    if (control is Label)
                    {
                        (control as Label).Font = font;
                    }
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            if (!IsCasePriorityOpened)
            {
                if (!IsCasePriorityInitialDone)
                {
                    string[] priorities = Regex.Split(CaseProperty["Priority"].ToString(), ",");
                    panel2.Size = new System.Drawing.Size(45, priorities.Length * 22 + 4);
                    panel2.Location = new System.Drawing.Point(label3.Left + label3.Width, label3.Top);
                    for (int i = 0; i < priorities.Length; i++)
                    {
                        Label label = new Label();
                        label.Text = priorities[i];
                        label.BackColor = LV_OP.PriorityColorDetail[priorities[i]];
                        label.ForeColor = Color.Black;
                        label.Left = 0;
                        label.Top = i * 22;
                        label.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
                        panel2.Controls.Add(label);
                    }
                    foreach (Label label in panel2.Controls)
                    {
                        label.Click += delegate(object s, EventArgs k)
                        {
                            ColorDialog ColorDlg = new ColorDialog();
                            ColorDlg.ShowHelp = true;
                            if (ColorDlg.ShowDialog(this) == DialogResult.OK)
                            {
                                for (int i = 0; i < panel2.Controls.Count; i++)
                                {
                                    if (s == panel2.Controls[i])
                                    {
                                        (s as Label).BackColor = ColorDlg.Color;
                                        LV_OP.PriorityColorDetail[priorities[i]] = ColorDlg.Color;
                                        Config.Customization_INI.writeValue("PriorityColor", (s as Label).Text, ColorDlg.Color.ToString());
                                        break;
                                    }
                                }
                            }
                        };
                    }
                }
               
                IsCasePriorityOpened = true;                       
                label3.Text = "CasePriority<";
                label1.Text = "ClientLevel>";
                label5.Text = "Notify>";
                panel1.Hide();
                panel3.Hide();
                panel2.Show();
                //panel2.BringToFront();    
            }
            else
            {
                IsCasePriorityOpened = false;
                label3.Text = "CasePriority>";
                panel2.Hide();
            }
            IsCasePriorityInitialDone = true;
        }

        private void label4_Click(object sender, EventArgs e)
        {
            if (IsColorOpened == false)
            {
                label1.Visible = true;
                label3.Visible = true;
                label5.Visible = true;
                IsColorOpened = true;
                return;
            }
            if (IsColorOpened == true)
            {
                label1.Visible = false;
                label3.Visible = false;
                label5.Visible = false;
                panel1.Visible = false;
                panel2.Visible = false;
                panel3.Visible = false;
                label3.Text = "CasePriority>";
                label1.Text = "ClientLevel>";
                label5.Text = "Notify>";
                IsColorOpened = false;
                return;
            }
        }

        public void labelClick(Label  label,int i,string colorName)
        {
            label.Click += delegate(object s, EventArgs k)
            {
                ColorDialog ColorDlg = new ColorDialog();
                ColorDlg.ShowHelp = true;
                if (ColorDlg.ShowDialog(this) == DialogResult.OK)
                {
                    label.BackColor = ColorDlg.Color;
                    Config.Customization_INI.writeValue("NotifyColor", colorName, ColorDlg.Color.ToString());
                    switch (i)
                    {
                        case 0:
                            {
                                LV_OP.Color_NewItem = ColorDlg.Color;
                                break;
                            }
                        case 1:
                            {
                                LV_OP.Color_EditItem = ColorDlg.Color;
                                break;
                            }
                        default:
                            break;
                    }
                }
            };
        }

        private void label5_Click(object sender, EventArgs e)
        {
            if (!IsNotifyOpened)
            {
                if (!IsNotifyInitialDone)
                {
                    panel3.Size = new System.Drawing.Size(60, 2 * 22 + 4);
                    panel3.Location = new System.Drawing.Point(label5.Left + label5.Width, label5.Top);
                    for (int i = 0; i < 2; i++)
                    {
                        Label label = new Label();
                        switch(i)
                        {
                            case 0:
                                {
                                    label.Text = "NewCase";
                                    label.BackColor = LV_OP.Color_NewItem;
                                    labelClick(label,0, "NewCase");
                                    break;
                                }
                            case 1:
                                {
                                    label.Text = "EditCase";
                                    label.BackColor = LV_OP.Color_EditItem;
                                    labelClick(label, 1, "EditCase");
                                    break;
                                }
                            default:
                                break;
                        }
                       // linkLable.VisitedLinkColor = Color.DarkOrange;
                        label.Left = 0;
                        label.Top = i * 22;
                        label.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
                        panel3.Controls.Add(label);
                    }
                }            
                IsNotifyOpened = true;                    
                label3.Text = "CasePriority>";
                label1.Text = "ClientLevel>";
                label5.Text = "Notify<";
                panel1.Hide();                
                panel2.Hide();
                panel3.Show();
                //panel3.BringToFront();           
            }
            else
            {
                IsNotifyOpened = false;
                label5.Text = "Notify>";
                panel3.Hide();
            }
            IsNotifyInitialDone = true;
        }

    }
}
