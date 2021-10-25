using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Helpers
{
    public enum MailCategory { Confirmation, RetrievePassword, SendInvitation, SendReceipt, Custom }
    public class MailService: IMailService
    {        
        public IConfiguration Configuration { get; set; }
        const string SENDER_NAME = "BoxCore";
        const string SENDER_EMAIL = "noreply@boxcore.net";
        MimeMessage Message { get; set; }

        MailCategory mailCategory;
        string recepientEmail;
        string confirmationString;
        string firstName;
        string lastName;
        string password;
        string hostFName;
        string hostLName;
        string name;
        decimal price;
        string referenceNr;



        public MailService(IConfiguration Configuration)
        {
            this.Configuration = Configuration;
        }
        /// <summary>
        /// A default account confirmation mail is sent with sender name as "BoxCore" and address as "noreply@boxcore.net".
        /// </summary>
        /// <param name="mailCategory"></param>
        /// <param name="recepientEmail"></param>
        /// <param name="confirmationString"></param>
        public void SendMail(MailCategory mailCategory, string recepientEmail, string confirmationString)
        {
            this.mailCategory = mailCategory;
            this.recepientEmail = recepientEmail;
            this.confirmationString = confirmationString;
            ConstructMail();          
        }
        /// <summary>
        ///  A default retrieve password mail is sent with sender name as "BoxCore" and address as "noreply@boxcore.net".
        /// </summary>
        /// <param name="mailCategory"></param>
        /// <param name="recepientEmail"></param>
        /// <param name="confirmationString"></param>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="password"></param>
        public void SendMail(MailCategory mailCategory, string recepientEmail, string confirmationString, string firstName, string lastName, string password)
        {
            this.mailCategory = mailCategory;
            this.recepientEmail = recepientEmail;
            this.confirmationString = confirmationString;
            this.firstName = firstName;
            this.lastName = lastName;
            this.password = password;
            ConstructMail();
        }
        /// <summary>
        /// A default account confirmation mail is sent with sender name as "BoxCore" and address as "noreply@boxcore.net".
        /// </summary>
        /// <param name="mailCategory"></param>
        /// <param name="recepientEmail"></param>
        /// <param name="hostFName"></param>
        /// <param name="hostLName"></param>
        public void SendMail(MailCategory mailCategory, string recepientEmail, string hostFName, string hostLName)
        {
            this.mailCategory = mailCategory;
            this.recepientEmail = recepientEmail;
            this.hostFName = hostFName;
            this.hostLName = hostLName;
            ConstructMail();
        }
        /// <summary>
        /// A default account confirmation mail is sent with sender name as "BoxCore" and address as "noreply@boxcore.net".
        /// </summary>
        /// <param name="mailCategory"></param>
        /// <param name="recepientEmail"></param>
        /// <param name="name"></param>
        /// <param name="price"></param>
        /// <param name="referenceNr"></param>
        public void SendMail(MailCategory mailCategory, string recepientEmail, string name, decimal price, string referenceNr)
        {
            this.mailCategory = mailCategory;
            this.recepientEmail = recepientEmail;
            this.name = name;
            this.price = price;
            this.referenceNr = referenceNr;
            ConstructMail();
        }
        void Send(MimeMessage message)
        {
            var address =  Configuration["MailService:Address"];
            var password = Configuration["MailService:Password"];

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate(address, password);
                client.Send(message);
                client.Disconnect(true);
            }
        }

        public void ConstructMail()
        {
            Message = new MimeMessage();
            Message.From.Add(new MailboxAddress(SENDER_NAME, SENDER_EMAIL));
            if(mailCategory == MailCategory.RetrievePassword)
            {
                Message.To.Add(new MailboxAddress("Lösenordsåterställning", recepientEmail));
                Message.Subject = "Lösenordsåterställning - BoxCore";
                Message.Body = RetrievePasswordEmail();
            }
            else if(mailCategory == MailCategory.Confirmation)
            {
                Message.To.Add(new MailboxAddress(firstName, recepientEmail));
                Message.Subject = "Verifiera din e-post innan du kan börja använda BoxCore.";
                Message.Body = ConfirmationEmail();
            }
            else if (mailCategory == MailCategory.SendInvitation)
            {
                Message.To.Add(new MailboxAddress(recepientEmail));
                Message.Subject = "Du har fått en inbjudan till BoxCore!";
                Message.Body = InvitationEmail();
            }
            else if (mailCategory == MailCategory.SendReceipt)
            {
                Message.To.Add(new MailboxAddress(recepientEmail));
                Message.Subject = "Kvitto på ditt köp hos BoxCore!";
                Message.Body = SendReceiptEmail();
            }

            Send(Message); 

        }
        TextPart ConfirmationEmail()
        {
            return new TextPart("html")
            {
                Text = "<p>Hej " + firstName + " " + lastName + "! <br /> <br />" +

                "Ditt e-postmeddelande har registrerats som ett användarkonto hos <b>BoxCore</b>.<br /><br />" +

                "Innan du kan börja använda tjänsten behöver du verifiera din e-post genom att klicka <a href=" + confirmationString + ">här</a>. <br /><br />" +

                "Vänligen se till att du fyller i all profilinformation och anger ett lösenord efter att du har bekräftat.<br /><br />" +

                "För att ändra ditt lösenord i framtiden väljer du menyn: <b>Atlet</b> -> <b>Ändra lösenord</b>.<br /><br />" +

                "Du kan såklart även redigera din information i menyn: <b>Atlet</b> -> <b>Redigera profil</b>.<br /><br />" +

                "Vi på <b>BoxCore</b> ser fram emot att ha dig ombord! <br /> <br />" +

                "Meddelande från <b>BoxCore</b>: <br /><br />" +
                 "<i><b>OBS!</b> Detta är ett automatiserat mail som skickas från <b>BoxCore</b>-tjänsten.<br />Det går inte att svara på detta mail, tack för att din förståelse.<br /><b>Mer info:</b> <a href='https://www.boxcore.net'>www.boxcore.net</a>.<br /><br /><i>Take care!</i>" +
                 "<hr>" +
                 "<table><tbody><tr><td width='140' valign='middle' style='width:140px; padding:0; font-family:Arial; text-align:center; vertical-align:middle;'><img width='100' height='100' style='width:100px; height:100px; border-radius:50px; border:0;' alt='Photograph' src='https://thumb.ibb.co/h5FRBG/Box_Core01.png' border='0'></td><td valign='top' style='font-family:Arial; border-bottom:2px solid #000000; padding:0; vertical-align:top;'><table style='font-family:Arial, sans-serif;' cellspacing='0' cellpadding='0'><tbody><tr><td valign='top' style='font-family:Arial; padding-bottom:6px; padding-top:0; padding-left:0; padding-right:0; vertical-align:top;'><strong><span style='font-family:Arial; color:#000000; font-size:14pt;'>BoxCore </span></strong><br><span style='font-family:Arial; color:#000000; font-size:10pt;'>Support</span></td></tr><tr><td valign='top' style='font-family:Arial; padding-bottom:6px; padding-top:0; padding-left:0; padding-right:0; line-height:18px; vertical-align:top;'><span style='font-family:Arial; font-size:10pt;'>E-post: support@boxcore.net<br> </span></td></tr></tbody></table></td></tr><tr><td width='140' valign='middle' style='font-family:Arial; width:140px; padding-top:6px; padding-left:0; padding-right:0; text-align:center; vertical-align:middle;'><span><a href='https://www.facebook.com/BoxCore-2000779003487467/' target='_blank'> <img width='16' style='border:0; height:16px; width:16px' alt='Facebook icon' src='https://signature-generator.cdn.codetwo.com/images/photo2/fb.png' border='0'></a></span><span><a href='https://twitter.com/BoxCore_Sverige' target='_blank'><img width='16' style='border:0; height:16px; width:16px' alt='Twitter icon' src='https://signature-generator.cdn.codetwo.com/images/photo2/tt.png' border='0'></a></span><span><a href='https://se.linkedin.com/company/boxcore?trk=ppro_cprof' target='_blank'><img width='16' style='border:0; height:16px; width:16px' alt='LinkedIn icon' src='https://signature-generator.cdn.codetwo.com/images/photo2/ln.png' border='0'></a></span><span><a href='https://www.instagram.com/boxcore_sverige/' target='_blank'><img width='16' style='border:0; height:16px; width:16px' alt='Instagram icon' src='https://signature-generator.cdn.codetwo.com/images/photo2/it.png' border='0'></a></span></td><td valign='middle' style='padding-top:6px; padding-bottom:0; padding-left:0; padding-right:0; font-family:Arial; vertical-align:middle;'>    <a style='color:#000000; font-family:Arial; font-size:10pt' href='http://www.boxcore.net' target='_blank'>www.boxcore.net</a></td></tr></tbody></table>"
            };
        }
         TextPart RetrievePasswordEmail()
        {
           return new TextPart("html")
            {
                Text = "Hej!<br /><br />Här kommer länken för lösenordsåterställning.<br />" +

                "Om du inte beställt detta kan du ignorera mailet, klicka <a href=" + confirmationString + ">här</a> för att gå vidare. <br /> <br />" +
                "Meddelande från <b>BoxCore</b>: <br /><br />" +
                 "<i><b>OBS!</b> Detta är ett automatiserat mail som skickas från <b>BoxCore</b>-tjänsten.<br />Det går inte att svara på detta mail, tack för att din förståelse.<br /><b>Mer info:</b> <a href='https://www.boxcore.net'>www.boxcore.net</a>.<br /><br /><i>Take care!</i>" +
                 "<hr>" +
                 "<table><tbody><tr><td width='140' valign='middle' style='width:140px; padding:0; font-family:Arial; text-align:center; vertical-align:middle;'><img width='100' height='100' style='width:100px; height:100px; border-radius:50px; border:0;' alt='Photograph' src='https://thumb.ibb.co/h5FRBG/Box_Core01.png' border='0'></td><td valign='top' style='font-family:Arial; border-bottom:2px solid #000000; padding:0; vertical-align:top;'><table style='font-family:Arial, sans-serif;' cellspacing='0' cellpadding='0'><tbody><tr><td valign='top' style='font-family:Arial; padding-bottom:6px; padding-top:0; padding-left:0; padding-right:0; vertical-align:top;'><strong><span style='font-family:Arial; color:#000000; font-size:14pt;'>BoxCore </span></strong><br><span style='font-family:Arial; color:#000000; font-size:10pt;'>Support</span></td></tr><tr><td valign='top' style='font-family:Arial; padding-bottom:6px; padding-top:0; padding-left:0; padding-right:0; line-height:18px; vertical-align:top;'><span style='font-family:Arial; font-size:10pt;'>E-post: support@boxcore.net<br> </span></td></tr></tbody></table></td></tr><tr><td width='140' valign='middle' style='font-family:Arial; width:140px; padding-top:6px; padding-left:0; padding-right:0; text-align:center; vertical-align:middle;'><span><a href='https://www.facebook.com/BoxCore-2000779003487467/' target='_blank'> <img width='16' style='border:0; height:16px; width:16px' alt='Facebook icon' src='https://signature-generator.cdn.codetwo.com/images/photo2/fb.png' border='0'></a></span><span><a href='https://twitter.com/BoxCore_Sverige' target='_blank'><img width='16' style='border:0; height:16px; width:16px' alt='Twitter icon' src='https://signature-generator.cdn.codetwo.com/images/photo2/tt.png' border='0'></a></span><span><a href='https://se.linkedin.com/company/boxcore?trk=ppro_cprof' target='_blank'><img width='16' style='border:0; height:16px; width:16px' alt='LinkedIn icon' src='https://signature-generator.cdn.codetwo.com/images/photo2/ln.png' border='0'></a></span><span><a href='https://www.instagram.com/boxcore_sverige/' target='_blank'><img width='16' style='border:0; height:16px; width:16px' alt='Instagram icon' src='https://signature-generator.cdn.codetwo.com/images/photo2/it.png' border='0'></a></span></td><td valign='middle' style='padding-top:6px; padding-bottom:0; padding-left:0; padding-right:0; font-family:Arial; vertical-align:middle;'>    <a style='color:#000000; font-family:Arial; font-size:10pt' href='http://www.boxcore.net' target='_blank'>www.boxcore.net</a></td></tr></tbody></table>"
           };
        }

        TextPart InvitationEmail()
        {
            return new TextPart("html")
            {
                Text = "Hej!<br /><br />Du har fått en inbjudan till <b>BoxCore</b>.<br />" +

                 "<b>"+hostFName +" " + hostLName + "</b> har bokat en tävling hos oss och önskar att ha dig som lagkamrat.<br />" +
                 "För att kunna lägga till dig behöver du vara en registrerad användare hos oss på <b>BoxCore</b>" +
                "<a href='https://www.boxcore.net'> Klicka här</a> <br />" +
                "<b>BoxCore</b> ser emot att ha dig ombord!<br /><br />" +
                "Meddelande från <b>BoxCore</b>: <br /><br />" +
                 "<i><b>OBS!</b> Detta är ett automatiserat mail som skickas från <b>BoxCore</b>-tjänsten.<br />Det går inte att svara på detta mail, tack för att din förståelse.<br /><b>Mer info:</b> <a href='https://www.boxcore.net'>www.boxcore.net</a>.<br /><br /><i>Take care!</i>" +
                 "<hr>" +
                 "<table><tbody><tr><td width='140' valign='middle' style='width:140px; padding:0; font-family:Arial; text-align:center; vertical-align:middle;'><img width='100' height='100' style='width:100px; height:100px; border-radius:50px; border:0;' alt='Photograph' src='https://thumb.ibb.co/h5FRBG/Box_Core01.png' border='0'></td><td valign='top' style='font-family:Arial; border-bottom:2px solid #000000; padding:0; vertical-align:top;'><table style='font-family:Arial, sans-serif;' cellspacing='0' cellpadding='0'><tbody><tr><td valign='top' style='font-family:Arial; padding-bottom:6px; padding-top:0; padding-left:0; padding-right:0; vertical-align:top;'><strong><span style='font-family:Arial; color:#000000; font-size:14pt;'>BoxCore </span></strong><br><span style='font-family:Arial; color:#000000; font-size:10pt;'>Support</span></td></tr><tr><td valign='top' style='font-family:Arial; padding-bottom:6px; padding-top:0; padding-left:0; padding-right:0; line-height:18px; vertical-align:top;'><span style='font-family:Arial; font-size:10pt;'>E-post: support@boxcore.net<br> </span></td></tr></tbody></table></td></tr><tr><td width='140' valign='middle' style='font-family:Arial; width:140px; padding-top:6px; padding-left:0; padding-right:0; text-align:center; vertical-align:middle;'><span><a href='https://www.facebook.com/BoxCore-2000779003487467/' target='_blank'> <img width='16' style='border:0; height:16px; width:16px' alt='Facebook icon' src='https://signature-generator.cdn.codetwo.com/images/photo2/fb.png' border='0'></a></span><span><a href='https://twitter.com/BoxCore_Sverige' target='_blank'><img width='16' style='border:0; height:16px; width:16px' alt='Twitter icon' src='https://signature-generator.cdn.codetwo.com/images/photo2/tt.png' border='0'></a></span><span><a href='https://se.linkedin.com/company/boxcore?trk=ppro_cprof' target='_blank'><img width='16' style='border:0; height:16px; width:16px' alt='LinkedIn icon' src='https://signature-generator.cdn.codetwo.com/images/photo2/ln.png' border='0'></a></span><span><a href='https://www.instagram.com/boxcore_sverige/' target='_blank'><img width='16' style='border:0; height:16px; width:16px' alt='Instagram icon' src='https://signature-generator.cdn.codetwo.com/images/photo2/it.png' border='0'></a></span></td><td valign='middle' style='padding-top:6px; padding-bottom:0; padding-left:0; padding-right:0; font-family:Arial; vertical-align:middle;'>    <a style='color:#000000; font-family:Arial; font-size:10pt' href='http://www.boxcore.net' target='_blank'>www.boxcore.net</a></td></tr></tbody></table>"
            };
        }

        TextPart SendReceiptEmail()
        {
            return new TextPart("html")
            {
                Text = "Hej,  <b>" + name + "</b>.<br /><br />" +

                 "Tack för din bokning!<br /><br />" +
                 "Den genomfördes " + DateTime.Now.ToString("yyyy-MM-dd")+ " och ditt referensnummer är <b>" + referenceNr + "</b> .<br /><br />" +
                 "Totalsumman för din bokning är <b>" + price+ " SEK </b> (inkl. moms). <br /><br />" +
                 "För vidare detaljer se menyn:<b> Atlet  ->  Min profil</b> på <a href='https://www.boxcore.net'> www.BoxCore.net</a> eller kontakta arrangören. <br /><br />" +

                "Meddelande från <b>BoxCore</b>: <br /><br />" +
                 "<i><b>OBS!</b> Detta är ett automatiserat mail som skickas från <b>BoxCore</b>-tjänsten.<br />Det går inte att svara på detta mail, tack för att din förståelse.<br /><b>Mer info:</b> <a href='https://www.boxcore.net'> www.BoxCore.net</a><br /><br /><i>Take care!</i>" +
                 "<hr>" +
                 "<table><tbody><tr><td width='140' valign='middle' style='width:140px; padding:0; font-family:Arial; text-align:center; vertical-align:middle;'><img width='100' height='100' style='width:100px; height:100px; border-radius:50px; border:0;' alt='Photograph' src='https://thumb.ibb.co/h5FRBG/Box_Core01.png' border='0'></td><td valign='top' style='font-family:Arial; border-bottom:2px solid #000000; padding:0; vertical-align:top;'><table style='font-family:Arial, sans-serif;' cellspacing='0' cellpadding='0'><tbody><tr><td valign='top' style='font-family:Arial; padding-bottom:6px; padding-top:0; padding-left:0; padding-right:0; vertical-align:top;'><strong><span style='font-family:Arial; color:#000000; font-size:14pt;'>BoxCore </span></strong><br><span style='font-family:Arial; color:#000000; font-size:10pt;'>Support</span></td></tr><tr><td valign='top' style='font-family:Arial; padding-bottom:6px; padding-top:0; padding-left:0; padding-right:0; line-height:18px; vertical-align:top;'><span style='font-family:Arial; font-size:10pt;'>E-post: support@boxcore.net<br> </span></td></tr></tbody></table></td></tr><tr><td width='140' valign='middle' style='font-family:Arial; width:140px; padding-top:6px; padding-left:0; padding-right:0; text-align:center; vertical-align:middle;'><span><a href='https://www.facebook.com/BoxCore-2000779003487467/' target='_blank'> <img width='16' style='border:0; height:16px; width:16px' alt='Facebook icon' src='https://signature-generator.cdn.codetwo.com/images/photo2/fb.png' border='0'></a></span><span><a href='https://twitter.com/BoxCore_Sverige' target='_blank'><img width='16' style='border:0; height:16px; width:16px' alt='Twitter icon' src='https://signature-generator.cdn.codetwo.com/images/photo2/tt.png' border='0'></a></span><span><a href='https://se.linkedin.com/company/boxcore?trk=ppro_cprof' target='_blank'><img width='16' style='border:0; height:16px; width:16px' alt='LinkedIn icon' src='https://signature-generator.cdn.codetwo.com/images/photo2/ln.png' border='0'></a></span><span><a href='https://www.instagram.com/boxcore_sverige/' target='_blank'><img width='16' style='border:0; height:16px; width:16px' alt='Instagram icon' src='https://signature-generator.cdn.codetwo.com/images/photo2/it.png' border='0'></a></span></td><td valign='middle' style='padding-top:6px; padding-bottom:0; padding-left:0; padding-right:0; font-family:Arial; vertical-align:middle;'>    <a style='color:#000000; font-family:Arial; font-size:10pt' href='http://www.boxcore.net' target='_blank'>www.boxcore.net</a></td></tr></tbody></table>"
            };
        }


    }
}
