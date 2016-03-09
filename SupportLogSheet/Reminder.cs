// 设置提醒任务
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
    public partial class Reminder : Form
    {
        private ListViewItem lvi;
        public Reminder(ListViewItem item)
        {
            InitializeComponent();
            this.lvi = (ListViewItem)item.Clone();
            Combo_OP.initialComboBox(Combo_Hour, new string[] { "1", "2", "3", "4", "5", "6", "7", "8" });
            Combo_OP.initialComboBox(Combo_Minute, new string[] { "10", "20", "30", "40", "50" });
            this.Text = lvi.SubItems[1].Text;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                Msg_OP msg_Op = new Msg_OP("N2", lvi, Config.UI_CaseKeys);
                msg_Op.setValueToPairs("180", getReminderTime());
                Config.SLS_Sock.inMsgQueue_add(msg_Op);
                this.Dispose();
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        public string getReminderTime()
        {
            if (Combo_Minute.Items.Contains(Combo_Minute.Text.Trim()))  // is a nap?
            {
                //return DateTime.Now.AddSeconds(5).ToString("yyyy-MM-dd HH:mm:ss");
                return DateTime.Now.AddMinutes(Int32.Parse(Combo_Minute.Text.Trim())).ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (Combo_Hour.Items.Contains(Combo_Hour.Text.Trim())) // is set hours?
            {
                return DateTime.Now.AddHours(Convert.ToInt32(Combo_Hour.Text.Trim())).ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (dateTimePicker1.Value.Date > DateTime.Now.Date)
            {
                DateTime temp = DateTime.Today;
                TimeSpan t = dateTimePicker1.Value - DateTime.Now;
                temp = temp.AddDays(t.TotalDays);
                temp = temp.AddHours(9);
                return temp.ToString("yyyy-MM-dd HH:mm:ss");
            }
            return DateTime.Now.AddMinutes(20).ToString("yyyy-MM-dd HH:mm:ss");
        }

    }
}
