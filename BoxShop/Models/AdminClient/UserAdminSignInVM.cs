using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoxShop.Models
{
    public class UserAdminSignInVM
    {
        [Display(Name = "Användarnamn")]
        [Required(ErrorMessage = "Användarnamnet måste anges")]
        public string UserName { get; set; }

        [Display(Name = "Lösenord")]
        [Required(ErrorMessage = "Lösenordet måste anges")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
