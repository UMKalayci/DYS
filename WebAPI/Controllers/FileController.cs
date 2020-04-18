using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Business.Abstract;
using Entities.Dtos;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace WebAPI.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
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
                model.UserId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var result = _fileService.UploadFileToBlob(model, fileBytes, AzureConnectionString);
                if (result.Success)
                {
                    return RedirectToAction("Upload");
                }
            }
            return Ok();
        }
    }

}