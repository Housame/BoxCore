using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxShop.Models
{
    public class ProductAddVM
    {
        public int Id { get; set; }
        public Categories Category { get; set; }
        public decimal Vat { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public IFormFile FormImage { get; set; }
        public Byte[] Image { get; set; }
        public string Description { get; set; }
        public bool? Available { get; set; }
    }
}
