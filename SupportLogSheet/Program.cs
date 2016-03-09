using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Text;
using System.Threading;
using System.Drawing;
namespace SupportLogSheet
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 

        [STAThread]
        static void Main()
        {
            bool OneUI;
            Mutex mutex_SLS = new Mutex(false, "OneUI", out OneUI);
            if (!OneUI)
            {
                MessageBox.Show("Only one instance can run");
            }
            else
            {
                try
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);
                    if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "configuration.ini"))
                    {
                        MessageBox.Show("No config file, application exits.");
                        Application.Exit();
                    }
                    else
                    {
                        Config.Common_INI = new INI_OP(AppDomain.CurrentDomain.BaseDirectory + "configuration.ini");
                        Config.Customization_INI = new INI_OP(AppDomain.CurrentDomain.BaseDirectory + "customization.ini");
                        Config.Font_Content = Config.Customization_INI.readValue("Font", "Content") == "" ? SystemFonts.DefaultFont : (Font)new FontConverter().ConvertFrom(Config.Customization_INI.readValue("Font", "Content"));
                        Config.SLS_Sock = new SLSocket();

                        Application.Run(new LoginForm());
                        if (LoginForm.IsLogin)
                        {
                            if (LoginForm.IsSupport)
                            {
                                MainForm mf = new MainForm();
                                Application.Run(mf);
                            }
                            else
                            {
                                ProductMaster PM = new ProductMaster(1);
                                Application.Run(PM);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

    }
}
