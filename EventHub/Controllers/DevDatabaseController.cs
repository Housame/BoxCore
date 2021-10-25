using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventHub.Models.Entities;
using EventHub.Models.UserClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace EventHub.Controllers
{
    [Authorize("ElevatedRights")]
    public class DevDatabaseController : Controller
    {
        private readonly IHostingEnvironment env;
        private readonly UserManager<IdentityUser> userManager;
        private readonly EventHubContext context;

        public DevDatabaseController(IHostingEnvironment env, UserManager<IdentityUser> userManager,
            EventHubContext context)
        {
            this.env = env;
            this.userManager = userManager;
            this.context = context;

        }
        public IActionResult Index()
        {
            ViewData["Environment"] = env.EnvironmentName;
            return View();
        }

        public async Task<IActionResult> Populate()
        {
            if (env.IsDevelopment())
            {
             //   await AddDefaultUsers();
              //  AddDefaultCompetitions();
                return Ok(env.EnvironmentName);
            }
            return NoContent();
        }
        //public IActionResult AddUserIdToAthlete()
        //{           
        //    foreach(var athlete in context.Athlete)
        //    {
        //        var athleteEnt = context.User.Where(o => o.Email == athlete.Email).FirstOrDefault();
        //        if(athleteEnt != null)
        //        {
        //            athlete.UserId = athleteEnt.Id;
        //            context.Athlete.Update(athlete);
        //            context.SaveChanges();
        //        }              
        //    }
        //    return Ok();
        //}
        //public IActionResult AddAllBoxes()
        //{
        //    var file = Path.Combine(Environment.CurrentDirectory, "Boxar från CrossFit.txt");
        //    var boxlist = System.IO.File.ReadAllLines(file, System.Text.Encoding.UTF8);
        //    foreach(var box in boxlist)
        //    {
        //        var sep = "  ||***********||  ";
        //        var wordArray = box.Split(sep);
        //        context.Add(
        //            new Box { Url = wordArray[0], Name = wordArray[1], Location = wordArray[2] }
        //            );
        //        context.SaveChanges();
        //    }
        //    return Ok();
        //}
        void AddDefaultCompetitions()
        {
            var eventAdmins = userManager.GetUsersInRoleAsync("EventAdmin").Result;

            var file = Path.Combine(Environment.CurrentDirectory, "Helpers", "DataBasePopulation", "DefaultCompetitions.csv");
            using (var streamReader = System.IO.File.OpenText(file))
            {
                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    var data = line.Split(new[] { ',' });
                    if (eventAdmins[int.Parse(data[0])] != null)
                    {
                        context.Add(new Competition
                        {
                            CreatorId = eventAdmins[int.Parse(data[0])].Id,
                            Name = data[1],
                            StartDate = Convert.ToDateTime(data[2]),
                            EndDate = Convert.ToDateTime(data[3]),
                            Location = data[4],
                            Description = data[5],
                            Published = Convert.ToBoolean(data[6]),
                            OpenForBookings = Convert.ToBoolean(data[7])
                        });
                    }
                }
            }
            context.SaveChanges();
        }


        async Task AddDefaultUsers()
        {
            var file = Path.Combine(Environment.CurrentDirectory, "Helpers", "DataBasePopulation", "DefaultUsers.csv");

            //if (!System.IO.File.Exists(file))
            //    return NotFound();

            using (var streamReader = System.IO.File.OpenText(file))
            {

                while (!streamReader.EndOfStream)
                {
                    var line = streamReader.ReadLine();
                    var data = line.Split(new[] { ',' });
                    var identityUser = new IdentityUser(data[3])
                    {
                        Email = data[3],
                        PhoneNumber = data[9]
                    };

                    var result = await userManager.CreateAsync(identityUser);
                    await userManager.AddToRoleAsync(identityUser, data[10]);

                    var user = new User()
                    {
                        FirstName = data[1],
                        LastName = data[2],
                        Email = data[3],
                        Gender = data[4],
                        DateOfBirth = Convert.ToDateTime(data[5]),
                        Location = data[6],
                        Team = data[7],
                        Size = data[8],
                        HashId = identityUser.Id
                    };
                    context.Add(user);
                }
            }
            context.SaveChanges();
        }

    }
}