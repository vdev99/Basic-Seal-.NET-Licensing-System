using System;
using System.Collections.Generic;
using System.Text;

namespace BasicSealClient.Key
{
    internal enum PkvKeyVerificationResult
    {
        KeyIsValid = 0,
        KeyIsInvalid = 1,
        KeySeedValid = 2
    }
}
