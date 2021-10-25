using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using EventHub.Models;
using MimeKit;
using MailKit.Net.Smtp;
using EventHub.Models.UserClient;
using EventHub.Models.Competition;
using EventHub.Helpers;
using static EventHub.Models.Competition.ListParticipantsVM;
using static EventHub.Models.AddAthleteSubCompVM;
using EventHub.Models.Competition.Enums;
using EventHub.Models.ConstructComp;

namespace EventHub.Models.Entities
{
    public partial class EventHubContext : DbContext
    {
        public EventHubContext(DbContextOptions<EventHubContext> options) : base(options)
        {
        }


        #region Reserve Event

        public void BookSingleSubComp(SingleReservationVM viewModel)
        {
            //Fetch current user.
            var user = User
                .FirstOrDefault(o => o.Id == viewModel.UserId);
            //Make the reservation
            Reservation.Add(new Reservation
            {
                UserId = viewModel.UserId,
                SubCompetitionId = viewModel.SubCompId,
                ReservationDate = viewModel.ReservationDate,
                Type = SubCompetitionTypes.Single.ToString(),
                PaymentSessionUrl = viewModel.PaymentSessionUrl,
                Reference = viewModel.Reference,
                Price = viewModel.Price,
                Discount = viewModel.Discount,
                Paid = viewModel.Paid
            });

            //Intersect the user and its mirrored competitor and the subcompetition.
            UsersToSubCompetitionsAndCompetitors.Add(new UsersToSubCompetitionsAndCompetitors
            {
                Competitor = new Competitor
                {
                    Type = "Athlete",
                    Name = user.FirstName + " " + user.LastName,
                    Gender = user.Gender,
                    IsCheckedIn = false,
                    Athlete = new Athlete
                    {
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Size = user.Size,
                        Gender = user.Gender,
                        Email = user.Email,
                        UserId = user.Id
                    }
                },
                SubCompetitionId = viewModel.SubCompId,
                UserId = viewModel.UserId,
            });

            var subComp = SubCompetition
                .FirstOrDefault(o => o.Id == viewModel.SubCompId);
            subComp.ConfirmedParticipants++;

            SaveChanges();
        }

        public void BookTeamSubComp(TeamReservationVM viewModel)
        {
            var user = User
                .FirstOrDefault(o => o.Id == viewModel.UserId);

            Reservation.Add(new Reservation
            {
                UserId = viewModel.UserId,
                SubCompetitionId = viewModel.SubCompId,
                ReservationDate = viewModel.ReservationDate,
                PaymentSessionUrl = viewModel.PaymentSessionUrl,
                Reference = viewModel.Reference,
                Type = SubCompetitionTypes.Team.ToString(),
                Price = viewModel.Price,
                Discount = viewModel.Discount,
                Paid = viewModel.Paid
            });

            var subComp = SubCompetition
                .FirstOrDefault(o => o.Id == viewModel.SubCompId);
            subComp.ConfirmedParticipants++;

            //Initialize a team for later use.
            var team = new Team
            {
                TeamName = user.Team,
                Gender = subComp.Gender
            };

            //Initialize an Athlete for later use.
            var userAthlete = new Athlete
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Size = user.Size,
                Gender = user.Gender,
                Email = user.Email,
                UserId = user.Id
            };

            //Add the user as a competitor of type Athlete. Add the initialized athlete.
            Competitor.Add(new Competitor
            {
                Name = user.FirstName + " " + user.LastName,
                Type = "Athlete",
                Gender = user.Gender,
                Athlete = userAthlete
            });

            //Save changes in order for Database to index and identify the athlete with Id.
            SaveChanges();

            //Intersect the User and the subcompetition and the Competitor Team. 
            UsersToSubCompetitionsAndCompetitors.Add(new UsersToSubCompetitionsAndCompetitors
            {
                Competitor = new Competitor
                {
                    Name = user.Team,
                    Type = "Team",
                    Team = team,
                    Gender = subComp.Gender,
                    IsCheckedIn = false,
                },
                SubCompetitionId = viewModel.SubCompId,
                UserId = viewModel.UserId
            });

            //Bind the athlete to the team. 
            AthleteInTeam.Add(new AthleteInTeam
            {
                AthleteId = userAthlete.Id,
                TeamId = team.Id
            });



            SaveChanges();
        }

        public MakeReservationVM GetMakeReservationVM(int userId)
        {
            return new MakeReservationVM
            {
                Competitions = ControllUserReservations(
                    GetCompetitionList(userId), userId)
            };
        }

        public IEnumerable<MakeReservationVM.CompetitionVM> GetCompetitionList(int userId)
        {
            var allCompetitions = Competition.Where(o => o.Published)
                .Include(o => o.SubCompetition)
                .Select(o => Mapper.Map<MakeReservationVM.CompetitionVM>(o))
                .ToList();
            return allCompetitions;
        }
        public IEnumerable<MakeReservationVM.CompetitionVM> ControllUserReservations(IEnumerable<MakeReservationVM.CompetitionVM> allCompetitions, int userId)
        {
            foreach (var comp in allCompetitions)
            {
                foreach (var subcomp in comp.SubCompetition)
                {
                    subcomp.IsBooked = User.Where(o => o.Id.Equals(userId))
                    .SelectMany(o => o.Reservation).Select(o => o.SubCompetitionId).Any(o => o.Equals(subcomp.Id));
                }
            }
            return allCompetitions;
        }
        public ModalSubCompetitionVM GetCompetitionInfo(int id, int userId)
        {
            var subEvent = SubCompetition.FirstOrDefault(o => o.Id == id);
            var comp = Competition
                .FirstOrDefault(o => o.Id == subEvent.CompetitionId);

            var currentUser = User.
                FirstOrDefault(o => o.Id == userId);

            if (String.IsNullOrEmpty(currentUser.Size))
                currentUser.Size = ShirtSizes.XS.ToString();

            return new ModalSubCompetitionVM
            {
                Difficulty = subEvent.Difficulty.ToString(),
                EventEndDate = comp.EndDate,
                Gender = subEvent.Gender,
                SubEventId = subEvent.Id,
                EventLocation = comp.Location,
                SubEventDate = subEvent.Date,
                EventName = comp.Name,
                Price = subEvent.Price,
                EventStartDate = comp.StartDate,
                Type = subEvent.Type,
                QuantityPerTeam = subEvent.QuantityPerTeam,
                CurrentEventUser = new EventUser
                {
                    UserId = currentUser.Id,
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    ShirtSize = (ShirtSizes)Enum.Parse(typeof(ShirtSizes), currentUser.Size),
                    TeamName = currentUser.Team,
                },
            };
        }

