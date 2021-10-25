using EventHub.Helpers.Authorization_Policies.Requirements;
using EventHub.Models.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Helpers.Authorization_Policies
{
    public class IsEventManager : AuthorizationHandler<EventManagerRequirement>
    {
       private readonly EventHubContext dBContext;
        private readonly UserManager<IdentityUser> userManager;

        public IsEventManager(EventHubContext dBContext, UserManager<IdentityUser> userManager)
        {
            this.dBContext = dBContext;
            this.userManager = userManager;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
            EventManagerRequirement requirement )
        {
            if (context.User.HasClaim(c => c.Type == "EventManager"))
            {
                var userId = userManager.GetUserId(context.User);
                var userBox = dBContext.User
                           .Where(o => o.HashId == userId)
                           .Select(p => p.BoxId).FirstOrDefault();

                if (userBox != null)
                {
                    if (context.User.HasClaim("EventManager", userBox.ToString()))
                    {
                        context.Succeed(requirement);
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
