using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EventHub.Models.ConstructComp;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using EventHub.Models.Entities;

namespace EventHub.Controllers
{

    [Authorize]
    public class ConstructCompController : Controller
    {

        EventHubContext context;

        public ConstructCompController(EventHubContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public IActionResult Index(int id)
        {
            var model = context.GetConstructComp(id);
            return View(model);
        }

        #region Get Form
        [HttpGet]
        public IActionResult GetEventForm()
        {
            return PartialView("_EventForm");
        }

        [HttpGet]
        public IActionResult GetSubEventForm(int id)
        {
            return PartialView("_SubEventForm", id);
        }
        #endregion

        #region Save Form
        [HttpPost]
        public IActionResult SaveEventForm(SubmitEventFormVM viewModel)
        {

            List<EventVM> construction;
            construction = DeSerializeConstruction();

            if (construction == null)
                construction = new List<EventVM>();

            var newEvent = new EventVM()
            {
                EventTempId = GenerateTempId(),
                Title = viewModel.Title,
                SubEvents = new List<SubEventVM>()
            };

            construction.Add(newEvent);
            SerializeConstruction(construction);

            return PartialView("_Tree", construction);
        }

        [HttpPost]
        public IActionResult SaveSubEventForm(SubmitSubEventFormVM subEvent)
        {

            var construction = DeSerializeConstruction();

            construction
                .FirstOrDefault(o => o.EventTempId == subEvent.EventTempId)
                .SubEvents.Add(new SubEventVM
                {
                    EventTempId = subEvent.EventTempId,
                    SubEventTempId = GenerateTempId(),
                    Title = subEvent.Title,
                    Description = subEvent.Description,
                    Type = subEvent.Type,
                    TimeCap = new TimeSpan(0, subEvent.TimeCapMinutes, subEvent.TimeCapSeconds),
                    TotalReps = subEvent.TotalReps,
                    SetUpTime = new TimeSpan(0, subEvent.TimeCapMinutes, subEvent.TimeCapSeconds),
                });

            SerializeConstruction(construction);

            return PartialView("_Tree", construction);
        }
        #endregion

        #region Delete Event
        [HttpPost]
        public IActionResult DeleteEvent(int id)
        {
            var construction = DeSerializeConstruction();
            construction.Remove(construction.SingleOrDefault(o => o.EventTempId == id));
            SerializeConstruction(construction);
            return PartialView("_Tree", construction);
        }

        [HttpPost]
        public IActionResult DeleteSubEvent(SubEventIndexLocatorVM viewModel)
        {
            var construction = DeSerializeConstruction();

            construction
                .FirstOrDefault(o => o.EventTempId == viewModel.EventId)
                .SubEvents
                .Remove(construction.FirstOrDefault(o => o.EventTempId == viewModel.EventId).SubEvents.FirstOrDefault(o => o.SubEventTempId == viewModel.SubEventId));

            SerializeConstruction(construction);

            return PartialView("_Tree", construction);
        }
        #endregion

        #region Edit Event
        [HttpGet]
        public IActionResult EditEvent(int id)
        {
            var construction = DeSerializeConstruction();
            return PartialView("_EditEventForm", construction.SingleOrDefault(o => o.EventTempId == id));
        }

        [HttpPost]
        public IActionResult EditEvent(EventVM model)
        {
            var construction = DeSerializeConstruction();

            foreach (var eventModel in construction)
            {
                if (eventModel.EventTempId == model.EventTempId)
                    eventModel.Title = model.Title;
            }

            SerializeConstruction(construction);

            return PartialView("_Tree", construction);
        }

        [HttpPost]
        public IActionResult GetEditSubEvent(SubEventIndexLocatorVM model)
        {
            var construction = DeSerializeConstruction();

            var subEvent = construction
                .SingleOrDefault(o => o.EventTempId == model.EventId)
                .SubEvents
                .SingleOrDefault(o => o.SubEventTempId == model.SubEventId);

            return PartialView("_EditSubEventForm", subEvent);
        }

        [HttpPost]
        public IActionResult SaveEditSubEvent(SubmitSubEventFormVM subEvent)
        {
            var construction = DeSerializeConstruction();

            foreach (var constructEvent in construction)
            {
                if (constructEvent.EventTempId == subEvent.EventTempId)
                {
                    foreach (var constructSubEvent in constructEvent.SubEvents)
                    {
                        if (constructSubEvent.SubEventTempId == subEvent.SubEventTempId)
                        {
                            constructSubEvent.Description = subEvent.Description;
                            constructSubEvent.SetUpTime = new TimeSpan(0, subEvent.SetUpTimeMinutes, subEvent.SetUpTimeSeconds);
                            constructSubEvent.TimeCap = new TimeSpan(0, subEvent.TimeCapMinutes, subEvent.TimeCapSeconds);
                            constructSubEvent.Title = subEvent.Title;
                            constructSubEvent.TotalReps = subEvent.TotalReps;
                            constructSubEvent.Type = subEvent.Type;
                            break;
                        }
                    }
                    break;
                }
            }

            SerializeConstruction(construction);

            return PartialView("_Tree", construction);
        }
        #endregion

        [HttpPost]
        public IActionResult SaveConstruction(int[] model)
        {

            var construction = DeSerializeConstruction();

            if (construction == null || construction.Count < 1)
                return Json(new { constructSaved = false, msg = "No data exists." });

            if (model == null || model.Length < 1)
                return Json(new { constructSaved = false, msg = "No sub comps selected." });

            context.PersistConstructedCompetition(construction, model);

            return Ok();
        }

        private List<EventVM> DeSerializeConstruction()
        {
            var json = HttpContext.Session.GetString("construction");

            if (String.IsNullOrEmpty(json))
                return null;
            else
                return JsonConvert.DeserializeObject<List<EventVM>>(json);
        }

        private void SerializeConstruction(List<EventVM> construction)
        {
            HttpContext.Session.SetString("construction", JsonConvert.SerializeObject(construction));
        }

        private int GenerateTempId()
        {
            var tempId = HttpContext.Session.GetInt32("tempId");

            if (tempId == null)
                tempId = 0;

            tempId++;
            HttpContext.Session.SetInt32("tempId", (int)tempId);
            return (int)tempId;
        }

    }
}