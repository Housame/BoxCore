using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.ConstructComp
{
    public class EventVM
    {
        public int EventTempId { get; set; }
        public string Title { get; set; }
        public int SubCompetitionId { get; set; }    
        public List<SubEventVM> SubEvents { get; set; }
    }
}
