using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models
{
    public class UserLoginVM
    {
        [EmailAddress(ErrorMessage = "Felaktig e-post.")]
        public string UserName { get; set; }
        //[Required(ErrorMessage = "Lösenord saknas.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberUser { get; set; }
        public bool Modal { get; set; } = false;
    }
}
