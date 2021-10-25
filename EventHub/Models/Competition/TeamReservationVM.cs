using EventHub.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class TeamReservationVM: IReservation
    {
        public int UserId { get; set; }
        [Required]
        public int SubCompId { get; set; }
        public DateTime ReservationDate { get; set; }
        public decimal Price { get; set; }
        public string EventName { get; set; }
        public string Reference { get; set; }
        public string PaymentSessionUrl { get; set; }
        public int? QuantityPerTeam { get; set; }
        public Genders Gender { get; set; }
        public decimal? Discount { get; set; }
        public decimal? Paid { get; set; }

    }
}
