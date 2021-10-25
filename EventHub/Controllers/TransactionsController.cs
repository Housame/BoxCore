using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Helpers;
using EventHub.Models;
using EventHub.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Controllers
{
    [Authorize("ElevatedRights")]
    public class TransactionsController : Controller
    {
        private readonly EventHubContext context;
        private readonly IPaymentManager paymentManager;

        public TransactionsController(EventHubContext context, IPaymentManager paymentManager)
        {
            this.context = context;
            this.paymentManager = paymentManager;
        }
        [HttpGet("transactions/{id:int}")]
        public async Task<IActionResult> Index(int id)
        {
            var model = context.GetTransactionsForCompetition(id);
            foreach (var reservation in model.Reservation)
            {
                try
                {
                    reservation.Instrument = await paymentManager.InspectPaymentInstrumentAsync(reservation.PaymentSessionUrl);
                }
                catch
                {
                    reservation.Instrument = "Failed to fetch data";
                }
            }
            return View(model);
        }
        public async Task<IActionResult> MakeNewTransaction(NewTransactionVM model)
        {
            context.MakeTransaction(model);
            var viewModel = context.GetTransactionsForCompetition(model.CompetitionId);
            foreach (var reservation in viewModel.Reservation)
            {
                try
                {
                    reservation.Instrument = await paymentManager.InspectPaymentInstrumentAsync(reservation.PaymentSessionUrl);
                }
                catch
                {
                    reservation.Instrument = "Failed to fetch data";
                }
               
            }
            return PartialView("Index", viewModel);
        }
    }
}