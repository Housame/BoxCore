using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.Competition
{
    public class ExerciseSimpleTimeVM
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }
        public bool? TieBreak { get; set; }
    }
}
