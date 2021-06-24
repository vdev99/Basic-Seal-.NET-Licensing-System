using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Key
{
    public class KeyByteSetComparer : IComparer
    {
        public int Compare(object x, object y)
        {
            var keyByteSetX = (KeyByteSet)x;
            var keyByteSetY = (KeyByteSet)y;

            if (keyByteSetX == null)
            {
                throw new ArgumentNullException(nameof(keyByteSetX));
            }

            if (keyByteSetY == null)
            {
                throw new ArgumentNullException(nameof(keyByteSetY));
            }

            if (keyByteSetX.KeyByteNumber > keyByteSetY.KeyByteNumber)
            {
                return 1;
            }

            if (keyByteSetX.KeyByteNumber < keyByteSetY.KeyByteNumber)
            {
                return -1;
            }

            return 0;
        }
    }
}
