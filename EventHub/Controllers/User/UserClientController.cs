using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using EventHub.Models.Entities;
using EventHub.Models.UserClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using EventHub.Models;
using System.IO;
using EventHub.Helpers;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventHub.Controllers
{
    public class UserClientController : Controller
    {
        #region Fields
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signInManager;
        IdentityDbContext identity;
        EventHubContext context;
        RoleManager<IdentityRole> roleManager;
        IMailService mailService;
        UserStateMgr state = new UserStateMgr();
        #endregion

        #region Constructors
        public UserClientController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IdentityDbContext identity,
            EventHubContext context, RoleManager<IdentityRole> roleManager,
            IMailService mailService)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.identity = identity;
            this.context = context;
            this.roleManager = roleManager;
            this.mailService = mailService;
        }
        #endregion

        #region Register
        // GET: /<controller>/

        [HttpGet]
        public IActionResult RegisterUser()
        {
            var model = new RegisterUserVM();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RegisterUser(RegisterUserVM viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);

            if (!String.Equals(viewModel.Email, viewModel.ConfirmEmail))
            {
                TempData["OperationFailed"] = "Mailadresserna som du har angett stämmer inte överens.";
                return View(viewModel);
            }

            var emailExists = await userManager.FindByEmailAsync(viewModel.Email);
            if (emailExists != null)
            {
                TempData["OperationFailed"] = "Mailadressen som du har angett är redan registrerad hos BoxCore.";
                return View(viewModel);
            }

            HttpContext.Session.SetString(state.FirstName, viewModel.FirstName);
            HttpContext.Session.SetString(state.LastName, viewModel.LastName);
            HttpContext.Session.SetString(state.PhoneNumber, viewModel.PhoneNumber);
            HttpContext.Session.SetString(state.Email, viewModel.Email);
            HttpContext.Session.SetString(state.ConfirmEmail, viewModel.ConfirmEmail);
            HttpContext.Session.SetString(state.Role, "Client");
            HttpContext.Session.SetString(state.AllowNewsletter, viewModel.AllowNewsLetter.ToString());
            HttpContext.Session.SetString(state.TermsAndConditions, viewModel.TermsAndConditions.ToString());


            return RedirectToAction(nameof(UserClientController.ConfirmUserRegistration));
        }
        [HttpGet]
        public IActionResult ConfirmUserRegistration()
        {
            RegisterUserVM viewModel = new RegisterUserVM();
            viewModel = UserRegisterModelCasting(viewModel);
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> ConfirmUserRegistration(RegisterUserVM viewModel)
        {
            viewModel = UserRegisterModelCasting(viewModel);
            #region Create user
            #region idea about UserName
            //Här kan man byta till användarnamn istället, kanske ta emot ett användarnamn 
            //OBS! FirstName ska inte vara det eftersom det ska vara unikt
            //För att kunna ta emot ett användarnamn måste viewModel ha det + att göra en check om det finns
            #endregion
            var user = new IdentityUser(viewModel.Email);
            await userManager.SetEmailAsync(user, viewModel.Email);

            await userManager.SetPhoneNumberAsync(user, viewModel.PhoneNumber);

            string password = GenerateRandomString(15);
            await userManager.AddPasswordAsync(user, password);

            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("FirstName", result.Errors.First().Description);
                return View(viewModel);
            }

            await userManager.AddToRoleAsync(user, "Client");

            var confirm = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "UserClient", new { userId = user.Id, code = confirm },
                protocol: HttpContext.Request.Scheme);

            #endregion        
            mailService.SendMail(MailCategory.Confirmation, viewModel.Email, callbackUrl, viewModel.FirstName, viewModel.LastName, password);
            var userHashId = await userManager.GetUserIdAsync(user);
            context.CreateUser(viewModel, userHashId);

            TempData["UserName"] = viewModel.FirstName;

            return RedirectToAction(nameof(UserClientController.UserCreated));
        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            var user = await userManager.FindByIdAsync(userId);

            var result = await userManager.ConfirmEmailAsync(user, code);

            var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("FirstUsePassword", "UserClient", new { userId = userId, code = passwordResetToken },
                protocol: HttpContext.Request.Scheme);
            ViewBag.Url = callbackUrl;
            return View();
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult FirstUsePassword(string userId, string code)
        {
            HttpContext.Session.SetString(state.ResetPasswordToken, code);
            HttpContext.Session.SetString(state.UserId, userId);

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> FirstUsePassword(FirstUseVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            if (!String.Equals(viewModel.Password, viewModel.ConfirmPassword))
            {
                TempData["OperationFailed"] = "Lösenordet du har angivit matchar inte.";
                return View(viewModel);
            }

            viewModel = FirstUsePasswordModelCasting(viewModel);
            var user = await userManager.FindByIdAsync(viewModel.UserId);
            var result = await userManager.ResetPasswordAsync(user, viewModel.ResetPasswordToken, viewModel.Password);

            if (!result.Succeeded)
            {
                TempData["OperationFailed"] = "Ett fel uppstod. Använd länken i utskickat mail med ämnesrad Verifiera din e-post och försök igen. Kontakta vår kundsupport om felet kvarstår.";
                return View(viewModel);
            }
            await signInManager.PasswordSignInAsync(user, viewModel.Password, false, false);
            var userToEditId = context.User.First(u => u.HashId == viewModel.UserId).Id;
            context.AddUserDetails(viewModel, userToEditId);
            return RedirectToAction(nameof(UserClientController.PasswordResetSuccess));
        }

        [HttpGet]
        public IActionResult UserCreated()
        {
            return View();
        }
        [HttpGet]
        public IActionResult RetrievePassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RetrievePassword(RetrievePasswordVM viewModel)
        {
            if (String.IsNullOrEmpty(viewModel.Email))
                return Json(new { mailSent = false, msg = "Var vänlig och fyll i en giltig e-post." });
            //HttpContext.Session.Clear();
            var email = viewModel.Email.ToString();
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                if (viewModel.Modal)
                {
                    // TempData["test"] = email + " finns inte i vårt register!";
                    return Json(new { mailSent = false, msg = email + " finns inte i vårt register!" });
                }
                else
                {
                    TempData["MailNotFound"] = email + " finns inte i vårt register!";
                    return View(viewModel);
                }
            }

            var passwordResetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "UserClient", new { userId = user.Id, code = passwordResetToken },
                protocol: HttpContext.Request.Scheme);

            mailService.SendMail(MailCategory.RetrievePassword, email, callbackUrl);

            if (viewModel.Modal)
            {
                return Json(new { mailSent = true, msg = "Var vänlig och använd länken i utskickat mail." });
            }
            else
            {
                TempData["MailSent"] = "Du kommer strax få ett mail med instruktioner om hur du återställer ditt lösenord.";
                return RedirectToAction(nameof(UserClientController.SignIn));
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string userId, string code)
        {
            HttpContext.Session.SetString(state.ResetPasswordToken, code);
            HttpContext.Session.SetString(state.UserId, userId);

            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ResetPassword(UserResetPasswordVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["OperationFailed"] = "Fyll i alla uppgifter.";
                return View();
            }

            if (!String.Equals(viewModel.ConfirmPassword, viewModel.Password))
            {
                TempData["OperationFailed"] = "Lösenorden matchade inte.";
                return View();
            }

            viewModel = ResetPasswordModelCasting(viewModel);
            var user = await userManager.FindByIdAsync(viewModel.UserId);
            var result = await userManager.ResetPasswordAsync(user, viewModel.ResetPasswordToken, viewModel.Password);

            if (!result.Succeeded)
            {
                TempData["InvalidToken"] = "true";
                return View();
            }
            await signInManager.PasswordSignInAsync(user, viewModel.Password, false, false);

            return RedirectToAction(nameof(UserClientController.PasswordResetSuccess));
        }

        [Authorize]
        [HttpGet]
        public IActionResult PasswordResetSuccess()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult ModalTerms()
        {
            return PartialView("_ModalTermsRegister");
        }
        #endregion

        #region Existing User
        // Signing in actions 
        [HttpGet]
        public IActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(UserLoginVM viewModel)
        {
            if (!ModelState.IsValid || String.IsNullOrEmpty(viewModel.Password) || String.IsNullOrEmpty(viewModel.UserName))
                return Json(new { loginSuccessful = false, msg = "Var vänlig och fyll i alla uppgifter." });
            else
            {
                var result = await signInManager.PasswordSignInAsync(viewModel.UserName, viewModel.Password, viewModel.RememberUser, false);

                if (result.Succeeded)
                {
                    if (viewModel.Modal)
                        return Json(new { loginSuccessful = true }); 
                    else
                        return RedirectToAction(nameof(ProfileController.Profile),"Profile");
                }
                else
                {
                    if (viewModel.Modal)
                        return Json(new { loginSuccessful = false, msg = "Felaktigt användarnamn eller lösenord." });
                    else
                    {
                        TempData["IncorrectCredentials"] = "Felaktigt användarnamn eller lösenord.";
                        return View(viewModel);
                    }
                }
            }
        }

        public async Task<IActionResult> LogOut()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("index", "home");
        }

        
        #region Profile settings

        [Authorize()]
        [HttpGet("changepassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }
        [Authorize()]
        [HttpPost("changepassword")]
        public async Task<IActionResult> ChangePassword(PasswordChangeVM viewModel)
        {
            if (!ModelState.IsValid)
            {
                TempData["OperationFailed"] = "Fyll i alla uppgifter.";
                return RedirectToAction(nameof(UserClientController.ChangePassword));
            }

            var user = await userManager.GetUserAsync(HttpContext.User);

            var isCorrectPassword = await userManager.CheckPasswordAsync(user, viewModel.CurrentPassword);

            if (!isCorrectPassword)
            {
                TempData["OperationFailed"] = "Ditt nuvarande lösenord stämmer inte.";
                return RedirectToAction(nameof(UserClientController.ChangePassword));
            }

            if (!String.Equals(viewModel.NewPassword, viewModel.ConfirmNewPassword))
            {
                TempData["OperationFailed"] = "Ditt nya lösenord stämmer inte överens.";
                return RedirectToAction(nameof(UserClientController.ChangePassword));
            }

            if (viewModel.NewPassword != null)
            {
                var isPasswordChanged = await userManager.ChangePasswordAsync(user, viewModel.CurrentPassword, viewModel.NewPassword);
                if (!isPasswordChanged.Succeeded)
                {
                    TempData["OperationFailed"] = "Ange ett nytt lösenord.";
                    return RedirectToAction(nameof(UserClientController.ChangePassword));
                }
            }

            return RedirectToAction(nameof(UserClientController.PasswordResetSuccess));
        }
        #endregion

        #region Client reservation
      

        [Authorize()]
        [HttpPost]
        public IActionResult AddAthletes(int id)
        {
            string userHashId = userManager.GetUserId(HttpContext.User);
            int currentUserId = context.User.SingleOrDefault(o => o.HashId.Equals(userHashId)).Id;
            ModalAddAthleteVM model = new ModalAddAthleteVM();
            model.CompInfo = context.GetCompetitionInfoAddAthlete(id, currentUserId);
            model.AllUsers = context.GetAllUsers(currentUserId);
            return PartialView("_AddAthletes", model);
        }

        [HttpPost]
        public IActionResult AddTeamMembers(TeamMembersAddVM viewModel)
        {
            string msg = "Se till att komplettera laget.";
            if ((viewModel.MembersId.Count + 1) != (viewModel.QunatityPerTeam) || viewModel.TeamName.Length == 0)
                return Json(new { success = false, responseText = msg });
            msg = "Ditt lag är nu komplett.";
            if (viewModel.RemoveUserOrNot == true)
            {
                context.RemoveCompetitorAsync(viewModel.ListOfUserToDelete, viewModel.SubCompId);
            }
            string userHashId = userManager.GetUserId(HttpContext.User);
            int currentUserId = context.User.SingleOrDefault(o => o.HashId.Equals(userHashId)).Id;
            if (viewModel.ListOfUserToDelete != null)
            {
                if (viewModel.ListOfUserToDelete[0] == viewModel.MembersId[0])
                {
                    context.ChangeTeamName(viewModel, currentUserId);
                }
                else
                {
                    context.AddTeamMembers(viewModel, currentUserId);
                }
            }
            else
            {
                context.AddTeamMembers(viewModel, currentUserId);
            }



            return Json(new { success = true, responseText = msg });
        }
        [HttpPost]
        public IActionResult SendInvitation(RetrievePasswordVM viewModel)
        {
            if ((String.IsNullOrEmpty(viewModel.Email)) || (!ModelState.IsValid))
                return Json(new { mailSent = false, msg = "Var vänlig och fyll i en giltig e-post." });
            //HttpContext.Session.Clear();
            var email = viewModel.Email.ToString();
            var currentUserHashId = userManager.GetUserId(HttpContext.User);

            string currentUserFName = context.User.SingleOrDefault(o => o.HashId.Equals(currentUserHashId)).FirstName;
            string currentUserLName = context.User.SingleOrDefault(o => o.HashId.Equals(currentUserHashId)).LastName;

            mailService.SendMail(MailCategory.SendInvitation, email, currentUserFName, currentUserLName);

            return Json(new { mailSent = true, msg = "Mailet är skickat!" });
        }
        #endregion

        #endregion

        #region helper
        private RegisterUserVM UserRegisterModelCasting(RegisterUserVM viewModel)
        {
            viewModel.FirstName = HttpContext.Session.GetString(state.FirstName);
            viewModel.LastName = HttpContext.Session.GetString(state.LastName);
            viewModel.PhoneNumber = HttpContext.Session.GetString(state.PhoneNumber);
            viewModel.Email = HttpContext.Session.GetString(state.Email);
            viewModel.ConfirmEmail = HttpContext.Session.GetString(state.ConfirmEmail);
            var allowNewsletter = HttpContext.Session.GetString(state.AllowNewsletter);
            if (allowNewsletter.ToLower() == "true")
            {
                viewModel.AllowNewsLetter = true;
            }
            else
            {
                viewModel.AllowNewsLetter = false;
            }

            var termsAndConditions = HttpContext.Session.GetString(state.TermsAndConditions);
            viewModel.TermsAndConditions = termsAndConditions.ToLower() == "true" ? true : false;

            return viewModel;
        }
        private string GenerateRandomString(int length)
        {
            //Removed O, o, 0, l, 1
            string allowedLetterChars = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";
            string allowedNumberChars = "123456789";
            char[] chars = new char[length];
            Random rd = new Random();

            bool useLetter = true;
            bool useToUpper = true;
            for (int i = 0; i < length; i++)
            {
                if (useLetter)
                {
                    if (useToUpper)
                    {
                        chars[i] = allowedLetterChars.ToUpper()[rd.Next(0, allowedLetterChars.Length)];
                        useToUpper = false;
                    }
                    else
                    {
                        chars[i] = allowedLetterChars[rd.Next(0, allowedLetterChars.Length)];
                        useToUpper = true;
                    }
                    useLetter = false;
                }
                else
                {
                    chars[i] = allowedNumberChars[rd.Next(0, allowedNumberChars.Length)];
                    useLetter = true;
                }

            }

            return new string(chars);
        }
        private UserResetPasswordVM ResetPasswordModelCasting(UserResetPasswordVM viewModel)
        {
            viewModel.UserId = HttpContext.Session.GetString(state.UserId);
            viewModel.ResetPasswordToken = HttpContext.Session.GetString(state.ResetPasswordToken);
            return viewModel;
        }
        private FirstUseVM FirstUsePasswordModelCasting(FirstUseVM viewModel)
        {
            viewModel.UserId = HttpContext.Session.GetString(state.UserId);
            viewModel.ResetPasswordToken = HttpContext.Session.GetString(state.ResetPasswordToken);
            return viewModel;
        }
        #endregion
    }
}
