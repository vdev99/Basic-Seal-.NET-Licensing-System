using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Key
{
    public class KeyByteSet
    {
        public KeyByteSet(int keyByteNumber, byte keyByteA, byte keyByteB, byte keyByteC)
        {
            if (keyByteNumber < 1)
            {
                throw new ArgumentException($"{nameof(keyByteNumber)} should be greater or equal to 1", nameof(keyByteNumber));
            }

            KeyByteNumber = keyByteNumber;
            KeyByteA = keyByteA;
            KeyByteB = keyByteB;
            KeyByteC = keyByteC;
        }

        public int KeyByteNumber { get; set; }
        public byte KeyByteA { get; set; }
        public byte KeyByteB { get; set; }
        public byte KeyByteC { get; set; }
    }
}
