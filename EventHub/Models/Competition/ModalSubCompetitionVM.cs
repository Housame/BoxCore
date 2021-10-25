using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class ModalSubCompetitionVM
    {
        // User creating event
        public EventUser CurrentEventUser { get; set; }
        // Event
        public string EventName { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string EventLocation { get; set; }
        // Sub Event
        public int SubEventId { get; set; }
        public DateTime SubEventDate { get; set; }
        public string Gender { get; set; }
        public string Difficulty { get; set; }
        public string Type { get; set; }
        public int? QuantityPerTeam { get; set; }
        public decimal Price { get; set; }
        public string PaymentSessionUrl { get; set; }
    }

    public class EventUser
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ShirtSizes ShirtSize { get; set; }
        public string TeamName { get; set; }
    }
}
