using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBlazor.Dtos
{
    public class LicenseFiltersDto
    {
        public bool? active { get; set; }
        public bool? expired { get; set; }
        public bool? banned { get; set; }
        public List<int> length { get; set; } = new List<int>();
        public bool? lifetime { get; set; }
    }
}