        public AddAthleteSubCompVM GetCompetitionInfoAddAthlete(int id, int userId)
        {
            var subEvent = SubCompetition.FirstOrDefault(o => o.Id == id);
            var comp = Competition
                .FirstOrDefault(o => o.Id == subEvent.CompetitionId);

            var currentUser = User.
                FirstOrDefault(o => o.Id == userId);

            var subComp = SubCompetition
                 //.Include(o => o.Reservation).ThenInclude(o => o.User)
                 //.Include(o => o.UsersToSubCompetitionsAndCompetitors).ThenInclude(re => re.Competitor.Athlete)
                 //.Include(o => o.UsersToSubCompetitionsAndCompetitors).ThenInclude(re => re.User)
                 //.Include(o => o.UsersToSubCompetitionsAndCompetitors).ThenInclude(re => re.Competitor.Team)
                 //.ThenInclude(team => team.AthleteInTeam)
                 //.ThenInclude(athlete => athlete.Athlete)
                 .FirstOrDefault(s => s.Id == id);
            var userToSubCompAndCompetitor = UsersToSubCompetitionsAndCompetitors.Where(o => o.UserId == userId && o.SubCompetitionId == id)
                .Include(o => o.Competitor)
                .ThenInclude(o => o.Team)
                .ThenInclude(o => o.AthleteInTeam)
                .ThenInclude(o => o.Athlete).ToList();
            if (String.IsNullOrEmpty(currentUser.Size))
                currentUser.Size = ShirtSizes.XS.ToString();

            return new AddAthleteSubCompVM
            {
                Competition = new CompetitionInfoVM
                {
                    Id = comp.Id,
                    Name = comp.Name,
                    StartDate = comp.StartDate,
                    EndDate = comp.EndDate,
                    Location = comp.Location,
                    Description = comp.Description,
                    SubCompetition = new AddAthleteSubCompVM.SubCompetitionInfoVM
                    {
                        Id = subComp.Id,
                        Date = subComp.Date,
                        Type = subComp.Type,
                        Gender = subComp.Gender,
                        Difficulty = subComp.Difficulty,
                        QuantityPerTeam = subComp.QuantityPerTeam,
                        UsersToSubCompetitionsAndCompetitors = userToSubCompAndCompetitor
                    }
                },
                CurrentEventUser = new CurrentUser
                {
                    UserId = currentUser.Id,
                    FirstName = currentUser.FirstName,
                    LastName = currentUser.LastName,
                    Email = currentUser.Email,
                    TeamName = currentUser.Team,
                },
            };
        }
        #endregion

        #region Create Event
        internal int AddCompetitionToDB(CreateCompetitiontVM viewModel)
        {
            var model = Mapper.Map<Competition>(viewModel);
            Competition.Add(model);
            SaveChanges();
            return model.Id;
        }


        #endregion

        #region Edit Competition
        public MakeReservationVM GetEditableEventsVM(string userId)
        {
            return new MakeReservationVM
            {
                Competitions = GetEditListVM(userId)
            };
        }

        internal IEnumerable<MakeReservationVM.CompetitionVM> GetEditListVM(string userId)
        {
            var allCompetitions = Competition
                .Where(c => c.CreatorId == userId)
                .Include(o => o.SubCompetition)
                .Select(o => Mapper.Map<MakeReservationVM.CompetitionVM>(o))
                .ToList();
            return allCompetitions;
        }

        public EditOneCompetVM.CompetitionVM GetCompetition(int id)
        {
            var competition = Competition.Find(id);
            if (competition != null)
            {
                Entry(competition).Collection(p => p.SubCompetition).Load();
            }
            return Mapper.Map<EditOneCompetVM.CompetitionVM>(competition);
        }
        public ListParticipantsVM.CompetitionVM GetCompetitionsInfo(int id)
        {
            var competition = Competition.Find(id);

            if (competition != null)
            {
                Entry(competition).Collection(p => p.SubCompetition).Load();
            }
            return Mapper.Map<ListParticipantsVM.CompetitionVM>(competition);
        }
        public ListParticipantsVM.SubCompetitionVM GetSubCompMembers(int id)
        {
            var subComp = SubCompetition
                .Include(o => o.Reservation).ThenInclude(o => o.User)
                .Include(o => o.UsersToSubCompetitionsAndCompetitors).ThenInclude(re => re.Competitor.Athlete)
                .Include(o => o.UsersToSubCompetitionsAndCompetitors).ThenInclude(re => re.Competitor.Team)
                .ThenInclude(team => team.AthleteInTeam)
                .ThenInclude(athlete => athlete.Athlete)
                .FirstOrDefault(s => s.Id == id);
            if (subComp.Type == SubCompetitionTypes.Team.ToString())
            {
                subComp.UsersToSubCompetitionsAndCompetitors = subComp.UsersToSubCompetitionsAndCompetitors.GroupBy(x => new { x.Competitor.IsCheckedIn, x.Competitor.Team.Id }).Select(y => y.First()).ToList();
            }

            return Mapper.Map<ListParticipantsVM.SubCompetitionVM>(subComp);
        }

