using Microsoft.EntityFrameworkCore;
using Start_Drive.API.Data;
using Start_Drive.API.IntegrationTests.ControllersTests.ControllerTestFixture;
using Start_Drive.API.IntegrationTests.ControllersTests.FakePolicyUser;
using Start_Drive.API.Models;
using FluentAssertions;
using System.Text.Json;
using Start_Drive.API.ModelsDto;
using System.Security.Claims;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Start_Drive.API.ControllerTests.ControllersTests
{
    public class AboutSchoolControllerTests : IClassFixture<ControllerTestFixture>
    {
        private readonly ControllerTestFixture _fixture;

        public AboutSchoolControllerTests(ControllerTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetADrivingSchool_GetAboutDrivingSchool_ADrivingSchoolDtoObject()
        {
            //arrange
            _fixture.SetTestUser(new Claim("schoolId", "1"), new Claim("roleApp", "school"));

            var factory = _fixture.CreateFactoryWithUser();

            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StartDriveDbContext>();

            context.ADrivingSchools.AddRange(new ADrivingSchool { Id = 1, DrivingSchoolId = 1, AboutText = "AboutText1" });
            context.ADrivingSchools.AddRange(new ADrivingSchool { Id = 2, DrivingSchoolId = 2, AboutText = "AboutText2" });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();
            var response = await client.GetAsync("startDrive/stronaGlowna/oSzkoleJazdy/1");
            var content = await response.Content.ReadAsStringAsync();
            var actualObjct = JsonSerializer.Deserialize<ADrivingSchoolDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true});

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            actualObjct.Should().BeEquivalentTo(new ADrivingSchoolDto { Id = 1, DrivingSchoolId = 1, AboutText = "AboutText1" });
        }

        [Fact]
        public async Task UppdateADrivingSchool_UppdateAboutSchool_UppdatedADrivingSchoolDtoObject()
        {
            //arrange
            _fixture.SetTestUser(new Claim("schoolId", "1"), new Claim("roleApp", "school"));
            

            var factory = _fixture.CreateFactoryWithUser();

            using var scope = factory.Services.CreateScope();
            var provider = scope.ServiceProvider.GetRequiredService<ITestClaimsProvider>() as TestClaimsProvider;
            provider.Claims = new List<Claim>
            {
                new Claim("schoolId", "1"), new Claim("roleApp", "school")
            };

            var context = scope.ServiceProvider.GetRequiredService<StartDriveDbContext>();

            context.ADrivingSchools.AddRange(new ADrivingSchool { Id = 1, DrivingSchoolId = 1, AboutText = "AboutText1" });
            context.ADrivingSchools.AddRange(new ADrivingSchool { Id = 2, DrivingSchoolId = 2, AboutText = "AboutText2" });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var uppdateObj = new ADrivingSchool { AboutText = "AboutText1Uppdated" };
            var response = await client.PutAsJsonAsync("startDrive/stronaGlowna/oSzkoleJazdy/1/1", uppdateObj);

            context.ChangeTracker.Clear();

            //assert
            var updateADrivingSchool = await context.ADrivingSchools.FirstOrDefaultAsync(u => u.Id == 1);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            updateADrivingSchool.Should().NotBeNull();
            updateADrivingSchool.Should().BeEquivalentTo(new ADrivingSchool
            {
                Id = 1,
                DrivingSchoolId = 1,
                AboutText = "AboutText1Uppdated"
            });
        }
    }
}
