using System;
using System.Collections.Generic;
using ESBasic;
using UpgradeCore;
using ESPlus.Rapid;
using ESPlus.Serialization;
using ESPlus.FileTransceiver;
using System.IO;
using ESBasic.Loggers;
using ESPlus.Application.Basic;
using Upgrader.Properties;
using ESBasic.Serialization;
using ESBasic.Helpers;
using System.Configuration;
using System.Diagnostics;

namespace Upgrader
{
    public class Updater
    {
        private readonly FileAgileLogger mLogger = new FileAgileLogger(AppDomain.CurrentDomain.BaseDirectory + "UpgradeLog.txt");

        private UpdateConfiguration mUpdateConfiguration = new UpdateConfiguration();
        private IRapidPassiveEngine mRapidPassiveEngine;

        private int mFileCountNeedUpdated = 0;
        private int mFileCountAlreadyUpdated = 0;
        private IList<FileUnit> mFileListNeedRemoved;
        private IList<string> mFileRelativePathListNeedDownloaded;

        public event CbGeneric Event_ConnectionInterrupted;
        public event CbGeneric Event_RelogonCompleted;

        public event CbGeneric Event_UpdateStarted;
        public event CbGeneric<string> Event_UpdateDisruptted;
        public event CbGeneric Event_UpdateCompleted;

        public event CbGeneric<int> Event_FileCountNeedUpdated;
        public event CbGeneric<int, string, ulong> Event_FileToBeUpdated;
        public event CbGeneric<ulong, ulong> Event_CurrentFileUpdatingProgress;


        /// <summary>
        /// Constructor.
        /// </summary>
        public Updater()
        {
            mRapidPassiveEngine = RapidEngineFactory.CreatePassiveEngine();
            mRapidPassiveEngine.AutoReconnect = true;
            mRapidPassiveEngine.ConnectionInterrupted += new CbGeneric(RapidPassiveEngine_ConnectionInterrupted);
            mRapidPassiveEngine.RelogonCompleted += new CbGeneric<LogonResponse>(RapidPassiveEngine_RelogonCompleted);

            Event_UpdateStarted += new CbGeneric(Updater_UpdateStarted);
            Event_UpdateDisruptted += new CbGeneric<string>(Updater_UpdateDisruptted);
            Event_UpdateCompleted += new CbGeneric(Updater_UpdateCompleted);
        }


