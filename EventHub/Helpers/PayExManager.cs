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
using Microsoft.Extensions.Configuration;
namespace EventHub.Helpers
{
    public class PayExManager: IPaymentManager
    {       
        public IConfiguration Configuration { get; set; }
        
        public PayExManager(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
        }

        public async Task<string> InitializePayment(IReservation model)
        {
            var merchantToken =  Configuration["Payex:MerchantToken"];
            var payexUrl = Configuration["Payex:PayexUrl"];
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + merchantToken);

            //        1.Retrieve the Home resource and locate the Payment Session base URL.
            var homeResponse = await httpClient.GetAsync(payexUrl);
            var homeContent = await homeResponse.Content.ReadAsStringAsync();
            var home = Newtonsoft.Json.Linq.JObject.Parse(homeContent);
            var paymentSessionBaseUrl = home["paymentSession"].ToString();
            string referenceId = GetReferenceId(model);
            //        2.Perform the POST request to the Payment Session base URL to create a Payment Session
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                amount = model.Price,
                vatAmount = Math.Round(Vatify(model.Price), 2),
                currency = "SEK",
                callbackUrl = "https://www.boxcore.net/reservation/paymentstatus/" + referenceId,
                reference = referenceId,
                acquire = new[] { "email" },
                culture = "sv-SE"
            });


            var paymentSessionContent = new StringContent(json, Encoding.UTF8, "application/json");
            var paymentSessionResponse = await httpClient.PostAsync(paymentSessionBaseUrl, paymentSessionContent);

            //        3.Find the URL of the created Payment Session in the HTTP 'Location' header and store it alongside the order, shopping cart, or similar.
            var paymentSessionUrl = paymentSessionResponse.Headers.Location;
            
            return paymentSessionUrl.AbsoluteUri;
        }

        public async Task<string> InspectPaymentStateAsync(string paymentSessionUri)
        {
            var merchantToken =  Configuration["Payex:MerchantToken"];
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + merchantToken);

            // 1. Retrieve the paymentSessionUrl related to the submitted form from storage.
            var paymentSessionUrl = paymentSessionUri;

            // 2. Retrieve the Payment Session and find the URL of the Payment.
            var paymentSessionResponse = await httpClient.GetAsync(paymentSessionUrl);
            var paymentSessionContent = await paymentSessionResponse.Content.ReadAsStringAsync();
            var paymentSession = Newtonsoft.Json.Linq.JObject.Parse(paymentSessionContent);
            var paymentUrl = paymentSession["payment"].ToString();

            // 3. Retrieve the Payment and inspect its state.
            var paymentResponse = await httpClient.GetAsync(paymentUrl);
            var paymentContent = await paymentResponse.Content.ReadAsStringAsync();
            var payment = Newtonsoft.Json.Linq.JObject.Parse(paymentContent);
            var paymentSessionPayment = payment["payment"];
            var paymentState = paymentSessionPayment["state"].ToString();

            return paymentState;
        }
        public async Task<string> InspectPaymentInstrumentAsync(string paymentSessionUri)
        {
            var merchantToken = Configuration["Payex:MerchantToken"];
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + merchantToken);

            // 1. Retrieve the paymentSessionUrl related to the submitted form from storage.
            var paymentSessionUrl = paymentSessionUri;

            // 2. Retrieve the Payment Session and find the URL of the Payment.
            var paymentSessionResponse = await httpClient.GetAsync(paymentSessionUrl);
            var paymentSessionContent = await paymentSessionResponse.Content.ReadAsStringAsync();
            var paymentSession = Newtonsoft.Json.Linq.JObject.Parse(paymentSessionContent);
            var paymentUrl = paymentSession["payment"].ToString();

            // 3. Retrieve the Payment and inspect its state.
            var paymentResponse = await httpClient.GetAsync(paymentUrl);
            var paymentContent = await paymentResponse.Content.ReadAsStringAsync();
            var payment = Newtonsoft.Json.Linq.JObject.Parse(paymentContent);
            var paymentSessionPayment = payment["payment"];
            var paymentInstrument = paymentSessionPayment["instrument"].ToString();

            return paymentInstrument;
        }
        public async Task<string> InspectTransactionEventAsync(string transactionUrl)
        {
            var merchantToken =  Configuration["Payex:MerchantToken"];
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + merchantToken);

            var transactionResponse = await httpClient.GetAsync(transactionUrl);
            var transactionContent = await transactionResponse.Content.ReadAsStringAsync();
            var transaction = Newtonsoft.Json.Linq.JObject.Parse(transactionContent);
            var transactionSession = transaction["transaction"];
            var transactionType = transactionSession["type"].ToString();

            return transactionType;
        }
       public async Task CapturePaymentAsync(string paymentSessionUri)
        {
            var merchantToken =  Configuration["Payex:MerchantToken"];
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + merchantToken);

            // 1. Retrieve the paymentSessionUrl related to the submitted form from storage.
            var paymentSessionUrl = paymentSessionUri;

            // 2. Retrieve the Payment Session and find the URL of the Payment.
            var paymentSessionResponse = await httpClient.GetAsync(paymentSessionUrl);
            var paymentSessionContent = await paymentSessionResponse.Content.ReadAsStringAsync();
            var paymentSession = Newtonsoft.Json.Linq.JObject.Parse(paymentSessionContent);
            var paymentUrl = paymentSession["payment"].ToString();

            // 3. Retrieve the Payment and find its 'create-checkout-capture' operation.
            var paymentResponse = await httpClient.GetAsync(paymentUrl);
            var paymentContent = await paymentResponse.Content.ReadAsStringAsync();
            var payment = Newtonsoft.Json.Linq.JObject.Parse(paymentContent);
            var captureUrl = payment["operations"]
                .Children()
                .First(x => x["rel"].ToString() == "create-checkout-capture")["href"]
                .ToString();

            // 4. Perform the capture request.
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(new
            {
                transaction = new
                {
                    description = "Captured the payment"
                }
            });
            var captureRequestContent = new StringContent(json, Encoding.UTF8, "application/json");
            var captureResponse = await httpClient.PostAsync(captureUrl, captureRequestContent);
            var captureResponseContent = await captureResponse.Content.ReadAsStringAsync();
            var capture = Newtonsoft.Json.Linq.JObject.Parse(captureResponseContent);
        }
      public string GetReferenceId(IReservation model)
        {
            string frstPart = DateTime.Now.ToString("yyyy");
            string scndPart = model.UserId.ToString();
            string thrdPart = model.EventName.Substring(0, 2);
            string frthPart = model.SubCompId.ToString();
            string referenceToGet = string.Concat("EH" + frstPart + scndPart + thrdPart + frthPart);
            return referenceToGet;
        }
        private decimal Vatify(decimal price)
        {
            var priceWithoutVat = price / 1.06M;
            return price - priceWithoutVat;
        }
    }
}
