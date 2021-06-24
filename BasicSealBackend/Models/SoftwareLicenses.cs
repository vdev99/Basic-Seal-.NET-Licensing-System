using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Models
{
    [Table("SoftwareLicenses")]
    public class SoftwareLicenses
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [ForeignKey("Softwares")]
        public long SoftwareId { get; set; }

        public string LicenseKey { get; set; }

        public int LicenseLength { get; set; }

        public DateTime LicenseStartDate { get; set; }

        public DateTime LicenseEndDate { get; set; }

        public bool IsLifetime { get; set; } = false;

        public bool IsActivated { get; set; } = false;

        public bool IsBanned { get; set; } = false;

        public bool enableOfflineVerification { get; set; } = true;

        public string CpuIdentifier { get; set; }

        public string HddIdentifier { get; set; }

        public string SystemUUID { get; set; }

        public bool isDeleted { get; set; } = false;

        public Softwares Softwares { get; set; }
    }
}
