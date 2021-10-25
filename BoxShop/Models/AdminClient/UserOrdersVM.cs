using BoxShop.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxShop.Models
{
    public class UserOrdersVM
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? BoxId { get; set; }
        public string UserId { get; set; }     
        public virtual ICollection<OrderVM> Order { get; set; }
    }
    public class OrderVM
    {
        public virtual ICollection<OrderItemVM> OrderItem { get; set; }
        public bool IsPaid { get; set; }
    }
}
