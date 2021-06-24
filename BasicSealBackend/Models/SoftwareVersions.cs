using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Models
{
    [Table("SoftwareVersions")]
    public class SoftwareVersions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("Users")]
        public long UserId { get; set; }

        [ForeignKey("Softwares")]
        public long SoftwareId { get; set; }

        public string Version { get; set; }

        public string Hash { get; set; }

        public Users Users { get; set; }

        public bool isDeleted { get; set; } = false;

        public Softwares Softwares { get; set; }
    }
}
