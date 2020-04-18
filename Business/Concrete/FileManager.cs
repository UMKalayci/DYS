using Business.Abstract;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class FileManager : IFileService
    {
        private readonly IFileDal _fileDal;

        public FileManager(IFileDal fileDal)
        {
            _fileDal = fileDal;
        }
        public IResult UploadFileToBlob(FileForUploadDto fileDto, byte[] fileData, string accessKey)
        {
            try
            {
                var _task = Task.Run(() => this.UploadFileToBlobAsync(fileDto.File.FileName, fileData, fileDto.File.ContentType, accessKey));
                _task.Wait();
                string filePath = _task.Result;
                File file = new File();
                file.FileName = fileDto.FileName;
                file.IsPrivate = fileDto.IsPrivate;
                file.Description = fileDto.FileDescription;
                file.FilePath = filePath;
                file.CreateUser = fileDto.UserId;
                file.FileType = fileDto.File.ContentType;
                _fileDal.Add(file);
                return new SuccessResult("Dosya ekleme işlemi başarılı");
            }
            catch (Exception ex)
            {
                return new ErrorResult("Dosya ekleme işlemi sırasında hata oluştu");
            }
        }

        public async void DeleteBlobData(string fileUrl, string accessKey)
        {
            Uri uriObj = new Uri(fileUrl);
            string BlobName = System.IO.Path.GetFileName(uriObj.LocalPath);

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(accessKey);
            CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
            string strContainerName = "uploads";
            CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);

            string pathPrefix = DateTime.Now.ToUniversalTime().ToString("yyyy-MM-dd") + "/";
            CloudBlobDirectory blobDirectory = cloudBlobContainer.GetDirectoryReference(pathPrefix);
            // get block blob refarence    
            CloudBlockBlob blockBlob = blobDirectory.GetBlockBlobReference(BlobName);

            // delete blob from container        
            await blockBlob.DeleteAsync();
        }


        private string GenerateFileName(string fileName, string fileMimeType)
        {
            string strFileName = string.Empty;
            string[] strName = fileName.Split('.');
            strFileName = fileMimeType + "\\" + DateTime.Now.ToUniversalTime().ToString("yyyyMMdd\\THHmmssfff") + "." + strName[strName.Length - 1];
            return strFileName;
        }

        private async Task<string> UploadFileToBlobAsync(string strFileName, byte[] fileData, string fileMimeType, string accessKey)
        {
            try
            {
                CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(accessKey);
                CloudBlobClient cloudBlobClient = cloudStorageAccount.CreateCloudBlobClient();
                string strContainerName = "uploads";
                CloudBlobContainer cloudBlobContainer = cloudBlobClient.GetContainerReference(strContainerName);
                string fileName = this.GenerateFileName(strFileName, fileMimeType);

                if (await cloudBlobContainer.CreateIfNotExistsAsync())
                {
                    await cloudBlobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob });
                }

                if (fileName != null && fileData != null)
                {
                    CloudBlockBlob cloudBlockBlob = cloudBlobContainer.GetBlockBlobReference(fileName);
                    cloudBlockBlob.Properties.ContentType = fileMimeType;
                    await cloudBlockBlob.UploadFromByteArrayAsync(fileData, 0, fileData.Length);
                    return cloudBlockBlob.Uri.AbsoluteUri;
                }
                return "";
            }
            catch (Exception ex)
            {
                throw (ex);
            }
        }

        public IDataResult<List<FileForListDto>> GetAllUserFiles(int userId)
        {
            try
            {
                var files = _fileDal.GetAllUserFiles(userId);
                List<FileForListDto> fileForListDtoList = new List<FileForListDto>();
                foreach (var item in files)
                {
                    fileForListDtoList.Add(new FileForListDto()
                    {
                        Id = item.Id,
                        FileName = item.FileName,
                        FileType = item.FileType,
                        Desc=item.Description,
                        CreateDate = item.CreateDate,
                        CreateUser = item.User.FirstName + " " + item.User.LastName
                    });
                }
                return new SuccessDataResult<List<FileForListDto>>(fileForListDtoList);
            }
            catch
            {
                return new  ErrorDataResult<List<FileForListDto>>(null,"Veriler listelenirken hata oluştu");
            }
        }
        public IDataResult<List<FileForListDto>> GetAllFiles(int userId)
        {
            try
            {
                var files = _fileDal.GetAllFiles(userId);
                List<FileForListDto> fileForListDtoList = new List<FileForListDto>();
                foreach (var item in files)
                {
                    fileForListDtoList.Add(new FileForListDto()
                    {
                        Id = item.Id,
                        FileName = item.FileName,
                        FileType = item.FileType,
                        Desc = item.Description,
                        CreateDate = item.CreateDate,
                        CreateUser = item.User.FirstName + " " + item.User.LastName
                    });
                }
                return new SuccessDataResult<List<FileForListDto>>(fileForListDtoList);
            }
            catch
            {
                return new ErrorDataResult<List<FileForListDto>>(null, "Veriler listelenirken hata oluştu");
            }
        }
    }
}
