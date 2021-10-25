using System;
using System.Collections.Generic;

namespace BoxShop.Models.Entities
{
    public partial class Product
    {
        public int Id { get; set; }
        public int? Category { get; set; }
        public decimal Vat { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public byte[] Image { get; set; }
        public string Description { get; set; }
        public bool? Available { get; set; }
        public int? BoxId { get; set; }
       

        public virtual Box Box { get; set; }
       
    }
}
