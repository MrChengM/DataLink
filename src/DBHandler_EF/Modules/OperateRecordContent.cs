using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DBHandler_EF.Modules
{
    public partial class OperateRecordContent : DbContext
    {
        public OperateRecordContent()
            : base("name=DataLinkContent")
        {
        }

        public virtual DbSet<OperateRecord> OperateRecords { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OperateRecord>()
                .Property(e => e.UserName)
                .IsUnicode(false);

            modelBuilder.Entity<OperateRecord>()
                .Property(e => e.Id)
                .IsUnicode(false);

            modelBuilder.Entity<OperateRecord>()
                .Property(e => e.ComputerInfor)
                .IsUnicode(false);

            modelBuilder.Entity<OperateRecord>()
                .Property(e => e.Message)
                .IsUnicode(false);
            modelBuilder.Entity<OperateRecord>()
                .Property(e => e.Transcode)
                .IsUnicode(false);
        }
    }
}
