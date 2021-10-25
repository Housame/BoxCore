using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxShop.Models.SuperAdmin
{
    public class RegisterBoxVM
    {
        public string Name { get; set; }
        public IFormFile FormImage { get; set; }
        public Byte[] Image { get; set; }
    }
}
