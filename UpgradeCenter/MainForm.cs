using System;
using System.Windows.Forms;
using ESPlus.Rapid;
using ESBasic.Helpers;
using UpgradeCenter.Properties;

namespace UpgradeCenter
{
    public partial class MainForm : Form
    {
        private IRapidServerEngine mRapidServerEngine;

       
        /// <summary>
        /// Constructor,
        /// </summary>
        /// <param name="engine">engine</param>
        public MainForm(IRapidServerEngine rapidServerEngine)
        {
            InitializeComponent();      
            mRapidServerEngine = rapidServerEngine;            
            label_Port.Text = string.Format("{0} [TCP]", mRapidServerEngine.Port);          
        }


        /// <summary>
        /// Link label clicked.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void LinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                FileVersionForm fileVersionForm = new FileVersionForm(Program.mUpdateConfiguration);
                fileVersionForm.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }


        /// <summary>
        /// Notify icon double clicked.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void NotifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Visible = !Visible;
        }


        /// <summary>
        /// Menu item 'Exit' clicked.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void ToolStripMenuItemExit_Click(object sender, EventArgs e)
        {
            Close();
        }


        /// <summary>
        /// MainForm closing.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!WindowsHelper.ShowQuery(Resources.ExitConfirmation))
            {
                Visible = false;
                e.Cancel = true;
                return;
            }
            mRapidServerEngine.Close();
        }

    }
}
