using Core.DataAccess.EntityFramework;
using Core.Entities.Concrete;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework.Contexts;
using Entities.Concrete;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfFileDal : EfEntityRepositoryBase<File, FileManagerContext>, IFileDal
    {
        public List<File> GetAllUserFiles(int userId)
        {
            using (var context = new FileManagerContext())
            {
                return context.Files.Where(x=>x.CreateUser==userId).Include(x=>x.User).ToList();

            }
        }
    }
}
