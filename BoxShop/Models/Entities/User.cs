using System;
using System.Collections.Generic;

namespace BoxShop.Models.Entities
{
    public partial class User
    {
        public User()
        {
            Order = new HashSet<Order>();
           
        }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? BoxId { get; set; }
        public string HashId { get; set; }
        public string Email { get; set; }
        public string PhoneNr { get; set; }
        public int? Pin { get; set; }

        public virtual ICollection<Order> Order { get; set; }
        public virtual Box Box { get; set; }
    }
}
