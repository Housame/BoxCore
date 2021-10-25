using BoxShop.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoxShop.Models
{
    public class AddOrderVM
    {
        [Required]
        public int UserId { get; set; }
        [Required]
        public int BoxId { get; set; }
        [Required]
        public virtual ICollection<OrderItemVM> OrderItem { get; set; }
    }
}
