using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxShop.Models
{
    public class ConfirmPurchase
    {
        public UserDetails UserDetail { get; set; }
        public List<ProductDisplayVM> productDetails { get; set; }
        public decimal ProductSum { get; set; }
    }

    public class UserDetails
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
