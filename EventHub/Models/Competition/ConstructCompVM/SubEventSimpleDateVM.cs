using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.Competition
{
    public class SubEventSimpleDateVM
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public int SetUpTimeMinutes { get; set; }
        public int SetUpTimeSeconds { get; set; }
        public int TimeCapMinutes { get; set; }
        public int TimeCapSeconds { get; set; }
        public int? TotalReps { get; set; }
    }
}
