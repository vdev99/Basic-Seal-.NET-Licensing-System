using BasicSealBackend.Dtos;
using BasicSealBackend.Key;
using BasicSealBackend.Services.Interface;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace BasicSealBackend.Services
{
    public class EncryptionService : PkvKeyBase, IEncryptionService
    {
        public byte[] AESEncrypt(string key, byte[] data, byte[] salt) //salt = 8 bytes
        {
            byte[] encrypted;

            var encKey = new Rfc2898DeriveBytes(Encoding.ASCII.GetBytes(key), salt, 1000);

            RijndaelManaged AES = new RijndaelManaged();
            AES.BlockSize = 128;
            AES.KeySize = 256;
            AES.Key = encKey.GetBytes(AES.KeySize / 8);
            AES.IV = encKey.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.PKCS7;
            AES.Mode = CipherMode.CBC;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cryptoStream.Write(data, 0, data.Length);
                    cryptoStream.Close();
                }
                encrypted = ms.ToArray();
            }

            return encrypted;
        }

        public byte[] GenerateRandomBytes(int size)
        {
            byte[] salt = new byte[size];
            RNGCryptoServiceProvider CSP = new RNGCryptoServiceProvider();
            CSP.GetNonZeroBytes(salt);
            return salt;
        }

        public string HashString(string toHash)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(toHash));
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            }
        }

        public KeyByteSet[] GenerateKeyByteSet()
        {
            KeyByteSet[] keyByteSet = new KeyByteSet[8];

            for (int i = 1; i <= 8; i++)
            {
                byte[] threeByte = GenerateRandomBytes(3);

                keyByteSet[i - 1] = new KeyByteSet(i, threeByte[0], threeByte[1], threeByte[2]);
            }

            return keyByteSet;
        }

        public int generatePkvSeed(string applicationHash, string license)
        {
            string licenseHash = HashString(license);

            byte[] appHashByte = Encoding.ASCII.GetBytes(applicationHash);
            byte[] licenseHashByte = Encoding.ASCII.GetBytes(licenseHash);

            int seed = (GetArraykSum(appHashByte) + GetArraykSum(licenseHashByte)) * 256;

            return seed;

            int GetArraykSum(byte[] arrayToCheck)
            {
                int sum = 0;

                foreach(byte myByte in arrayToCheck)
                {
                    sum += myByte;
                }

                return sum;
            }
        }

        // This Key Generator was built with the assistance of the article linked below (credit to Brandon Staggs). 
        // Brandon Staggs article provided example in Delphi. Some functionality has been ported to C#
        // to create this key generator.
        // https://github.com/appsoftwareltd/dotnet-licence-key-generator
        // http://www.brandonstaggs.com/2007/07/26/implementing-a-partial-serial-number-verification-system-in-delphi/

        public string GeneratePkvKey(int seed, KeyByteSet[] keyByteSets)
        {
            if (keyByteSets.Length < 2)
            {
                return null; //The KeyByteSet array must be of length 2 or greater.
            }

            // Check that array is in correct order as this will cause errors if passed in incorrectly

            Array.Sort(keyByteSets, new KeyByteSetComparer());

            bool allKeyByteNumbersDistinct = true;

            var keyByteCheckedNos = new List<int>();

            int maxKeyByteNumber = 0;

            foreach (var keyByteSet in keyByteSets)
            {
                if (!(keyByteCheckedNos.Contains(keyByteSet.KeyByteNumber)))
                {
                    keyByteCheckedNos.Add(keyByteSet.KeyByteNumber);

                    if (keyByteSet.KeyByteNumber > maxKeyByteNumber)
                    {
                        maxKeyByteNumber = keyByteSet.KeyByteNumber;
                    }
                }
                else
                {
                    allKeyByteNumbersDistinct = false;
                    break;
                }
            }

            if (!allKeyByteNumbersDistinct)
            {
                return null;
            }

            if (maxKeyByteNumber != keyByteSets.Length)
            {
                return null; 
            }

            var keyBytes = new byte[keyByteSets.Length];

            for (int i = 0; i < keyByteSets.Length; i++)
            {
                keyBytes[i] = GetKeyByte(
                                    seed,
                                    keyByteSets[i].KeyByteA,
                                    keyByteSets[i].KeyByteB,
                                    keyByteSets[i].KeyByteC
                              );
            }

            string result = seed.ToString("X8"); // 8 digit hex;

            for (int i = 0; i < keyBytes.Length; i++)
            {
                result = result + keyBytes[i].ToString("X2");
            }

            result = result + GetChecksum(result);

            return result;
        }

        public string GenerateLicenseKey(int size)
        {
            string licenseKey = "";
            string randomString = "";
            byte[] randomBytes = new byte[size];

            RNGCryptoServiceProvider cryptoSP = new RNGCryptoServiceProvider();
            cryptoSP.GetNonZeroBytes(randomBytes);

            randomString = HashString(Convert.ToBase64String(randomBytes)).ToUpper();

            for (int i = 1; i < size + 4; i++)
            {
                if (i % 5 != 0)
                {
                    licenseKey += randomString[i];
                }
                else
                {
                    licenseKey += "-";
                }
            }

            return licenseKey;
        }
    }
}
