using System;
using System.Collections.Generic;
using System.Text;

namespace BasicSealClient.Dtos
{
    internal class ClientLicenseVerifyDto
    {
        public string softwareId { get; set; }
        public string version { get; set; }
        public string softwareHash { get; set; }
        public string licenseKey { get; set; }
        public string cpuIdentifier { get; set; }
        public string hddIdentifier { get; set; }
        public string systemUUID { get; set; }
    }
}
