using EventHub.Models.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Helpers
{
    public interface IPaymentManager
    {
        Task<string> InspectTransactionEventAsync(string transactionUrl);
        Task<string> InitializePayment(IReservation model);
        Task<string> InspectPaymentStateAsync(string paymentSessionUri);
        Task<string> InspectPaymentInstrumentAsync(string paymentSessionUri);
        Task CapturePaymentAsync(string paymentSessionUri);      
        IConfiguration Configuration { get;  set; }
        string GetReferenceId(IReservation model);


    }
}
