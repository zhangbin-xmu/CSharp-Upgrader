using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ESBasic;
using ESBasic.Helpers;
using UpgradeCenter.Properties;
using UpgradeCore;

namespace UpgradeCenter
{
    public partial class ModifyFileVersionForm : Form
    {       
        private UpdateConfiguration mUpdateConfiguration;
        public event CbGeneric Event_ModifyFileVersion;
        private bool mIsNewFile = true;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="updateConfiguration">update configuration</param>
        /// <param name="fileUnit">file unit</param>
        public ModifyFileVersionForm(UpdateConfiguration updateConfiguration, FileUnit fileUnit)
        {
            InitializeComponent();

            mUpdateConfiguration = updateConfiguration;
            List<string> files = FileHelper.GetOffspringFiles(AppDomain.CurrentDomain.BaseDirectory + "FileFolder\\");
            files.Sort();
            comboBox.DataSource = files;            
            if (files.Count > 0)
            {
                comboBox.SelectedIndex = 0;
            }

            mIsNewFile = (fileUnit == null);
            if (!mIsNewFile)
            {
                comboBox.Text = fileUnit.FileRelativePath;
                textBox_FileVersion.Text = fileUnit.Version.ToString();
                comboBox.Enabled = false;
            }
        }


        /// <summary>
        /// Button OK clicked.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void Button_OK_Click(object sender, EventArgs e)
        {
            FileUnit fileUnit = new FileUnit
            {
                FileRelativePath = comboBox.Text.Trim()
            };

            try
            {
                fileUnit.Version = float.Parse(textBox_FileVersion.Text.Trim());
            }
            catch
            {
                MessageBox.Show(Resources.FormatError);
                return;
            }

            if (mIsNewFile)
            {
                if (mUpdateConfiguration.FileList.Contains(fileUnit))
                {
                    MessageBox.Show(Resources.RepeatedAddition);
                    return;
                }
                else
                {
                    mUpdateConfiguration.FileList.Add(fileUnit);                   
                    mUpdateConfiguration.Save();
                    Event_ModifyFileVersion();
                    Close();
                }
            }
            else
            {
                foreach (FileUnit file in mUpdateConfiguration.FileList)
                {
                    if (file.FileRelativePath.Equals(comboBox.Text.Trim()))
                    {
                        file.Version = float.Parse(textBox_FileVersion.Text.Trim());                       
                        break;
                    }
                }                           
                mUpdateConfiguration.Save();              
                Event_ModifyFileVersion();
                Close();
            }
        }

    }
}
