using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class ListProfileDetailsVM
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        public string Location { get; set; }
        public string Email { get; set; }
        public string Box { get; set; }
        public string Gender { get; set; }
        public CompetitionProfileView[] Competitions { get; set; }
        public byte[] ProfileImage { get; set; }
        public string ShirtSize { get; set; }
    }

    public class CompetitionProfileView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        public string Difficulty { get; set; }
        public decimal Cost { get; set; }
        public int? MaxAttendants { get; set; }
        public string Location { get; set; }
        public string Alias { get; set; }
    }
}
