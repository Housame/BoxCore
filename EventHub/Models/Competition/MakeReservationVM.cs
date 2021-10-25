using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.Competition
{
    public class MakeReservationVM
    {
        public IEnumerable<CompetitionVM> Competitions { get; set; }
        public FilterCompetitionsVM Filter { get; set; }
        public class CompetitionVM
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Description { get; set; }
            public string Location { get; set; }
            public string CreatorId { get; set; }
            public bool OpenForBookings { get; set; }
            public virtual ICollection<SubCompetitionVM> SubCompetition { get; set; }
        }
        public class SubCompetitionVM
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string Gender { get; set; }
            public Difficulties Difficulty { get; set; }
            public int Quantity { get; set; }
            public int? QuantityPerTeam { get; set; }
            public int? ConfirmedParticipants { get; set; }
            public decimal Price { get; set; }
            public int CompetitionId { get; set; }
            public string Type { get; set; }
            public bool IsBooked { get; set; }
            public bool IsFullyBooked { get; set; }
        }
    }
 
}
