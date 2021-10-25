using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.Competition
{
    public class UpdateResultVM
    {
        public string SubEventId { get; set; }
        public string CompetitorId { get; set; }
        public string CompEventId { get; set; }
        public string RepScore { get; set; }
        public string WeightScore { get; set; }
        public string TimeScore { get; set; }
        public string PointScore { get; set; }
        public string TieBreak { get; set; }
        public string Type { get; set; }
    }
}
