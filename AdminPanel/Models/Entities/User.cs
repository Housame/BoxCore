using System;
using System.Collections.Generic;

namespace AdminPanel.Models.Entities
{
    public partial class User
    {
        public User()
        {
            Reservation = new HashSet<Reservation>();
            UsersToSubCompetitionsAndCompetitors = new HashSet<UsersToSubCompetitionsAndCompetitors>();
        }

        public int Id { get; set; }
        public string Team { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public byte[] ProfileImage { get; set; }
        public string HashId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string Location { get; set; }
        public int? BoxId { get; set; }
        public string Size { get; set; }
        public string Email { get; set; }
        public bool? IsAllowingNewsLetter { get; set; }

        public ICollection<Reservation> Reservation { get; set; }
        public ICollection<UsersToSubCompetitionsAndCompetitors> UsersToSubCompetitionsAndCompetitors { get; set; }
    }
}
