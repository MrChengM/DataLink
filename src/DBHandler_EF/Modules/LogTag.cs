namespace DBHandler_EF.Modules
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class LogTag
    {
        public long ID { get; set; }

        [Required]
        [StringLength(50)]
        public string PointName { get; set; }

        public DateTime TimeStamp { get; set; }

        [Required]
        [StringLength(20)]
        public string ValueType { get; set; }

        [Required]
        [StringLength(50)]
        public string Value { get; set; }

        [Required]
        [StringLength(20)]
        public string Quality { get; set; }
    }
}
