using System;
using System.Collections.Generic;

namespace AdminPanel.Models.Entities
{
    public partial class Reservation
    {
        public int UserId { get; set; }
        public int SubCompetitionId { get; set; }
        public DateTime ReservationDate { get; set; }
        public string Type { get; set; }
        public string Reference { get; set; }
        public string PaymentSessionUrl { get; set; }

        public SubCompetition SubCompetition { get; set; }
        public User User { get; set; }
    }
}
