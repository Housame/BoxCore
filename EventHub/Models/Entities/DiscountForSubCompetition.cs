using System;
using System.Collections.Generic;

namespace EventHub.Models.Entities
{
    public partial class DiscountForSubCompetition
    {
        public int DiscountId { get; set; }
        public int SubCompetitionId { get; set; }

        public Discount Discount { get; set; }
        public SubCompetition SubCompetition { get; set; }
    }
}
