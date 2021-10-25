using System;
using System.Collections.Generic;

namespace BoxShop.Models.Entities
{
    public partial class Box
    {
        public Box()
        {
            Order = new HashSet<Order>();
            Product = new HashSet<Product>();
            User = new HashSet<User>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Image { get; set; }

        public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<Product> Product { get; set; }
        public virtual ICollection<User> User { get; set; }
    }
}
