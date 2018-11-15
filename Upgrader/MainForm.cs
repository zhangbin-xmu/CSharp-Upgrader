using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using ESBasic;
using Upgrader.Properties;
using ESBasic.Helpers;
using System.Diagnostics;

namespace Upgrader
{
    public partial class MainForm : Form
    {
        private Timer mTimer = new Timer();
        private DateTime mLatestShowTime = DateTime.Now;

        private int mFileCountNeedUpdated = 0; 
        private string mCallbackExeName = string.Empty;
        private string mCallbackExePath = string.Empty;       


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="serverIP">server ip</param>
        /// <param name="serverPort">server port</param>
        /// <param name="callbackExeName">callback exe name</param>
        public MainForm(string serverIP, int serverPort, string callbackExeName)
        {
            InitializeComponent();

            progressBar.Visible = false;
            label_Title.Text = Resources.InitialInformation;
            
            mTimer.Interval = 1000;
            mTimer.Tick += new EventHandler(Timer_Tick);
           
            DirectoryInfo directoryInfo = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);           
            mCallbackExeName = callbackExeName;
            mCallbackExePath = directoryInfo.Parent.FullName + "\\" + mCallbackExeName;

            Updater updater = new Updater();
            updater.Event_ConnectionInterrupted += new CbGeneric(ConnectionInterruptedCallback);
            updater.Event_RelogonCompleted += new CbGeneric(RelogonCompletedCallback);
            updater.Event_UpdateStarted += new CbGeneric(UpdateStartedCallback);
            updater.Event_UpdateDisruptted += new CbGeneric<string>(UpdateDisrupttedCallback);
            updater.Event_UpdateCompleted += new CbGeneric(UpdateCompletedCallback);
            updater.Event_FileCountNeedUpdated += new CbGeneric<int>(FileCountNeedUpdatedCallback);
            updater.Event_FileToBeUpdated += new CbGeneric<int, string, ulong>(FileToBeUpdatedCallback);
            updater.Event_CurrentFileUpdatingProgress += new CbGeneric<ulong, ulong>(CurrentFileUpdatingProgressCallback);
            updater.Start(serverIP, serverPort, callbackExeName);             
        }


        /// <summary>
        /// Connection interrupted event callback.
        /// </summary>
        void ConnectionInterruptedCallback()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new CbGeneric(ConnectionInterruptedCallback));
            }
            else
            {
                label_Reconnect.Visible = true;
                label_Reconnect.Text = Resources.Reconnecting;
            }
        }


        /// <summary>
        /// Relogon completed event callback.
        /// </summary>
        void RelogonCompletedCallback()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new CbGeneric(RelogonCompletedCallback));
            }
            else
            {
                label_Reconnect.Text = Resources.Retransmitting;
            }
        }


        /// <summary>
        /// Update started callback.
        /// </summary>
        void UpdateStartedCallback()
        {
            ShowMessage(2);
        }


        /// <summary>
        /// Update disruptted event callback.
        /// </summary>
        /// <param name="disrupttedType">disruptted type</param>
        void UpdateDisrupttedCallback(string disrupttedType)
        {
            if (InvokeRequired)
            {
                Invoke(new CbGeneric<string>(UpdateDisrupttedCallback), disrupttedType);
            }
            else
            {
                label_Title.Text = Resources.UpdateFailed;
                label_Title.ForeColor = Color.Red;
                Close();
            }
        }


        /// <summary>
        /// Update completed event callback.
        /// </summary>
        private void UpdateCompletedCallback()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new CbGeneric(UpdateCompletedCallback));
            }
            else
            {
                label_Title.Text = Resources.UpdateSuccess;
                mTimer.Start();
            }
        }


        /// <summary>
        /// File count need updated event callback.
        /// </summary>
        /// <param name="fileCount">file count</param>
        void FileCountNeedUpdatedCallback(int fileCount)
        {
            mFileCountNeedUpdated = fileCount;
            if (fileCount == 0)
            {
                ShowMessage(1);
            }
        }


        /// <summary>
        /// File to be updated event callback.
        /// </summary>
        /// <param name="fileIndex">file index</param>
        /// <param name="fileName">file name</param>
        /// <param name="fileSize">file size</param>
        void FileToBeUpdatedCallback(int fileIndex, string fileName, ulong fileSize)
        {
            if (InvokeRequired)
            {
                Invoke(new CbGeneric<int, string, ulong>(FileToBeUpdatedCallback), fileIndex, fileName, fileSize);
            }
            else
            {
                label_Title.Text = string.Format("{0}{1}{2}{3}{4}", Resources.Updateing_string1, mFileCountNeedUpdated, Resources.Updateing_string2, fileIndex + 1, Resources.Updateing_string3);
                label_File.Text = fileName;
            }
        }


        /// <summary>
        /// Current file updating progress event callback.
        /// </summary>
        /// <param name="total">total</param>
        /// <param name="transfered">transfered</param>
        void CurrentFileUpdatingProgressCallback(ulong total, ulong transfered)
        {
            SetProgress(total, transfered);
        }


        /// <summary>
        /// Show messahe.
        /// </summary>
        /// <param name="messageType">message type</param>
        private void ShowMessage(int messageType)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new CbGeneric<int>(ShowMessage), messageType);
            }
            else
            {
                string message = string.Empty;
                switch (messageType)
                {
                    case 1:
                        message = Resources.NoFileNeedUpdate;
                        mTimer.Start();
                        break;
                    case 2:
                        message = string.Format("{0}{1}{2}", Resources.NeedUpdate_string1, mFileCountNeedUpdated, Resources.NeedUpdate_string2);
                        progressBar.Visible = true;
                        break;
                    default:
                        break;
                }
                label_Title.Text = message;
            }
        }       
       

        /// <summary>
        /// Timer tick.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        void Timer_Tick(object sender, EventArgs e)
        {
            TimeUp();
        }


        /// <summary>
        /// Time up.
        /// </summary>
        private void TimeUp()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new CbGeneric(TimeUp));
            }
            else
            {
                mTimer.Stop();
                Close();  
            }
        }       


        /// <summary>
        /// Set progress.
        /// </summary>       
        private void SetProgress(ulong total, ulong transfered)
        {
            if (InvokeRequired)
            {
                object[] args = { total, transfered };
                BeginInvoke(new CbGeneric<ulong, ulong>(SetProgress), args);
            }
            else
            {
                progressBar.Maximum = 1000;
                progressBar.Value = (int)(transfered * 1000 / total);

                TimeSpan span = DateTime.Now - mLatestShowTime;
                if (span.TotalSeconds >= 1)
                {
                    mLatestShowTime = DateTime.Now;
                    label_Progress.Text = string.Format("{0}/{1}", PublicHelper.GetSizeString(transfered), PublicHelper.GetSizeString(total));
                }
            }
        }    


        /// <summary>
        /// MainForm closing.
        /// </summary>
        /// <param name="sender">sender</param>
        /// <param name="e">event</param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                string processName = mCallbackExeName.Substring(0, mCallbackExeName.Length - 4);
                ApplicationHelper.ReleaseAppInstance(processName);
                if (File.Exists(mCallbackExePath))
                {
                    Process process = Process.Start(mCallbackExePath);                    
                }                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        
    }
}
