using Microsoft.EntityFrameworkCore;
using Core.Entities.Concrete;
using Entities.Concrete;
using System.Linq;
using Core.Entities.Abstract;
using System;

namespace DataAccess.Concrete.EntityFramework.Contexts
{
    public class FileManagerContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server = tcp:umkalayci.database.windows.net, 1433; Initial Catalog = FileTestDB; Persist Security Info = False; User ID = ugurmutlu; Password = Qazwsx112; MultipleActiveResultSets = False; Encrypt = True; TrustServerCertificate = False; Connection Timeout = 30; ");
        }
        public override int SaveChanges()
        {
            var entries = ChangeTracker
                .Entries()
                .Where(e => e.Entity is BaseEntity && (
                        e.State == EntityState.Added
                        || e.State == EntityState.Modified));

            foreach (var entityEntry in entries)
            {
                if (entityEntry.State == EntityState.Modified)
                {
                    ((BaseEntity)entityEntry.Entity).UpdateDate = DateTime.Now;
                }
                if (entityEntry.State == EntityState.Added)
                {
                    ((BaseEntity)entityEntry.Entity).CreateDate = DateTime.Now;
                }
            }

            return base.SaveChanges();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<File>()
                .HasOne<User>(s => s.User)
                .WithMany()
                .HasForeignKey(s => s.CreateUser);
        }
        public DbSet<OperationClaim> OperationClaims { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserOperationClaim> UserOperationClaims { get; set; }
        public DbSet<File> Files { get; set; }

    }
}
