using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class DiscountIndexVM
    {
        public List<ExistingCompetition> ExistingCompetitions { get; set; }
        public List<ExistingDiscountCode> ExistingDiscountCodes { get; set; }
    }

    public class ExistingCompetition
    {
        public int CompId { get; set; }
        public string CompName { get; set; }
    }

    public class ExistingDiscountCode
    {
        public string CompName { get; set; }
        public int SubCompId { get; set; }
        public string SubCompName { get; set; }
        public List<DiscountCode> DiscountCodes { get; set; }
    }

    public class DiscountCode
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public int? PercentageOff { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

}
