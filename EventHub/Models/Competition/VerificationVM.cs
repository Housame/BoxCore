using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.Competition
{
    public class VerificationVM
    {
        public string EventName { get; set; }
        public DateTime EventStartDate { get; set; }
        public DateTime EventEndDate { get; set; }
        public string EventLocation { get; set; }
        public DateTime SubEventDate { get; set; }
        public string ReferenceNumber { get; set; }
    }
}
