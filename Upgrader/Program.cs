using Upgrader.Properties;
using System;
using System.Configuration;
using System.Windows.Forms;
using ESBasic.Helpers;

namespace Upgrader
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            try
            {
                string serverIP = ConfigurationManager.AppSettings[Resources.CONFIG_KEY_SERVER_IP];
                int serverPort = int.Parse(ConfigurationManager.AppSettings[Resources.CONFIG_KEY_SERVER_PORT]);               
                string callbackExeName = ConfigurationManager.AppSettings[Resources.CONFIG_KEY_CALLBACK_EXE];
                if (ApplicationHelper.IsAppInstanceExist(callbackExeName.Substring(0, callbackExeName.Length - 4)))
                {
                    MessageBox.Show(Resources.TargetIsRunning);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                MainForm mainForm = new MainForm(serverIP, serverPort, callbackExeName)
                {
                    Text = ConfigurationManager.AppSettings[Resources.CONFIG_KEY_FORM_TITLE],
                };
                if (args[0] != null && args[0].Equals(Resources.UPGRADE_FORM_HIDE))
                {
                    mainForm.WindowState = FormWindowState.Minimized;
                    mainForm.ShowInTaskbar = false;
                }
                Application.Run(mainForm);         
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
