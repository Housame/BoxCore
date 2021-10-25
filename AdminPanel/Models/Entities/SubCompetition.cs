using System;
using System.Collections.Generic;

namespace AdminPanel.Models.Entities
{
    public partial class SubCompetition
    {
        public SubCompetition()
        {
            CompEvent = new HashSet<CompEvent>();
            Reservation = new HashSet<Reservation>();
            UsersToSubCompetitionsAndCompetitors = new HashSet<UsersToSubCompetitionsAndCompetitors>();
        }

        public int Id { get; set; }
        public string Gender { get; set; }
        public string Difficulty { get; set; }
        public int Quantity { get; set; }
        public int? QuantityPerTeam { get; set; }
        public int? ConfirmedParticipants { get; set; }
        public decimal Price { get; set; }
        public int CompetitionId { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }

        public Competition Competition { get; set; }
        public ICollection<CompEvent> CompEvent { get; set; }
        public ICollection<Reservation> Reservation { get; set; }
        public ICollection<UsersToSubCompetitionsAndCompetitors> UsersToSubCompetitionsAndCompetitors { get; set; }
    }
}
