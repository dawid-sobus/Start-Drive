using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Start_Drive.API.Data;
using Start_Drive.API.IntegrationTests.ControllersTests.FakePolicyUser;
using System.Security.Claims;

namespace Start_Drive.API.IntegrationTests.ControllersTests.ControllerTestFixture
{
    public class ControllerTestFixture : WebApplicationFactory<Program>
    {
        public ClaimsPrincipal _testUser = new ClaimsPrincipal();

        public WebApplicationFactory<Program> CreateFactoryWithUser()
        {
            return this.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    

                    ConfigureInMemoryDb(services);
                    ConfigureMapper(services);

                    services.AddSingleton<ITestClaimsProvider, TestClaimsProvider>();
                    services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                    services.AddMvc(option => option.Filters.Add(new FakeUserFilter(_testUser)));
                });
            });
        }

        private void ConfigureInMemoryDb(IServiceCollection services)
        {
            var descriptor = services.Where(d => d.ServiceType == typeof(DbContextOptions<StartDriveDbContext>)).ToList();
            foreach(var d in descriptor)
            {
                services.Remove(d);
            }


            string _dbName = Guid.NewGuid().ToString();
            services.AddDbContext<StartDriveDbContext>(options =>
            {
                options.UseInMemoryDatabase(_dbName);
            });
        }

        private void ConfigureMapper(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Program));
        }

        public void SetTestUser(params Claim[] claims)
        {
            _testUser = new ClaimsPrincipal(new ClaimsIdentity(claims, "FakeAuth"));
        }
    }
}
