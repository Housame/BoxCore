using System;
using System.Collections.Generic;

namespace EventHub.Models.Entities
{
    public partial class Reservation
    {
        public int UserId { get; set; }
        public int SubCompetitionId { get; set; }
        public DateTime ReservationDate { get; set; }
        public string Type { get; set; }
        public string Reference { get; set; }
        public string PaymentSessionUrl { get; set; }
        public int? TransactionId { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Paid { get; set; }

        public SubCompetition SubCompetition { get; set; }
        public Transaction Transaction { get; set; }
        public User User { get; set; }
    }
}
