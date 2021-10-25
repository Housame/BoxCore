using System;
using System.Collections.Generic;

namespace EventHub.Models.Entities
{
    public partial class AthleteInTeam
    {
        public int AthleteId { get; set; }
        public int TeamId { get; set; }

        public Athlete Athlete { get; set; }
        public Team Team { get; set; }
    }
}
