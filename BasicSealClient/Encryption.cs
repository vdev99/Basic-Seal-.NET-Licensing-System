using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using BasicSealClient.Dtos;

namespace BasicSealClient
{
    internal class Encryption
    {
        internal class AES
        {
            public byte[] Encrypt(string key, byte[] data, byte[] salt)
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

            public byte[] Decrypt(string key, byte[] data) 
            {
                byte[] decrypted;

                byte[] salt = new byte[8];
                Array.Copy(data, 0, salt, 0, 8);

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
                    using (CryptoStream cryptoStream = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(data, salt.Length, data.Length - salt.Length);
                        cryptoStream.Close();
                    }
                    decrypted = ms.ToArray();
                }

                return decrypted;
            }
        }

        public byte[] GetSalt(int size)
        {
            byte[] salt = new byte[size];
            RNGCryptoServiceProvider CSP = new RNGCryptoServiceProvider();
            CSP.GetNonZeroBytes(salt);
            return salt;
        }


        public string HashFile(string path)
        {
            using (MD5 md5 = MD5.Create())
            {
                using (FileStream stream = File.OpenRead(path))
                {
                    byte[] hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
                }
            }
        }

        public string HashString(string toHash)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(Encoding.ASCII.GetBytes(toHash));
                return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
            }
        }

        public int generatePkvSeed(string applicationHash, string licenseHash)
        {
            byte[] appHashByte = Encoding.ASCII.GetBytes(applicationHash);
            byte[] licenseHashByte = Encoding.ASCII.GetBytes(licenseHash);

            int seed = (GetArraykSum(appHashByte) + GetArraykSum(licenseHashByte)) * 256;

            return seed;

            int GetArraykSum(byte[] arrayToCheck)
            {
                int sum = 0;

                foreach (byte myByte in arrayToCheck)
                {
                    sum += myByte;
                }

                return sum;
            }
        }
    }
}