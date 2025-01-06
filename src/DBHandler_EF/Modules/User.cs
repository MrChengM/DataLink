namespace DBHandler_EF.Modules
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("User")]
    public partial class User
    {
        [StringLength(50)]
        public string Id { get; set; }

        [Required]
        [StringLength(255)]
        public string Account { get; set; }

        [Required]
        [StringLength(255)]
        public string Password { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        public int SSex { get; set; }

        public int Status { get; set; }

        public DateTime CreateTime { get; set; }

        [StringLength(50)]
        public string CreateId { get; set; }
    }
}
