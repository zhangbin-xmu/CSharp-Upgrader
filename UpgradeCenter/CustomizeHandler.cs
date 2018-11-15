using System;
using ESPlus.Application.FileTransfering.Server;
using ESPlus.Serialization;
using ESPlus.FileTransceiver;
using ESPlus.Application.FileTransfering;
using ESPlus.Application;
using ESPlus.Application.CustomizeInfo;
using UpgradeCore;

namespace UpgradeCenter
{
    class CustomizeHandler : ICustomizeHandler
    {
        private UpdateConfiguration mUpdateConfiguration;
        private IFileController mFileController;


        /// <summary>
        /// Initialize.
        /// </summary>
        /// <param name="fileController">file controller</param>
        /// <param name="updateConfiguration">update configuration</param>
        public void Initialize(IFileController fileController, UpdateConfiguration updateConfiguration)
        {          
            mUpdateConfiguration = updateConfiguration;
            mFileController = fileController;
            mFileController.FileRequestReceived += new CbFileRequestReceived(FileController_FileRequestReceived);
        }


        /// <summary>
        /// File rquest received event callback.
        /// </summary>
        /// <param name="projectID">project id</param>
        /// <param name="senderID">sender id</param>
        /// <param name="fileName">file name</param>
        /// <param name="totalSize">total size</param>
        /// <param name="resumedFileItem">resumed file item</param>
        /// <param name="savePath">save path</param>
        void FileController_FileRequestReceived(string projectID, string senderID, string fileName, ulong totalSize, ResumedProjectItem resumedFileItem, string savePath)
        {
            mFileController.BeginReceiveFile(projectID, savePath);
        }


        /// <summary>
        /// Handle information.
        /// </summary>
        /// <param name="sourceUserID">source user id</param>
        /// <param name="informationType">information type</param>
        /// <param name="information">information</param>
        public void HandleInformation(string sourceUserID, int informationType, byte[] information)
        {
            if (informationType == InformationTypes.DownloadFiles)
            {
                DownloadFileContract downloadFileContract = CompactPropertySerializer.Default.Deserialize<DownloadFileContract>(information, 0);
                string filePath = string.Format("{0}FileFolder\\{1}", AppDomain.CurrentDomain.BaseDirectory, downloadFileContract.FileName);
                mFileController.BeginSendFile(sourceUserID, filePath, downloadFileContract.FileName, out string fileID);
            }
        }


        /// <summary>
        /// Handle query.
        /// </summary>
        /// <param name="sourceUserID">source user id</param>
        /// <param name="informationType">information type</param>
        /// <param name="information">information</param>
        /// <returns>result</returns>
        public byte[] HandleQuery(string sourceUserID, int informationType, byte[] information)
        {
            if (informationType == InformationTypes.GetAllFilesInfo)
            {
                FilesInfoContract filesInfoContract = new FilesInfoContract
                {
                    AllFileInfoList = mUpdateConfiguration.FileList
                };
                return CompactPropertySerializer.Default.Serialize(filesInfoContract);
            }
            else if (informationType == InformationTypes.GetLatestUpdateTime)
            {
                LatestUpdateTimeContract latestUpdateTimeContract = new LatestUpdateTimeContract(mUpdateConfiguration.ClientVersion);                
                return CompactPropertySerializer.Default.Serialize(latestUpdateTimeContract);
            }
            return null;
        }


        /// <summary>
        /// Transmit failed.
        /// </summary>
        /// <param name="information">information</param>
        public void OnTransmitFailed(Information information) {}

    }
}
