using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Dtos
{
    public class FileForUploadDto
    {
        public string FileName { get; set; }
        public string FileDescription { get; set; }
        public int UserId { get; set; }
        public bool IsPrivate { get; set; }
        public IFormFile File { get; set; }
    }
}
