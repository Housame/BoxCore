using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class NewTransactionVM
    {
        public string[] References { get; set; }
        public DateTime Date { get; set; }
        public string Credit { get; set; }
        public string Sum { get; set; }
        public string Debt { get; set; }
        public string Discount { get; set; }
        public int CompetitionId { get; set; }
        public string PricePlan { get; set; }
        public string Reference { get; set; }
    }
}
