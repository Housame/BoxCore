using System;
using System.Collections.Generic;

namespace AdminPanel.Models.Entities
{
    public partial class Athlete
    {
        public Athlete()
        {
            AthleteInTeam = new HashSet<AthleteInTeam>();
        }

        public int Id { get; set; }
        public string Size { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public string Email { get; set; }
        public bool IsCheckedIn { get; set; }

        public Competitor IdNavigation { get; set; }
        public ICollection<AthleteInTeam> AthleteInTeam { get; set; }
    }
}
