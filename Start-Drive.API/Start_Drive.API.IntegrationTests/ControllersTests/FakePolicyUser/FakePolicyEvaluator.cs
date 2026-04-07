using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using System.Security.Claims;

namespace Start_Drive.API.IntegrationTests.ControllersTests.FakePolicyUser
{
    public class FakePolicyEvaluator : IPolicyEvaluator
    {
        private readonly ITestClaimsProvider _claimsProvider;

        public FakePolicyEvaluator(ITestClaimsProvider claimsProvider)
        {
            _claimsProvider = claimsProvider;
        }

        public Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
        {
            var claims = _claimsProvider.GetClaims();
            var identity = new ClaimsIdentity(claims, "FakeScheme");
            var principal = new ClaimsPrincipal(identity);

            context.User = principal;
            var ticket = new AuthenticationTicket(principal, "FakeScheme");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }

        public Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object? resource)
        {
            return Task.FromResult(PolicyAuthorizationResult.Success());
        }
    }
}
