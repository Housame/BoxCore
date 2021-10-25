using System;
using System.Collections.Generic;

namespace AdminPanel.Models.Entities
{
    public partial class SubEvent
    {
        public SubEvent()
        {
            CompetitorResult = new HashSet<CompetitorResult>();
            Exercise = new HashSet<Exercise>();
        }

        public int Id { get; set; }
        public int CompEventId { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string Title { get; set; }
        public TimeSpan? SetUpTime { get; set; }
        public TimeSpan? TimeCap { get; set; }
        public int? TotalReps { get; set; }

        public CompEvent CompEvent { get; set; }
        public ICollection<CompetitorResult> CompetitorResult { get; set; }
        public ICollection<Exercise> Exercise { get; set; }
    }
}
