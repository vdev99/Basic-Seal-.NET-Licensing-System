using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBlazor.Dtos
{
    public class LicenseRespDto
    {
        public int softwareId { get; set; }
        public int pageNumber { get; set; }
        public int pageSize { get; set; }
        public int totalRecords { get; set; }
        public int totalPages
        {
            get
            {
                return Convert.ToInt32(Math.Ceiling(((double)totalRecords / (double)pageSize)));
            }
            set { }
        }
        public bool hasPreviousPage
        {
            get
            {
                return (pageNumber > 1);
            }
            set { }
        }
        public bool HasNextPage
        {
            get
            {
                return (pageNumber < totalPages);
            }
            set { }
        }

        public List<SoftwareLicenseDto> softwareLicenseDtos { get; set; } = new List<SoftwareLicenseDto>();
    }
}
