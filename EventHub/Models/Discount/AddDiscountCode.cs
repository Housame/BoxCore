using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class AddDiscountCode
    {
        public List<int> SubCompId { get; set; }
        public string Code { get; set; }
        public int PercentageOff { get; set; }
        public DateTime ExpiryDate { get; set; }
        //public bool CodeValidOnce { get; set; }
    }
}