        /// <summary>
        /// Connection interrupted.
        /// </summary>
        void RapidPassiveEngine_ConnectionInterrupted()
        {
            Event_ConnectionInterrupted?.Invoke();
            mLogger.LogWithTime(Resources.Reconnecting);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        void RapidPassiveEngine_RelogonCompleted(LogonResponse response)
        {
            if (response.LogonResult == LogonResult.Succeed)
            {               
                DownloadNextFile();           
                Event_RelogonCompleted?.Invoke();
                mLogger.LogWithTime(Resources.Retransmitting);
                return;
            }

            mLogger.LogWithTime(Resources.ReconnectionFailed);
            Event_UpdateDisruptted?.Invoke(FileTransDisrupttedType.SelfOffline.ToString());
        }


        /// <summary>
        /// Update started.
        /// </summary>
        void Updater_UpdateStarted()
        {
            mLogger.LogWithTime(string.Format("{0}{1}{2}", Resources.NeedUpdate_string1, mFileCountNeedUpdated, Resources.NeedUpdate_string2));
        }


        /// <summary>
        /// Update disruptted.
        /// </summary>
        /// <param name="fileTransDisrupttedType">file transmission disruptted type</param>
        void Updater_UpdateDisruptted(string fileTransDisrupttedType)
        {
            mLogger.LogWithTime(string.Format("{0}[{1}]", Resources.UpdateFailed, fileTransDisrupttedType));
        }


        /// <summary>
        /// Update completed.
        /// </summary>
        void Updater_UpdateCompleted()
        {
            mLogger.LogWithTime(Resources.UpdateSuccess);
        }


        /// <summary>
        /// Start.
        /// </summary>
        /// <param name="serverIP">server ip</param>
        /// <param name="serverPort">server port</param>
        /// <param name="callerExe">callerExe</param>
        public void Start(string serverIP, int serverPort, string callerExe)
        {
            try
            {
                Random random = new Random();
                for (int i = 0; i < 100; i++)
                {
                    string userID = random.Next(1000000).ToString("00000");
                    string logonPassword = string.Empty;
                    LogonResponse logonResponse = mRapidPassiveEngine.Initialize(userID, logonPassword, serverIP, serverPort, null);
                    if (logonResponse.LogonResult == LogonResult.Succeed) break;
                }

                if (!File.Exists(UpdateConfiguration.ConfigurationPath))
                {
                    mUpdateConfiguration.Save();
                }
                else
                {
                    mUpdateConfiguration = (UpdateConfiguration)AgileConfiguration.Load(UpdateConfiguration.ConfigurationPath);
                }

                GetUpdateInfo(out mFileRelativePathListNeedDownloaded, out mFileListNeedRemoved);
                mFileCountNeedUpdated = mFileRelativePathListNeedDownloaded.Count;
                Event_FileCountNeedUpdated?.Invoke(mFileCountNeedUpdated);
                
                if (mFileCountNeedUpdated == 0 && mFileListNeedRemoved.Count == 0) return;
                Event_UpdateStarted?.Invoke();

                Process[] processes = Process.GetProcessesByName(callerExe.Substring(0, callerExe.Length - 4));
                foreach (Process process in processes) process.Kill();

                CbGeneric cbGeneric = new CbGeneric(UdpateThread);
                cbGeneric.BeginInvoke(null, null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                Event_UpdateDisruptted?.Invoke(Resources.ConnectionFailed);
            }
        }


        /// <summary>
        /// Update thread.
        /// </summary>
        private void UdpateThread()
        {
            try
            {
                mRapidPassiveEngine.FileOutter.FileRequestReceived += new ESPlus.Application.FileTransfering.CbFileRequestReceived(FileOutter_FileRequestReceived);
                mRapidPassiveEngine.FileOutter.FileReceivingEvents.FileTransStarted += new CbGeneric<TransferingProject>(FileReceivingEvents_FileTransStarted);
                mRapidPassiveEngine.FileOutter.FileReceivingEvents.FileTransCompleted += new CbGeneric<TransferingProject>(FileReceivingEvents_FileTransCompleted);
                mRapidPassiveEngine.FileOutter.FileReceivingEvents.FileTransDisruptted += new CbGeneric<TransferingProject, FileTransDisrupttedType>(FileReceivingEvents_FileTransDisruptted);
                mRapidPassiveEngine.FileOutter.FileReceivingEvents.FileTransProgress += new CbFileSendedProgress(FileReceivingEvents_FileTransProgress);

                if (mFileRelativePathListNeedDownloaded.Count <= 0)
                {
                    if (mFileListNeedRemoved.Count > 0)
                    {
                        foreach (FileUnit file in mFileListNeedRemoved)
                        {
                            FileHelper.DeleteFile(new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName 
                                + "\\" + file.FileRelativePath);
                        }
                        mUpdateConfiguration.Save();
                        Event_UpdateCompleted?.Invoke();
                    }
                    return;
                }

                DownloadFileContract downLoadFileContract = new DownloadFileContract
                {
                    FileName = mFileRelativePathListNeedDownloaded[0]
                };
                mRapidPassiveEngine.CustomizeOutter.Send(InformationTypes.DownloadFiles, CompactPropertySerializer.Default.Serialize(downLoadFileContract));
            }
            catch (Exception e)
            {
                mLogger.Log(e, "Upgrade.Updater.UdpateThread", ErrorLevel.High);
                Event_UpdateDisruptted?.Invoke(FileTransDisrupttedType.InnerError.ToString());
            }
        }


        /// <summary>
        /// Get update information.
        /// </summary>       
        private void GetUpdateInfo(out IList<string> fileRelativePathListNeedDownloaded, out IList<FileUnit> fileListNeedRemoved)
        {
            byte[] latestUpdateTime = mRapidPassiveEngine.CustomizeOutter.Query(InformationTypes.GetLatestUpdateTime, null);
            LatestUpdateTimeContract latestUpdateTimeContract = CompactPropertySerializer.Default.Deserialize<LatestUpdateTimeContract>(latestUpdateTime, 0);

            fileRelativePathListNeedDownloaded = new List<string>();
            fileListNeedRemoved = new List<FileUnit>();

            if (mUpdateConfiguration.ClientVersion != latestUpdateTimeContract.ClientVersion)
            {
                byte[] fileInfoBytes = mRapidPassiveEngine.CustomizeOutter.Query(InformationTypes.GetAllFilesInfo, null);
                FilesInfoContract filesInfoContract = CompactPropertySerializer.Default.Deserialize<FilesInfoContract>(fileInfoBytes, 0);

                foreach (FileUnit file in mUpdateConfiguration.FileList)
                {
                    FileUnit fileAtServer = ContainsFile(file.FileRelativePath, filesInfoContract.AllFileInfoList);
                    if (fileAtServer != null)
                    {
                        if (file.Version < fileAtServer.Version)
                        {
                            fileRelativePathListNeedDownloaded.Add(file.FileRelativePath);
                        }
                    }
                    else
                    {
                        fileListNeedRemoved.Add(file);
                    }
                }

                foreach (FileUnit file in filesInfoContract.AllFileInfoList)
                {
                    FileUnit fileAtServer = ContainsFile(file.FileRelativePath, mUpdateConfiguration.FileList);
                    if (fileAtServer == null)
                    {
                        fileRelativePathListNeedDownloaded.Add(file.FileRelativePath);
                    }
                }
                mUpdateConfiguration.FileList = filesInfoContract.AllFileInfoList;
                mUpdateConfiguration.ClientVersion = latestUpdateTimeContract.ClientVersion;
            }
        }


        /// <summary>
        /// Find file from file list.
        /// </summary>
        /// <param name="fileName">file name</param>
        /// <param name="fileList">file list</param>
        /// <returns>result</returns>
        private FileUnit ContainsFile(string fileName, IList<FileUnit> fileList)
        {
            foreach (FileUnit file in fileList)
            {
                if (file.FileRelativePath.Equals(fileName)) return file;
            }
            return null;
        }


        /// <summary>
        /// File request received.
        /// </summary>
        /// <param name="projectID">project id</param>
        /// <param name="senderID">sender id</param>
        /// <param name="fileName">file name</param>
        /// <param name="totalSize">total size</param>
        /// <param name="resumedFileItem">resumed file item</param>
        /// <param name="comment">comment</param>
        void FileOutter_FileRequestReceived(string projectID, string senderID, string fileName, ulong totalSize, ResumedProjectItem resumedFileItem, string comment)
        {
            string relativePath = comment;
            string localSavePath = AppDomain.CurrentDomain.BaseDirectory + "temp\\";
            if (!Directory.Exists(localSavePath)) Directory.CreateDirectory(localSavePath);
            mRapidPassiveEngine.FileOutter.BeginReceiveFile(projectID, localSavePath + relativePath, true);
        }


        /// <summary>
        /// File transmission start.
        /// </summary>
        /// <param name="transferingProject">transfering project</param>
        void FileReceivingEvents_FileTransStarted(TransferingProject transferingProject)
        {
            Event_FileToBeUpdated?.Invoke(mFileCountAlreadyUpdated, transferingProject.ProjectName, transferingProject.TotalSize);
        }


        /// <summary>
        /// File transmission progress.
        /// </summary>
        /// <param name="fileID">file id</param>
        /// <param name="total">total</param>
        /// <param name="transfered">transfered</param>
        void FileReceivingEvents_FileTransProgress(string fileID, ulong total, ulong transfered)
        {
            Event_CurrentFileUpdatingProgress?.Invoke(total, transfered);
        }


        /// <summary>
        /// File transmission disruptted.
        /// </summary>
        /// <param name="transmittingFileInfo">transmitting file information</param>
        /// <param name="fileTransDisrupttedType">file transmission disruptted type</param>
        void FileReceivingEvents_FileTransDisruptted(TransferingProject transmittingFileInfo, FileTransDisrupttedType fileTransDisrupttedType)
        {
            if (fileTransDisrupttedType == FileTransDisrupttedType.SelfOffline) return;
            string sourcePath = AppDomain.CurrentDomain.BaseDirectory + "temp\\";
            FileHelper.DeleteDirectory(sourcePath);
            Event_UpdateDisruptted?.Invoke(fileTransDisrupttedType.ToString());
        }


        /// <summary>
        /// File transmission completed.
        /// </summary>
        /// <param name="transferingProject">transfering project</param>
        void FileReceivingEvents_FileTransCompleted(TransferingProject transferingProject)
        {
            try
            {
                mFileCountAlreadyUpdated++;
                if (mFileCountAlreadyUpdated < mFileCountNeedUpdated)
                {
                    DownloadNextFile();
                    return;
                }
                
                string destinationPath = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName + "\\";
                string sourcePath = AppDomain.CurrentDomain.BaseDirectory + "temp\\";
                if (!Directory.Exists(sourcePath)) Directory.CreateDirectory(sourcePath);

                foreach (string fileRelativePath in mFileRelativePathListNeedDownloaded)
                {
                    string destinationFile = destinationPath + fileRelativePath;
                    string sourceFile = sourcePath + fileRelativePath;                  
                    if (File.Exists(sourceFile)) File.Copy(sourceFile, destinationFile, true);
                }
                FileHelper.DeleteDirectory(sourcePath);

                foreach (FileUnit file in mFileListNeedRemoved)
                {
                    FileHelper.DeleteFile(destinationPath + file.FileRelativePath);
                }

                mUpdateConfiguration.Save();
                Event_UpdateCompleted?.Invoke();
            }
            catch (Exception e)
            {
                mLogger.Log(e, "Upgrade.Updater.UdpateThread", ErrorLevel.High);
                Event_UpdateDisruptted?.Invoke(FileTransDisrupttedType.InnerError.ToString());
            }
        }


        /// <summary>
        /// Download next file.
        /// </summary>
        private void DownloadNextFile()
        {
            if (mFileCountAlreadyUpdated >= mFileCountNeedUpdated) return;
            DownloadFileContract downLoadFileContract = new DownloadFileContract
            {
                FileName = mFileRelativePathListNeedDownloaded[mFileCountAlreadyUpdated]
            };
            mRapidPassiveEngine.CustomizeOutter.Send(InformationTypes.DownloadFiles, CompactPropertySerializer.Default.Serialize(downLoadFileContract));
        }

    }
}
