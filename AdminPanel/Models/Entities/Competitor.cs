using System;
using System.Collections.Generic;

namespace AdminPanel.Models.Entities
{
    public partial class Competitor
    {
        public Competitor()
        {
            CompetitorResult = new HashSet<CompetitorResult>();
            UsersToSubCompetitionsAndCompetitors = new HashSet<UsersToSubCompetitionsAndCompetitors>();
        }

        public int Id { get; set; }
        public string Gender { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public bool IsCheckedIn { get; set; }

        public Athlete Athlete { get; set; }
        public Team Team { get; set; }
        public ICollection<CompetitorResult> CompetitorResult { get; set; }
        public ICollection<UsersToSubCompetitionsAndCompetitors> UsersToSubCompetitionsAndCompetitors { get; set; }
    }
}
