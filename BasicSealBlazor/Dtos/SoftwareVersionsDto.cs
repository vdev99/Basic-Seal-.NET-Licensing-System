using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBlazor.Dtos
{
    public class SoftwareVersionsDto
    {
        public long SoftwareId { get; set; }

        public string SoftwareName { get; set; }

        public string generatedSoftwareId { get; set; }

        public List<SoftVersion> version { get; set; }
    }

    public class SoftVersion
    {
        public long Id { get; set; }

        public string Version { get; set; }

        public string Hash { get; set; }

        public string isLoading { get; set; }
    }
}
