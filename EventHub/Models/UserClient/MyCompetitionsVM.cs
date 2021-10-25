using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.UserClient
{
    public class MyCompetitionsVM
    {
        public CompetitionVM Competition { get; set; }

        public class CompetitionVM
        {
            public int Id { get; set; }
            public string CompName { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
            public string Description { get; set; }
            public string Location { get; set; }
            public virtual ICollection<SubCompetitionVM> SubCompetitions { get; set; }

        }
        public class SubCompetitionVM
        {
            public int Id { get; set; }
            public DateTime Date { get; set; }
            public string Description { get; set; }
            public string Type { get; set; }
            public decimal Cost { get; set; }
            public int? MaxAttendants { get; set; }
            public string Location { get; set; }
            public int? QuantityPerTeam { get; set; }
        }
    }


}
