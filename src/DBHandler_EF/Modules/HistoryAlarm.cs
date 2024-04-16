namespace DBHandler_EF.Modules
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class HistoryAlarm
    {
        public long ID { get; set; }

        [Required]
        [StringLength(50)]
        public string AlarmName { get; set; }

        [Required]
        [StringLength(50)]
        public string PartName { get; set; }

        [Required]
        [StringLength(50)]
        public string AlarmDescrible { get; set; }

        public int AlarmLevel { get; set; }

        [Required]
        [StringLength(10)]
        public string AlarmNumber { get; set; }

        [Required]
        [StringLength(50)]
        public string L1View { get; set; }

        [Required]
        [StringLength(50)]
        public string L2View { get; set; }

        [Required]
        [StringLength(50)]
        public string AlarmGroup { get; set; }

        public DateTime AppearTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
