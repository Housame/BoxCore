using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.AdminHub
{
    public class CreateUserVM
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string Location { get; set; }
        public int? BoxId { get; set; }
        public string Size { get; set; }
        public string Team { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string[] Claims { get; set; }
    }
}
