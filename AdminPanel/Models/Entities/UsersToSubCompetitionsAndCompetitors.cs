using System;
using System.Collections.Generic;

namespace AdminPanel.Models.Entities
{
    public partial class UsersToSubCompetitionsAndCompetitors
    {
        public int UserId { get; set; }
        public int CompetitorId { get; set; }
        public int SubCompetitionId { get; set; }

        public Competitor Competitor { get; set; }
        public SubCompetition SubCompetition { get; set; }
        public User User { get; set; }
    }
}
