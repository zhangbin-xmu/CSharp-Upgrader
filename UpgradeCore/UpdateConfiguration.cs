using System;
using System.Collections.Generic;
using ESBasic.Serialization;

namespace UpgradeCore
{
    public class UpdateConfiguration : AgileConfiguration
    {
        public static string ConfigurationPath
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory + "UpdateConfiguration.xml";
            }
        }

        public IList<FileUnit> FileList { get; set; } = new List<FileUnit>();
        public int ClientVersion { get; set; } = 0;


        public void Save()
        {
            Save(ConfigurationPath);
        }
    }
}
