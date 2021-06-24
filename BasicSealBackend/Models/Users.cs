using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Models
{
    [Table("Users")]
    public class Users
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public ICollection<SoftwareVersions> SoftwareVersions { get; set; }
        public ICollection<Softwares> Softwares { get; set; }

    }
}
