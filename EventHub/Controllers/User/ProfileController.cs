using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Models;
using EventHub.Models.Competition;
using EventHub.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventHub.Controllers
{
    [Authorize()]
    public class ProfileController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly EventHubContext context;
        private readonly ILogger<ProfileController> logger;

        public ProfileController(UserManager<IdentityUser> userManager, EventHubContext context, ILogger<ProfileController> logger)
        {
            this.userManager = userManager;
            this.context = context;
            this.logger = logger;
        }
      


        [HttpGet("myprofile")]
        public async Task<IActionResult> Profile()
        {
            string userHashId = userManager.GetUserId(HttpContext.User);
            int id = context.User.SingleOrDefault(o => o.HashId.Equals(userHashId)).Id;
            var model = context.GetUserProfile(id);
            var user = await userManager.GetUserAsync(User);
            model.ListProfile.UserName = user.UserName;
            model.ListProfile.Email = user.Email;
            return View(model);
        }

        [HttpPost("myprofile")]
        public async Task<IActionResult> Profile(CreateProfileImageVM viewModel)
        {
            string userHashId = userManager.GetUserId(HttpContext.User);
            int userId = context.User.SingleOrDefault(o => o.HashId.Equals(userHashId)).Id;

            if (viewModel.Image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await viewModel.Image.CopyToAsync(memoryStream);

                    if (memoryStream.ToArray().Length > 4000000)
                    {
                        TempData["ImgSizeMsg"] = "Profilbild får inte överskrida 4 MB";
                        return RedirectToAction(nameof(Profile));
                    }
                    context.AddProfileImageToDBUser(memoryStream.ToArray(), userId);
                }
            }

            return RedirectToAction(nameof(ProfileController.Profile));
        }

        [HttpGet("profile/{id}")]
        public IActionResult PublicProfile(int id)
        {
            var publicProfile = context.User.FirstOrDefault(u => u.Id == id).PublicProfile;
            var access = (publicProfile == true || publicProfile == null) ? true : false;
            string userHashId = userManager.GetUserId(HttpContext.User);
            int userId = context.User.SingleOrDefault(o => o.HashId.Equals(userHashId)).Id;

            if (access || userId == id)
            {
                var model = context.GetUserProfile(id);
                return View(model);
            }
            else
            {
                return RedirectToAction(nameof(ProfileController.Profile));
            }
        }
        

        [HttpGet("editprofile")]
        public IActionResult Edit()
        {
            string userHashId = userManager.GetUserId(HttpContext.User);
            int userId = context.User.SingleOrDefault(o => o.HashId.Equals(userHashId)).Id;
            var viewModel = context.GetUser(userId);

            return View(viewModel);
        }
 
        [HttpPost("editprofile")]
        public IActionResult Edit(EditUserVM viewModel)
        {
            if (!ModelState.IsValid)
                return View();

            string userHashId = userManager.GetUserId(HttpContext.User);
            int userId = context.User.SingleOrDefault(o => o.HashId.Equals(userHashId)).Id;
            context.EditUser(userId, viewModel);
            return RedirectToAction(nameof(ProfileController.Profile));
        }

        [HttpGet("mycompetitions")]
        public IActionResult ListCompetitionEdit()
        {
            string hashId = userManager.GetUserId(HttpContext.User);
            var model = context.GetEditableEventsVM(hashId);

            if (HttpContext.User.HasClaim(c => c.Type == "EventManager"|| c.Type=="CompetitionManager"))
            {
                model.Competitions = GetAdditionalCompetitions(model.Competitions);
            }
         

            model.Filter = context.GetFilterCompetitionsVM(model.Competitions);
            return View(model);
        }
        private List<MakeReservationVM.CompetitionVM> GetAdditionalCompetitions(IEnumerable<MakeReservationVM.CompetitionVM> currentCompetitions)
        {
            var competitions = new List<MakeReservationVM.CompetitionVM>();
            var claims = HttpContext.User.Claims;
            foreach (var claim in claims)
            {
                if (claim.Type == "EventManager")
                {
                    competitions
                        .AddRange(context.GetEditListVM(context.Box.Find(int.Parse(claim.Value)).Owner));
                }
                if(claim.Type == "Box")
                {
                    competitions
                        .AddRange(context.GetEditListVM(context.Box.Find(int.Parse(claim.Value)).Owner));
                }
            }
            competitions.AddRange(currentCompetitions);
            return competitions;
        }
    }
}
