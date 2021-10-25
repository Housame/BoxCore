using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using System.IO;
using Microsoft.AspNetCore.Http;
using MimeKit;
using MailKit.Net.Smtp;
using iTextSharp.text;
using iTextSharp.text.pdf;
using BoxShop.Models.SuperAdmin;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BoxShop.Models.Entities
{
    public partial class BoxShopContext : DbContext
    {

        public BoxShopContext(DbContextOptions<BoxShopContext> options) : base(options)
        {

        }

        #region AdminClientController
        public void AddProductToDB(ProductAddVM viewModel, int boxId)
        {
            var model = Mapper.Map<Product>(viewModel);
            model.Available = true;
            model.Vat = CalculateVat(model.Price, model.Category);
            model.BoxId = boxId;
            if (viewModel.Image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    viewModel.FormImage.CopyToAsync(memoryStream);
                    model.Image = memoryStream.ToArray();
                }
            }
            Product.Add(model);
            SaveChanges();
        }

        private decimal CalculateVat(decimal price, int? category)
        {
            var tempPrice = (double)price;
            if (category == 1)
                return (decimal)(tempPrice * 0.06);
            if (category == 2)
                return (decimal)(tempPrice * 0.12);
            else return (decimal)(tempPrice * 0.25);
        }

        public void AddUser(User tempUser)
        {
            User.Add(tempUser);
            SaveChanges();
        }

        internal ProductShowVM[] GetProducts(int boxId)
        {
            return Product
                .Where(p => p.BoxId == boxId)
                .Select(p => Mapper.Map<ProductShowVM>(p))
                .OrderBy(p => p.Category)
                .ToArray();
        }

        internal ProductAddVM GetProduct(int id, int boxId)
        {
            return Mapper.Map<ProductAddVM>(
                Product
                .FirstOrDefault(p => p.Id == id && p.BoxId == boxId));
        }

        internal void EditProduct(int id, ProductAddVM viewModel, int boxId)
        {
            var tempProduct = Product.FirstOrDefault(p => p.Id == id);
            tempProduct.Name = viewModel.Name;
            tempProduct.Available = viewModel.Available;
            tempProduct.Description = viewModel.Description;
            tempProduct.Price = viewModel.Price;
            tempProduct.Category = (int)viewModel.Category;
            tempProduct.Vat = CalculateVat(viewModel.Price, (int)viewModel.Category);
            if (viewModel.FormImage != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    viewModel.FormImage.CopyToAsync(memoryStream);
                    tempProduct.Image = memoryStream.ToArray();
                }
            }
            tempProduct.BoxId = boxId;
            SaveChanges();
        }

        //internal List<UserOrdersVM> GetAllBills()
        //{
        //    return GetUnpaidOrdersForBox(6);
        //    var orders = Order
        //        .Include(item => item.OrderItem)
        //        .Include(item => item.User)
        //        .ToList();
        //    return orders.Select(o => Mapper.Map<UserOrdersVM>(o)).ToList();
        //}
        internal List<UserOrdersVM> GetUnpaidOrdersForBox(int userId)
        {
            var boxId = User.Where(o => o.Id == userId).Select(o => o.BoxId).FirstOrDefault();
            var box = Box.Find(boxId);

            Entry(box).Collection(b => b.Order)
           .Query()
           .Where(p => !p.IsPaid)
           .Load();

            foreach (var order in box.Order)
            {
                Entry(order).Collection(b => b.OrderItem).Load();
                Entry(order).Reference(b => b.User).Load();
            }
            return box.User.Select(o => Mapper.Map<UserOrdersVM>(o)).ToList();
        }

        internal void SendBills(List<UserOrdersVM> model, string sWebRootFolder)
        {
            foreach (var bill in model)
            {
                var strFileName = string.Format("Kvitto-" + bill.FirstName + "-" + bill.LastName + ".pdf");
                var filePath = string.Format(Path.Combine(sWebRootFolder, strFileName));
                BillOneUser(bill, filePath);
                FileInfo file = new FileInfo(filePath);
                file.Delete();//ifall man vill att kvitton tas bort
            }

        }

        private void BillOneUser(UserOrdersVM bill, string filePath)
        {
            //    Document document = new iTextSharp.text.Document();
            //    var name = string.Format(bill.FirstName + " " + bill.LastName);
            //    //var userEmail = User.FirstOrDefault(u => u.HashId == bill.UserInfo.HashId).Email;
            //    using (var fileStream = new FileStream(filePath, FileMode.Create))
            //    {
            //        iTextSharp.text.pdf.PdfWriter.GetInstance(document, fileStream);


            //        document.Open();
            //        decimal totalPrice = 0;
            //        int tableLength = bill.OrderItem.Count;
            //        PdfPTable table = new PdfPTable(8);
            //        table.AddCell("Datum");
            //        table.AddCell("Produkt");
            //        table.AddCell("Antal");
            //        table.AddCell("Pris(Kr)/st");
            //        table.AddCell("Moms(Kr)/st");
            //        table.AddCell("Moms(%)");
            //        table.AddCell("Summa(Kr)");
            //        table.AddCell("Moms Summa");

            //        foreach (var purchase in bill.OrderItem)
            //        {
            //            table.AddCell(purchase.DateOfPurchase.ToString("yyyy-MM-dd"));
            //            table.AddCell(purchase.ProductName);
            //            table.AddCell(purchase.Quantity.ToString());
            //            table.AddCell(purchase.Price.ToString());
            //            table.AddCell(purchase.Vat.ToString());
            //            table.AddCell(Stringify(purchase.Category));
            //            table.AddCell((purchase.Price * purchase.Quantity).ToString());
            //            table.AddCell((purchase.Vat * purchase.Quantity).ToString());
            //            totalPrice += (purchase.Price * purchase.Quantity);

            //        }
            //        string strTotalPrice = string.Format("Slut summan : " + totalPrice + " kronor.");
            //        document.Add(table);
            //        document.Add(new Paragraph(Environment.NewLine));
            //        document.Add(new Paragraph(strTotalPrice));
            //        document.Close();
            //        fileStream.Dispose();
            //        SendReceit(filePath, name);//userEmail skall oxå skickas med
            //    }
        }

        private string Stringify(int category)
        {
            if (category == 1)
            {
                return "6%";
            }
            if (category == 2)
            {
                return "12%";
            }
            else
            {
                return "25%";
            }
        }

        private void SendReceit(string filePath, string name)
        {
            var message = new MimeMessage();
            var builder = new BodyBuilder();
            message.From.Add(new MailboxAddress("CrossFit Södertörns", "housame.oueslati@hotmail.com"));
            message.To.Add(new MailboxAddress(name, "housame.oueslati@hotmail.com"));

            message.Subject = string.Format("Kvitto " + DateTime.Now.Date.ToString("yyyy+MM"));

            // Set the plain-text version of the message text
            builder.TextBody = @"Hej! " + name + Environment.NewLine +
                "Här kommer din månads kvitto från CrossFit Södertörns." + Environment.NewLine +
                "Vi kommer att dra pengarna den " + DateTime.Now.Date.AddDays(1).ToString("yyyy+MM+dd") + Environment.NewLine +
               "Med vänliga hälsningar " + Environment.NewLine +
                "CrossFit Södertörns";
            // We may also want to attach the pdf file...
            builder.Attachments.Add(filePath);

            // Now we just need to set the message body and we're done
            message.Body = builder.ToMessageBody();
            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("housame.oueslati@gmail.com", "adiqgcoasguitpbm");
                client.Send(message);
                client.Disconnect(true);
            }
        }
        #endregion

        #region SuperAdminController

        internal void AddBoxToDB(RegisterBoxVM viewModel)
        {
            var model = new Box() { };
            model.Name = viewModel.Name;
            if (viewModel.FormImage != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    viewModel.FormImage.CopyToAsync(memoryStream);
                    model.Image = memoryStream.ToArray();
                }
            }
            Box.Add(model);
            SaveChanges();
        }

        internal BoxShowVM[] GetBoxes()
        {
            var allBoxes = Box.Select(p => new BoxShowVM
            {
                Id = p.Id,
                Name = p.Name,
                Image = p.Image,
            }).OrderBy(p => p.Name).ToArray();
            return allBoxes;
        }

        internal void DeleteBox(int id)
        {
            var model = Box.SingleOrDefault(r => r.Id == id);
            Box.Remove(model);
            SaveChanges();
        }


        internal List<SelectListItem> GetAllBoxesNames()
        {
            var boxNames = Box
                .GroupBy(x => x.Name)
                .Select(x => x.First())
                .Select(x => new SelectListItem { Text = x.Name }).ToList();
            //boxNames.Insert(0, new SelectListItem { Text = "Select a box" });
            return boxNames;
        }

        public void SendEmailConfirmationMail(string email, string firstName, string lastName, string confirmationString, string password)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("BoxShop", "noreply@localhost.com"));
            message.To.Add(new MailboxAddress(firstName, email));
            message.Subject = "Verifiera din email för att kunna börja med BoxShop ";
            message.Body = new TextPart("html")
            {
                Text = "<p>Hej " + firstName + " " + lastName + "! <br /> <br />" +

                "Ditt e-postmeddelande har registrerats som ett användarkonto för Box Shop. Innan du börjar använda tjänsten, verifiera ditt mail med <a href=\'" + confirmationString + "\'>att klicka på länken</a>. <br /><br />" +

                "Vänligen ange följande lösenord första gången du loggar in: </p>" + password +
                "<P><br /> För att ändra ditt lösenord, klicka på Mitt konto -> Kontoinformation -> Ändra lösenord <br /><br />" +

                "Ser fram emot att ha dig ombord! <br /> <br />" +

                "BoxShop Notifikation: <br /> <br />" +
                "<i>Observera, det här är ett automatiserat mail som skickas från Box Shop-tjänsten. Det går inte att svara denna mail. Tack för att din förståelse. Om du vill kontakta BoxShop, besök <a href='www.syntax3rror.com'>www.syntax3rror.com</a>. Take care!</i></p>"
            };

            using (var client = new SmtpClient())
            {
                client.Connect("smtp.gmail.com", 587, false);
                client.Authenticate("housame.oueslati@gmail.com", "adiqgcoasguitpbm");
                client.Send(message);
                client.Disconnect(true);
            }
        }

        internal void CreateUser(RegisterAdminVM viewModel, string userHashId)
        {
            var userToCreate = new User
            {
                HashId = userHashId,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email
            };
            if (viewModel.Box != null)
            {
                var boxId = Box.SingleOrDefault(c => c.Name.Equals(viewModel.Box)).Id;
                userToCreate.BoxId = boxId;
            }


            User.Add(userToCreate);
            SaveChanges();
        }

        internal int? GetUserBoxID(int userId)
        {
            return User
                .Where(o => o.Id == userId)
                .Select(o => o.BoxId)
                .First();
        }

        internal List<UserShowVM> GetUserRoles(IdentityUser[] adminUsers)
        {

            var tempAdmins = User.Where(b => adminUsers.Any(a => a.Id == b.HashId)).ToArray();
            var admins = tempAdmins
                .Select(a => Mapper.Map<UserShowVM>(a))
                .OrderBy(a => a.FirstName)
                .ToList();
            foreach (var item in admins)
            {
                if (item.BoxId != 0)//Added temporary, it works but needs a optimizing maybe
                {
                    var boxName = Box.FirstOrDefault(b => b.Id == item.BoxId).Name;
                    item.Box = boxName;
                }
            }
            return admins;
        }

        internal RegisterBoxVM GetBox(int id)
        {
            return Mapper.Map<RegisterBoxVM>(
              Box
              .FirstOrDefault(p => p.Id == id));
        }

        internal void EditBox(int id, RegisterBoxVM viewModel)
        {
            var tempBox = Box.FirstOrDefault(p => p.Id == id);
            tempBox.Name = viewModel.Name;
            if (viewModel.FormImage != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    viewModel.FormImage.CopyToAsync(memoryStream);
                    tempBox.Image = memoryStream.ToArray();
                }
            }
            SaveChanges();
        }

        #endregion

        #region UserClientController

        public UserDisplayVM[] GetUsers()
        {
            return User
                .Select(o => Mapper.Map<UserDisplayVM>(o))
                .OrderBy(o => o.FirstName)
                .ToArray();
        }

        public UserDisplayVM[] FilterUsers(string input)
        {
            return User
                .Where(o => o.FirstName.ToLower().Contains(input) || o.LastName.ToLower().Contains(input))
                .Select(o => Mapper.Map<UserDisplayVM>(o))
                .ToArray();
        }

        public UserDisplayVM GetUserById(int id)
        {
            var model = User.FirstOrDefault(o => o.Id == id);
            return Mapper.Map<UserDisplayVM>(model);
        }

        #endregion

        #region StoreController
        public ProductDisplayVM[] GetAvailableProductsFromBox()
        {
            return Product
                .Where(o => o.Available == true)
                .Select(o => Mapper.Map<ProductDisplayVM>(o))
                .OrderBy(o => o.Name)
                .ToArray();
        }

        public ProductDisplayVM GetProductById(int id)
        {
            return Mapper.Map<ProductDisplayVM>(
                Product
                .SingleOrDefault(o => o.Id == id));
        }

        public void PersistOrder(AddOrderVM model)
        {
            var newOrder = Mapper.Map<Order>(model);
            Order.Add(newOrder);
            SaveChanges();
        }

        public VerifyUserInStore[] GetUsersForStore()
        {
            return User
                .Select(o => new VerifyUserInStore()
                {
                    Id = o.Id,
                    FirstName = o.FirstName,
                    LastName = o.LastName,
                    Pin = o.Pin,
                })
                .ToArray();
        }

        public VerifyUserInStore[] FilterUserStore(string input)
        {
            return User
                .Where(o => o.FirstName.ToLower().Contains(input) || o.LastName.ToLower().Contains(input))
                .Select(o => new VerifyUserInStore()
                {
                    Id = o.Id,
                    FirstName = o.FirstName,
                    LastName = o.LastName,
                    Pin = o.Pin,
                })
                .ToArray();
        }
        #endregion

    }
}