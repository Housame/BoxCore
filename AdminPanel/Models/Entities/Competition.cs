using System;
using System.Collections.Generic;

namespace AdminPanel.Models.Entities
{
    public partial class Competition
    {
        public Competition()
        {
            SubCompetition = new HashSet<SubCompetition>();
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
    }
}
