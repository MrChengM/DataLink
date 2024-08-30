using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace DBHandler_EF.Modules
{
    public partial class LogContent : DbContext
    {
        public LogContent()
            : base("name=DataLinkContent")
        {
        }

        public virtual DbSet<LogHistoryAlarm> HistoryAlarms { get; set; }
        public virtual DbSet<LogTag> LogTags { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogHistoryAlarm>()
                .Property(e => e.AlarmName)
                .IsFixedLength();

            modelBuilder.Entity<LogHistoryAlarm>()
                .Property(e => e.PartName)
                .IsFixedLength();

            modelBuilder.Entity<LogHistoryAlarm>()
                .Property(e => e.AlarmDescrible)
                .IsFixedLength();

            modelBuilder.Entity<LogHistoryAlarm>()
                .Property(e => e.AlarmNumber)
                .IsFixedLength();

            modelBuilder.Entity<LogHistoryAlarm>()
                .Property(e => e.L1View)
                .IsFixedLength();

            modelBuilder.Entity<LogHistoryAlarm>()
                .Property(e => e.L2View)
                .IsFixedLength();

            modelBuilder.Entity<LogHistoryAlarm>()
                .Property(e => e.AlarmGroup)
                .IsFixedLength();

            modelBuilder.Entity<LogTag>()
                .Property(e => e.PointName)
                .IsFixedLength();

            modelBuilder.Entity<LogTag>()
                .Property(e => e.ValueType)
                .IsFixedLength();

            modelBuilder.Entity<LogTag>()
                .Property(e => e.Value)
                .IsFixedLength();

            modelBuilder.Entity<LogTag>()
                .Property(e => e.Quality)
                .IsFixedLength();
        }
    }
}
