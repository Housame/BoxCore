using System;
using System.Collections.Generic;

namespace EventHub.Models.Entities
{
    public partial class CompetitorResult
    {
        public int SubEventId { get; set; }
        public int CompetitorId { get; set; }
        public int CompEventId { get; set; }
        public int? RepScore { get; set; }
        public decimal? WeightScore { get; set; }
        public TimeSpan? TimeScore { get; set; }
        public decimal? PointScore { get; set; }
        public decimal? Score { get; set; }
        public int? TieBreak { get; set; }

        public CompEvent CompEvent { get; set; }
        public Competitor Competitor { get; set; }
        public SubEvent SubEvent { get; set; }
    }
}
