using EventHub.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Helpers.Authorization_Policies
{
    public class CompetitionAuthorizationCRUDHandler : AuthorizationHandler<OperationAuthorizationRequirement>
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly EventHubContext dBContext;
        private readonly UserManager<IdentityUser> userManager;

        public CompetitionAuthorizationCRUDHandler(IHttpContextAccessor httpContextAccessor,
            EventHubContext dBContext,
            UserManager<IdentityUser> userManager)
        {
            this.httpContextAccessor = httpContextAccessor;
            this.dBContext = dBContext;
            this.userManager = userManager;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, OperationAuthorizationRequirement requirement)
        {
            if (context.User.IsInRole("Admin"))
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }

            if(requirement.Name == nameof(Operations.Create))
            {
                if (context.User.IsInRole("EventAdmin"))
                    context.Succeed(requirement);
            }

            else if(requirement.Name ==nameof(Operations.Update))
            {
                //Get the URI-value 
                var requestedCompetition = httpContextAccessor.HttpContext.GetRouteData().Values["id"];

                if (context.User.IsInRole("EventAdmin"))
                {                  
                    if (requestedCompetition != null)
                    {
                        var creatorId = dBContext.Competition
                            .Where(o => o.Id == int.Parse(requestedCompetition.ToString()))
                            .Select(p => p.CreatorId).FirstOrDefault();

                        //TODO Check authorization ajax called uri

                        if (creatorId != null)
                        {
                            if (creatorId.ToString() == userManager.GetUserId(context.User))
                            {
                                context.Succeed(requirement);
                            }
                            if(context.User.HasClaim("Box", creatorId) )
                            {
                                context.Succeed(requirement);
                            }
                        }
                    }
                    else
                    {
                        context.Succeed(requirement);
                    }
                }
                else if(context.User.IsInRole("Client"))
                {
                    if (context.User.HasClaim("CompetitionManager", requirement.Name))
                    {
                        if (requestedCompetition != null)
                        {
                            var boxId = dBContext.Competition
                                .Where(o => o.Id == int.Parse(requestedCompetition.ToString()))
                                .Select(p => p.CreatorId).FirstOrDefault();

                            //TODO correct parsing! TryParse
                            //Security Issue. If user has claim update from one box and 
                            //view from other he will have update on both.
                            if (boxId != null)
                            {
                                if (context.User.HasClaim("Box", boxId))
                                {
                                    context.Succeed(requirement);
                                }
                            }
                        }
                        else
                        {
                            context.Succeed(requirement);
                        }
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
