using Core.DataAccess;
using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccess.Abstract
{
    public interface IFileDal : IEntityRepository<File>
    {
        List<File> GetAllFiles(int userId);
        List<File> GetAllUserFiles(int userId);
    }
}
