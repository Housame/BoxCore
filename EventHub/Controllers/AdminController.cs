using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EventHub.Helpers;
using EventHub.Models.AdminHub;
using EventHub.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventHub.Controllers
{
    [Authorize("ElevatedRights")]
    [Route("api/[controller]")]
    public class AdminController : Controller
    {
        UserManager<IdentityUser> userMngr;
        SignInManager<IdentityUser> signInMngr;
        RoleManager<IdentityRole> roleMngr;
        EventHubContext context;
        IHostingEnvironment env;
        IMailService mailService;

        public AdminController(UserManager<IdentityUser> userMngr,
            SignInManager<IdentityUser> signInMngr,
            RoleManager<IdentityRole> roleMngr,
            EventHubContext context,
            IHostingEnvironment env,
            IMailService mailService)
        {
            this.userMngr = userMngr;
            this.signInMngr = signInMngr;
            this.roleMngr = roleMngr;
            this.context = context;
            this.env = env;
            this.mailService = mailService;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet, Route("getusers")]
        public async Task<IActionResult> GetAllUsersWithClaims()
        {
            var listOfUserWithClaims = new List<AllUsersVM>();
            foreach (var user in userMngr.Users.ToList())
            {
                listOfUserWithClaims.Add(new AllUsersVM
                {
                    User = user,
                    Role = await userMngr.GetRolesAsync(user),
                    Claims = await userMngr.GetClaimsAsync(user)
                });
            }
            listOfUserWithClaims = StringfyClaimValue(listOfUserWithClaims);
            return Ok(listOfUserWithClaims.OrderBy(u => u.User.Email));
        }
        //Helper method that change the claimValue to the box name --if there is a box
        private List<AllUsersVM> StringfyClaimValue(List<AllUsersVM> listOfUserWithClaims)
        {
            foreach (var user in listOfUserWithClaims)
            {
                foreach (var claim in user.Claims)
                {
                    if (claim.Type == "EventManager")
                    {
                        var Box = context.Box.FirstOrDefault(b => b.Id == int.Parse(claim.Value));
                        user.BoxName = Box.Name;
                        user.BoxId = Box.Id;
                    }
                }
            }
            return listOfUserWithClaims;
        }

        [HttpGet, Route("getclaims")]
        public IActionResult GetAllClaims()
        {
            var listOfClaims = new List<string>() { "claim1", "claim2", "claim3", "claim4", };
            return Ok(listOfClaims);
        }

        [HttpGet, Route("getboxes")]
        public IActionResult GetAllBoxes()
        {
            var boxes = context.Box.ToList();
            return Ok(boxes);
        }

        [HttpGet, Route("{id}")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = userMngr.Users.FirstOrDefault(t => t.Id == id);
            var bcUser = context.User.FirstOrDefault(t => t.HashId == id);

            if (user == null)
            {
                return NotFound();
            }
            var userWithClaims = new AllUsersVM
            {
                User = user,
                Role = await userMngr.GetRolesAsync(user),
                Claims = await userMngr.GetClaimsAsync(user),
                BCUser = bcUser
            };

            return Ok(userWithClaims);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserAsync(CreateUserVM userModel)
        {
            if (userModel == null)
            {
                return BadRequest("Ange information.");
            }
            var user = new IdentityUser(userModel.Email);

            await userMngr.SetEmailAsync(user, userModel.Email);
            string password = GenerateRandomString(15);
            await userMngr.AddPasswordAsync(user, password);
            var result = await userMngr.CreateAsync(user);
            if (!result.Succeeded)
            {
                ModelState.AddModelError("FirstName", result.Errors.First().Description);
                return NotFound();
            }

            await userMngr.AddToRoleAsync(user, "Client");

            var confirm = await userMngr.GenerateEmailConfirmationTokenAsync(user);
            var callbackUrl = Url.Action("ConfirmEmail", "UserClient", new { userId = user.Id, code = confirm },
                protocol: HttpContext.Request.Scheme);

            mailService.SendMail(MailCategory.Confirmation, user.Email, callbackUrl, userModel.FirstName, userModel.LastName, password);
            var hashId = await userMngr.GetUserIdAsync(user);

            var userToCreate = new User
            {
                HashId = hashId,
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Email = userModel.Email,
                Gender = userModel.Gender,
                Location = userModel.Location,
                Size = userModel.Size,
                Team = userModel.Team,
                DateOfBirth = DateTime.Parse(userModel.DateOfBirth),
                BoxId = userModel.BoxId 
            };

            context.User.Add(userToCreate);
            context.SaveChanges();
            await userMngr.AddToRoleAsync(user, userModel.Role);
            if (userModel.Claims != null)
            {
                //var claimsToRemove = await userMngr.GetClaimsAsync(user);
                //await userMngr.RemoveClaimsAsync(user, claimsToRemove);
                foreach (var claim in userModel.Claims)
                {
                    switch (claim)
                    {
                        case "EventManager":
                            if (userModel.BoxId != null)
                            {
                                await userMngr.AddClaimAsync(user, new Claim("EventManager", userModel.BoxId.ToString()));
                            }
                            break;
                        case "Box":
                            if (userModel.BoxId != null)
                            {
                                await userMngr.AddClaimAsync(user, new Claim("Box", userModel.BoxId.ToString()));
                            }
                            break;
                        case "CompetitionManager":
                            await userMngr.AddClaimAsync(user, new Claim("CompetitionManager", "Update"));
                            break;
                        case "CompetitionCreator":
                            await userMngr.AddClaimAsync(user, new Claim("CompetitionManager", "Create"));
                            break;
                    }
                }
                context.SaveChanges();
            }
            return Ok(user);
        }

        [HttpPut]
        public async Task<IActionResult> EditUser(UserEditVM userToEdit)
        {

            var user = userMngr.Users.FirstOrDefault(t => t.Id == userToEdit.Id);





            var existingCustomer = context.User.FirstOrDefault(t => t.HashId == userToEdit.Id);
            if (existingCustomer == null)
            {
                var userToCreate = new User
                {
                    HashId = userToEdit.Id,
                    FirstName = userToEdit.FirstName,
                    LastName = userToEdit.LastName,
                    Email = userToEdit.Email,
                    Gender = userToEdit.Gender,
                    Location = userToEdit.Location,
                    Size = userToEdit.Size,
                    Team = userToEdit.Team,
                    DateOfBirth = DateTime.Parse(userToEdit.DateOfBirth),
                    BoxId = userToEdit.BoxId
                };

                context.User.Add(userToCreate);
                context.SaveChanges();
            }

            else
            {
                existingCustomer.FirstName = userToEdit.FirstName;
                existingCustomer.LastName = userToEdit.LastName;
                existingCustomer.Gender = userToEdit.Gender;
                existingCustomer.DateOfBirth = DateTime.Parse(userToEdit.DateOfBirth);
                existingCustomer.Location = userToEdit.Location;
                existingCustomer.Size = userToEdit.Size;
                existingCustomer.Team = userToEdit.Team;
                existingCustomer.BoxId = userToEdit.BoxId != null ? userToEdit.BoxId : null;
                context.User.Update(existingCustomer);
                context.SaveChanges();
            }
            var rolesToRemove = await userMngr.GetRolesAsync(user);
            if (userToEdit.Role != null)
            {
                await userMngr.RemoveFromRolesAsync(user, rolesToRemove);
                await userMngr.AddToRoleAsync(user, userToEdit.Role);
                //var claimsToRemove = await userMngr.GetClaimsAsync(user);
                //await userMngr.RemoveClaimsAsync(user, claimsToRemove);
            }
            if (userToEdit.Claims != null)
            {
                //var claimsToRemove = await userMngr.GetClaimsAsync(user);
                //await userMngr.RemoveClaimsAsync(user, claimsToRemove);
                foreach (var claim in userToEdit.Claims)
                {
                    switch (claim)
                    {
                        case "EventManager":
                            if (userToEdit.BoxId != null)
                            {
                                await userMngr.AddClaimAsync(user, new Claim("EventManager", userToEdit.BoxId.ToString()));
                            }
                            break;
                        case "Box":
                            if (userToEdit.BoxId != null)
                            {
                                await userMngr.AddClaimAsync(user, new Claim("Box", userToEdit.BoxId.ToString()));
                            }
                            break;
                        case "CompetitionManager":
                            await userMngr.AddClaimAsync(user, new Claim("CompetitionManager", "Update"));
                            break;
                        case "CompetitionCreator":
                            await userMngr.AddClaimAsync(user, new Claim("CompetitionManager", "Create"));
                            break;
                    }
                }
            }
            //context.Entry(user).State = EntityState.Modified;
            context.SaveChanges();

            return Ok(userToEdit);
        }

        [HttpDelete]
        public async Task<IActionResult> RemoveClaims(string id)
        {
            var user = userMngr.Users.FirstOrDefault(t => t.Id == id);
            var claimsToRemove = await userMngr.GetClaimsAsync(user);
            await userMngr.RemoveClaimsAsync(user, claimsToRemove);
            context.SaveChanges();

            return Ok("Delete one user succeeded");
        }
        private string GenerateRandomString(int length)
        {
            //Removed O, o, 0, l, 1
            string allowedLetterChars = "abcdefghijkmnpqrstuvwxyzABCDEFGHJKLMNPQRSTUVWXYZ";
            string allowedNumberChars = "123456789";
            char[] chars = new char[length];
            Random rd = new Random();

            bool useLetter = true;
            bool useToUpper = true;
            for (int i = 0; i < length; i++)
            {
                if (useLetter)
                {
                    if (useToUpper)
                    {
                        chars[i] = allowedLetterChars.ToUpper()[rd.Next(0, allowedLetterChars.Length)];
                        useToUpper = false;
                    }
                    else
                    {
                        chars[i] = allowedLetterChars[rd.Next(0, allowedLetterChars.Length)];
                        useToUpper = true;
                    }
                    useLetter = false;
                }
                else
                {
                    chars[i] = allowedNumberChars[rd.Next(0, allowedNumberChars.Length)];
                    useLetter = true;
                }

            }

            return new string(chars);
        }
    }
}
