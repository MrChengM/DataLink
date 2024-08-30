using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DBHandler_EF.Modules
{
    public partial class PermissionContent : DbContext
    {
        public PermissionContent()
            : base("name = DataLinkContent")
        {
        }

        public virtual DbSet<Resource> Resource { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<Role_Resource> Role_Resource { get; set; }
        public virtual DbSet<User_Role> User_Role { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Resource>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Resource>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Resource>()
                .Property(e => e.Description)
                .IsUnicode(false);

            modelBuilder.Entity<Resource>()
                .Property(e => e.ParentName)
                .IsUnicode(false);

            modelBuilder.Entity<Resource>()
                .Property(e => e.ParentId)
                .IsUnicode(false);

            modelBuilder.Entity<Resource>()
                .Property(e => e.CreateUserId)
                .IsUnicode(false);

            modelBuilder.Entity<Resource>()
                .Property(e => e.UpdateUserId)
                .IsUnicode(false);

            modelBuilder.Entity<Role>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<Role>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Role>()
                .Property(e => e.CreateId)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Account)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Password)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<User>()
                .Property(e => e.CreateId)
                .IsUnicode(false);

            modelBuilder.Entity<Role_Resource>()
                .Property(e => e.RoleId)
                .IsUnicode(false);

            modelBuilder.Entity<Role_Resource>()
                .Property(e => e.ResourceId)
                .IsUnicode(false);

            modelBuilder.Entity<User_Role>()
                .Property(e => e.UserId)
                .IsUnicode(false);

            modelBuilder.Entity<User_Role>()
                .Property(e => e.RoleId)
                .IsUnicode(false);
        }
    }
}
