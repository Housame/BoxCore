using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BoxShop.Models.Entities;
using BoxShop.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoxShop.Controllers
{
    public class AdminClientController : Controller
    {
        #region Fields
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signInManager;
        IdentityDbContext identity;
        BoxShopContext context;
        RoleManager<IdentityRole> roleManager;
        private readonly IHostingEnvironment hostingEnvironment;
        #endregion

        #region Constructors
        public AdminClientController(UserManager<IdentityUser> userManager,
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

        #region Properties
        /// <summary>
        /// Wraps the current user ID from the session state cookie and lazy loads it if empty.
        /// </summary>
        public int CurrentUserId
        {
            get
            {                            
                return String.IsNullOrEmpty(HttpContext.Session.GetString("CurrentUserId")) ? SetAndGetCurrentUserId() : int.Parse(HttpContext.Session.GetString("CurrentUserId")) ;        
            }
        }
        /// <summary>
        /// Wraps the current box ID from the session state cookie and lazy loads it if empty.
        /// </summary>
        public int CurrentBoxId
        {
            get
            {
                return String.IsNullOrEmpty(HttpContext.Session.GetString("CurrentBoxId")) ? SetAndGetCurrentUserId() : int.Parse(HttpContext.Session.GetString("CurrentBoxId"));
            }
        }
        #endregion

        [Authorize]
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        //[HttpPost]
        //public IActionResult Index()
        //{
        //    return View();
        //}
        [HttpGet]
        public async Task<IActionResult> SignIn()
        {
            #region Körs en gång bara för att spara rollerna i DB
            // OBS!!! Måste först ändra --IActionResult-- till --async Task<IActionResult>--

            //await roleManager.CreateAsync(new IdentityRole("SuperAdmin"));
            //await roleManager.CreateAsync(new IdentityRole("Admin"));
            //await roleManager.CreateAsync(new IdentityRole("Client"));
            #endregion

            #region Körs ifall vi vill hårdkoda en användare
            //OBS!!!Måste först ändra --IActionResult-- till--async Task<IActionResult>--

            var user = new IdentityUser("TestSuperAdmin");//Hårdkodat användarnamn 

            await userManager.SetEmailAsync(user, "superAdmin@test.se");//Hårdkodat email

            var result = await userManager.CreateAsync(user, "SuperAdminTest123");//Hårdkodat lösenord
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, "SuperAdmin");//Hårdkodat roll (SuperAdmin, Admin eller Client)
            }
            //var tempUser = new User();
            //tempUser.FirstName = "Admin";
            //tempUser.LastName = "Test";
            //tempUser.HashId = userManager.GetUserId(HttpContext.User);
            //context.AddUser(tempUser);
            #endregion
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SignIn(UserAdminSignInVM viewModel)
        {

            var user = await userManager.FindByNameAsync(viewModel.UserName);
            if (user == null)
            {
                ModelState.AddModelError("Password", "Fel Användarnamnet finns inte");//Valet sen om vi ska informera om användarnamnet finns ej eller bara ett felmeddelande?
                return View(viewModel);
            }

            var result = await signInManager.PasswordSignInAsync(viewModel.UserName, viewModel.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("Password", "Fel användarnam eller lösenord");
                return View(viewModel);
            }

            return Redirect(nameof(AdminClientController.Index));
        }
        public async Task<IActionResult> SignOut()
        {
            await signInManager.SignOutAsync();

            return Redirect(nameof(AdminClientController.Index));
        }
        [Authorize(Roles = "Admin , BoxAdmin")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [Authorize(Roles = "Admin , BoxAdmin")]
        [HttpPost]
        public async Task<IActionResult> Add(ProductAddVM viewModel)
        {
            //if (!ModelState.IsValid)
            //    return View(viewModel);
            context.AddProductToDB(viewModel, CurrentBoxId);
            return RedirectToAction(nameof(AdminClientController.Overview));
        }
        public IActionResult Overview()
        {
            var viewModel = context.GetProducts(CurrentBoxId);
            return View(viewModel);
        }
        // Edit Products Actions
        [Authorize(Roles = "Admin , BoxAdmin")]
        [HttpGet]
        public IActionResult EditProduct(int id)
        {
            var viewModel = context.GetProduct(id, CurrentBoxId);
            if (viewModel != null) 
            return View(viewModel);

            return Unauthorized();
        }
        [Authorize(Roles = "Admin , BoxAdmin")]
        [HttpPost]
        public IActionResult EditProduct(int id, ProductAddVM viewModel)
        {
            context.EditProduct(id, viewModel, CurrentBoxId);
            return RedirectToAction(nameof(AdminClientController.Overview));
        }
        [Authorize(Roles = "Admin , BoxAdmin")]
        public IActionResult Billing()
        {
            var viewModel = context.GetUnpaidOrdersForBox(CurrentUserId);
            return View(viewModel);
        }
        [Authorize(Roles = "Admin , BoxAdmin")]
        public IActionResult SendBills()
        {
            string sWebRootFolder = hostingEnvironment.WebRootPath;
            //int userId = GetCurrentUserId(); ----------för att hämta admins email och skicka ifrån det ----läggs till senare
            var model = context.GetUnpaidOrdersForBox(CurrentUserId);
            context.SendBills(model, sWebRootFolder);
            return RedirectToAction(nameof(AdminClientController.Index));
        }
        //[HttpPost]
        //public IActionResult Billing(BillingVM viewModel)
        //{
        //    return View();
        //}
         private int SetAndGetCurrentUserId()
        {
            string userHashId = userManager.GetUserId(HttpContext.User);
            var currentUserId = context.User.Where(o => o.HashId == userHashId).Select(p => p.Id).First();
            SaveUserIdInSessionState(currentUserId);
            return currentUserId;
        }
         void SaveUserIdInSessionState(int CurrentUserId)
        {
             HttpContext.Session.SetString("CurrentUserId", CurrentUserId.ToString());
        }

        private int? SetAndGetCurrentBoxId()
        {
            string userHashId = userManager.GetUserId(HttpContext.User);
            var currentBoxId = context.User.Where(o => o.HashId == userHashId).Select(p => p.BoxId).First();
            SaveBoxIdInSessionState(currentBoxId);
            return currentBoxId;
        }
        void SaveBoxIdInSessionState(int? currentBoxId)
        {
            HttpContext.Session.SetString("CurrentBoxId", currentBoxId.ToString());
        }
    }
}
