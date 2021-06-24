using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Dtos
{
    public class GetLicenseReqDto
    {
        public int softwareId { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public LicenseFiltersDto filters { get; set; }
    }
}
