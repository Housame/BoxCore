using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.UserClient
{
    public class RegisterUserVM
    {
        [MaxLength(65, ErrorMessage = "Max längd är 65 bokstäver.")]
        public string FirstName { get; set; }

        [MaxLength(65, ErrorMessage = "Max längd är 65 bokstäver.")]
        public string LastName { get; set; }

        public string PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Felaktig e-post.")]
        public string Email { get; set; }

        public string ConfirmEmail { get; set; }

        [Range(typeof(bool), "true", "true", ErrorMessage = "Du måste godkänna användarvillkor!")]
        public bool TermsAndConditions { get; set; }
        public bool AllowNewsLetter { get; set; }
    }
}
