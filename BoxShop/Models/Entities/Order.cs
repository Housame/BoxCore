using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BoxShop.Models.Entities
{
    public partial class Order
    {
        public Order()
        {
            OrderItem = new HashSet<OrderItem>();
        }

        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime DateOfPurchase { get; set; }
        public bool IsPaid { get; set; }
        public int BoxId { get; set; }

        public virtual ICollection<OrderItem> OrderItem { get; set; }
        public virtual Box Box { get; set; }
        [JsonIgnore]
        public virtual User User { get; set; }
    }
}
