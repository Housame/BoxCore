using System;
using System.Collections.Generic;

namespace AdminPanel.Models.Entities
{
    public partial class CompEvent
    {
        public CompEvent()
        {
            CompetitorResult = new HashSet<CompetitorResult>();
            SubEvent = new HashSet<SubEvent>();
        }

        public int Id { get; set; }
        public int SubCompetitionId { get; set; }
        public string Title { get; set; }

        public SubCompetition SubCompetition { get; set; }
        public ICollection<CompetitorResult> CompetitorResult { get; set; }
        public ICollection<SubEvent> SubEvent { get; set; }
    }
}