        public SubCompetition GetSubCompMembersWithResults(int id)
        {
            var subComp = SubCompetition

                .Include(o => o.UsersToSubCompetitionsAndCompetitors).ThenInclude(re => re.Competitor.CompetitorResult)
                .Include(o => o.CompEvent).ThenInclude(ce => ce.SubEvent)
                .ThenInclude(ce => ce.CompetitorResult).ThenInclude(ce => ce.Competitor)
                .Include(o => o.CompEvent).ThenInclude(c => c.CompetitorResult)

                .FirstOrDefault(o => o.Id == id);
            return subComp;
        }

        internal void UpdateResults(List<UpdateResultVM> results)
        {
            CompetitorResult entry;

            foreach (var result in results)
            {
                entry = CompetitorResult
                    .Where(o => o.SubEventId == Int32.Parse(result.SubEventId) && o.CompetitorId == Int32.Parse(result.CompetitorId) && o.CompEventId == Int32.Parse(result.CompEventId)).FirstOrDefault();
                if (entry != null)
                {
                    if (result.PointScore != null)
                    { entry.PointScore = Decimal.Parse(result.PointScore); }
                    if (result.RepScore != null)
                    { entry.RepScore = Int32.Parse(result.RepScore); }

                    if (result.TimeScore != null)
                    { entry.TimeScore = TimeSpan.Parse(result.TimeScore); }
                    if (result.WeightScore != null)
                    { entry.WeightScore = Decimal.Parse(result.WeightScore); }
                    if (result.TieBreak != null)
                    { entry.TieBreak = Int32.Parse(result.TieBreak); }
                }
            }
            SaveChanges();
            var subEventId = Int32.Parse(results[0].SubEventId);
            var subEventType = results[0].Type;
            UpdateRanking(subEventId, subEventType);
        }

        private void UpdateRanking(int id, string type)
        {
            List<CompetitorResult> listOfResults;
            listOfResults = CompetitorResult
                    .Where(o => o.SubEventId == id).ToList();
            if (type == "Time")
            {
                listOfResults = listOfResults.OrderByDescending(o => o.RepScore).ThenBy(o => o.TimeScore).ThenByDescending(o=>o.TieBreak).ToList();
                var index = 0;
                var i = 0;
                foreach (var item in listOfResults)
                {

                    if (item.TimeScore.ToString() == "00:00:00" && item.RepScore == 0)
                    {
                        item.Score = 0;
                    }
                    else
                    {
                        if (i != 0 && item.TimeScore == listOfResults[i - 1].TimeScore && item.RepScore == listOfResults[i - 1].RepScore && item.TieBreak == listOfResults[i-1].TieBreak)
                        {
                            item.Score = listOfResults[i - 1].Score;
                        }
                        else
                        {
                            item.Score = CFStaticScores.ScoringList[index];
                            index++;
                        }
                    }
                    i++;
                }
                SaveChanges();

            }
            if (type == "Reps")
            {
                listOfResults = listOfResults.OrderByDescending(o => o.RepScore).ThenByDescending(o => o.TieBreak).ToList();
                var index = 0;
                var i = 0;
                foreach (var item in listOfResults)
                {

                    if (item.RepScore == 0 && item.TieBreak == 0)
                    {
                        item.Score = 0;
                    }
                    else
                    {
                        if (i != 0 && item.RepScore == listOfResults[i - 1].RepScore && item.TieBreak == listOfResults[i - 1].TieBreak)
                        {
                            item.Score = listOfResults[i - 1].Score;
                        }
                        else
                        {
                            item.Score = CFStaticScores.ScoringList[index];
                            index++;
                        }
                    }
                    i++;
                }
                SaveChanges();
            }
            if (type == "Weight")
            {
                listOfResults = listOfResults.OrderByDescending(o => o.WeightScore).ToList();
                var index = 0;
                var i = 0;
                foreach (var item in listOfResults)
                {

                    if (item.WeightScore == 0)
                    {
                        item.Score = 0;
                    }
                    else
                    {
                        if (i != 0 && item.WeightScore == listOfResults[i - 1].WeightScore)
                        {
                            item.Score = listOfResults[i - 1].Score;
                        }
                        else
                        {
                            item.Score = CFStaticScores.ScoringList[index];
                            index++;
                        }

                    }
                    i++;
                }
                SaveChanges();
            }
            if (type == "Point")
            {
                listOfResults = listOfResults.OrderByDescending(o => o.PointScore).ToList();
                var index = 0;
                var i = 0;
                foreach (var item in listOfResults)
                {

                    if (item.PointScore == 0)
                    {
                        item.Score = 0;
                    }
                    else
                    {
                        if (i != 0 && item.PointScore == listOfResults[i - 1].PointScore)
                        {
                            item.Score = listOfResults[i - 1].Score;
                        }
                        else
                        {
                            item.Score = CFStaticScores.ScoringList[index];
                            index++;
                        }

                    }
                    i++;
                }
                SaveChanges();
            }
        }
        internal List<CompEventVM> GetCompEvents(int id)
        {
            var compEvents = SubEvent
                .Where(s => s.CompEvent.SubCompetitionId == id)
                .Include(s => s.CompEvent)
                .Select(s => new CompEventVM
                {
                    Id = s.CompEvent.Id,
                    Title = s.CompEvent.Title,
                    SubEvents = new List<CompEventVM.SubEventVM>()
                    {
                        new CompEventVM.SubEventVM
                        {
                            Id = s.Id,
                            Type = (ScoreTypes) Enum.Parse(typeof(ScoreTypes), s.Type) ,
                            SetUpTime = s.SetUpTime,
                            TimeCap = s.TimeCap,
                            TotalReps = s.TotalReps
                        }
                    }
                })
                .ToList();
            //var c = new List<CompEventVM>();
            //foreach (var item in compEvents)
            //{
            //    c.Add(Mapper.Map<CompEventVM>(item));
            //}

            return compEvents;
        }

