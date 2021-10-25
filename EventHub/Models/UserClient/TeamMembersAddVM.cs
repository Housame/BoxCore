using EventHub.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.UserClient
{
    public class TeamMembersAddVM
    {
        public int SubCompId { get; set; }
        public string TeamName { get; set; }
        public List<string> MembersId { get; set; }
        public int QunatityPerTeam { get; set; }
        public bool RemoveUserOrNot { get; set; }
        public virtual List<string> ListOfUserToDelete { get; set; }
    }
}
