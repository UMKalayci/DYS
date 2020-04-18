using Core.Entities;
using Core.Entities.Abstract;
using Core.Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete
{
    public class File : BaseEntity, IEntity
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public string Description { get; set; }
        public bool IsPrivate { get; set; }
        public string FilePath { get; set; }
        public User User { get; set; }
    }
}
