using System;
using System.Collections.Generic;
using System.Text;

namespace BasicSealClient.Key
{
    internal class PkvKeyBase
    {
        protected byte GetKeyByte(long seed, byte a, byte b, byte c)
        {
            int aTemp = a % 25;
            int bTemp = b % 3;

            long result;

            if ((a % 2) == 0)
            {
                result = ((seed >> aTemp) & 0xFF) ^ ((seed >> bTemp) | c);
            }
            else
            {
                result = ((seed >> aTemp) & 0xFF) ^ ((seed >> bTemp) & c);
            }

            return (byte)result;
        }

        protected string GetChecksum(string str)
        {
            ushort left = 0x56;
            ushort right = 0xAF;

            if (str.Length > 0)
            {
                // 0xFF hex for 255

                for (int cnt = 0; cnt < str.Length; cnt++)
                {
                    right = (ushort)(right + Convert.ToByte(str[cnt]));

                    if (right > 0xFF)
                    {
                        right -= 0xFF;
                    }

                    left += right;

                    if (left > 0xFF)
                    {
                        left -= 0xFF;
                    }
                }
            }

            ushort sum = (ushort)((left << 8) + right);

            return sum.ToString("X4"); // 4 char hex
        }
    }
}
