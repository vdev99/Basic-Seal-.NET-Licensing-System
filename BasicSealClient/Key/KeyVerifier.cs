using System;
using System.Collections.Generic;
using System.Text;
using BasicSealClient.Dtos;

namespace BasicSealClient.Key
{
    internal class KeyVerifier : PkvKeyBase
    {
        // This Key Generator was built with the assistance of the article linked below (credit to Brandon Staggs). 
        // Brandon Staggs article provided example in Delphi. Some functionality has been ported to C#
        // to create this key generator.
        // https://github.com/appsoftwareltd/dotnet-licence-key-generator
        // http://www.brandonstaggs.com/2007/07/26/implementing-a-partial-serial-number-verification-system-in-delphi/

        public PkvKeyVerificationResult VerifyKey(
            string key,
            KeyByteSet[] keyByteSetsToVerify,
            int totalKeyByteSets,
            string keySeed
        )
        {
            PkvKeyVerificationResult pkvKeyVerificationResult = PkvKeyVerificationResult.KeyIsInvalid;

            bool checksumPass = CheckKeyChecksum(key, totalKeyByteSets);

            if (checksumPass)
            {
                if (keySeed != null && key.StartsWith(keySeed))
                {
                    pkvKeyVerificationResult = PkvKeyVerificationResult.KeySeedValid;
                }

                if (pkvKeyVerificationResult == PkvKeyVerificationResult.KeySeedValid)
                {
                    pkvKeyVerificationResult = PkvKeyVerificationResult.KeyIsInvalid;

                    int seed;

                    bool seedParsed = int.TryParse(key.Substring(0, 8), System.Globalization.NumberStyles.HexNumber, null, out seed);

                    if (seedParsed)
                    {
                        foreach (var keyByteSet in keyByteSetsToVerify)
                        {
                            var keySubstringStart = GetKeySubstringStart(keyByteSet.KeyByteNumber);

                            if (keySubstringStart - 1 > key.Length)
                            {
                                return PkvKeyVerificationResult.KeyIsInvalid;
                            }

                            var keyBytes = key.Substring(keySubstringStart, 2);

                            var b = GetKeyByte(seed, keyByteSet.KeyByteA, keyByteSet.KeyByteB, keyByteSet.KeyByteC);

                            if (keyBytes != b.ToString("X2"))
                            {
                                return pkvKeyVerificationResult; 
                            }
                        }

                        pkvKeyVerificationResult = PkvKeyVerificationResult.KeyIsValid;
                    }
                }
            }

            return pkvKeyVerificationResult;
        }

        private int GetKeySubstringStart(int keyByteNumber)
        {
            return (keyByteNumber * 2) + 6;
        }

        private bool CheckKeyChecksum(string key, int totalKeyByteSets)
        {
            bool result = false;

            if (key.Length == (8 + 4 + (2 * totalKeyByteSets))) // First 8 are seed, 4 for check sum, plus 2 for each KeyByte
            {
                int keyLessChecksumLength = key.Length - 4;

                string checkSum = key.Substring(keyLessChecksumLength, 4); // Last 4 chars are checksum

                string keyWithoutChecksum = key.Substring(0, keyLessChecksumLength);

                result = GetChecksum(keyWithoutChecksum) == checkSum;
            }

            return result;
        }
    }
}
