using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Models.Competition;
using EventHub.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventHub.Controllers
{
    [Authorize("EventManagers")]
    public class ResultController : Controller
    {
        #region Fields
        private readonly EventHubContext context;
        #endregion

        #region Constructor
        public ResultController(EventHubContext context)
        {
            this.context = context;
        }
        #endregion

       
        [HttpPost]
        public IActionResult RegisterResult(int id)
        {
            var addResultsModel = new AddResultsVM();
            addResultsModel.SubCompMember = context.GetSubCompMembersWithResults(id);
            addResultsModel.SubCompMember.CompEvent = addResultsModel.SubCompMember.CompEvent.OrderBy(c => c.Title).ToList();
            addResultsModel.CompEvent = context.GetCompEvents(id);
            return PartialView("_RegisterResultContainer", addResultsModel);
        }
        [HttpPost]
        public IActionResult UpdateResult(List<UpdateResultVM> viewModel)
        {
            context.UpdateResults(viewModel);

            return Json(new { success = true });
        }

  

        [HttpGet]
        public IActionResult Slideshow(int id)
        {
            var viewModel = context.GetSubCompResults(id);
            return View(viewModel);
        }
    }
}
