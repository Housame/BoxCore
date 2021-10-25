using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Helpers;
using EventHub.Models;
using EventHub.Models.Competition;
using EventHub.Models.Entities;
using EventHub.Models.Interfaces;
using EventHub.Models.UserClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using NLog.Targets;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EventHub.Controllers
{
    [Authorize()]
    public class ReservationController : Controller
    {
        #region Properties
        /// <summary>
        ///Wraps the current user ID from the session state cookie and lazy loads it if empty.
        /// </summary>
        public int CurrentUserId
        {
            get
            {
                return String.IsNullOrEmpty(HttpContext.Session.GetString("CurrentUserId")) ? SetAndGetCurrentUserId() : int.Parse(HttpContext.Session.GetString("CurrentUserId"));
            }
        }
        #endregion

        #region Fields
        private readonly EventHubContext context;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IPaymentManager paymentManager;
        private readonly IMailService mailService;
        private readonly ILogger<ReservationController> logger;
        const string sessionKeySubEvent = "sessionKeySubEvent";
        const string sessionKeyType = "sessionKeyType";
        const string sessionKeyTempReservation = "sessionKeyTempReservation";
        const string sessionKeyPayexUri = "sessionKeyPayexUri";
        #endregion

        #region Constructor
        public ReservationController(EventHubContext context,
            UserManager<IdentityUser> userManager,
            IPaymentManager paymentManager,
            IMailService mailService,
            ILogger<ReservationController> logger)
        {
            this.context = context;
            this.userManager = userManager;
            this.paymentManager = paymentManager;
            this.mailService = mailService;
            this.logger = logger;
        }

        #endregion

        #region Public Methods
        [HttpGet("myreservations")]
        public IActionResult MyReservations()
        {
            string userHashId = userManager.GetUserId(HttpContext.User);
            int id = context.User.SingleOrDefault(o => o.HashId.Equals(userHashId)).Id;
            List<MyCompetitionsVM.CompetitionVM> model = new List<MyCompetitionsVM.CompetitionVM>();
            model = context.GetUserReservations(id);
            return View(model);
        }
        [HttpGet("competitions")]
        public IActionResult Reserve()
        {
            var user = context.GetUserProfile(CurrentUserId);

            if (String.IsNullOrEmpty(user.ListProfile.Gender))
            {
                TempData["ProfileIncomplete"] = "Du måste fylla i din profil innan du kan börja boka tävlingar.";
                return RedirectToAction("Edit", "UserClient");
            }

            var model = context.GetMakeReservationVM(CurrentUserId);
            model.Filter = context.GetFilterCompetitionsVM(model.Competitions);
            return View(model);
        }

        [HttpGet()]
        public IActionResult ModalReserve(int id)
        {
            if (id == 0)
                return PartialView("_FaceBookModal");

            var model = context.GetCompetitionInfo(id, CurrentUserId);

            HttpContext.Session.Remove("discount");
            HttpContext.Session.SetString(sessionKeySubEvent,
                JsonConvert.SerializeObject(model,
               new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));

            logger.LogInformation("User: " + CurrentUserId + " SubCompID: " + id);

            return PartialView("_ModalReserve", model);
        }

        [HttpPost]
        public IActionResult ModalReserveOnClose()
        {
            logger.LogInformation("User: " + CurrentUserId);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> BookSingleSubEvent(SingleReservationVM viewModel)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, responseText = "Alla fält måste vara ifyllda..." });

            //Save the subcompetition-type in sessionstate for correct mapping of properties later.
            HttpContext.Session.SetString(sessionKeyType, SubCompetitionTypes.Single.ToString());

            //Save the whole reservation in session state to be picked up later for persistment in DB.
            SaveTemporaryReservationInSession(viewModel);

            var url = await paymentManager.InitializePayment(viewModel);

            logger.LogInformation("User: " + CurrentUserId + " PayexUrl: " + url);
            //Save the PayEx payment session string for later use. 
            HttpContext.Session.SetString(sessionKeyPayexUri, url);
            return Json(new { success = true, responseText = url });
        }

        [HttpPost]
        public async Task<IActionResult> BookTeamSubEvent(TeamReservationVM viewModel)
        {
            string msg = "Alla fält måste vara ifyllda...";
            if (!ModelState.IsValid)
                return Json(new { success = false, responseText = msg });

            //Save the subcompetition-type in sessionstate for correct mapping of properties later.
            HttpContext.Session.SetString(sessionKeyType, SubCompetitionTypes.Team.ToString());

            //Save the whole reservation in session state to be picked up later for persistment in DB.
            SaveTemporaryReservationInSession(viewModel);
            var url = await paymentManager.InitializePayment(viewModel);

            logger.LogInformation("User: " + CurrentUserId + " PayexUrl: " + url);

            //Save the PayEx payment session string for later use. 
            HttpContext.Session.SetString(sessionKeyPayexUri, url);
            return Json(new { success = true, responseText = url });
        }

        //This method is called from the frontend when every step of the Payex modal is completed.
        public async Task<IActionResult> FinalizeReservation(string status)
        {
            //TODO Create views for every possible fall out
            const string reservation = "Reservation";

            if (status == "Completed")
            {
                //Fetch the paymentSessionUri saved earlier. 
                var paymentSessionUri = HttpContext.Session.GetString(sessionKeyPayexUri);

                //Check status of the payment and persist the reservation if everything is ok. 
                var paymentState = await paymentManager.InspectPaymentStateAsync(paymentSessionUri);

                if (paymentState == PayexPaymentStatus.Aborted.ToString())
                    return Json(Url.Action(nameof(Verification), reservation));

                else if (paymentState == PayexPaymentStatus.Failed.ToString())
                    return Json(Url.Action(nameof(Verification), reservation));

                else if (paymentState == PayexPaymentStatus.Ready.ToString() || paymentState == PayexPaymentStatus.Pending.ToString())
                {
                    PersistReservation();
                    paymentManager.CapturePaymentAsync(paymentSessionUri);
                    //TODO Send mail verification
                    //TODO Event should be lifted here?
                    logger.LogInformation("User: " + CurrentUserId + "|Transaction: 2/2" + " PayexUrl: " + HttpContext.Session.GetString(sessionKeyPayexUri));
                    return Json(Url.Action(nameof(Verification), reservation));
                }
            }
            return Json(Url.Action(nameof(ReservationController.Reserve), reservation));
        }
        [HttpPost]
        public IActionResult PayexLaunched()
        {
            logger.LogInformation("User: " + CurrentUserId + "|Transaction: 1/2");
            return Ok();
        }
        //This method is called from the frontend if the Payex modal is closed.
        [HttpDelete]
        public IActionResult ClearPaymentSession()
        {
            logger.LogWarning("User: " + CurrentUserId + "|Transaction: Aborted" + " PayexUrl: " + HttpContext.Session.GetString(sessionKeyPayexUri));

            HttpContext.Session.Remove(sessionKeyPayexUri);
            HttpContext.Session.Remove(sessionKeyTempReservation);

            return NoContent();
        }
        [HttpPost]
        public IActionResult PayexFailed()
        {
            logger.LogError("User: " + CurrentUserId + " PayexUrl: " + HttpContext.Session.GetString(sessionKeyPayexUri));
            return NoContent();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PaymentStatus(string id, [FromBody] JObject model)
        {
            logger.LogInformation("A Callback was made on: " + id);
            var paymentStatusUrl = model["payment"].ToString();
            var paymentNumber = model["paymentNumber"].ToString();
            var transactionStatusUrl = model["transaction"].ToString();
            var transactionNumber = model["transactionNumber"].ToString();
            try
            {
                var transactionEvent = await paymentManager.InspectTransactionEventAsync(transactionStatusUrl);
                logger.LogInformation("Callback Success: " + id + "TransactionEvent: " + transactionEvent);
            }
            catch (Exception e)
            {
                logger.LogError("An exception was thrown");
            }
            finally
            {
                logger.LogInformation("Callback on " + id + "END");
            }



            //TODO Update paymentstatus in DB??
            return Ok();
        }

        [HttpGet]
        public IActionResult Verification()
        {
            var model = new VerificationVM();
            try
            {
                var info = JsonConvert.DeserializeObject<ModalSubCompetitionVM>(HttpContext.Session.GetString(sessionKeySubEvent));
                model.EventName = info.EventName;
                model.EventStartDate = info.EventStartDate;
                model.EventEndDate = info.EventEndDate;
                model.EventLocation = info.EventLocation;
                model.SubEventDate = info.SubEventDate;
                model.ReferenceNumber = TempData["Reference"].ToString();
                sendReceipt(info.Price, TempData["Reference"].ToString());

            }
            catch
            {
                return new StatusCodeResult(500);
            }

            return View(nameof(Verification), model);
        }

        [HttpGet]
        public IActionResult GetFilteredCompetitions(string difficulty, string location, bool isInEditView)
        {
            //Todo hämta listan från frontend för att slippa hämta ur db hela tiden. 
            int userId = CurrentUserId;
            var model = context.ControllUserReservations(
                    context.GetCompetitionList(userId), userId);

            // Filter by parameters, every if-statment is iterated.
            if (difficulty != "0")
                model = context.GetFilteredListCompetitionsVM(model,
                    o => o.SubCompetition.Any(p => p.Difficulty
                    .Equals((Difficulties)int.Parse(difficulty))));
            if (location != "Alla")
                model = context.GetFilteredListCompetitionsVM(model,
                    o => o.Location.Equals(location));

            //A partial view is returned.  //TODO Stop return of competitions in editview 
            //if the user-id is not the owner of all competitions
            if (isInEditView)
                return PartialView("_CompetitionBoxToEdit", model);

            return PartialView("_CompetitionBox", model);
        }

        #endregion

        #region Private Methods
        private int SetAndGetCurrentUserId()
        {
            string userHashId = userManager.GetUserId(HttpContext.User);
            var currentUserId = context.User.Where(o => o.HashId == userHashId).Select(p => p.Id).First();
            SaveUserIdInSessionState(currentUserId);
            return currentUserId;
        }
        void SaveUserIdInSessionState(int CurrentUserId)
        {
            HttpContext.Session.SetString("CurrentUserId", CurrentUserId.ToString());
        }

        private void sendReceipt(decimal price, string referenceNr)
        {
            var user = context.User.FirstOrDefault(u => u.Id == CurrentUserId);
            var Email = user.Email;
            var Name = user.FirstName + " " + user.LastName;

            mailService.SendMail(MailCategory.SendReceipt, Email, Name, price, referenceNr);
            logger.LogInformation("User: " + CurrentUserId);
        }

        void PersistReservation()
        {
            var reservationJson = HttpContext.Session.GetString(sessionKeyTempReservation);
            var type = HttpContext.Session.GetString(sessionKeyType);

            //Check what kind of subcompetition the session state holds and persist with the desired method.
            if (type == SubCompetitionTypes.Single.ToString())
            {
                var reservation = JsonConvert.DeserializeObject<SingleReservationVM>(reservationJson,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                reservation.PaymentSessionUrl = HttpContext.Session.GetString(sessionKeyPayexUri);
                TempData["Reference"] = reservation.Reference;
                CalculateReservationPrice(reservation);
                context.BookSingleSubComp(reservation);
            }
            else
            {
                var reservation = JsonConvert.DeserializeObject<TeamReservationVM>(reservationJson,
                    new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
                reservation.PaymentSessionUrl = HttpContext.Session.GetString(sessionKeyPayexUri);
                TempData["Reference"] = reservation.Reference;
                CalculateReservationPrice(reservation);
                context.BookTeamSubComp(reservation);
            }
        }

        private IReservation CalculateReservationPrice(IReservation reservation)
        {
            reservation.Price = decimal.Parse(HttpContext.Session.GetString("originalPrice"));

            if (HttpContext.Session.GetString("discount") != null)
            {
                var discountPercentage = decimal.Parse(HttpContext.Session.GetString("discount"));
                reservation.Discount = reservation.Price * (discountPercentage / 100);
                reservation.Paid = reservation.Price - reservation.Discount;
            }
            else
            {
                reservation.Discount = 0;
                reservation.Paid = reservation.Price;
            }

            return reservation;
        }

        void SaveTemporaryReservationInSession(IReservation reservation)
        {  //Fetch the temporary subevent from session state.
            var subEventJson = HttpContext.Session.GetString(sessionKeySubEvent);
            var subCompetition = JsonConvert.DeserializeObject<ModalSubCompetitionVM>(subEventJson, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
            HttpContext.Session.SetString("originalPrice", subCompetition.Price.ToString());
            reservation.EventName = subCompetition.EventName;
            reservation.UserId = CurrentUserId;
            if (HttpContext.Session.GetString("discount") != null)
            {
                var discount = decimal.Parse(HttpContext.Session.GetString("discount"));
                subCompetition.Price -= (subCompetition.Price * discount / 100);
            }
            reservation.Price = subCompetition.Price;
            reservation.ReservationDate = DateTime.Now;
            //Reference must be set last
            var referenceNr = paymentManager.GetReferenceId(reservation);
            reservation.Reference = referenceNr;
            HttpContext.Session.SetString(sessionKeyTempReservation,
                JsonConvert.SerializeObject(reservation,
               new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto }));
        }
        #endregion
    }
}
