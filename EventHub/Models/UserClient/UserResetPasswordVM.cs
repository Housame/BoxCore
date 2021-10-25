using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.UserClient
{
    public class UserResetPasswordVM
    {

        [Required(ErrorMessage = "Ange ett nytt lösenord")]
        [RegularExpression(@".{6,}$", ErrorMessage = "Lösenordet måste vara minst sex tecken långt.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string UserId { get; set; }
        public string ResetPasswordToken { get; set; }
    }
}
