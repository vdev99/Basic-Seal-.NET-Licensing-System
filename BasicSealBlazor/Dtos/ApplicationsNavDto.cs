using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBlazor.Dtos
{
    public class ApplicationsNavDto
    {
        public long Id { get; set; }
        public string SoftwareName { get; set; }
        public bool display { get; set; } = false;
    }
}
