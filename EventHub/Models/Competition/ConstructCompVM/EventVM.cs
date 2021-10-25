using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class EventVM
    {
        public int SubCompetitionId { get; set; }
        public string Title { get; set; }
        public List<SubEventVM> SubEvent { get; set; }
    }
}
