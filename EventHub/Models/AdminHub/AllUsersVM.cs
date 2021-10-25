using EventHub.Models.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EventHub.Models.AdminHub
{
    public class AllUsersVM
    {
        public User BCUser { get; set; }
        public IdentityUser User { get; set; }
        public IList<string> Role { get; set; }
        public IList<Claim> Claims { get; set; }
        public string BoxName { get; set; }
        public int? BoxId { get; set; }
    }
}
