using EventHub.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class AddAthleteSubCompVM
    {
        // User creating event
        public CurrentUser CurrentEventUser { get; set; }
        // Event
        public CompetitionInfoVM Competition { get; set; }

        public class CompetitionInfoVM
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

            public virtual SubCompetitionInfoVM SubCompetition { get; set; }
        }
        public class SubCompetitionInfoVM
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string Type { get; set; }
            public string Difficulty { get; set; }
            public string Gender { get; set; }
            public int? QuantityPerTeam { get; set; }
            public virtual List<UsersToSubCompetitionsAndCompetitors> UsersToSubCompetitionsAndCompetitors { get; set; }

        }
        public class CurrentUser
        {
            public int UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            //public ShirtSizes ShirtSize { get; set; }
            public string TeamName { get; set; }
        }
    }
}


