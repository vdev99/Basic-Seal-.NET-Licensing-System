using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Models
{
    [Table("Softwares")]
    public class Softwares
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("Users")]
        public long UserId { get; set; }

        public string SoftwareName { get; set; }

        public Users Users { get; set; }

        public bool isDeleted { get; set; } = false;

        public ICollection<SoftwareVersions> SoftwareVersions { get; set; }
        public ICollection<SoftwareLicenses> SoftwareLicenses { get; set; }
    }
}
