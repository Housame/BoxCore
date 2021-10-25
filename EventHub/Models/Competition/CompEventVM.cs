using EventHub.Models.Competition.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.Competition
{
    public class CompEventVM
    {
        public int Id { get; set; }
        public string Title { get; set; }
        
        public List<SubEventVM> SubEvents { get; set; }
        public class SubEventVM
        {
            public int Id { get; set; }
            public ScoreTypes Type { get; set; }
            public TimeSpan? SetUpTime { get; set; }
            public TimeSpan? TimeCap { get; set; }
            public int? TotalReps { get; set; }

        }
    }
}
