using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BasicSealBackend.Dtos
{
    public class ChartDataDtocs
    {
        public ActiveInactive ActiveInactive { get; set; }
        public ActivationInMonths ActivationInMonths { get; set; }
        public ExpiredNotExpired expiredNotExpired { get; set; }
    }

    public class ActiveInactive
    {
        public int active { get; set; }
        public int inactive { get; set; }
    }

    public class ActivationInMonths
    {
        public int january { get; set; }
        public int february { get; set; }
        public int march { get; set; }
        public int april { get; set; }
        public int may { get; set; }
        public int june { get; set; }
        public int july { get; set; }
        public int august { get; set; }
        public int september { get; set; }
        public int october { get; set; }
        public int november { get; set; }
        public int december { get; set; }
    }

    public class ExpiredNotExpired
    {
        public int expired { get; set; }
        public int notExpired { get; set; }
    }
}
