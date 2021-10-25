using System;
using System.Collections.Generic;

namespace EventHub.Models.Entities
{
    public partial class Transaction
    {
        public Transaction()
        {
            Reservation = new HashSet<Reservation>();
        }

        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Sum { get; set; }
        public int? BoxId { get; set; }
        public int CompetitionId { get; set; }
        public string PricePlan { get; set; }
        public decimal Debt { get; set; }
        public decimal Credit { get; set; }
        public string Reference { get; set; }
        public decimal Discount { get; set; }

        public Competition Competition { get; set; }
        public ICollection<Reservation> Reservation { get; set; }
    }
}
