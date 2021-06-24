using BasicSealClient.Dtos;
using BasicSealClient.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace BasicSealClient
{
    public class BasicSeal
    {
        private static string softwareVersion { get; set; }
        private static string appId { get; set; }
        private static bool messageBoxEnabled { get; set; }
        internal static bool isWindows = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        public static async Task Start(string _softwareVersion, string _appId, bool enableMessageBox = false, 
                                       bool enableDeveloperMode = false)
        {
            softwareVersion = _softwareVersion;
            appId = _appId;
            messageBoxEnabled = enableMessageBox;
            
            Security sec = isWindows ? sec = new Security.Windows() : sec = new Security.Linux();
            Security.licenseVerified = enableDeveloperMode;
            sec.RunSecurityThread();

            if (isInternetConnectionAvailable())
            {
                Network net = new Network();
                DateTime? serverTime = await net.GetServerTimeAsync();

                if(serverTime != null)
                {
                    sec.createDateFile(serverTime);
                }
            }
        }


        public static async Task<BasicSealResult> ActivateLicense(string licenseKey)
        {
            IMessageBox messageBox = isWindows ? messageBox = new MessageboxWindows() : messageBox = new MessageboxLinux();
            messageBox.isEnabled = messageBoxEnabled;

            BasicSealResult licenseReportDto = new BasicSealResult();

            ClientLicenseVerifyDto clVerifyDto = GatherSystemInformation();

            if(clVerifyDto.cpuIdentifier == null && clVerifyDto.hddIdentifier == null && clVerifyDto.systemUUID == null)
            {
                licenseReportDto.errorGettingHardwareInfo = true;
                messageBox.Show("Error getting hardware information");
                return licenseReportDto;
            }

            clVerifyDto.licenseKey = licenseKey;

            if (isInternetConnectionAvailable()) 
            {
                Network net = new Network();

                clVerifyDto.softwareId = appId;
                GetLicenseResponseDto license = await net.GetLicenseAsync(clVerifyDto);

                if(license == null)
                {
                    licenseReportDto.isInternetAvailable = false;
                    return licenseReportDto;
                }

                licenseReportDto.licenseExists = license.licenseExists;
                licenseReportDto.isBanned = license.isBanned;
                licenseReportDto.isExpired = license.isExpired;
                licenseReportDto.isFileModified = license.isFileModified;
                licenseReportDto.isInternetAvailable = true;

                if(!license.licenseExists)
                {
                    messageBox.Show("License does not exist!");
                    return licenseReportDto;
                }
                if (license.isFileModified)
                {
                    messageBox.Show("Application has been modified!");
                    return licenseReportDto;
                }
                if (license.isExpired)
                {
                    messageBox.Show("License expired!");
                    return licenseReportDto;
                }
                if (license.isBanned)
                {
                    messageBox.Show("License is banned!");
                    return licenseReportDto;
                }

                string licenseSavePath = isWindows ? Environment.CurrentDirectory + @"\bsc.lic" : Environment.CurrentDirectory + @"/bsc.lic";
                byte[] keyBytes = Encoding.ASCII.GetBytes(license.key);
                byte[] licenseBytes = Convert.FromBase64String(license.licenseData);

                try
                {
                    using (FileStream fs = new FileStream(licenseSavePath, FileMode.Create))
                    {
                        fs.Write(keyBytes, 0, keyBytes.Length);
                        fs.Write(licenseBytes, 0, licenseBytes.Length);
                        fs.Close();
                    }

                }
                catch (Exception)
                {
                    licenseReportDto.errorWritingFile = true;
                    messageBox.Show("Error writing license file!");
                    return licenseReportDto;
                }

                BasicSealResult result = await VerifyLicense();

                licenseReportDto.successfulVerification = result.successfulVerification;
                licenseReportDto.isValidLicenseKey = result.isValidLicenseKey;
                licenseReportDto.enableOfflineVerification = result.enableOfflineVerification;
            }
            else
            {
                messageBox.Show("License key activation requires internet connection!");
            }

            return licenseReportDto;
        }

        public static async Task<BasicSealResult> VerifyLicense()
        {
            IMessageBox messageBox = isWindows ? messageBox = new MessageboxWindows() : messageBox = new MessageboxLinux();
            messageBox.isEnabled = messageBoxEnabled;

            BasicSealResult verifyResult = new BasicSealResult();

            LicenseSaveDto licenseSave = GetDecryptedLicense();
            if (licenseSave == null) { DeleteLicense(); return verifyResult; }

            verifyResult.licenseExists = true;

            verifyResult.enableOfflineVerification = licenseSave.licenseData.enableOfflineVerification;

            Security sec = new Security();
            sec.CheckAppIntegrity(licenseSave.licenseData.softwareHash);

            ClientLicenseVerifyDto hwInfo = GatherSystemInformation();

            if (hwInfo.cpuIdentifier == null && hwInfo.hddIdentifier == null && hwInfo.systemUUID == null)
            {
                verifyResult.errorGettingHardwareInfo = true;
                messageBox.Show("Error getting hardware information");
                return verifyResult;
            }

            int hardwareChange = 0;
            hardwareChange = licenseSave.licenseData.cpuIdentifier == hwInfo.cpuIdentifier ? hardwareChange += 1 : hardwareChange;
            hardwareChange = licenseSave.licenseData.hddIdentifier == hwInfo.hddIdentifier ? hardwareChange += 1 : hardwareChange;
            hardwareChange = licenseSave.licenseData.systemUUID == hwInfo.systemUUID ? hardwareChange += 1 : hardwareChange;

            if (isInternetConnectionAvailable())
            {
                Network net = new Network();

                if (hardwareChange != 3)
                {
                    licenseSave.licenseData.cpuIdentifier = hwInfo.cpuIdentifier;
                    licenseSave.licenseData.hddIdentifier = hwInfo.hddIdentifier;
                    licenseSave.licenseData.systemUUID = hwInfo.systemUUID;
                }

                LicenseVerifyDataDto licData = new LicenseVerifyDataDto() { softwareId = appId, license = licenseSave.licenseData};
                VerifyResponseDto verifyResponse = await net.VerifyLicenseAsync(licData);
                if (verifyResponse == null) { verifyResult.isInternetAvailable = false; return verifyResult; }

                verifyResult.isValidLicenseKey = true;

                if (verifyResponse.isFileModified)
                { 
                    DeleteLicense();
                    messageBox.Show("Application has been modified"); 
                    verifyResult.isFileModified = true;
                    return verifyResult; 
                }

                if (verifyResponse.isLicenseModified) 
                {
                    BasicSealResult renew = await ActivateLicense(licenseSave.licenseData.licenseKey);
                    return renew;
                }

                if (verifyResponse.isExpired) 
                {
                    DeleteLicense();
                    messageBox.Show("The license key is expired"); 
                    verifyResult.isExpired = true;
                    return verifyResult;
                }

                if (verifyResponse.isBanned) 
                {
                    DeleteLicense();
                    messageBox.Show("The license key is banned");
                    verifyResult.isBanned = true;
                    return verifyResult;
                }

                if (verifyResponse.hardwareChanged) 
                {
                    DeleteLicense();
                    messageBox.Show("Hardware changed, please contact support"); 
                    verifyResult.hardwareChanged = true;
                    return verifyResult;
                }

                if (verifyResponse.exists) 
                { 
                    Security.licenseVerified = true;
                    verifyResult.successfulVerification = true;
                    return verifyResult;
                }
                else 
                { 
                    DeleteLicense();
                    messageBox.Show("The license key is not valid");
                    verifyResult.isValidLicenseKey = false;
                    return verifyResult;
                }
            }
            else
            {
                verifyResult.licenseExists = true;
                verifyResult.isValidLicenseKey = true;

                if (!licenseSave.licenseData.enableOfflineVerification) 
                {
                    messageBox.Show("To use this software you need to have working internet connection"); 
                    return verifyResult;
                }

                if (hardwareChange < 2) 
                {
                    messageBox.Show("Hardware changed, please contact support"); 
                    verifyResult.hardwareChanged = true;
                    return verifyResult;
                }

                if (licenseSave.licenseData.isBanned) 
                {
                    messageBox.Show("License is banned");
                    verifyResult.isBanned = true;
                    return verifyResult;
                }

                if (licenseSave.licenseData.isLifetime) 
                { 
                    Security.licenseVerified = true;
                    verifyResult.successfulVerification = true;
                    return verifyResult;
                }

                if (IsSystemTimeSafe())
                {
                    if (DateTime.Compare(licenseSave.licenseData.licenseEndDate, DateTime.UtcNow) >= 0) 
                    { 
                        Security.licenseVerified = true;
                        verifyResult.successfulVerification = true;
                        return verifyResult;
                    }
                    else 
                    {
                        messageBox.Show("The license key is expired"); 
                        verifyResult.isExpired = true;
                        return verifyResult;
                    }
                }
                return verifyResult;
            }

            void DeleteLicense()
            {
                try
                {
                    File.Delete(isWindows ? Environment.CurrentDirectory + @"\bsc.lic" : Environment.CurrentDirectory + @"/bsc.lic");
                }
                catch (Exception) { }
            }
        }

        private static bool IsSystemTimeSafe()
        {
            Security sec = new Security();

            DateTime? lastSaveTime = sec.getDateFromFile();
            if(lastSaveTime == null) { return false; }

            DateTime systemUtcTime = DateTime.UtcNow;

            if(systemUtcTime > lastSaveTime) { return true; }

            return false;
        }

        private static ClientLicenseVerifyDto GatherSystemInformation()
        {
            ClientLicenseVerifyDto hwSwInformationDto = new ClientLicenseVerifyDto();

            hwSwInformationDto.version = softwareVersion;
            hwSwInformationDto.softwareHash = new Encryption().HashFile(Path.GetFullPath(Assembly.GetEntryAssembly().Location));

            ISystemInformation sysInfo = isWindows ? sysInfo = new ISystemInformation.Windows() : sysInfo = new ISystemInformation.Linux();

            hwSwInformationDto.cpuIdentifier = sysInfo.getCPUId();
            hwSwInformationDto.hddIdentifier = sysInfo.getHDDSerial();
            hwSwInformationDto.systemUUID = sysInfo.getSystemUUID();

            return hwSwInformationDto;
        }

        private static bool isInternetConnectionAvailable()
        {
            Network net = new Network();
            int checkThreeTime = 0;
            bool internetConnection = false;

            while (checkThreeTime < 3)
            {
                if (!net.CheckConnectivity())
                {
                    checkThreeTime++;
                    internetConnection = false;

                    Thread.Sleep(1000);
                }
                else
                {
                    internetConnection = true;
                    break;
                }
            }

            return internetConnection;
        }

        internal static LicenseSaveDto GetDecryptedLicense()  // if returns null file does not exist or file is corrupt
        {
            IMessageBox messageBox = isWindows ? messageBox = new MessageboxWindows() : messageBox = new MessageboxLinux();
            messageBox.isEnabled = messageBoxEnabled;

            string licensePath = isWindows ? Environment.CurrentDirectory + @"\bsc.lic" : Environment.CurrentDirectory + @"/bsc.lic";

            if (File.Exists(licensePath))
            {
                byte[] keyBytes = new byte[28]; // Key size is always 28
                byte[] licenseDtoBytes;

                try
                {
                    using (FileStream fs = new FileStream(licensePath, FileMode.Open, FileAccess.Read))
                    {
                        licenseDtoBytes = new byte[fs.Length - 28];

                        fs.Seek(0, SeekOrigin.Begin);
                        fs.Read(keyBytes, 0, keyBytes.Length);

                        fs.Read(licenseDtoBytes, 0, licenseDtoBytes.Length);
                        fs.Close();
                    }

                    string key = Encoding.ASCII.GetString(keyBytes);

                    byte[] licenseDto = new Encryption.AES().Decrypt(key, licenseDtoBytes);

                    LicenseSaveDto licenseSaveDto = JsonSerializer.Deserialize<LicenseSaveDto>(Encoding.ASCII.GetString(licenseDto));

                    string licensejson = JsonSerializer.Serialize(licenseSaveDto.licenseData);

                    string licenseHash = new Encryption().HashString(JsonSerializer.Serialize(licenseSaveDto.licenseData));

                    Security sec = new Security();

                    bool encKeyValid = sec.VerifyEncryptionKey(key, licenseSaveDto.keyBytes, licenseHash);

                    if (!encKeyValid)
                    {
                        messageBox.Show("License encryption key is not valid");
                        return null;
                    }

                    return licenseSaveDto;
                }
                catch (Exception)
                {
                    messageBox.Show("License decryption failed!");
                    return null;
                }
            }
            return null;
        }
    }
}
