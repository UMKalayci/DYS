using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Business.Abstract;
using Entities.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebAPI.Controllers
{
    public class FileController : Controller
    {
        IFileService _fileService;
        IConfiguration _config;
        private string AzureConnectionString;
        public FileController(IConfiguration config, IFileService fileService)
        {
            _fileService = fileService;
            _config = config;
            AzureConnectionString = _config.GetConnectionString("AccessKey");
        }


        public IActionResult Upload()
        {
            var model = new FileForUploadDto();
            return View(model);
        }
        [HttpPost]
        public IActionResult Upload(FileForUploadDto model)
        {
            byte[] fileBytes;
            if (model.File.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    model.File.CopyTo(ms);
                    fileBytes = ms.ToArray();
                }
               _fileService.UploadFileToBlob(model, fileBytes, AzureConnectionString);

            }
            return Ok();
        }
    }

}