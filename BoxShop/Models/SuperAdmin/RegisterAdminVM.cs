using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BoxShop.Models.SuperAdmin
{
    public class RegisterAdminVM
    {
        [Required(ErrorMessage = "The first name field is required")]
        [RegularExpression(@"^[a-zåäöA-ZÅÄÖ]+$", ErrorMessage = "First name can only contain letters")]
        [MaxLength(65, ErrorMessage = "Max length is 65 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "The last name field is required")]
        [MaxLength(65, ErrorMessage = "Max length is 65 characters")]
        [RegularExpression(@"^[a-zåäöA-ZÅÄÖ]+$", ErrorMessage = "Last name can only contain letters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "The email field is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email adress")]
        public string Email { get; set; }

        [Display(Name = "CrossFit Box")]
        public List<SelectListItem> BoxNames { get; set; }
        

        //[Required(ErrorMessage = "Please select a Box")]
        public string Box { get; set; }
        public bool Creator { get; set; }
        public string Role { get; set; }
    }
}
