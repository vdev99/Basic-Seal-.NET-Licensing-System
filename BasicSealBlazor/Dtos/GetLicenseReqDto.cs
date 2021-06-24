using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBlazor.Dtos
{
    public class GetLicenseReqDto
    {
        public int softwareId { get; set; }
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 10;
        public LicenseFiltersDto filters { get; set; } = new LicenseFiltersDto();
    }
}
