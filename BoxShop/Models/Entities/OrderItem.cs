using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace BoxShop.Models.Entities
{
    public partial class OrderItem
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Category { get; set; }
        public decimal Vat { get; set; }
        public int Quantity { get; set; }
        public int OrderId { get; set; }
        public int? ArticleGroup { get; set; }
        public DateTime DateOfPurchase { get; set; }
        [JsonIgnore]
        public virtual Order Order { get; set; }
    }
}
