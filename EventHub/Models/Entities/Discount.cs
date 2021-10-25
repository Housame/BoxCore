using System;
using System.Collections.Generic;

namespace EventHub.Models.Entities
{
    public partial class Discount
    {
        public Discount()
        {
            DiscountForSubCompetition = new HashSet<DiscountForSubCompetition>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public int? PercentageOff { get; set; }
        public DateTime? ExpiryDate { get; set; }

        public ICollection<DiscountForSubCompetition> DiscountForSubCompetition { get; set; }
    }
}
