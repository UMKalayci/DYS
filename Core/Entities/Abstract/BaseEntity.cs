using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Entities.Abstract
{
    public abstract class BaseEntity : IEntity
    {
        public int CreateUser { get; set; }
        public int? UpdateUser { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
