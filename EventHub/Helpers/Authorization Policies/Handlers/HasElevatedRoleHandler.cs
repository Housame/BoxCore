using EventHub.Helpers.Authorization_Policies.Requirements;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EventHub.Helpers.Authorization_Policies
{
    public class HasElevatedRoleHandler : AuthorizationHandler<EventManagerRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, EventManagerRequirement requirement)
        {
            if (context.User.IsInRole("Admin") || context.User.IsInRole("EventAdmin"))
            {               
                //Todo evaluate the EventAdmin seperately
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }

      
    }
}
