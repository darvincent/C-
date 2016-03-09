using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Net;
using System.Data.SqlClient;


namespace SLS
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            Server S = new Server();
            S.Run(Server.port);
            this.checkBox1.Checked = true;      
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            IoServer.isSLSRunning = false;
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Server.configFile.Write("CASE", "seq", Server.caseSeqNo.ToString());
            Server.configFile.Write("ProductHint", "seq", Server.productHintSeqNo.ToString());
        }
           
    }
}
