namespace DBHandler_EF.Modules
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("OperateRecord")]
    public partial class OperateRecord
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string UserName { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string Id { get; set; }

        [Key]
        [Column(Order = 2)]
        public DateTime Time { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(50)]
        public string ComputerInfor { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(200)]
        public string Message { get; set; }
        [Key]
        [Column(Order = 5)]
        [StringLength(200)]
        public string Transcode { get; set; }
    }
}
