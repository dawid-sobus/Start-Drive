using Start_Drive.API.Data;
using Start_Drive.API.IntegrationTests.ControllersTests.ControllerTestFixture;
using Start_Drive.API.IntegrationTests.ControllersTests.FakePolicyUser;
using Start_Drive.API.Models;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using System.Text.Json;
using Start_Drive.API.ModelsDto;

namespace Start_Drive.API.ControllersTests.ControllersTests
{
    public class InformationControllerTests : IClassFixture<ControllerTestFixture>
    {
        private readonly ControllerTestFixture _fixture;

        public InformationControllerTests(ControllerTestFixture fixture)
        {
            _fixture = fixture;
        }


        [Fact]
        public async Task CreateQuestionObject_CreateQuestion_AddedQuestion()
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

            context.RegisterSchools.AddRange(new RegisterSchool
            {
                Id = 1,
                Email = "school@gmail.com",
                Password = "1234",
                Name = "Name",
                Address = "Address",
                City = "City",
                PhoneNumber = "000000000"
            });
            context.Informations.AddRange(new Information
            {
                Id = 1,
                DrivingSchoolId = 1,
                ForWhom = "instructor",
                Info = "Some instructor info"
            });
            context.SaveChanges();

        //act
        var client = factory.CreateClient();

            var createObj = new Information
            {
                Id = 2,
                DrivingSchoolId = 1,
                ForWhom = "student",
                Info = "Some students info"
            };
            var response = await client.PostAsJsonAsync("startDrive/stronaGlowna/informacje/1", createObj);

            context.ChangeTracker.Clear();

            //assert
            var createdInformation = await context.Informations.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            createdInformation.Should().NotBeNull();
            createdInformation.Should().BeEquivalentTo(new Information
            {
                Id = 2,
                DrivingSchoolId = 1,
                ForWhom = "student",
                Info = "Some students info"
            });
        }

        [Fact]
        public async Task GetInformationsObject_GetInformationsAll_InformationsDtoObjects()
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

            context.Informations.AddRange(new Information
            {
                Id = 1,
                DrivingSchoolId = 1,
                ForWhom = "instructor",
                Info = "Some instructor info"
            });
            context.Informations.AddRange(new Information
            {
                Id = 2,
                DrivingSchoolId = 1,
                ForWhom = "student",
                Info = "Some students info"
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();
            var response = await client.GetAsync("startDrive/stronaGlowna/informacje/1");
            var content = await response.Content.ReadAsStringAsync();
            var actualObjct = JsonSerializer.Deserialize<List<InformationDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            actualObjct.Should().BeEquivalentTo(new List<InformationDto>
            {
                new InformationDto()
                {
                    Id = 1,
                    DrivingSchoolId = 1,
                    ForWhom = "instructor",
                    Info = "Some instructor info"
                },
                new InformationDto()
                {
                    Id = 2,
                    DrivingSchoolId = 1,
                    ForWhom = "student",
                    Info = "Some students info"
                }
            });
        }

        [Fact]
        public async Task DeleteInformationObject_DeleteInformation_ReturnNullObject()
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

            context.Informations.AddRange(new Information
            {
                Id = 1,
                DrivingSchoolId = 1,
                ForWhom = "instructor",
                Info = "Some instructor info"
            });
            context.Informations.AddRange(new Information
            {
                Id = 2,
                DrivingSchoolId = 1,
                ForWhom = "student",
                Info = "Some students info"
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var response = await client.DeleteAsync("startDrive/stronaGlowna/informacje/1/2");

            context.ChangeTracker.Clear();

            //assert
            var deletedInformation = await context.Informations.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            deletedInformation.Should().BeNull();
        }
    }
}
