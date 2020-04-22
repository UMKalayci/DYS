using Business.Abstract;
using Business.ValidationRules.FluentValidation;
using Core.Aspects.Autofac.Validation;
using Core.CrossCuttingConcerns.Validation;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entities.Concrete;
using Entities.Dtos;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [ValidationAspect(typeof(FileForUploadDtoValidator))]
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
            strFileName = strName[strName.Length - 1] + "\\" + DateTime.Now.ToUniversalTime().ToString("yyyyMMdd\\THHmmssfff") + "." + strName[strName.Length - 1];
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
        public IDataResult<bool> IsUserFileAccess(int userId,int fileId)
        {
            try
            {
                var result =_fileDal.IsUserFileAccess(userId,fileId);
                if (result)
                   return  new SuccessDataResult<bool>(true);
                else
                   return  new SuccessDataResult<bool>(false);
            }
            catch
            {
                return new ErrorDataResult<bool>(false,"Dosya kontrolü sırasında hata oluştu");
            }
        }
        private string TypeConverter(string type)
        {
            switch (type)
            {
                case "doc": return "application/msword";
                case "dot": return "application/msword";
                case "docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case "dotx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.template";
                case "docm": return "application/vnd.ms-word.document.macroEnabled.12";
                case "dotm": return "application/vnd.ms-word.template.macroEnabled.12";
                case "xls": return "application/vnd.ms-excel";
                case "xlt": return "application/vnd.ms-excel";
                case "xla": return "application/vnd.ms-excel";
                case "xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case "xltx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.template";
                case "xlsm": return "application/vnd.ms-excel.sheet.macroEnabled.12";
                case "xltm": return "application/vnd.ms-excel.template.macroEnabled.12";
                case "xlam": return "application/vnd.ms-excel.addin.macroEnabled.12";
                case "xlsb": return "application/vnd.ms-excel.sheet.binary.macroEnabled.12";
                case "ppt": return "application/vnd.ms-powerpoint";
                case "pot": return "application/vnd.ms-powerpoint";
                case "pps": return "application/vnd.ms-powerpoint";
                case "ppa": return "application/vnd.ms-powerpoint";
                case "pptx": return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                case "potx": return "application/vnd.openxmlformats-officedocument.presentationml.template";
                case "ppsx": return "application/vnd.openxmlformats-officedocument.presentationml.slideshow";
                case "ppam": return "application/vnd.ms-powerpoint.addin.macroEnabled.12";
                case "pptm": return "application/vnd.ms-powerpoint.presentation.macroEnabled.12";
                case "potm": return "application/vnd.ms-powerpoint.template.macroEnabled.12";
                case "ppsm": return "application/vnd.ms-powerpoint.slideshow.macroEnabled.12";
                case "mdb": return "application/vnd.ms-access";
                default:
                    return "hata";
            }
   
        }
    }
}
