using System;
using System.Collections.Generic;

namespace AdminPanel.Models.Entities
{
    public partial class Team
    {
        public Team()
        {
            AthleteInTeam = new HashSet<AthleteInTeam>();
        }

        public int Id { get; set; }
        public string TeamName { get; set; }
        public string Gender { get; set; }

        public Competitor IdNavigation { get; set; }
        public ICollection<AthleteInTeam> AthleteInTeam { get; set; }
    }
}
