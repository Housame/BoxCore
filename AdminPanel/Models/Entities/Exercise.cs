using System;
using System.Collections.Generic;

namespace AdminPanel.Models.Entities
{
    public partial class Exercise
    {
        public int Id { get; set; }
        public int SubEventId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public TimeSpan? Transition { get; set; }
        public bool? TieBreak { get; set; }

        public SubEvent SubEvent { get; set; }
    }
}
