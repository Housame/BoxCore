using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.UserClient
{
    public class FirstUseVM
    {
        [Display(Name = "Lösenord:", Prompt = "Lösenord")]
        [Required(ErrorMessage = "Ange ett nytt lösenord")]
        [RegularExpression(@".{6,}$", ErrorMessage = "Lösenordet måste vara minst sex tecken långt.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Ange samma lösenord som ovan.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [Required]
        [Display(Name = "Födelsedatum:", Prompt = "Födelsedatum")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }
        [Required(ErrorMessage = "Ange stad")]
        [Display(Name = "Stad:", Prompt = "Stad")]
        public string Location { get; set; }
        [Required]
        [Display(Name = "Kön:", Prompt = "Kön")]
        public ShirtSizes Gender { get; set; }
        [Display(Name = "Tröjstorlek:", Prompt = "Tröjstorlek")]
        [Required(ErrorMessage = "Måste välja ett alternativ")]
        public SingleGenders ShirtSize { get; set; }
        public string UserId { get; set; }
        public string ResetPasswordToken { get; set; }
    }
}
