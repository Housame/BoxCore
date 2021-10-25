using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BoxShop.Models.Entities;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoxShop.Controllers
{
    public class HomeController : Controller
    {
        #region Fields
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signInManager;
        IdentityDbContext identity;
        BoxShopContext context;
        #endregion

        #region Constructors
        public HomeController(UserManager<IdentityUser> userManager,
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

        #region Methods
        [HttpGet]
        public IActionResult Index()
        {
            // Create the DB Schema. 
            //identity.Database.EnsureCreatedAsync();
            
           
            return View();
        }
        #endregion
    }
}
