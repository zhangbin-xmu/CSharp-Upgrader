using System;
using System.Windows.Forms;
using System.IO;
using System.Configuration;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Remoting.Channels;
using ESPlus;
using ESPlus.Rapid;
using ESBasic.Serialization;
using UpgradeCore;
using UpgradeCenter.Properties;

namespace UpgradeCenter
{
    static class Program
    {
        private static IRapidServerEngine mRapidServerEngine = RapidEngineFactory.CreateServerEngine();
        internal static UpdateConfiguration mUpdateConfiguration = null;


        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            try
            {
                // ESFramework authorized user.             
                GlobalUtil.SetAuthorizedUser("FreeUser", "");

                // Get update configuration.                           
                if (File.Exists(UpdateConfiguration.ConfigurationPath))
                {
                    mUpdateConfiguration = (UpdateConfiguration)AgileConfiguration.Load(UpdateConfiguration.ConfigurationPath);
                }
                else
                {
                    mUpdateConfiguration = new UpdateConfiguration();
                    mUpdateConfiguration.Save();
                }

                // Initialize rapid engine of server.
                int port = int.Parse(ConfigurationManager.AppSettings[Resources.CONFIG_KEY_PORT]);
                CustomizeHandler customizeHandler = new CustomizeHandler();                
                mRapidServerEngine.WriteTimeoutInSecs = -1;
                mRapidServerEngine.Initialize(port, customizeHandler);                
                customizeHandler.Initialize(mRapidServerEngine.FileController, mUpdateConfiguration);

                // Remote service.
                if (bool.Parse(ConfigurationManager.AppSettings[Resources.CONFIG_KEY_REMOTE]))
                {
                    ChannelServices.RegisterChannel(new TcpChannel(port + 2), false);
                    UpgradeService upgradeService = new UpgradeService(mUpdateConfiguration);
                    RemotingServices.Marshal(upgradeService, "UpgradeCenter"); 
                }

                // Create main form.
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                MainForm mainForm = new MainForm(mRapidServerEngine)
                {
                    Text = ConfigurationManager.AppSettings[Resources.CONFIG_KEY_FORM_TITLE]
                };
                Application.Run(mainForm);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }      
    }
}
