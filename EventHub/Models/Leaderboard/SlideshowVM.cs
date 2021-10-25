using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class SlideshowVM
    {
        public string CompetitionName { get; set; }
        public List<string> SubCompNames { get; set; }
        public List<SubClassResultVM> leaderboardResult { get; set; }
    }
}
