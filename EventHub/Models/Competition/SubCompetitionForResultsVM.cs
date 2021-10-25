using EventHub.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.Competition
{

    public class SubCompetitionForResultsVM
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public SubCompetitionTypes Type { get; set; }
        public Difficulties Difficulty { get; set; }
        public Genders Gender { get; set; }
        public virtual CompetitorResult CompetitorResult { get; set; }
        public virtual List<Reservation> Reservation { get; set; }
        public virtual List<UsersToSubCompetitionsAndCompetitors> UsersToSubCompetitionsAndCompetitors { get; set; }
        public virtual List<CompEventVM> CompEvent { get; set; }

    }



}

