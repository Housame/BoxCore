using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.UserClient
{
    public class RetrievePasswordVM
    {
        // Email
        [Display(Name = "Email Address:", Prompt = "you@yours.com")]
        [Required(ErrorMessage = "*Obligatorisk")]
        [EmailAddress]
        public string Email { get; set; }
        public bool Modal { get; set; } = false;
    }
}
