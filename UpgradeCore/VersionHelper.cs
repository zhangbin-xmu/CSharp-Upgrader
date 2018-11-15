using ESBasic.Serialization;
using System;

namespace UpgradeCore
{
    /// <summary>
    /// 给应用的客户端使用，用于获取升级的版本信息。
    /// </summary>
    public static class VersionHelper
    {
        /// <summary>
        /// Get current version of client.
        /// </summary>        
        public static int GetCurrentVersion()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + "Upgrade\\UpdateConfiguration.xml";
            UpdateConfiguration updateConfiguration = (UpdateConfiguration)AgileConfiguration.Load(path);
            return updateConfiguration.ClientVersion;
        }


        /// <summary>
        /// Get latest version of client.
        /// </summary>
        /// <param name="serverIP">server ip</param>
        /// <param name="serverPort">server port</param>        
        public static int GetLatestVersion(string serverIP, int serverPort)
        {
            IUpgradeService upgradeService = (IUpgradeService)Activator.GetObject(typeof(IUpgradeService), 
                string.Format("tcp://{0}:{1}/UpgradeCenter", serverIP, serverPort + 2));
            return upgradeService.GetLatestVersion();        
        }


        /// <summary>
        /// Check new version.
        /// </summary>
        /// <param name="serverIP">server ip</param>
        /// <param name="serverPort">server port</param>        
        public static bool HasNewVersion(string serverIP, int serverPort)
        {
            return GetLatestVersion(serverIP, serverPort) > GetCurrentVersion();
        }
    }
}
