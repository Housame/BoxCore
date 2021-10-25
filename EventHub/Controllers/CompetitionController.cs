using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using EventHub.Models.Entities;
using EventHub.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Net.Http;
using System.Text;
using EventHub.Models.Interfaces;
using EventHub.Models.Competition;
using EventHub.Helpers;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventHub.Controllers
{

    [Authorize(Policy = "CompetitionManagers")]
    public class CompetitionController : Controller
    {
        #region Fields
        const string sessionKeySubcompetitions = "subCompetitionsJson";

        UserManager<IdentityUser> userManager;
        SignInManager<IdentityUser> signInManager;
        IdentityDbContext identity;
        EventHubContext context;
        #endregion
        #region Properties
        [TempData]
        public int AddedCompetitionId { get; set; }
        #endregion
        #region Constructor
        public CompetitionController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IdentityDbContext identity,
            EventHubContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.identity = identity;
            this.context = context;
        }
        #endregion

        [Authorize(Policy = "CompetitionCreators")]
        #region Create Competition
        [HttpGet("createcompetition")]
        public IActionResult CreateCompetition()
        {
            HttpContext.Session.Clear();
            if (User.IsInRole("Admin"))
                return View(new CreateCompetitiontVM
                {
                    Boxes = context.Box
                    .Where(box => box.Owner != null)
                    .Select(x => new SelectListItem
                    { Text = x.Name, Value = x.Owner }).ToList()
                });

            return View();
        }

        [HttpPost("createcompetition")]
        public IActionResult CreateCompetition(CreateCompetitiontVM viewModel)
        {
            if (!User.IsInRole("Admin"))
                viewModel.Published = false;

            viewModel.SubCompetition = new List<ISubCompetition>();
            var subCompetitions = HttpContext.Session.GetString(sessionKeySubcompetitions);
            if (subCompetitions != null)
            {
                viewModel.SubCompetition.AddRange(JsonConvert.DeserializeObject<List<ISubCompetition>>(subCompetitions, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
            }

            if (!ModelState.IsValid)
                return View(viewModel);

            if (User.IsInRole("Admin"))
                viewModel.CreatorId = viewModel.Box;

            if(User.IsInRole("EventAdmin"))
            viewModel.CreatorId = userManager.GetUserId(HttpContext.User);

            AddedCompetitionId = context.AddCompetitionToDB(viewModel);

            return RedirectToAction(nameof(CompetitionController.Confirmation));
        }

        [HttpGet]
        public IActionResult Confirmation()
        {
            var viewModel = context.GetCompetition(AddedCompetitionId);

            //Uncomment to test the view without having to create a competition
           // var viewModel = context.GetCompetition(49);

            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }
        #endregion

        #region Add SubCompetitions (Competition Classes)
        [HttpGet]
        public IActionResult AddTeamCompForm()
        {
            return PartialView("_TeamComp");
        }

        [HttpPost]
        public IActionResult SubmitTeamComp(CreateCompetitiontVM.TeamSubEventVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    invalidModelState = true,
                    errorMsg = "Samtliga fält måste vara ifyllda när du skapar en lagtävling."
                });
            }

            var subCompetitions = UpdateAndFetchSubCompetitions(model);
            return PartialView("_SubCompetitions", subCompetitions);
        }

        [HttpGet]
        public IActionResult AddSoloCompForm()
        {
            return PartialView("_SoloComp");
        }

        [HttpPost]
        public IActionResult SubmitSoloComp(CreateCompetitiontVM.SoloSubEventVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    invalidModelState = true,
                    errorMsg = "Samtliga fält måste vara ifyllda när du skapar en individuelltävling."
                });
            }
            var subCompetitions = UpdateAndFetchSubCompetitions(model);
            return PartialView("_SubCompetitions", subCompetitions);
        }


        [HttpDelete]
        public IActionResult DeleteSubCompetition(string id)
        {
            var currentSubCompetitions = HttpContext.Session.GetString(sessionKeySubcompetitions);
            var subCompetitions = JsonConvert.DeserializeObject<List<ISubCompetition>>(currentSubCompetitions, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            subCompetitions.RemoveAll(o => o.Id == id);
            HttpContext.Session.SetString(sessionKeySubcompetitions, JsonConvert.SerializeObject(subCompetitions, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
            return PartialView("_SubCompetitions", subCompetitions);
        }
        #endregion

        #region Edit
        [HttpGet]
        public IActionResult EditCompetition(int id)
        {
            var viewModel = context.GetCompetition(id);
            if (viewModel == null)
                return NotFound();

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult EditCompetition(int id, EditOneCompetVM.CompetitionVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (User.IsInRole("Admin"))
                model.IsAdmin = true;

            context.EditCompetition(model);
            return RedirectToAction(nameof(ProfileController.ListCompetitionEdit), "Profile");
        }



        // List Competition Edit Actions

        [HttpPost]
        public IActionResult DeleteSubEvent(int id)
        {
            context.DeleteSubEvent(id);
            //redirect isn't needed or called men just to post Deleting process
            return RedirectToAction(nameof(ProfileController.ListCompetitionEdit), "Profile");
        }
        public IActionResult AddSoloCompOnEdit(CreateCompetitiontVM.SoloSubEventVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    invalidModelState = true,
                    errorMsg = "Samtliga fält måste vara ifyllda när du skapar en individuelltävling."
                });
            }
            context.SaveNewRangeSolo(model);
            return PartialView("_SoloCompAdded", model);
        }
        public IActionResult AddTeamCompOnEdit(CreateCompetitiontVM.TeamSubEventVM model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new
                {
                    invalidModelState = true,
                    errorMsg = "Samtliga fält måste vara ifyllda när du skapar en individuelltävling."
                });
            }
            context.SaveNewRangeTeam(model);
            return PartialView("_TeamCompAdded", model);
        }
        #endregion

        #region Private Methods

        //Temporarly save the subcompetiton in session state and get the current list of subcompetitions
        private List<ISubCompetition> UpdateAndFetchSubCompetitions(ISubCompetition model)
        {
            List<ISubCompetition> subCompetitions;
            var currentSubCompetitions = HttpContext.Session.GetString(sessionKeySubcompetitions);

            if (currentSubCompetitions != null)
            {
                subCompetitions = JsonConvert.DeserializeObject<List<ISubCompetition>>(currentSubCompetitions, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                subCompetitions.Add(model);
            }
            else
            {
                subCompetitions = new List<ISubCompetition>();
                subCompetitions.Add(model);
            }
            HttpContext.Session.SetString(sessionKeySubcompetitions, JsonConvert.SerializeObject(subCompetitions, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
            return subCompetitions;
        }

        #endregion


    }
}
