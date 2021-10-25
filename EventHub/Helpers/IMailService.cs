using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static EventHub.Helpers.MailService;

namespace EventHub.Helpers
{
   public interface IMailService
    {
        IConfiguration Configuration { get; set; }
        void SendMail(MailCategory mailCategory, string recepientEmail, string confirmationString, string firstName, string lastName, string password);
        void SendMail(MailCategory mailCategory, string recepientEmail, string confirmationString);
        void SendMail(MailCategory mailCategory, string recepientEmail, string hostFName, string hostLName);
        void SendMail(MailCategory mailCategory, string recepientEmail, string name, decimal price, string referenceNr);
    }
}
