using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BoxShop.Models.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using BoxShop.Models.SuperAdmin;
using Microsoft.AspNetCore.Http;
using AutoMapper;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoxShop.Controllers
{
    public class SuperAdminController : Controller
    {
        #region Fields
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signInManager;
        IdentityDbContext identity;
        BoxShopContext context;
        RoleManager<IdentityRole> roleManager;
        UserStateMgr state = new UserStateMgr();
        private readonly IHostingEnvironment hostingEnvironment;
        #endregion

        #region Constructors
        public SuperAdminController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IdentityDbContext identity,
            BoxShopContext context, RoleManager<IdentityRole> roleManager, IHostingEnvironment hostingEnvironment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.identity = identity;
            this.context = context;
            this.roleManager = roleManager;
            this.hostingEnvironment = hostingEnvironment;
        }
        #endregion

        [Authorize(Roles = "SuperAdmin")]
        public IActionResult Index()
        {
            return View();
        }

        #region admin-panel
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public IActionResult RegisterAdmin()
        {
            HttpContext.Session.Clear();

            var model = new RegisterAdminVM();
            model.BoxNames = context.GetAllBoxesNames();
            return View(model);
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> RegisterAdmin(RegisterAdminVM viewModel)
        {
            #region Add Roles
            //await roleManager.CreateAsync(new IdentityRole("EventAdmin"));
            //await roleManager.CreateAsync(new IdentityRole("BoxAdmin"));
            #endregion
            #region Validate viewmodel
            if (!ModelState.IsValid)
                return View(viewModel);
            #endregion

            var emailExists = await userManager.FindByEmailAsync(viewModel.Email);
            if (emailExists != null)
            {
                ModelState.AddModelError("Email", "Email is already in use");
                return View(viewModel);
            }

            HttpContext.Session.SetString(state.FirstName, viewModel.FirstName);
            HttpContext.Session.SetString(state.LastName, viewModel.LastName);
            //HttpContext.Session.SetString(state.PhoneNumber, viewModel.PhoneNumber);
            HttpContext.Session.SetString(state.Email, viewModel.Email);

            #region Setting role and box if exists
            if (viewModel.Box != null)
            {
                HttpContext.Session.SetString(state.Box, viewModel.Box);
                if (viewModel.Creator == true)
                {
                    HttpContext.Session.SetString(state.Role, "Admin");
                }
                else
                {
                    HttpContext.Session.SetString(state.Role, "BoxAdmin");
                }
            }

            if (viewModel.Box == null && viewModel.Creator == true)
            {
                HttpContext.Session.SetString(state.Role, "EventAdmin");
            }
            #endregion
            return RedirectToAction(nameof(SuperAdminController.ConfirmAdminRegistration));
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public IActionResult ConfirmAdminRegistration()
        {
            RegisterAdminVM viewModel = new RegisterAdminVM();
            viewModel = UserRegisterModelCasting(viewModel);
            return View(viewModel);
        }
        [Authorize(Roles = "SuperAdmin")]
        [HttpPost]
        public async Task<IActionResult> ConfirmAdminRegistration(RegisterAdminVM viewModel)
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

            //await userManager.SetPhoneNumberAsync(user, viewModel.PhoneNumber);

            string password = GenerateRandomString(15);
            await userManager.AddPasswordAsync(user, password);

            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("FirstName", result.Errors.First().Description);
                return View(viewModel);
            }
            if (viewModel.Role != null)
            {
                await userManager.AddToRoleAsync(user, viewModel.Role);
            }

            var confirm = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "User", new { userId = user.Id, code = confirm },
                protocol: HttpContext.Request.Scheme);

            #endregion        

            context.SendEmailConfirmationMail(viewModel.Email, viewModel.FirstName, viewModel.LastName, callbackUrl, password);
            var userHashId = await userManager.GetUserIdAsync(user);
            context.CreateUser(viewModel, userHashId);

            TempData["UserName"] = viewModel.FirstName;

            return RedirectToAction(nameof(SuperAdminController.UserCreated));
        }
        [HttpGet]
        [Authorize(Roles = "Admin ,EventAdmin , BoxAdmin")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            var user = await userManager.FindByIdAsync(userId);

            var result = await userManager.ConfirmEmailAsync(user, code);
            return View(nameof(SuperAdminController.ConfirmEmail));
        }
        [HttpGet]
        public IActionResult UserCreated()
        {
            return View();
        }

        //------------------Those two actions has to refactored, maybe with angular or ajax
        [Authorize(Roles = "SuperAdmin")]
        [HttpGet]
        public IActionResult AdminsOverview()
        {
            //var role = roleManager.Roles.FirstOrDefault(r => r.Name == "Admin" && r.Name == "EventAdmin" && r.Name == "BoxAdmin");
            //var adminUsers = identity.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(role.Id)).ToArray();
            //var model = context.GetAllAdmins(adminUsers);

            var roleBox = roleManager.Roles.FirstOrDefault(r => r.Name == "BoxAdmin");
            var boxAdminUsers = identity.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(roleBox.Id)).ToArray();
            var roleAdmin = roleManager.Roles.FirstOrDefault(r => r.Name == "Admin");
            var adminUsers = identity.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(roleAdmin.Id)).ToArray();
            var roleEvent = roleManager.Roles.FirstOrDefault(r => r.Name == "EventAdmin");
            var eventAdminUsers = identity.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(roleEvent.Id)).ToArray();

            var model = new List<UserShowVM>();
            var modelBox = context.GetUserRoles(boxAdminUsers);
            foreach (var item in modelBox)
            {
                item.Role = "Box Admin";
                model.Add(item);
            }

            var modelAdmin = context.GetUserRoles(adminUsers);
            foreach (var item in modelAdmin)
            {
                item.Role = "Admin";
                model.Add(item);
            }

            var modelEvent = context.GetUserRoles(eventAdminUsers);
            foreach (var item in modelEvent)
            {
                item.Role = "Event Admin";
                model.Add(item);
            }

            return View(model);
        }
        [Authorize(Roles ="SuperAdmin")]
        [HttpGet]
        public IActionResult UsersPanel()
        {
            var roleBox = roleManager.Roles.FirstOrDefault(r => r.Name == "BoxAdmin");
            var boxAdminUsers = identity.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(roleBox.Id)).ToArray();
            var roleAdmin = roleManager.Roles.FirstOrDefault(r => r.Name == "Admin");
            var adminUsers = identity.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(roleAdmin.Id)).ToArray();
            var roleEvent = roleManager.Roles.FirstOrDefault(r => r.Name == "EventAdmin");
            var eventAdminUsers = identity.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(roleEvent.Id)).ToArray();
            var roleClient = roleManager.Roles.FirstOrDefault(r => r.Name == "Client");
            var clientUsers = identity.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(roleClient.Id)).ToArray();

            var model = new List<UserShowVM>();
            var modelBox = context.GetUserRoles(boxAdminUsers);
            foreach (var item in modelBox)
            {
                item.Role = "Box Admin";
                model.Add(item);
            }

            var modelAdmin = context.GetUserRoles(adminUsers);
            foreach (var item in modelAdmin)
            {
                item.Role = "Admin";
                model.Add(item);
            }

            var modelEvent = context.GetUserRoles(eventAdminUsers);
            foreach (var item in modelEvent)
            {
                item.Role = "Event Admin";
                model.Add(item);
            }

            var modelClient = context.GetUserRoles(clientUsers);
            foreach (var item in modelClient)
            {
                item.Role = "Client";
                model.Add(item);
            }

            return View(model);
            //var allUsers = identity.Users.ToList();
            //var model = context.GetAllUsersBoxes(allUsers);
            //return View(model);
        }
        [HttpGet]
        public IActionResult GetChangeDiv(int id)
        {
            ViewBag.Id = id;
            return PartialView("_AuthChangeDiv");
        }
        [HttpPost]
        public IActionResult PostChangeDiv(RoleChangesVM model)
        {
            return PartialView("_SubmitRole",model);
        }
        #endregion

        #region box-panel
        [HttpGet]
        public IActionResult RegisterBox()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegisterBox(RegisterBoxVM viewModel)
        {
            if (!ModelState.IsValid)
                return View(viewModel);
            context.AddBoxToDB(viewModel);
            return RedirectToAction(nameof(SuperAdminController.BoxOverview));
        }
        public IActionResult BoxOverview()
        {
            var viewModel = context.GetBoxes();
            return View(viewModel);
        }
        public IActionResult DeleteBox(int id)
        {
            context.DeleteBox(id);
            return RedirectToAction(nameof(SuperAdminController.BoxOverview));
        }
        //[HttpPost]
        //public IActionResult DeleteBox()
        //{
        //    return View();
        //}

        [HttpGet]
        public IActionResult EditBox(int id)
        {
            var viewModel = context.GetBox(id);
            if (viewModel != null)
                return View(viewModel);

            return Unauthorized();
        }
        [HttpPost]
        public IActionResult EditBox(int id, RegisterBoxVM viewModel)
        {
            context.EditBox(id, viewModel);
            return RedirectToAction(nameof(SuperAdminController.BoxOverview));
        }
        #endregion

        #region helper
        private RegisterAdminVM UserRegisterModelCasting(RegisterAdminVM viewModel)
        {
            viewModel.FirstName = HttpContext.Session.GetString(state.FirstName);
            viewModel.LastName = HttpContext.Session.GetString(state.LastName);
            //viewModel.PhoneNumber = HttpContext.Session.GetString(state.PhoneNumber);
            viewModel.Email = HttpContext.Session.GetString(state.Email);
            viewModel.Box = HttpContext.Session.GetString(state.Box);
            viewModel.Role = HttpContext.Session.GetString(state.Role);
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
        //public IdentityUser[] GetUsersInRole(string roleName)
        //{
        //    var role = roleManager.FindByNameAsync(roleName).Users.First();
        //    var usersInRole = identity.Users.Where(u => u.Roles.Select(r => r.RoleId).Contains(role.RoleId)).ToArray();
        //    return usersInRole;
        //}
        #endregion




    }
}
