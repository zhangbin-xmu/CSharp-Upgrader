using System;
using System.Collections.Generic;
using System.Windows.Forms;
using UpgradeCore;
using ESBasic.Helpers;
using System.IO;
using UpgradeCenter.Properties;
using ESBasic;

namespace UpgradeCenter
{
    public partial class FileVersionForm : Form
    {
        private UpdateConfiguration mUpdateConfiguration;
        private bool mConfigurationChanged = false;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="updateConfiguration">update configuration</param>
        public FileVersionForm(UpdateConfiguration updateConfiguration)
        {
            InitializeComponent();
            mUpdateConfiguration = updateConfiguration;     
            GetUpdateInfo();
        }


        /// <summary>
        /// Get update information from update configuration.
        /// </summary>
        private void GetUpdateInfo()
        {
            if (mUpdateConfiguration.FileList.Count == 0)
            {
                List<string> files = FileHelper.GetOffspringFiles(AppDomain.CurrentDomain.BaseDirectory + "FileFolder\\");
                foreach (string fileRelativePath in files)
                {
                    FileInfo fileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "FileFolder\\" + fileRelativePath);
                    mUpdateConfiguration.FileList.Add(new FileUnit(fileRelativePath, 1 ,(int)fileInfo.Length, fileInfo.LastWriteTime));
                }                
                mUpdateConfiguration.Save();
                mConfigurationChanged = true;
            }

            ((List<FileUnit>)(mUpdateConfiguration.FileList)).Sort();
            dataGridView.DataSource = null;
            dataGridView.DataSource = mUpdateConfiguration.FileList;    
            label_LatestVersion.Text = Resources.LatestVersion + mUpdateConfiguration.ClientVersion;
            label_LatestUpdateTime.Text = Resources.LatestUpdateTime + DateTime.Now.ToString();
        }


        /// <summary>
        /// Modify file version callback.
        /// </summary>
        void ModifyFileVersionCallback()
        {       
            mConfigurationChanged = true;
            GetUpdateInfo();
        }


        /// <summary>
        /// Button add clicked.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void Button_Add_Click(object sender, EventArgs e)
        {
            ModifyFileVersionForm modifyFileVersionForm = new ModifyFileVersionForm(mUpdateConfiguration, null);
            modifyFileVersionForm.Event_ModifyFileVersion += new CbGeneric(ModifyFileVersionCallback);
            modifyFileVersionForm.Show();
        }


        /// <summary>
        /// Button delete clicked.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void Button_Delete_Click(object sender, EventArgs e)
        {
            if (dataGridView.SelectedRows.Count == 0)
            {
                MessageBox.Show(Resources.NoFileSelected);
                return;
            }

            FileUnit fileUnit = dataGridView.SelectedRows[0].DataBoundItem as FileUnit;
            if (DialogResult.OK == MessageBox.Show(string.Format("{0}{1}？", Resources.DeleteConfirmation, fileUnit.FileRelativePath), Resources.MessageTip, MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
            {
                mUpdateConfiguration.FileList.Remove(fileUnit); 
                mUpdateConfiguration.Save();               
                mConfigurationChanged = true;
                GetUpdateInfo();
            }
        }


        /// <summary>
        /// Button scan clicked.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void Button_Scan_Click(object sender, EventArgs e)
        {
            int newFileCount = 0;
            int modifiedFileCount = 0;
            int deletedFileCount = 0;

            List<string> files = FileHelper.GetOffspringFiles(AppDomain.CurrentDomain.BaseDirectory + "FileFolder\\");
            foreach (string fileRelativePath in files)
            {
                FileInfo fileInfo = new FileInfo(AppDomain.CurrentDomain.BaseDirectory + "FileFolder\\" + fileRelativePath);
                FileUnit fileUnit = GetFileUnit(fileRelativePath);
                if (fileUnit == null)
                {
                    fileUnit = new FileUnit(fileRelativePath, 1, (int)fileInfo.Length, fileInfo.LastWriteTime);
                    mUpdateConfiguration.FileList.Add(fileUnit);
                    newFileCount++;
                }
                else if (fileUnit.FileSize != fileInfo.Length || fileUnit.LatestUpdateTime.ToString() != fileInfo.LastWriteTime.ToString())
                {
                    fileUnit.Version += 1;
                    fileUnit.FileSize = (int)fileInfo.Length;
                    fileUnit.LatestUpdateTime = fileInfo.LastWriteTime;
                    modifiedFileCount++;
                }
            }

            foreach (FileUnit fileUnit in mUpdateConfiguration.FileList)
            {
                bool found = false;
                foreach (string fileRelativePath in files)
                {
                    if (fileRelativePath.Equals(fileUnit.FileRelativePath))
                    {
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    deletedFileCount++;
                    mUpdateConfiguration.FileList.Remove(fileUnit);
                }
            }

            if (newFileCount > 0 || modifiedFileCount > 0 || deletedFileCount > 0)
            {
                MessageBox.Show(string.Format("{0}{1}，{2}{3}，{4}{5}", Resources.Modify, modifiedFileCount, Resources.New, newFileCount, Resources.Delete, deletedFileCount));
                mUpdateConfiguration.Save();
                mConfigurationChanged = true;
                GetUpdateInfo();
            }
            else
            {
                MessageBox.Show(Resources.NothingChanged);
            }
        }


        /// <summary>
        /// Find file from update configuration.
        /// </summary>
        /// <param name="fileRelativePath">file relative path</param>
        /// <returns>result</returns>
        private FileUnit GetFileUnit(string fileRelativePath)
        {
            foreach (FileUnit fileUnit in mUpdateConfiguration.FileList)
            {
                if (fileUnit.FileRelativePath.Equals(fileRelativePath)) return fileUnit;
            }
            return null;
        }


        /// <summary>
        /// Data grid view double clicked.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void DataGridView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
        
            DataGridView.HitTestInfo hitTestInfo = dataGridView.HitTest(e.X, e.Y);
            if (hitTestInfo.RowIndex >= 0)
            {
                FileUnit fileUnit = dataGridView.SelectedRows[0].DataBoundItem as FileUnit;
                ModifyFileVersionForm modifyFileVersionForm = new ModifyFileVersionForm(mUpdateConfiguration, fileUnit);
                modifyFileVersionForm.Event_ModifyFileVersion += new CbGeneric(ModifyFileVersionCallback);
                modifyFileVersionForm.Show();
            }
        }


        /// <summary>
        /// FileVersionForm closing.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void FileVersionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mConfigurationChanged)
            {
                mUpdateConfiguration.ClientVersion++;
                mUpdateConfiguration.Save();
                MessageBox.Show(string.Format("{0}{1}", Resources.UpdatedVersion, mUpdateConfiguration.ClientVersion));
            }
        }

    }
}
