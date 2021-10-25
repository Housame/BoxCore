using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Models.UserClient
{
    public class UserStateMgr
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string ConfirmEmail { get; set; }
        public string DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string ShirtSize { get; set; }
        public string Location { get; set; }
        public string Box { get; set; }
        public string Role { get; set; }

        public string ResetPasswordToken { get; set; }
        public string UserId { get; set; }

        public string BoxName { get; set; }
        public string AllowNewsletter { get; set; }

        public string TermsAndConditions { get; set; }

        public UserStateMgr()
        {
            this.FirstName = "firstNameKey";
            this.LastName = "lastNameKey";
            this.PhoneNumber = "phoneKey";
            this.Email = "emailKey";
            this.ConfirmEmail = "confirmEmailKey";
            this.DateOfBirth = "dateOfBirthKey";
            this.Gender = "genderKey";
            this.ShirtSize = "shirtSizeKey";
            this.Location = "locationKey";
            this.Box = "boxKey";
            this.Role = "roleKey";
            this.ResetPasswordToken = "resetPasswordTokenKey";
            this.UserId = "userIdKey";
            this.AllowNewsletter = "AllowNewsletterKey";
            this.TermsAndConditions = "TermsAndConditionKey";
            this.BoxName = "BoxNameKey";
        }
    }
}
