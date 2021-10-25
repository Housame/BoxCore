using EventHub.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.Competition
{
    public class AddResultsVM
    {
        public SubCompetition SubCompMember { get; set; }
        public List<CompEventVM> CompEvent { get; set; }
    }
}
