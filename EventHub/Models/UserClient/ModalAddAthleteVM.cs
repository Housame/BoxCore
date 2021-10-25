using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.UserClient
{
    public class ModalAddAthleteVM
    {
        public AddAthleteSubCompVM CompInfo { get; set; }
        public List<AllUsersVM> AllUsers { get; set; }
    }
}
