// ==++==
// 
//   Copyright (c) Teo Jörsin.  All rights reserved.
// 
// ==--==
/*============================================================
**
**  Class:  StorageService
**
**
**  TODO:   Update it from WindowsAzure to Azure.
**
===========================================================*/

namespace Channel365 {
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Azure.Storage.Blobs;
    using Microsoft.WindowsAzure.Storage;
    using Microsoft.WindowsAzure.Storage.Blob;
    public class StorageService {
        String ctnName = "files";
        private string accessKey = String.Empty;

        public StorageService() { accessKey = AppConfiguration.GetConfiguration("AccessKey"); }

        public async Task Upload(string containerName,string containerFile) {
            BlobContainerClient containerClient = new BlobContainerClient(accessKey, containerName);
            BlobClient blob = containerClient.GetBlobClient(containerFile);

            using(FileStream file = File.OpenRead(containerFile)) {
                await blob.UploadAsync(file);
            }
        }

        public string UploadFileToStorage(string strFileName,
                                          byte[] fileData,
                                          string fileMimeType) {
            try {
                var _task = Task.Run(() => UploadFileToStorageAsync(strFileName, fileData, fileMimeType));
                _task.Wait();
                string fileUrl = _task.Result;
                return fileUrl;
            } catch (Exception ex) { throw (ex); }
        }

        public async void DeleteStorageData(string fileUrl) {
            Uri uriObj = new Uri(fileUrl);
            string BlobName = Path.GetFileName(uriObj.LocalPath);

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(accessKey);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            string strContainerName = ctnName;
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);

            string pathPrefix = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + "/";
            CloudBlobDirectory blobDirectory = cloudBlobContainer.GetDirectoryReference(pathPrefix);

            //  get the block blob ref
            CloudBlockBlob blockBlob = blobDirectory.GetBlockBlobReference(BlobName);

            await blockBlob.DeleteAsync();
        }

        private String GenerateFileName(string fileName) {
            string strFileName = String.Empty;
            String[] strName = fileName.Split('.');
            strFileName = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd")
                + "/"
                + DateTime.Now.ToUniversalTime().ToString("yyyyMMdd\\THHmmssfff")
                + "." + strName[strName.Length - 1];
            return strFileName;
        }

        private async Task<String> UploadFileToStorageAsync(string strFileName,
                                                            byte[] fileData,
                                                            string fileMimeType) {
            try {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(accessKey);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();

                string strCtnName = ctnName;
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strCtnName);
                string fileName = GenerateFileName(strFileName);

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                if (fileName != null && fileData != null) {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                    cloudBlockBlob.Properties.ContentType = fileMimeType;
                    await cloudBlockBlob.UploadFromByteArrayAsync(fileData, 0, fileData.Length);
                    return cloudBlockBlob.Uri.AbsoluteUri;
                }
                return "";
            } catch(Exception ex) { throw (ex); }
        }
    }
}