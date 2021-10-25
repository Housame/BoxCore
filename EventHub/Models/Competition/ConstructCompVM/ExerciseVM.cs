using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class ExerciseVM
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public TimeSpan? Transition { get; set; }
        public bool? TieBreak { get; set; }
    }
}
