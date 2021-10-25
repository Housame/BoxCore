using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.ConstructComp
{
    public class SubEventVM
    {
        public int EventTempId { get; set; }
        public int SubEventTempId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public TimeSpan? TimeCap { get; set; }
        public int? TotalReps { get; set; }
        public TimeSpan? SetUpTime { get; set; }
    }
}
