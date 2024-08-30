namespace DBHandler_EF.Modules
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Role_Resource
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(50)]
        public string RoleId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(50)]
        public string ResourceId { get; set; }
    }
}
