using System;
using System.Collections.Generic;
using System.Text;

namespace BasicSealClient
{
    public class BasicSealResult
    {
        public bool isInternetAvailable { get; set; } = false;
        public bool licenseExists { get; set; } = false;
        public bool successfulVerification { get; set; } = false;
        public bool isValidLicenseKey { get; set; } = false;
        public bool enableOfflineVerification { get; set; } = false;
        public bool isFileModified { get; set; } = false;
        public bool isLicenseModified { get; set; } = false;
        public bool hardwareChanged { get; set; } = false;
        public bool isExpired { get; set; } = false;
        public bool isBanned { get; set; } = false;
        public bool errorWritingFile { get; set; } = false;
        public bool errorGettingHardwareInfo { get; set; } = false;
    }
}
