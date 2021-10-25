using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Models;
using EventHub.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventHub.Controllers
{

    [Authorize]
    public class DiscountController : Controller
    {

        private readonly UserManager<IdentityUser> userManager;
        private readonly EventHubContext context;

        public DiscountController(UserManager<IdentityUser> userManager, EventHubContext context)
        {
            this.userManager = userManager;
            this.context = context;
        }

        [HttpGet]
        public IActionResult Index()
        {
            string hashId = userManager.GetUserId(HttpContext.User);
            var model = context.GetDiscountVM(hashId);
            if (model.ExistingCompetitions.Count == 0)
            {
                model.ExistingCompetitions.Add(new ExistingCompetition { CompId = -1 });
            }
            return View(model);
        }

        [HttpPost]
        public IActionResult AddCode(AddDiscountCode viewModel)
        {

            if (viewModel.SubCompId == null)
                return Json(new { valid = false, msg = "Ange åtminstone en klass för att kunna skapa rabattkod. Alternativt skapa en ny tävling som innehåller önskad klass." });

            if (!ModelState.IsValid)
                return Json(new { valid = false, msg = "Fyll i samtliga fält." });

            context.AddDiscountCodes(viewModel);
            string hashId = userManager.GetUserId(HttpContext.User);
            var model = context.GetDiscountVM(hashId);
            return PartialView("_Codes", model.ExistingDiscountCodes);
        }

        [HttpGet]
        public IActionResult GetSubComps(int id)
        {
            var model = context.GetAvailableSubComps(id);
            return PartialView("_AvailableSubComps", model);
        }

        [HttpPost]
        public IActionResult AddDiscount(SubmitDiscount model)
        {

            IDictionary<int, string> outputMsg = new Dictionary<int, string>();
            outputMsg.Add(1, "Ange en kod för att ta fram rabatt.");
            outputMsg.Add(2, "Den angivna koden gäller inte.");
            outputMsg.Add(3, "Koden har slutat gälla.");
            outputMsg.Add(4, "% kommer att dras av vid betalning.");

            if (String.IsNullOrEmpty(model.Code))
                return Json(new { valid = false, msg = outputMsg[1] });

            var subCompsWithCodes = context.DiscountForSubCompetition
                .Where(o => o.SubCompetitionId == model.SubCompId)
                .Include(o => o.Discount)
                .Select(o => o);

            foreach (var code in subCompsWithCodes)
            {
                if (String.Equals(code.Discount.Code, model.Code))
                {
                    if (code.Discount.ExpiryDate < DateTime.Now)
                    {
                        return Json(new { valid = false, msg = outputMsg[3] });
                    }
                    else
                    {
                        HttpContext.Session.SetString("discount", code.Discount.PercentageOff.ToString());
                        return Json(new { valid = true, msg = String.Format($"{code.Discount.PercentageOff} {outputMsg[4]}") });
                    }
                }
            }

            return Json(new { valid = false, msg = outputMsg[2] });
        }

        [HttpPatch]
        public IActionResult EditCode()
        {
            return Ok();
        }

        [HttpDelete]
        public IActionResult DeleteCode(DeleteCode viewModel)
        {
            context.DeleteDiscountCode(viewModel);
            string hashId = userManager.GetUserId(HttpContext.User);
            var model = context.GetDiscountVM(hashId);
            return PartialView("_Codes", model.ExistingDiscountCodes);
        }

    }
}
