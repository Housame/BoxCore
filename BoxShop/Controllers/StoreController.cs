using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using BoxShop.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using BoxShop.Models;
using Microsoft.AspNetCore.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BoxShop.Controllers
{

    [Authorize]
    public class StoreController : Controller
    {

        static List<ProductDisplayVM> products;
        static UserDetails validatedUser;

        #region Fields
        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signInManager;
        IdentityDbContext identity;
        BoxShopContext context;
        #endregion

        #region Constructors
        public StoreController(UserManager<IdentityUser> userManager,
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
            products = new List<ProductDisplayVM>();
            validatedUser = new UserDetails();
            var model = context.GetAvailableProductsFromBox();
            return View(model);
        }

        [HttpGet]
        public IActionResult AddProductToCart(int id)
        {
            var model = context.GetProductById(id);
            products.Add(model);
            return PartialView("_Cart", model);
        }

        [HttpGet]
        public IActionResult ValidateUser()
        {
            var model = context.GetUsersForStore();
            return View(model);
        }

        [HttpGet]
        public IActionResult FilterUserStore(string input)
        {
            if (String.IsNullOrEmpty(input))
                return PartialView("_UserView", null);

            var model = context.FilterUserStore(input.ToLower());
            return PartialView("_UserView", model);
        }

        [HttpPost]
        public IActionResult ConfirmUserAgainstPIN(int[] id)
        {

            int pinInput = id[0];
            int userId = id[1];

            var user = context.GetUserById(userId);

            if (user.Pin == pinInput)
            {
                validatedUser.UserId = user.Id;
                validatedUser.FirstName = user.FirstName;
                validatedUser.LastName = user.LastName;

                return Json(new
                {
                    userAuthenticated = true,
                    url = Url.Action("ConfirmPurchase", "Store"),
                });
            }
            else
                return Json(new
                {
                    userAuthenticated = false,
                    String.Empty,
                });
        }

        [HttpGet]
        public IActionResult ConfirmPurchase()
        {
            decimal sum = 0;
            foreach (ProductDisplayVM product in products)
            {
                sum += product.Price;
            }

            var model = new ConfirmPurchase
            {
                UserDetail = validatedUser,
                productDetails = products,
                ProductSum = sum,
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult PersistOrder()
        {

            return null;
        }

        [HttpPost]
        public void RemoveProductFromCart(int id)
        {
            products.Remove(products.Single(o => o.Id == id));
        }
        #endregion
    }
}
