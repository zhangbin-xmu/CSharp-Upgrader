using System;
using UpgradeCore;

namespace UpgradeCenter
{
    internal class UpgradeService : MarshalByRefObject, IUpgradeService
    {
        private UpdateConfiguration mUpdateConfiguration;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="updateConfiguration">update configuration</param>
        public UpgradeService(UpdateConfiguration updateConfiguration)
        {
            mUpdateConfiguration = updateConfiguration;
        }


        /// <summary>
        /// Initialize life time service.
        /// </summary>
        /// <returns></returns>
        public override object InitializeLifetimeService()
        {
            return null;
        }


        /// <summary>
        /// Get latest version of client.
        /// </summary>
        /// <returns>result</returns>
        public int GetLatestVersion()
        {
            return mUpdateConfiguration.ClientVersion;
        }

    }
}
