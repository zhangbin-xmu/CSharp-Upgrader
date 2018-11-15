using System.Collections.Generic;

namespace UpgradeCore
{
    public class FilesInfoContract
    {
        public IList<FileUnit> AllFileInfoList { get; set; } = new List<FileUnit>();
    }
}
