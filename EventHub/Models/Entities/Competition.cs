using System;
using System.Collections.Generic;

namespace EventHub.Models.Entities
{
    public partial class Competition
    {
        public Competition()
        {
            SubCompetition = new HashSet<SubCompetition>();
            Transaction = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Description { get; set; }
        public string Location { get; set; }
        public string CreatorId { get; set; }
        public int? BoxId { get; set; }
        public bool Published { get; set; }
        public bool OpenForBookings { get; set; }

        public ICollection<SubCompetition> SubCompetition { get; set; }
        public ICollection<Transaction> Transaction { get; set; }
    }
}
