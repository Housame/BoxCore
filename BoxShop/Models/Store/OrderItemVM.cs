using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxShop.Models
{
    public class OrderItemVM
    {

        public int OrderId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Category { get; set; }
        public decimal Vat { get; set; }
        public int Quantity { get; set; }
        public int? ArticleGroup { get; set; }
        public DateTime DateOfPurchase { get; set; }
    }
}
