using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EventHub.Models;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

namespace EventHub.Controllers
{

    [AllowAnonymous]
    public class LeaderboardController : Controller
    {

        private readonly UserManager<IdentityUser> userManager;
        private readonly EventHubContext context;
        private readonly ILogger<ReservationController> logger;

        public LeaderboardController(UserManager<IdentityUser> userManager, EventHubContext context, ILogger<ReservationController> logger)
        {
            this.userManager = userManager;
            this.context = context;
            this.logger = logger;
        }

        [HttpGet]
        [Route("leaderboard/global")]
        public IActionResult Global()
        {
            logger.LogInformation("Global leaderboard was accessed");
            var model = context.GetAllComps();
            return View(model);
        }

        [HttpGet]
        [Route("leaderboard/comp/{id}")]
        public IActionResult Index(int id)
        {
            var viewModel = context.GetLeaderboardIndex(id);
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult SubClass(int id)
        {
            logger.LogInformation("SubClass leaderboard was accessed");
            var viewModel = context.GetResultForCompClass(id);
            return PartialView("_SubClass", viewModel);
        }

    }
}