        internal void DeleteSubEvent(int id)
        {
            var model = SubCompetition.FirstOrDefault(r => r.Id == id);

            if (model.ConfirmedParticipants == 0)
                SubCompetition.Remove(model);

            SaveChanges();
        }
        internal void EditCompetition(EditOneCompetVM.CompetitionVM viewModel)
        {
            var selectedCompetition = Competition.Find(viewModel.Id);
            selectedCompetition.Location = viewModel.Location;
            selectedCompetition.Name = viewModel.Name;
            selectedCompetition.StartDate = viewModel.StartDate;
            selectedCompetition.Description = viewModel.Description;
            if (viewModel.IsAdmin)
            {
                selectedCompetition.Published = viewModel.Published;
            }
            selectedCompetition.OpenForBookings = viewModel.OpenForBookings;
            foreach (var subcomp in viewModel.SubCompetition)
            {
                var selectedSubComp = SubCompetition.Find(subcomp.Id);
                selectedSubComp.Date = subcomp.Date;
                selectedSubComp.Difficulty = subcomp.Difficulty.ToString();
                selectedSubComp.Gender = subcomp.Gender.ToString();
                selectedSubComp.Price = subcomp.Price;
                selectedSubComp.Quantity = subcomp.Quantity;
                if (subcomp.Type == SubCompetitionTypes.Team)
                    selectedSubComp.QuantityPerTeam = subcomp.QuantityPerTeam;
                SaveChanges();
            }
            SaveChanges();
        }
        internal void SaveNewRangeSolo(CreateCompetitiontVM.SoloSubEventVM soloSubEvent)
        {
            SubCompetition.Add(
                Mapper.Map<SubCompetition>(soloSubEvent));

            SaveChanges();
        }
        internal void SaveNewRangeTeam(CreateCompetitiontVM.TeamSubEventVM teamSubEvents)
        {

            var subCompt = Mapper.Map<SubCompetition>(teamSubEvents);
            SubCompetition.Add(subCompt);

            SaveChanges();
        }
        #endregion

        #region common for Competition maintenance
        //Deliver the elements for dropdown-options in filtering
        public FilterCompetitionsVM GetFilterCompetitionsVM(IEnumerable<MakeReservationVM.CompetitionVM> competitions)
        {
            var locations = competitions
                .GroupBy(x => x.Location)
                .Select(x => x.First())
                .Select(x => new SelectListItem { Text = x.Location }).ToList();
            locations.Insert(0, new SelectListItem { Text = "Alla" });
            return new FilterCompetitionsVM { Locations = locations };
        }

        //The filtering parameters is set in the calling method
        public IEnumerable<MakeReservationVM.CompetitionVM> GetFilteredListCompetitionsVM(IEnumerable<MakeReservationVM.CompetitionVM> model, Func<MakeReservationVM.CompetitionVM, bool> Filter)
        {
            return model
                .Where(Filter);
        }
        #endregion

        #region User
        public EditUserVM GetUser(int userId)
        {
            var model = User.FirstOrDefault(p => p.Id == userId);
            var viewModel = Mapper.Map<EditUserVM>(model);

            if (String.IsNullOrEmpty(model.Size))
                viewModel.ShirtSize = ShirtSizes.XS;
            else
                viewModel.ShirtSize = (ShirtSizes)Enum.Parse(typeof(ShirtSizes), model.Size);

            if (String.IsNullOrEmpty(model.Gender))
                viewModel.Gender = SingleGenders.Male;
            else
                viewModel.Gender = (SingleGenders)Enum.Parse(typeof(SingleGenders), model.Gender);

            viewModel.Boxes = Box
                    .Select(x => new SelectListItem
                    {
                        Text = x.Name,
                        Value = x.Id.ToString()
                    })
                    .ToList();

            return viewModel;
        }

        public void EditUser(int id, EditUserVM viewModel)
        {
            var user = User
                .SingleOrDefault(o => o.Id == id);

            user.FirstName = viewModel.FirstName;
            user.LastName = viewModel.LastName;
            user.Location = viewModel.Location;
            user.DateOfBirth = viewModel.DateOfBirth;
            user.Team = viewModel.Team;
            user.Size = viewModel.ShirtSize.ToString();
            user.Gender = viewModel.Gender.ToString();
            user.BoxId = viewModel.BoxId;
            user.IsAllowingNewsLetter = viewModel.IsAllowingNewsLetter;
            user.PublicProfile = viewModel.PublicProfile;
            SaveChanges();
        }

        internal void AddProfileImageToDBUser(byte[] image, int id)
        {
            User
                .SingleOrDefault(o => o.Id == id)
                .ProfileImage = image;
            SaveChanges();
        }

