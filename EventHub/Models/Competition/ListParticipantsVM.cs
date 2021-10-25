using EventHub.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.Competition
{
    public class ListParticipantsVM
    {
        public CompetitionVM Competition { get; set; }
        public class CompetitionVM
        {
            public int Id { get; set; }
            // Event name
            public string Name { get; set; }
            //  StartDate
            public DateTime StartDate { get; set; }
            //  EndDate
            public DateTime EndDate { get; set; }
            // Location
            public string Location { get; set; }
            // Description
            public string Description { get; set; }
            public string CreatorId { get; set; }
            public virtual List<SubCompetitionVM> SubCompetition { get; set; }
        }
        public class SubCompetitionVM
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public SubCompetitionTypes Type { get; set; }
            public Difficulties Difficulty { get; set; }
            public Genders Gender { get; set; }
            public virtual List<Reservation> Reservation { get; set; }
            public virtual List<UsersToSubCompetitionsAndCompetitors> UsersToSubCompetitionsAndCompetitors { get; set; }


        }
        

       
    }
}
