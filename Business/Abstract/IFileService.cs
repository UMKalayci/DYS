using Core.Utilities.Results;
using Entities.Dtos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IFileService
    {
        IResult UploadFileToBlob(FileForUploadDto fileDto, byte[] fileData, string accessKey);
        void DeleteBlobData(string fileUrl, string accessKey);

        IDataResult<List<FileForListDto>> GetAllUserFiles(int userId);
        IDataResult<List<FileForListDto>> GetAllFiles(int userId);
    }
}
