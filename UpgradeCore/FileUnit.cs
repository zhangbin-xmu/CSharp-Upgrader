using System;

namespace UpgradeCore
{
    public class FileUnit :IComparable<FileUnit>
    {
        public string FileRelativePath { get; set; }
        public float Version { get; set; }
        public int FileSize { get; set; } = 0;
        public DateTime LatestUpdateTime { get; set; } = DateTime.Now;
        

        /// <summary>
        /// Constructor.
        /// </summary>
        public FileUnit() {}


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="file">file</param>
        /// <param name="version">version</param>
        /// <param name="size">size</param>
        /// <param name="updateTime">update time</param>
        public FileUnit(string file, float version, int size, DateTime updateTime)
        {
            FileRelativePath = file;
            Version = version;
            FileSize = size;
            LatestUpdateTime = updateTime;
        }


        /// <summary>
        /// Compare file.
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>result</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is FileUnit fileUnit))
            {
                return false;
            }
            return FileRelativePath.Equals(fileUnit.FileRelativePath);
        }


        /// <summary>
        /// Compare file.
        /// </summary>
        /// <param name="fileUnit">file unit</param>
        /// <returns>result</returns>
        public int CompareTo(FileUnit fileUnit)
        {
            return FileRelativePath.CompareTo(fileUnit.FileRelativePath);
        }


        /// <summary>
        /// Get hash code of file,
        /// </summary>
        /// <returns>result</returns>
        public override int GetHashCode()
        {
            return FileRelativePath.GetHashCode();
        }
    }
}
