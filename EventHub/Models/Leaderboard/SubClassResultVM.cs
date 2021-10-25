using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class SubClassResultVM
    {
        public List<Title> Titles { get; set; }
        public List<TeamResult> TeamResults { get; set; }
    }

    public class Title
    {
        public string Event { get; set; }
        public string Type { get; set; }
    }

    public class TeamResult
    {
        public int TeamId { get; set; }
        public string Name { get; set; }
        public List<Member> Members { get; set; }
        public List<Score> Scores { get; set; }
        public decimal TotalScore { get; set; }
    }

    public class Member
    {
        public int? UserId { get; set; }
        public int AthleteId { get; set; }
        public string Name { get; set; }
        public bool PublicProfile  { get; set; }
    }

    public class Score
    {
        public string Type { get; set; }
        public decimal? Event { get; set; }
        public string EventTitle { get; set; }
    }

}