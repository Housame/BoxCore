using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using EventHub.Models.Entities;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using EventHub.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using EventHub.Models.Competition;
using EventHub.Helpers;
using EventHub.Models.UserClient;
using Microsoft.AspNetCore.Identity;
using EventHub.Helpers.Authorization_Policies.Requirements;
using EventHub.Helpers.Authorization_Policies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;


namespace EventHub
{
    public class Startup
    {

        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            var connstring = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<EventHubContext>(
                options => options.UseSqlServer(connstring));

            services.AddDbContext<IdentityDbContext>(
                options => options.UseSqlServer(connstring));

            services.AddIdentity<IdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
            })
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options => options.LoginPath = "/userclient/signin");

            //Mapp VM to DB-Model here.
            Mapper.Initialize((config) =>
            {
                config.CreateMap<CreateUserVM, User>();
             config.CreateMap<User, EditUserVM>()
            .ForMember(p => p.PublicProfile, opt => opt.NullSubstitute(true));

                config.CreateMap<CreateCompetitiontVM.TeamSubEventVM, SubCompetition>()
                .ForMember(p => p.Type, x => x.UseValue(SubCompetitionTypes.Team))
                .ForMember(p => p.Id, opt => opt.Ignore());


                config.CreateMap<CreateCompetitiontVM.SoloSubEventVM, SubCompetition>()
                .ForMember(p => p.Type, x => x.UseValue(SubCompetitionTypes.Single))
                .ForMember(p => p.Id, opt => opt.Ignore());


                config.CreateMap<CreateCompetitiontVM, Competition>();

                config.CreateMap<SubCompetition, MakeReservationVM.SubCompetitionVM>()
                .ForMember(p => p.IsFullyBooked, x => x.MapFrom(p => p.ConfirmedParticipants == p.Quantity));
                config.CreateMap<Competition, MakeReservationVM.CompetitionVM>();

                config.CreateMap<SubCompetition, EditOneCompetVM.SubCompetitionVM>()
                .ForMember(p => p.HasReservations, x => x.MapFrom(o => o.ConfirmedParticipants > 0));
                config.CreateMap<Competition, EditOneCompetVM.CompetitionVM>();

                config.CreateMap<SubCompetition, ListParticipantsVM.SubCompetitionVM>();
                config.CreateMap<SubCompetition, SubCompetitionForResultsVM>();

                config.CreateMap<Competition, ListParticipantsVM.CompetitionVM>();

                config.CreateMap<SubCompetition, CreateCompetitiontVM.SoloSubEventVM>();

                config.CreateMap<SubCompetition, CreateCompetitiontVM.TeamSubEventVM>();

                config.CreateMap<SubCompetition, AddAthleteSubCompVM.SubCompetitionInfoVM>();
                config.CreateMap<AddAthleteSubCompVM.SubCompetitionInfoVM, SubCompetition>();

                config.CreateMap<Competition, CreateCompetitiontVM>();
                config.CreateMap<CompEvent, CompEventVM>();

                config.CreateMap<EventHub.Models.ConstructComp.SubEventVM, SubEvent>()
                .ForMember(p => p.Id, opt => opt.Ignore()); 
                config.CreateMap<EventHub.Models.ConstructComp.EventVM, CompEvent>()
                .ForMember(p => p.SubEvent, x => x.MapFrom(p => p.SubEvents))
                .ForMember(p => p.Id, opt => opt.Ignore());

                config.CreateMap<User, AllUsersVM>();
                config.CreateMap<AllUsersVM, User>();

                config.CreateMap<User, Athlete>();
                config.CreateMap<Athlete, User>();

                config.CreateMap<Transaction, TransactionsVM>();
                config.CreateMap<Reservation, TransactionReservationsVM>();
                config.CreateMap<NewTransactionVM, Transaction>()
                .ForMember(dest => dest.Debt, x => x.MapFrom(src => Convert.ToDecimal(src.Debt)))
                .ForMember(dest => dest.Discount, x => x.MapFrom(src => Convert.ToDecimal(src.Discount)))
                .ForMember(dest => dest.Credit, x => x.MapFrom(src => Convert.ToDecimal(src.Credit)))
                .ForMember(dest => dest.Sum, x => x.MapFrom(src => Convert.ToDecimal(src.Sum)));

                config.CreateMap<SubCompetition, TransactionSubCompetitionVM>();
                config.CreateMap<Competition, CompetitionTransactionsVM>();
            });

            services.AddSession();
            services.AddMvc();

            services.AddAuthorization(options =>
            {
                options.AddPolicy(
                    "EventManagers",
                    policyBuilder => policyBuilder.AddRequirements(
                        new EventManagerRequirement()));

                options.AddPolicy("CompetitionManagers",
                policyBuilder => policyBuilder.AddRequirements(
                    Operations.Update));

                options.AddPolicy("CompetitionCreators",
              policyBuilder => policyBuilder.AddRequirements(
                  Operations.Create));

                options.AddPolicy("ElevatedRights",
            policy => policy.RequireRole("Admin"));
            });

            //Policy handlers
            services.AddSingleton<IAuthorizationHandler, HasElevatedRoleHandler>();
            services.AddScoped<IAuthorizationHandler, IsEventManager>();
            services.AddScoped<IAuthorizationHandler, CompetitionAuthorizationCRUDHandler>();


            /*The configuration dependency injection is used to be able to override connstrings and secret passwords from azure.
             https://blogs.msdn.microsoft.com/jpsanders/2017/05/16/azure-net-core-application-settings/ */
            services.AddSingleton<IConfiguration>(Configuration);

            //The HttpContextAccessor is used i authorization policies for reading URI.
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            //The PaymentManager Dependency Injection handles all PayEx logic.
            services.AddTransient<IPaymentManager, PayExManager>();
            //The MailService Dependency has connection with the Configuration and handles mailservices.
            services.AddScoped<IMailService, MailService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {

            app.UseSession();
            app.UseDeveloperExceptionPage();
            app.UseAuthentication();
            app.UseStatusCodePagesWithReExecute("/error/{0}");
            app.UseMvcWithDefaultRoute();
            app.UseStaticFiles();
        }
    }
}
