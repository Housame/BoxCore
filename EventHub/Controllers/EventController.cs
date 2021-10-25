using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Models;
using EventHub.Models.Competition;
using EventHub.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventHub.Controllers
{
    [Authorize(Policy ="EventManagers")]
    public class EventController : Controller
    {
        #region Fields
        private readonly EventHubContext context;
        private readonly UserManager<IdentityUser> userManager;
        #endregion

        #region Constructor
        public EventController(EventHubContext context,
            UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }
        #endregion

        #region Competition Event Overview
        [HttpGet("mycompetitions/overview/{id:int}")]
        public IActionResult ParticipantsList(int id)
        {
            var viewModel = context.GetCompetitionsInfo(id);
            if (viewModel == null)
                return NotFound();

                return View(viewModel);

        }

        [HttpGet]
        public IActionResult GetSubCompOptions(int id)
        {
            return PartialView("_ParticipantsListOptions", id);
        }

        [HttpGet]
        public IActionResult GetSubCompInfo(int id)
        {
            var participantsList = context.GetSubCompMembers(id);
            return PartialView("_ParticipantsList", participantsList);
        }

        [HttpPatch]
        public bool ToggleCheckIn(ToggleCheckInVM model)
        {
            return context.CheckInAthlete(model);
        }
        #endregion

        #region Construct Competition Event

        [HttpGet]
        public IActionResult SaveConstructedCompetition()
        {
            var newEvents = JsonConvert.DeserializeObject<List<EventVM>>(HttpContext.Session.GetString("newEvents"));
            context.AddConstructedCompetition(newEvents);

            return null;
        }

        [HttpPost]
        public IActionResult ConstructComp(int id)
        {
            var newEvents = new List<EventVM>();
            var newSubEvents = new List<SubEventVM>();
            HttpContext.Session.SetString("newEvents", JsonConvert.SerializeObject(newEvents));
            HttpContext.Session.SetString("newSubEvents", JsonConvert.SerializeObject(newSubEvents));

            return PartialView("_ConstructComp", id);
        }

        [HttpPost]
        public IActionResult SaveEvent(EventVM viewModel)
        {
            var newEvent = new EventVM()
            {
                Title = viewModel.Title,
                SubCompetitionId = viewModel.SubCompetitionId,
                SubEvent = new List<SubEventVM>(),
            };

            newEvent.SubEvent.AddRange(JsonConvert.DeserializeObject<List<SubEventVM>>(HttpContext.Session.GetString("newSubEvents")));

            var newEvents = JsonConvert.DeserializeObject<List<EventVM>>(HttpContext.Session.GetString("newEvents"));
            newEvents.Add(newEvent);
            HttpContext.Session.SetString("newEvents", JsonConvert.SerializeObject(newEvents));

            var newSubEvents = new List<SubEventVM>();
            HttpContext.Session.SetString("newSubEvents", JsonConvert.SerializeObject(newSubEvents));

            return null;
        }

        [HttpGet]
        public IActionResult AddSubEventForm()
        {
            SubEventVM newSubEvent = new SubEventVM();
            List<ExerciseVM> newExercises = new List<ExerciseVM>();
            HttpContext.Session.SetString("newSubEvent", JsonConvert.SerializeObject(newSubEvent));
            HttpContext.Session.SetString("newExercises", JsonConvert.SerializeObject(newExercises));

            return PartialView("_SubEventForm");
        }

        [HttpPost]
        public IActionResult SaveSubEvent(SubEventSimpleDateVM viewModel)
        {
            var newSubEvent = new SubEventVM()
            {
                Title = viewModel.Title,
                Description = viewModel.Description,
                Type = viewModel.Type,
                TimeCap = new TimeSpan(0, viewModel.TimeCapMinutes, viewModel.TimeCapSeconds),
                SetUpTime = new TimeSpan(0, viewModel.SetUpTimeMinutes, viewModel.SetUpTimeSeconds),
                Exercise = new List<ExerciseVM>(),
            };

            if (viewModel.TotalReps != null)
                newSubEvent.TotalReps = viewModel.TotalReps;

            var exercises = JsonConvert.DeserializeObject<List<ExerciseVM>>(HttpContext.Session.GetString("newExercises"));
            newSubEvent.Exercise.AddRange(exercises);
            var subEvents = JsonConvert.DeserializeObject<List<SubEventVM>>(HttpContext.Session.GetString("newSubEvents"));
            subEvents.Add(newSubEvent);
            HttpContext.Session.SetString("newSubEvents", JsonConvert.SerializeObject(subEvents));

            return Json(new { msg = "Operation successful" });
        }

        [HttpGet]
        public IActionResult AddExerciseForm()
        {
            return PartialView("_ExerciseForm");
        }

        [HttpPost]
        public IActionResult SaveExercise(ExerciseSimpleTimeVM viewModel)
        {
            var newExercise = new ExerciseVM()
            {
                Title = viewModel.Title,
                Description = viewModel.Description,
                TieBreak = viewModel.TieBreak,
                Transition = new TimeSpan(0, viewModel.Minutes, viewModel.Seconds),
            };

            var newExercises = JsonConvert.DeserializeObject<List<ExerciseVM>>(HttpContext.Session.GetString("newExercises"));
            newExercises.Add(newExercise);

            HttpContext.Session.SetString("newExercises", JsonConvert.SerializeObject(newExercises));

            return Json(new { msg = "Operation successful" });
        }
        #endregion
    }
}
