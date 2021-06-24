using BasicSealBackend.Dtos;
using BasicSealBackend.Key;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BasicSealBackend.Services.Interface
{
    public interface IEncryptionService
    {
        byte[] AESEncrypt(string key, byte[] data, byte[] salt);
        byte[] GenerateRandomBytes(int size);
        string HashString(string toHash);

        KeyByteSet[] GenerateKeyByteSet();
        string GeneratePkvKey(int seed, KeyByteSet[] keyByteSets);
        int generatePkvSeed(string applicationHash, string license);
        string GenerateLicenseKey(int size);
    }
}
