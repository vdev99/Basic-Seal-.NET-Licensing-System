using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBlazor.Services
{
    public class UserAuthStateService
    {
        public bool isLoggedIn { get; set; } = false;
        public string email { get; set; }
        public string token { get; set; }
    }
}
