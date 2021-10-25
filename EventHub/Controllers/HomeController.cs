using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EventHub.Models.Entities;
using EventHub.Models;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventHub.Controllers
{
    public class HomeController : Controller
    {
        EventHubContext context;
        private readonly ILogger<HomeController> logger;

        public HomeController(EventHubContext context, ILogger<HomeController> logger)
        {
            this.context = context;
            this.logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Modal_Login()
        {
            return PartialView("_Modal-Login");
        }

        [HttpGet]
        public IActionResult Modal_Register()
        {
            return PartialView("_Modal-Register");
        }

        [HttpGet]
        public IActionResult Modal_RetrievePassword()
        {
            return PartialView("_Modal-RetrievePassword");
        }

        [HttpGet]
        public IActionResult About()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Cookies()
        {
            return View();
        }

        [HttpGet]
        public IActionResult OldTerms()
        {
            return View();
        }
        public IActionResult Terms()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Events()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Leaderboard()
        {
            return View();
        }
    }
}