        public ProfileVM GetUserProfile(int id)
        {
            var user = User.Find(id);

            var competitions = UsersToSubCompetitionsAndCompetitors
                .Where(o => o.UserId == id)
                .Include(o => o.SubCompetition.Competition)
                .Include(o => o.Competitor)
                .ToArray();
            var boxName = (user.BoxId != null) ? Box.FirstOrDefault(b => b.Id == user.BoxId).Name : null;

            var profileView = new ProfileVM
            {
                ListProfile = new ListProfileDetailsVM()
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Location = user.Location,
                    DateOfBirth = user.DateOfBirth,
                    Gender = user.Gender,
                    Box = boxName,
                    ProfileImage = user.ProfileImage,
                    ShirtSize = user.Size,
                    Competitions = competitions
                .Select(o => new CompetitionProfileView
                {
                    Id = o.SubCompetition.CompetitionId,
                    Name = o.SubCompetition.Competition.Name,
                    Date = o.SubCompetition.Competition.StartDate,
                    Cost = o.SubCompetition.Price,
                    Description = o.SubCompetition.Type + " "
                    + o.SubCompetition.Gender,
                    Difficulty = o.SubCompetition.Difficulty,
                    Location = o.SubCompetition.Competition.Location,
                    MaxAttendants = o.SubCompetition.Quantity,
                    Alias = o.Competitor.Name
                })
                .OrderBy(o => o.Date.Date).ThenBy(o => o.Date.TimeOfDay)
                .ToArray()
                }
            };

