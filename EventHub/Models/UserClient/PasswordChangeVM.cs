using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.UserClient
{
    public class PasswordChangeVM
    {
        [Required(ErrorMessage = "Ange ditt nuvarande lösenord")]
        [Display(Name = "Nuvarande lösenord")]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Display(Name = "Nytt lösenord")]
        [DataType(DataType.Password)]
        [RegularExpression(@".{6,}$", ErrorMessage = "Lösenordet måste vara minst sex tecken långt.")]
        public string NewPassword { get; set; }

        [Display(Name = "Bekräfta det nya lösenordet")]
        [DataType(DataType.Password)]
        public string ConfirmNewPassword { get; set; }
    }
}
