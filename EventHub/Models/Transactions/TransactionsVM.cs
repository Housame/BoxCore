using EventHub.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class CompetitionTransactionsVM
    {
        [HiddenInput]
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }       
        public string Reference { get; set; }
        public List<TransactionsVM> Transaction { get; set; }
        public TransactionBriefVM Brief { get; set; }
        public List<TransactionReservationsVM> Reservation { get; set; }
        public List<TransactionSubCompetitionVM> SubCompetition { get; set; }

    }
    public class TransactionBriefVM
    {
        public int TotalReservations { get; set; }
        public int ReservationsTransacted { get; set; }
        public decimal TotalAccumulation { get; set; }
        public int NumberOfTransactions { get; set; }
        public decimal TotalCredits { get; set; }
        public decimal TotalDiscounts { get; set; }
        public decimal PayOut { get; set; }
    }
    public class TransactionsVM
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Sum { get; set; }
        public int BoxId { get; set; }
        public int CompetitionId { get; set; }
        public string PricePlan { get; set; }
        public decimal Debt { get; set; }
        public decimal Credit { get; set; }
        public string Reference { get; set; }
        public decimal Discount { get; set; }
        public List<Reservation> Reservation { get; set; }
    }
    public class TransactionReservationsVM
    {
        public string Reference { get; set; }
        public DateTime Date { get; set; }
        public bool IsChecked { get; set; }
        public int? TransactionId { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Paid { get; set; }
        public string Instrument { get; set; }
        public string PaymentSessionUrl { get; set; }
    }
   public class TransactionSubCompetitionVM
    {
        public int Id { get; set; }
        public decimal Price { get; set; }
    }
}
