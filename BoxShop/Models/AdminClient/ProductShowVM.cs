using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxShop.Models
{
    public class ProductShowVM
    {
        public int Id { get; set; }
        public int? Category { get; set; }
        public decimal Vat { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public byte[] Image { get; set; }
        public string Description { get; set; }
        public bool? Available { get; set; }
    }
}
