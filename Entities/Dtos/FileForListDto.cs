using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Dtos
{
    public class FileForListDto
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string Desc { get; set; }
        public string CreateUser { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
