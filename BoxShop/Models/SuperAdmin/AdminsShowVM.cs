using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxShop.Models.SuperAdmin
{
    public class AdminsShowVM
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Box { get; set; }
        public int BoxId { get; set; }
        public string Role { get; set; }
    }
}
