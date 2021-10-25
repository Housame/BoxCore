using AutoMapper;
using BoxShop.Models;
using BoxShop.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoxShop.Controllers
{
    public class UserClientController : Controller
    {
        #region Fields
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signInManager;
        IdentityDbContext identity;
        BoxShopContext context;
        #endregion

        #region Constructors
        public UserClientController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IdentityDbContext identity,
            BoxShopContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.identity = identity;
            this.context = context;
        }
        #endregion

        [HttpGet]
        public IActionResult Index()
        {
            var model = new UserClientIndexVM() { };
            model.userDisplay = context.GetUsers();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserSignInVM viewModel)
        {
            var model = new UserClientIndexVM()
            {
                userDisplay = context.GetUsers(),
            };

            if (!ModelState.IsValid)
                return View(model);

            var user = await userManager.FindByNameAsync(viewModel.UserName);
            if (user == null)
            {
                ModelState.AddModelError("Password", "User not found");
                return View(model);
            }

            var result = await signInManager.PasswordSignInAsync(viewModel.UserName, viewModel.Password, false, false);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("Password", "Incorrect information");
                return View(model);
            }

            return RedirectToAction(nameof(StoreController.Index));
        }

        [HttpGet]
        public IActionResult FilterUser(string input)
        {
            if (String.IsNullOrEmpty(input))
                return PartialView("_User", null);

            var model = context.FilterUsers(input.ToLower());
            return PartialView("_User", model);
        }

        [HttpGet]
        public IActionResult EditUser(int id)
        {
            var model = context.GetUserById(id);
            return View(model);
        }

        [HttpPost]
        public IActionResult EditUser(UserDisplayVM model)
        {
            var user = Mapper.Map<User>(model);
            context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();

            return RedirectToAction(nameof(UserClientController.Index));
        }

        [HttpPost]
        public IActionResult RemoveUser(int id)
        {
            var user = context.User.FirstOrDefault(o => o.Id == id);
            context.Remove(user);
            context.SaveChanges();

            return RedirectToAction(nameof(UserClientController.Index));
        }

    }
}
