using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.Interfaces
{
    public interface IReservation
    {
        int UserId { get; set; }
        int SubCompId { get; set; }
        decimal Price { get; set; }
        decimal? Discount { get; set; }
        decimal? Paid { get; set; }
        string EventName { get; set; }
        DateTime ReservationDate {get; set;}
        string Reference { get; set; }
        string PaymentSessionUrl { get; set; }
        Genders Gender { get; set; }
    }
}
