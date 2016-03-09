// 增删改客户的服务器信息
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
    public partial class ActClientServer : Form
    {
        private string Type;
        private string exServer;
        // add Type = 0
        public ActClientServer(List<string> ClientList)
        {
            InitializeComponent();
            Combo_OP.initialComboBox(comboBox1, ClientList);
            this.Text = "AddServer";
            button2.Text = "Clear";
            button1.Text = "Add";
            this.Type = "PM1";
        }

        //edit Type = 1
        public ActClientServer(string client, string server, List<string> ClientList)
        {
            InitializeComponent();
            Combo_OP.initialComboBox(comboBox1, ClientList);
            this.Text = "EditServer";
            this.Type = "PM2";
            List<message> msgs = initialClientServer(client, server);
            if (msgs != null)
            {
                message msg = msgs[0];
                exServer = msg.getValueFromPairs("160");
                comboBox1.Text = client;
                textBox1.Text = msg.getValueFromPairs("160");
                textBox2.Text = msg.getValueFromPairs("163");
                textBox3.Text = msg.getValueFromPairs("164");
                textBox4.Text = msg.getValueFromPairs("165");
                textBox5.Text = msg.getValueFromPairs("162");
                if (msg.getValueFromPairs("161") == "N")
                {
                    checkBox1.Checked = false;
                }
                else
                {
                    checkBox1.Checked = true;
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            message msg = new message();
            msg.setKeyValuePair("4", comboBox1.Text);
            msg.setKeyValuePair("160", textBox1.Text);
            if (checkBox1.Checked)
            {
                msg.setKeyValuePair("161", "Y");
            }
            else
            {
                msg.setKeyValuePair("161", "N");
            }
            msg.setKeyValuePair("162", textBox5.Text);
            msg.setKeyValuePair("163", textBox2.Text);
            msg.setKeyValuePair("164", textBox3.Text);
            msg.setKeyValuePair("165", textBox4.Text);
            if (Type.Equals("PM2"))
            {
                msg.setKeyValuePair("173", exServer);   
            }
            Config.SLS_Sock.socketMsg(Type, msg, this);
        }

        private List<message> initialClientServer(string client, string server)
        {
            string cmd = SQL.selectSqlCmd(Config.DBName_ClientServer, null, new string[] { "4", "160" }, new string[] { client, server });
            return initial_GetListMsg(cmd, Config.ClientServerKeys, SQL.dbConnect());
        }

        private List<message> initial_GetListMsg(string cmd, string[] keys, SqlConnection sqlConnection)
        {
            List<message> contents = new List<message>();
            try
            {
                using (sqlConnection)
                {
                    using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlConnection))
                    {
                        using (SqlDataReader re = sqlcmd.ExecuteReader())
                        {
                            if (!re.HasRows)
                            {
                                return contents;
                            }
                            while (re.Read())
                            {
                                message msg = new message();
                                string[] DBColumnNames = Config.getValues(keys);
                                for (int i = 0; i < DBColumnNames.Length; i++)
                                {
                                    msg.setKeyValuePair(keys[i], re.GetValue(re.GetOrdinal(DBColumnNames[i])).ToString().Trim(' '));
                                }
                                contents.Add(msg);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
            return contents;
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            if (Type.Equals("PM1"))
            {
                utility.clearContent(this);
            }
            else
            {
                DialogResult result = MessageBox.Show("Delete ?", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);
                if (result.ToString().Equals("OK"))
                {
                    message msg = new message();
                    msg.setKeyValuePair("4", comboBox1.Text);
                    msg.setKeyValuePair("160", textBox1.Text);
                    Config.SLS_Sock.socketMsg("PM3", msg, this);
                }
            }
        }

    }
}
