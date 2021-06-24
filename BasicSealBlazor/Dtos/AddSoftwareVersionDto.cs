using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBlazor.Dtos
{
    public class AddSoftwareVersionDto
    {
        public long SoftwareId { get; set; }

        public string Version { get; set; }

        public string Hash { get; set; }
    }
}
