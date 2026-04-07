using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Start_Drive.API.IntegrationTests.ControllersTests.FakePolicyUser
{
    public class FakeUserFilter : IAsyncActionFilter
    {
        private readonly ClaimsPrincipal _claims;

        public FakeUserFilter(ClaimsPrincipal claims)
        {
            _claims = claims;
        }
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.User = _claims;
            await next();
        }
    }
}