            return profileView;
        }
        public List<MyCompetitionsVM.CompetitionVM> GetUserReservations(int id)
        {
            var user = User.Find(id);

            var reservations = Reservation
                .Where(o => o.UserId == id)
                .Include(o => o.SubCompetition.Competition)
                .Select(o => new MyCompetitionsVM.CompetitionVM
                {
                    Id = o.SubCompetition.Competition.Id,
                    CompName = o.SubCompetition.Competition.Name,
                    StartDate = o.SubCompetition.Competition.StartDate,
                    EndDate = o.SubCompetition.Competition.EndDate,
                    Description = o.SubCompetition.Competition.Description,
                    Location = o.SubCompetition.Competition.Location,
                    SubCompetitions = new List<MyCompetitionsVM.SubCompetitionVM>()
                    {
                        new MyCompetitionsVM.SubCompetitionVM
                        {
                                Id=o.SubCompetition.Id,
                                Date = o.SubCompetition.Date,
                                Cost = o.SubCompetition.Price,
                                Description = o.SubCompetition.Type + " "
                                    + o.SubCompetition.Gender + " " + o.SubCompetition.Difficulty,
                                Type = o.Type,
                                Location = o.SubCompetition.Competition.Location,
                                MaxAttendants = o.SubCompetition.Quantity,
                                QuantityPerTeam = o.SubCompetition.QuantityPerTeam
                        }
                    }.OrderBy(s => s.Date).ThenBy(s => s.Date.TimeOfDay).ToList()


                })
                .OrderBy(o => o.StartDate.Date).ThenBy(o => o.StartDate.TimeOfDay)

                .ToList();
            return reservations;
        }

        internal void RemoveCompetitorAsync(List<string> listOfUserToDelete, int subCompId)
        {
            List<AthleteInTeam> athltesInTeamToRemove = new List<AthleteInTeam>();
            List<Athlete> athltesToRemove = new List<Athlete>();
            List<Competitor> competitorsToRemove = new List<Competitor>();
            List<UsersToSubCompetitionsAndCompetitors> usersToSubComToDelete = new List<UsersToSubCompetitionsAndCompetitors>();
            foreach (var userId in listOfUserToDelete)
            {
                var id = Int32.Parse(userId);
                var athleteInTeamToDelete = AthleteInTeam.FirstOrDefault(u => u.AthleteId == id);
                var athleteToDelete = Athlete.FirstOrDefault(u => u.Id == id);
                var competitorToDelete = Competitor.FirstOrDefault(u => u.Athlete.Id == athleteToDelete.Id);
                var userIdToSubCompToDelete = User.FirstOrDefault(u => u.Email == athleteToDelete.Email);
                var userToSubCompToDelete = UsersToSubCompetitionsAndCompetitors.FirstOrDefault(u => u.UserId == userIdToSubCompToDelete.Id && u.CompetitorId == athleteInTeamToDelete.TeamId);
                athltesInTeamToRemove.Add(athleteInTeamToDelete);
                competitorsToRemove.Add(competitorToDelete);
                athltesToRemove.Add(athleteToDelete);
                usersToSubComToDelete.Add(userToSubCompToDelete);

            }
            AthleteInTeam.RemoveRange(athltesInTeamToRemove);
            SaveChanges();
            UsersToSubCompetitionsAndCompetitors.RemoveRange(usersToSubComToDelete);
            SaveChanges();
            Athlete.RemoveRange(athltesToRemove);
            SaveChanges();
            Competitor.RemoveRange(competitorsToRemove);
            SaveChanges();

        }



        //Getting all users FirstName, LastName and email for autocomplete
        public List<AllUsersVM> GetAllUsers(int userId)
        {
            var allUsers = User.Where(u => u.Id != userId).Select(u => new AllUsersVM
            {
                value = u.FirstName + " " + u.LastName + " " + u.Email,
                id = u.Id
            }).ToList();
            return allUsers;
        }
        internal void ChangeTeamName(TeamMembersAddVM viewModel, int currentUserId)
        {
            Competitor team = UsersToSubCompetitionsAndCompetitors.Include(o => o.Competitor).ThenInclude(o => o.Team)
               .FirstOrDefault(u => u.SubCompetitionId == viewModel.SubCompId && u.UserId == currentUserId).Competitor;

            team.Name = viewModel.TeamName;
            team.Team.TeamName = viewModel.TeamName;
            SaveChanges();
        }

        internal bool CheckInAthlete(ToggleCheckInVM model)
        {
            var competitor = Competitor.Find(model.Id);
            bool status;
            if (competitor.IsCheckedIn)
            {
                status = false;
                competitor.IsCheckedIn = status;
            }
            else
            {
                var subcomp = SubCompetition.Where(o => o.Id == model.SubCompId)
                    .Include(o => o.CompEvent)
                        .ThenInclude(o => o.SubEvent)
                        .FirstOrDefault();


                foreach (var compEvent in subcomp.CompEvent)
                {

                    foreach (var subEvent in compEvent.SubEvent)
                    {
                        var resultEntry = new CompetitorResult
                        {
                            CompetitorId = model.Id,
                            CompEventId = compEvent.Id,
                            SubEventId = subEvent.Id,
                            Score = 0
                        };
                        try
                        {
                            CompetitorResult.Add(resultEntry);
                            SaveChanges();
                        }
                        catch
                        {
                            CompetitorResult.Remove(resultEntry);
                            SaveChanges();
                        }
                    }
                }
                status = true;
                competitor.IsCheckedIn = status;
            }
            SaveChanges();
            return status;
        }

        internal void AddTeamMembers(TeamMembersAddVM viewModel, int currentUserId)
        {
            Competitor team = UsersToSubCompetitionsAndCompetitors.Include(o => o.Competitor).ThenInclude(o => o.Team)
                .FirstOrDefault(u => u.SubCompetitionId == viewModel.SubCompId && u.UserId == currentUserId).Competitor;

            team.Name = viewModel.TeamName;
            team.Team.TeamName = viewModel.TeamName;
            //SaveChanges();
            foreach (var userId in viewModel.MembersId)
            {
                var user = User.FirstOrDefault(u => u.Id == Int32.Parse(userId));
                var newAthlete = new Athlete
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    Gender = user.Gender,
                    Size = user.Size,
                    UserId = user.Id
                };
                Competitor.Add(new Competitor
                {
                    Athlete = newAthlete,
                    Name = user.FirstName + " " + user.LastName,
                    Type = "Athlete",
                    Gender = user.Gender,
                    IsCheckedIn = false,
                });
                SaveChanges();
                AthleteInTeam.Add(new AthleteInTeam
                {
                    AthleteId = newAthlete.Id,
                    TeamId = team.Id
                });

                UsersToSubCompetitionsAndCompetitors.Add(new UsersToSubCompetitionsAndCompetitors
                {
                    SubCompetitionId = viewModel.SubCompId,
                    CompetitorId = team.Id,
                    UserId = Int32.Parse(userId)
                });
                SaveChanges();
            }
        }

        internal void AddUserToDB(CreateUserVM viewModel)
        {
            var model = Mapper.Map<User>(viewModel);
            model.DateOfBirth = DateTime.Now;
            model.Location = "Jag kommer ifrån...";
            model.HashId = viewModel.HashId;
            User.Add(model);
            SaveChanges();
        }

        #endregion

        #region Register User
        internal void CreateUser(RegisterUserVM viewModel, string userHashId)
        {
            var userToCreate = new User
            {
                HashId = userHashId,
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                IsAllowingNewsLetter = viewModel.AllowNewsLetter,
                PoliciesAccepted = viewModel.TermsAndConditions,
                PoliciesAcceptedDate = DateTime.Now,
            };

            User.Add(userToCreate);
            SaveChanges();
        }
        internal void AddUserDetails(FirstUseVM viewModel, int id)
        {
            var user = User
                 .SingleOrDefault(o => o.Id == id);
            user.Location = viewModel.Location;
            user.DateOfBirth = viewModel.DateOfBirth;
            user.Size = ((ShirtSizes)viewModel.ShirtSize).ToString();
            user.Gender = ((SingleGenders)viewModel.Gender).ToString();

            SaveChanges();
        }
        #endregion

        #region Leaderboard
        public LeaderboardIndexVM[] GetLeaderboardIndex(int compId)
        {
            var comp = Competition
                .Where(o => o.Id == compId)
                .Include(o => o.SubCompetition)
                .FirstOrDefault();

            return comp.SubCompetition
                .Select(o => new LeaderboardIndexVM()
                {
                    CompetitionName = comp.Name,
                    Id = o.Id,
                    Name = String.Format($"{o.Type} - {o.Gender} - {o.Difficulty}")
                })
                .ToArray();
        }
        public SubClassResultVM GetResultForCompClass(int subCompId)
        {
            var eventResults = SubCompetition.Where(o => o.Id == subCompId)
                .Include(o => o.UsersToSubCompetitionsAndCompetitors).ThenInclude(o => o.Competitor)
                .Include(o => o.CompEvent).ThenInclude(o => o.SubEvent)
                .ThenInclude(o => o.CompetitorResult)
                .ThenInclude(o => o.Competitor)
                .FirstOrDefault();

            List<Title> titles = new List<Title>();
            List<TeamResult> teamResults = new List<TeamResult>();

            var compNames = CompEvent
                .Where(o => o.SubCompetitionId == subCompId)
                .Select(o => o).OrderBy(o => o.Title);

            foreach (var compName in compNames)
            {
                if (compName.SubEvent.Count > 1)
                {
                    foreach (var subEvent in compName.SubEvent)
                    {
                        var title = new Title();
                        title.Event = compName.Title + subEvent.Title;
                        title.Type = TranslateEventType(subEvent.Type);
                        titles.Add(title);
                    }
                }
                else
                {
                    var title = new Title();
                    title.Event = compName.Title;
                    title.Type = TranslateEventType(compName.SubEvent.FirstOrDefault().Type);
                    titles.Add(title);
                }
            }

            foreach (var competitor in eventResults.UsersToSubCompetitionsAndCompetitors)
            {

                if (!competitor.Competitor.IsCheckedIn)
                {
                    continue;
                }

                var teamResult = new TeamResult();
                teamResult.Name = competitor.Competitor.Name;
                teamResult.TeamId = competitor.Competitor.Id;
                teamResult.Scores = new List<Score>();
                List<Score> scores = new List<Score>();
                decimal totalScore = 0;

                var members = AthleteInTeam
                    .Include(o => o.Athlete)
                    .Where(o => o.TeamId == competitor.Competitor.Id)
                    .OrderBy(o => o.Athlete.FirstName)
                    .Select(o => new Member
                    {
                        UserId = o.Athlete.UserId,
                        AthleteId = o.Athlete.Id,
                        Name = String.Format($"{o.Athlete.FirstName} {o.Athlete.LastName}")
                    })
                    .ToList();

                teamResult.Members = members;

                foreach (var result in competitor.Competitor.CompetitorResult)
                {

                    Score score = new Score();

                    if (result.Score != null)
                    {
                        score.Event = result.Score;
                        score.EventTitle = result.CompEvent.Title;
                        var type = result.SubEvent.Type;
                       

                        switch (type)
                        {
                            case "Time":
                                //score.Type = String.Format($"{result.TimeScore.Value.Minutes}:{result.TimeScore.Value.Seconds.ToString("D2")}");
                                // score.Type = result.TimeScore.ToString();
                                score.Type = (result.TimeScore != null) ? String.Format($"{result.TimeScore.Value.Minutes}:{result.TimeScore.Value.Seconds.ToString("D2")}") : "--:--";
                                break;
                            case "Weight":
                                score.Type = result.WeightScore.ToString();
                                break;
                            case "Reps":
                                score.Type = result.RepScore.ToString();
                                break;
                            case "Point":
                                score.Type = result.PointScore.ToString();
                                break;
                        }

                        totalScore += (decimal)result.Score;
                    }
                    else
                    {
                        score.Event = 0;
                        score.Type = 0.ToString();
                    }

                    scores.Add(score);
                }

                teamResult.TotalScore = totalScore;
                teamResult.Scores = scores;
                teamResult.Scores = teamResult.Scores.OrderBy(o => o.EventTitle).ToList();
                teamResults.Add(teamResult);
            }
           
            var results = new SubClassResultVM
            {
                Titles = titles,
                TeamResults = teamResults.GroupBy(o => o.TeamId).Select(o => o.First()).OrderByDescending(o => o.TotalScore).ToList()
            };
            foreach (var item in results.TeamResults)
            {
                foreach (var member in item.Members)
                {

                    var publicProfile = User.FirstOrDefault(u => u.Id == member.UserId).PublicProfile;
                    member.PublicProfile = (publicProfile == true || publicProfile == null) ? true : false;
                }
            }

            return results;
        }

        private string TranslateEventType(string type)
        {
            switch (type)
            {
                case "Time":
                    return "Tid";
                case "Weight":
                    return "Vikt";
                case "Reps":
                    return type;
                case "Point":
                    return "Resultat";
                default:
                    return type;
            }
        }

        #endregion

        #region Save Constructed Competition
        public void AddConstructedCompetition(List<EventVM> events)
        {
            CompEvent.AddRange(Mapper.Map<List<EventVM>, List<CompEvent>>(events));
            SaveChanges();
        }

        public void PersistConstructedCompetition(List<EventHub.Models.ConstructComp.EventVM> constructEvents, int[] subCompIds)
        {
            // För varje subcompid dvs klass kommer constructEvents, dvs listan på events lagras
            foreach (var subCompId in subCompIds)
            {
                constructEvents.ForEach(o =>
                {                
                    o.SubCompetitionId = subCompId; Add(Mapper.Map<EventHub.Models.ConstructComp.EventVM, CompEvent>(o));
                });              
            }
            SaveChanges();
        }
        #endregion

        #region Discounts
        public DiscountIndexVM GetDiscountVM(string userId)
        {

            var existingCompetitions = Competition
                .Where(o => o.CreatorId == userId)
                .Select(o => new ExistingCompetition
                {
                    CompId = o.Id,
                    CompName = o.Name
                })
                .ToList();

            var existingCodes = new List<ExistingDiscountCode>();

            var allComps = Competition
                .Where(c => c.CreatorId == userId)
                .Include(o => o.SubCompetition)
                .Select(o => o);

            foreach (var comp in allComps)
            {
                foreach (var subComp in comp.SubCompetition)
                {

                    var discounts = DiscountForSubCompetition
                        .Where(o => o.SubCompetitionId == subComp.Id)
                        .Select(o => o.Discount);

                    var existingCode = new ExistingDiscountCode
                    {
                        CompName = comp.Name,
                        SubCompId = subComp.Id,
                        SubCompName = String.Format($"{subComp.Type} - {subComp.Gender} - {subComp.Difficulty}"),
                        DiscountCodes = new List<DiscountCode>()
                    };

                    foreach (var discount in discounts)
                    {
                        existingCode.DiscountCodes.Add(new DiscountCode
                        {
                            Id = discount.Id,
                            Code = discount.Code,
                            ExpiryDate = discount.ExpiryDate,
                            PercentageOff = discount.PercentageOff,
                        });
                    }

                    existingCodes.Add(existingCode);
                }
            }

            var viewModel = new DiscountIndexVM
            {
                ExistingDiscountCodes = existingCodes,
                ExistingCompetitions = existingCompetitions
            };

            return viewModel;
        }

        public void AddDiscountCodes(AddDiscountCode viewModel)
        {

            var newCode = new Discount
            {
                Code = viewModel.Code,
                ExpiryDate = viewModel.ExpiryDate,
                PercentageOff = viewModel.PercentageOff
            };

            Discount.Add(newCode);
            SaveChanges();

            foreach (var subCompId in viewModel.SubCompId)
            {
                DiscountForSubCompetition.Add(new Entities.DiscountForSubCompetition
                {
                    DiscountId = newCode.Id,
                    SubCompetitionId = subCompId
                });
            }

            SaveChanges();
        }

        public ExistingSubComps[] GetAvailableSubComps(int compId)
        {
            var comp = Competition
                .Include(o => o.SubCompetition)
                .SingleOrDefault(o => o.Id == compId);

            return comp.SubCompetition
                .Select(o => new ExistingSubComps
                {
                    SubCompId = o.Id,
                    SubCompName = String.Format($"{o.Type} - {o.Gender} - {o.Difficulty}")
                })
                .ToArray();
        }

        public void DeleteDiscountCode(DeleteCode viewModel)
        {

            var entitiy = DiscountForSubCompetition
                .Where(o => o.DiscountId == viewModel.DiscountId && o.SubCompetitionId == viewModel.SubCompId)
                .SingleOrDefault();

            DiscountForSubCompetition.Remove(entitiy);

            SaveChanges();
        }
        #endregion

        public SlideshowVM GetSubCompResults(int compId)
        {
            List<SubClassResultVM> leaderboards = new List<SubClassResultVM>();
            List<string> subCompNames = new List<string>();

            var comp = Competition
                .Include(o => o.SubCompetition)
                .SingleOrDefault(o => o.Id == compId);

            foreach (var subComp in comp.SubCompetition.Skip(2))
            {
                SubClassResultVM leaderboard = new SubClassResultVM();
                subCompNames.Add(String.Format($"{subComp.Type} - {subComp.Difficulty} - {subComp.Gender}"));
                leaderboard = GetResultForCompClass(subComp.Id);
                leaderboards.Add(leaderboard);
            }

            var slideShow = new SlideshowVM
            {
                CompetitionName = comp.Name,
                leaderboardResult = leaderboards,
                SubCompNames = subCompNames
            };

            return slideShow;
        }

        public List<GlobalVM> GetAllComps()
        {

            return Competition
                .Where(o => o.StartDate < DateTime.Now)
                .OrderBy(o => o.StartDate)
                .Select(o => new GlobalVM
                {
                    CompId = o.Id,
                    CompName = o.Name,
                    Location = o.Location,
                    ShortDate = o.StartDate.ToString("yyyy-MM-dd"),
                })
                .ToList();

        }

        internal CompetitionTransactionsVM GetTransactionsForCompetition(int id)
        {
            var model = Competition
                 .Include(o => o.Transaction)
                 .ThenInclude(o => o.Reservation)
                 .Include(o => o.SubCompetition)
                 .Where(o => o.Id == id)
                 .Select(o => Mapper.Map<CompetitionTransactionsVM>(o))
                 .FirstOrDefault();
            model.Reservation = new List<TransactionReservationsVM>();

            model.SubCompetition.ForEach(o =>
            model.Reservation
            .AddRange(
                Reservation
                .Where(x => x.SubCompetitionId == o.Id)
                .Select(y => Mapper.Map<TransactionReservationsVM>(y))));

            model.Brief = new TransactionBriefVM
            {
                NumberOfTransactions = model.Transaction.Count,
                TotalReservations = model.Reservation.Count,


            };
            foreach (var transaction in model.Transaction)
            {
                model.Brief.PayOut += transaction.Debt;
                model.Brief.ReservationsTransacted += transaction.Reservation.Count;
                model.Brief.TotalCredits += transaction.Credit;
                model.Brief.TotalDiscounts += transaction.Discount;
            }
            foreach (var reservation in model.Reservation)
            {
                model.Brief.TotalAccumulation += reservation.Price ?? 0;
            }
            model.Reference = GenerateTransactionReference(id);
            return model;
        }
        internal void MakeTransaction(NewTransactionVM model)
        {
            model.Reference = GenerateTransactionReference(model.CompetitionId);
            var transaction = Mapper.Map<Transaction>(model);
            transaction.BoxId = Competition.Find(model.CompetitionId).BoxId;
            transaction.Date = DateTime.Now;
            transaction.PricePlan = transaction.PricePlan[transaction.PricePlan.Length - 1].ToString();
            Transaction.Add(transaction);
            SaveChanges();
            foreach (var reference in model.References)
            {
                var reservation = Reservation.Where(o => o.Reference == reference).FirstOrDefault();
                if (reservation != null)
                {
                    reservation.TransactionId = transaction.Id;
                    Reservation.Update(reservation);
                }

            }
            SaveChanges();
        }

        string GenerateTransactionReference(int id)
        {
            const string bxcrPayout = "BXCRPAYOUT";
            const string comp = "COMP";
            const char splitter = '-';

            var compString = comp + id;
            var date = DateTime.Now.ToString("yyyyMMdd");
            var number = Transaction.Where(o => o.CompetitionId == id).Count() + 1;
            return String.Join(splitter, bxcrPayout, compString, date, number);
        }

        public IndexVM GetConstructComp(int id)
        {

            var comp = Competition
                .Include(o => o.SubCompetition)
                .FirstOrDefault(o => o.Id == id);

            var newIndexVM = new IndexVM();
            newIndexVM.SubCompOptions = comp.SubCompetition
                .Select(o => new SubCompOptionVM
                {
                    SubCompId = o.Id,
                    SubCompTitle = String.Format($"{o.Type} - {o.Gender} - {o.Difficulty}")
                })
                .ToList();

            return newIndexVM;
        }
    }
}