// 对历史case 图形化显示 统计
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Drawing.Drawing2D;
using System.Threading;
using System.Web;
using System.Web.UI;


namespace SupportLogSheet
{
    public partial class Statistic_Graphic : Form
    {
        private Dictionary<int, int> caseCount = new Dictionary<int, int>();
        private string dateToday = DateTime.Now.ToLongTimeString();
        private SqlConnection sqlconnection = SQL.dbConnect();
        private int startYear = 2012;
        public List<int> years = new List<int>();
        private int[] monthDays = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private int[] monthDays_LeapYear = { 31, 29, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
        private string category;
        private string subCategory;
        private int year;
        private int month;
        private string inteval = "";
        private delegate void  Del_drawImage(Dictionary<int,int> caseCount) ;
        private Dictionary<string, string> CaseProperty;
        private List<string> ClientList;
        private List<string> UserList;
        private Dictionary<string, string> ProductCate_Pair;
        public Statistic_Graphic(Dictionary<string, string> CaseProperty, List<string> ClientList, List<string> UserList, Dictionary<string, string> ProductCate_Pair)
        {
            InitializeComponent();
            this.CaseProperty = CaseProperty;
            Combo_OP.initialComboBox(Combo_Cate, Config.CategoryType1);
            Combo_OP.initialComboBox(Comb_Interval, new string[] { "Yearly", "Monthly", "Daily" });
            this.ClientList = ClientList;
            this.UserList = UserList;
            this.ProductCate_Pair = ProductCate_Pair;
            int thisYear = DateTime.Now.Year;
            for (int i = startYear; i <= thisYear; i++)
            {
                Combo_Year.Items.Add(i);
                years.Add(i);
            }
            for (int i = 1; i <= 12; i++)
            {
                Combo_Month.Items.Add(i);
            }
        }

        public string addZeroToFront(int i)
        {
            if (i < 10)
            {
                return "0" + i;
            }
            return i.ToString();
        }

        public int[] getFullDivision(int number, int division)
        {
            int[] CountAndShifting = new int[2];
            int reminder = number % division;
            if (number % division == 0)
            {
                CountAndShifting[0] = number / division;
            }
            else
            {
                CountAndShifting[0] = (number - number % division) / division + 1;
            }
            CountAndShifting[1] = division;
            return CountAndShifting;
        }

        public int[] getShiftingNo(int maxNo)
        {
            int[] NoAndShifting = new int[2];
            if (maxNo < 5)
            {
                return getFullDivision(maxNo, 1);
            }
            else if (maxNo <= 10)
            {
                return getFullDivision(maxNo, 2);
            }
            else if(maxNo<= 40)
            {
                return getFullDivision(maxNo, 5);
            }
            else if (maxNo <= 100)
            {
                return getFullDivision(maxNo, 10);
            }
            else if (maxNo <= 400)
            {
                return getFullDivision(maxNo, 50);
            }
            else if (maxNo <= 1000)
            {
                return getFullDivision(maxNo, 100);
            }
            else 
            {
                return getFullDivision(maxNo, 500);
            }
        }

        public void getData(object type )
        {
            try
            {
                inteval = type.ToString();
                if (inteval.Equals(""))
                {
                    return;
                }
                string cmd_initial = new StringBuilder("select count(*) from SupportLogSheet where ").Append(category).Append("='").Append(subCategory).Append("'").ToString();
                caseCount.Clear();
                switch (inteval)
                {
                    case "Yearly":
                        {
                            for (int i = 0; i < years.Count; i++)
                            {
                                string cmd = new StringBuilder(cmd_initial).Append(" and CreateTime>='").Append(years[i]).Append("0101' and CreateTime<'").Append((years[i] + 1)).Append("0101'").ToString();
                                using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlconnection))
                                {
                                    caseCount.Add(years[i], Convert.ToInt32(sqlcmd.ExecuteScalar()));
                                }
                            }
                            break;
                        }
                    case "Monthly":
                        {
                            int thisYear = DateTime.Now.Year;
                            int months = 12;
                            if (thisYear == year)
                            {
                                months = DateTime.Now.Month;
                            }
                            for (int i = 1; i < months; i++)
                            {
                                string cmd = new StringBuilder(cmd_initial).Append(" and CreateTime>='").Append(year).Append(addZeroToFront(i)).Append("01' and CreateTime<'").Append(year).Append(addZeroToFront(i + 1)).Append("01'").ToString();
                                using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlconnection))
                                {
                                    caseCount.Add(i, Convert.ToInt32(sqlcmd.ExecuteScalar()));
                                }
                            }
                            if (months == 12)
                            {
                                string cmd1 = new StringBuilder(cmd_initial).Append(" and CreateTime>='").Append(year).Append("1201' and CreateTime<'").Append((year + 1)).Append("0101'").ToString();
                                using (SqlCommand sqlcmd = new SqlCommand(cmd1, sqlconnection))
                                {
                                    caseCount.Add(12, Convert.ToInt32(sqlcmd.ExecuteScalar()));
                                }
                            }
                            else
                            {
                                string cmd = new StringBuilder(cmd_initial).Append(" and CreateTime>='").Append(year).Append(addZeroToFront(months)).Append("01' and CreateTime<'").Append(year).Append(addZeroToFront(months + 1)).Append("01'").ToString();
                                using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlconnection))
                                {
                                    caseCount.Add(months, Convert.ToInt32(sqlcmd.ExecuteScalar()));
                                }
                            }
                            break;
                        }
                    case "Daily":
                        {
                            int thisYear = DateTime.Now.Year;
                            int thisMonth = DateTime.Now.Month;
                            int days = 0;
                            if (thisYear == year)
                            {
                                if (thisMonth == month)
                                {
                                    days = DateTime.Now.Day;
                                }
                                else
                                {
                                    if (DateTime.IsLeapYear(year))
                                    {
                                        days = monthDays_LeapYear[month - 1];
                                    }
                                    else
                                    {
                                        days = monthDays[month - 1];
                                    }
                                }
                            }
                            else
                            {
                                if (DateTime.IsLeapYear(year))
                                {
                                    days = monthDays_LeapYear[month - 1];
                                }
                                else
                                {
                                    days = monthDays[month - 1];
                                }
                            }
                            for (int i = 1; i <= days; i++)
                            {
                                string cmd = new StringBuilder(cmd_initial).Append(" and CreateTime>='").Append(year).Append(addZeroToFront(month)).Append(addZeroToFront(i)).Append(" 00:00:00' and CreateTime<='").Append(year).Append(addZeroToFront(month)).Append(addZeroToFront(i)).Append(" 23:59:59'").ToString();
                                using (SqlCommand sqlcmd = new SqlCommand(cmd, sqlconnection))
                                {
                                    caseCount.Add(i, Convert.ToInt32(sqlcmd.ExecuteScalar()));
                                }
                            }
                            break;
                        }
                    default:
                        break;
                }
                this.Invoke(new Del_drawImage(CreateImage), new object[] { caseCount });
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                category = Combo_Cate.Text.Trim(' ');
                subCategory = Combo_SubCate.Text.Trim(' ');
                if (category!=""&& subCategory!="")
                {
                    if (!string.IsNullOrEmpty(Combo_Year.Text))
                    {
                        year = Int32.Parse(Combo_Year.Text.Trim(' '));
                    }
                    else
                    {
                        year = DateTime.Now.Year;
                    }
                    if(!string.IsNullOrEmpty(Combo_Month.Text))
                    {
                        month = Int32.Parse(Combo_Month.Text.Trim(' '));
                    }
                    else
                    {
                        month = DateTime.Now.Month;
                    }
                    switch (Comb_Interval.Text.Trim(' '))
                    {
                        case "Yearly":
                            {
                                inteval="Yearly";
                                break;
                            }
                        case "Monthly":
                            {
                                inteval="Monthly";
                                break;
                            }
                        case "Daily":
                            {
                                inteval="Daily";
                                break;
                            }
                        default:
                            {
                                inteval = "Yearly";
                                break;
                            }
                    }
                    try
                    {
                        label6.Text = "Loading...";
                        ThreadPool.QueueUserWorkItem(new WaitCallback(getData), inteval);
                    }
                    catch (Exception ex)
                    {
                        Config.logWriter.writeErrorLog(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void CreateImage(Dictionary<int, int> cases)
        {           
            int height = 480, width = 700;
            Bitmap image = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(image);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            try
            {
                Pen mypen = new Pen(Color.DodgerBlue, 1);
                Pen mypen2 = new Pen(Color.SeaGreen, 1);
                Font font = new System.Drawing.Font("MS UI Gothic", 10, FontStyle.Regular);

                g.DrawString("No", font, Brushes.DodgerBlue, 2, 35);
                g.DrawString("Time", font, Brushes.DodgerBlue, 670, 450);

                int x_shifting = (int)(640 / (cases.Count + 1));
                int maxNo = cases.Values.Max();
                int count = getShiftingNo(maxNo)[0];
                int y_shifting_No = getShiftingNo(maxNo)[1];
                int y_shifting = (int)(400 / (count + 1));
                int[] x_args = cases.Keys.ToArray();
                g.DrawLine(mypen, 30, 40, 30, 440);
                g.DrawLine(mypen, 30, 440, 670, 440);
                //x轴
                int x = 30 + x_shifting;
                for (int i = 0; i < x_args.Length; i++)
                {
                    g.FillEllipse(Brushes.DarkBlue, new Rectangle(x-2, 438, 4, 4));
                    g.DrawString(x_args[i].ToString(), font, Brushes.DodgerBlue, x, 450);
                    x += x_shifting;
                }

                //y轴
                int y = 440 - y_shifting;
                for (int i = 0; i < count; i++)
                {
                    g.FillEllipse(Brushes.DarkBlue, new Rectangle(28, y - 2, 4, 4));
                    g.DrawString((y_shifting_No * (i + 1)).ToString(), font, Brushes.DodgerBlue, 1, y-6);
                    y -= y_shifting;
                }

                Point[] points = new Point[cases.Count];
                for (int i = 0; i < cases.Count; i++)
                {
                    points[i].X = x_shifting * (i + 1) + 30;
                    points[i].Y = 440 - (cases[x_args[i]] * y_shifting / y_shifting_No);
                    g.FillEllipse(Brushes.SeaGreen, new Rectangle(points[i].X-2, points[i].Y-2,4,4));
                    g.DrawString(cases[x_args[i]].ToString(), font, Brushes.DodgerBlue, points[i].X + 7, points[i].Y - 15);
                }
                g.DrawLines(mypen2, points);

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                pictureBox1.Image = image;
                switch(inteval)
                {
                    case "Yearly":
                        {
                            label6.Text=subCategory+", Yearly";
                            break;
                        }
                    case "Monthly":
                        {
                            label6.Text = new StringBuilder(subCategory).Append(", Monthly, ").Append(year).ToString();
                            break;
                        }
                    case "Daily":
                        {
                            label6.Text = new StringBuilder(subCategory).Append(", Daily, ").Append(year).Append(addZeroToFront(month)).ToString();
                            break;
                        }
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Config.logWriter.writeErrorLog(ex);
            }
        }

        private void comboBox1_TextChanged(object sender, EventArgs e)
        {
            switch (Combo_Cate.Text.Trim(' '))
            {
                case "Client":
                    {
                        Combo_OP.initialComboBox(Combo_SubCate, ClientList);
                        break;
                    }
                case "Incharge":
                    {
                        Combo_OP.initialComboBox(Combo_SubCate, UserList);
                        break;
                    }
                case "Product":
                    {
                        Combo_OP.initialComboBox(Combo_SubCate, ProductCate_Pair.Keys.ToArray());
                        break;
                    }
                case "Priority":
                    {
                         Combo_OP.initialComboBox(Combo_SubCate, CaseProperty["Priority"].ToString(), ",");
                        break;
                    }
                case "Issue":
                    {
                         Combo_OP.initialComboBox(Combo_SubCate, CaseProperty["Issue"].ToString(), ",");
                        break;
                    }
                case "Type":
                    {
                         Combo_OP.initialComboBox(Combo_SubCate, CaseProperty["Type"].ToString(), ",");
                        break;
                    }
                case "Status":
                    {
                         Combo_OP.initialComboBox(Combo_SubCate, CaseProperty["Status"].ToString(), ",");
                        break;
                    }
                default:
                    break;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            foreach (System.Windows.Forms.Control c in this.Controls)
            {
                if (c is ComboBox)
                {
                    c.Text = "";
                }
            }
            pictureBox1.Image = null;
            label6.Text="Graph";
        }

        private void comboBox5_TextChanged(object sender, EventArgs e)
        {
            Combo_Month.Enabled = true;
            Combo_Year.Enabled = true;
            string Inteval = Comb_Interval.Text.Trim(' ');
            switch (Inteval)
            {
                case "Yearly":
                    {
                        Combo_Month.Enabled = false;
                        Combo_Year.Enabled = false;
                        break;
                    }
                case "Monthly":
                    {
                        Combo_Month.Enabled = false;
                        break;
                    }
                case "Daily":
                    {
                        break;
                    }
                default:
                    break;
            }
        }
    }
}
