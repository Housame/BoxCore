using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class SubEventVM
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public TimeSpan? SetUpTime { get; set; }
        public TimeSpan? TimeCap { get; set; }
        public int? TotalReps { get; set; }
        public List<ExerciseVM> Exercise { get; set; }
    }
}
