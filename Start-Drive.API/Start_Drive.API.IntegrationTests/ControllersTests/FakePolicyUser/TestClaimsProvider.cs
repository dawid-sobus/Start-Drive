using System.Security.Claims;

namespace Start_Drive.API.IntegrationTests.ControllersTests.FakePolicyUser
{
    public interface ITestClaimsProvider
    {
        List<Claim> GetClaims();
    }
    public class TestClaimsProvider : ITestClaimsProvider
    {
        public List<Claim> Claims { get; set; }
        public List<Claim> GetClaims() => Claims;
    }
}
