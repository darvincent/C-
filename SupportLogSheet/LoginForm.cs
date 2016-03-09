// 登录 注册
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace SupportLogSheet
{
    public partial class LoginForm : Form
    {
        public static bool IsLogin = false;
        public static string LoginUser;
        public static string MyID;
        public static bool IsSupport = false;
        public bool Switcher = false;
        public LoginForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (login.Text.Trim(' ').Equals ("Login"))
            {
                int status = utility.checkLogin(username.Text.Trim(' '),password.Text.Trim(' '));
                if (status == 1)
                {
                    this.Dispose();
                    IsLogin = true;
                }
                else if (status == -1)
                {
                    MessageBox.Show("User not exist!");
                }
                else if (status == -2)
                {
                    MessageBox.Show("You are Inactive, please contact SuperUsers!");
                }
                else {
                    MessageBox.Show("Wrong Password!");
                }
            }
            if (login.Text.Trim(' ').Equals( "register"))
            {
                password.UseSystemPasswordChar = false;
                bool done = true;
                if (username.Text.Trim(' ').Equals(""))
                {
                    done = false;
                    MessageBox.Show("Please input UserName!");
                }
                if (password.Text.Trim(' ').Equals(""))
                {
                    done = false;
                    MessageBox.Show("Please input Password!");
                }
                if (confirmPwd.Text.Trim(' ').Equals(""))
                {
                    done = false;
                    MessageBox.Show("Please Comfirm Password!");
                }
                if (email.Text.Trim(' ').Equals(""))
                {
                    done = false;
                    MessageBox.Show("Please input Email!");
                }
                if (chineseName.Text.Trim(' ').Equals(""))
                {
                    done = false;
                    MessageBox.Show("Please input ChineseName!");
                }
                if (!password.Text.Trim(' ').Equals( confirmPwd.Text.Trim(' ')))
                {
                    done = false;
                    MessageBox.Show("Inputed two passwords are not the same!");
                }
                if (done)
                {            
                    bool existUser = register(username.Text.Trim(' '), chineseName.Text.Trim(' '), password.Text.Trim(' '), email.Text.Trim(' '));
                    if (!existUser)
                    {
                        MessageBox.Show("This user already registed.");
                    }
                    else
                    {
                        MessageBox.Show("register done, Please contact SuperUsers for activation!");
                        login.Text = "Login";
                        registerToJoin.Text = "register to join";
                        username.Clear();
                        password.Clear();
                        confirmPwd.Clear();
                        email.Clear();
                        chineseName.Clear();
                        label4.Visible = false;
                        label5.Visible = false;
                        label6.Visible = false;
                        confirmPwd.Visible = false;
                        email.Visible = false;
                        chineseName.Visible = false;
                        password.UseSystemPasswordChar = true;
                    }
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {
            Switcher = true;
            if (registerToJoin.Text.Trim(' ').Equals( "Register to join") && Switcher)
            {
                registerToJoin.Text = "Back to login";
                label4.Visible = true;
                confirmPwd.Visible = true;
                label5.Visible = true;
                email.Visible = true;
                label6.Visible = true;          
                chineseName.Visible = true;
                login.Text = "register";
                Switcher = false;
                password.UseSystemPasswordChar = false;
            }
            if (registerToJoin.Text.Trim(' ').Equals( "Back to login") && Switcher)
            {
                registerToJoin.Text = "Register to join";
                login.Text = "Login";          
                chineseName.Visible = false;
                label6.Visible = false;
                email.Visible = false;
                label5.Visible = false;
                confirmPwd.Visible = false;
                label4.Visible = false;
                Switcher = false;
                username.Clear();
                password.Clear();
                confirmPwd.Clear();
                email.Clear();
                chineseName.Clear();
                password.UseSystemPasswordChar = true;
            }
        }

        private void textBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Enter)
            {
                this.button1_Click(null, null);
            }
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                password.Focus();
            }
        }

        public bool register(string userName, string chineseName, string password, string email)
        {
            // random salt 
            // you can also use RNGCryptoServiceProvider class            
            //System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider(); 
            //byte[] saltBytes = new byte[36]; 
            //rng.GetBytes(saltBytes); 
            //string salt = Convert.ToBase64String(saltBytes); 
            //string salt = toHexString(saltBytes); 

            string sqlcmdTemp = "select UserName from UserFile";
            using (SqlConnection sqlConnection = SQL.dbConnect())
            {
                using (SqlCommand cmdTemp = new SqlCommand(sqlcmdTemp, sqlConnection))
                {
                    try
                    {
                        using (SqlDataReader re = cmdTemp.ExecuteReader())
                        {
                            string username;
                            while (re.Read())
                            {
                                username = re.GetValue(re.GetOrdinal(Config.getValue("80"))).ToString().Trim(' ');
                                if (username.ToUpper() == userName.ToUpper())
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Query database error!");
                    }
                    string salt = Guid.NewGuid().ToString();
                    byte[] passwordAndSaltBytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
                    byte[] hashBytes = new System.Security.Cryptography.SHA256Managed().ComputeHash(passwordAndSaltBytes);
                    string hashString = Convert.ToBase64String(hashBytes);

                    message msg = new message();
                    msg.setKeyValuePair("80", userName);
                    msg.setKeyValuePair("81", chineseName);
                    msg.setKeyValuePair("82", password);
                    msg.setKeyValuePair("83", hashString);
                    msg.setKeyValuePair("84", salt);
                    msg.setKeyValuePair("85", email);
                    msg.setKeyValuePair("86", "N");
                    msg.setKeyValuePair("87", "");
                    msg.setKeyValuePair("88", "N");
                    msg.setKeyValuePair("90", "N");

                    string sqlcmd = SQL.addSqlCmd(Config.DBName_UserFile, Config.UserFileKeys, msg);
                    using (SqlCommand cmd = new SqlCommand(sqlcmd, sqlConnection))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }
    }
}
