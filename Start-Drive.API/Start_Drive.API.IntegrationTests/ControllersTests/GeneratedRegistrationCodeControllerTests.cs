using Start_Drive.API.Data;
using Start_Drive.API.IntegrationTests.ControllersTests.ControllerTestFixture;
using Start_Drive.API.IntegrationTests.ControllersTests.FakePolicyUser;
using Start_Drive.API.Models;
using System.Security.Claims;
using Start_Drive.API.Models.instructorModel;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;

namespace Start_Drive.API.ControllerTests.ControllersTests
{
    public class GeneratedRegistrationCodeControllerTests : IClassFixture<ControllerTestFixture>
    {
        private readonly ControllerTestFixture _fixture;

        public GeneratedRegistrationCodeControllerTests(ControllerTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task CreateRegistrationCodeObject_CreateRegistrationCode_AddedRegistrationCode()
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
            context.Instructors.AddRange(new Instructor
            {
                Id = 1,
                DrivingSchoolId = 1,
                Email = "instructor@gmail.com",
                Password = "1234",
                Name = "InstructorName",
                Address = "Address",
                PhoneNumber = "000000000"
            });
            context.GeneratedRegistrationCodes.AddRange(new GeneratedRegistrationCode
            {
                Id = 1,
                DrivingSchoolId = 1,
                DrivingSchoolEmail = "school@gmail.com",
                PersonType = "student",
                PersonId = 1,
                PersonEmail = "student@gmail.com",
                GeneratedCode = "sdflksjfkshjksbkjsvjsfeuwhdiwefkncajb"
            });
            context.SaveChanges();

            //act
            var client = factory.CreateClient();

            var createObj = new GeneratedRegistrationCode
            {
                Id = 2,
                DrivingSchoolId = 1,
                DrivingSchoolEmail = "school@gmail.com",
                PersonType = "instructor",
                PersonId = 1,
                PersonEmail = "instructor@gmail.com",
                GeneratedCode = "instructor@gmail.comkghjhgjh"
            };
            var response = await client.PostAsJsonAsync("startDrive/generujKod/1", createObj);

            context.ChangeTracker.Clear();

            //assert
            var createdCode = await context.GeneratedRegistrationCodes.FirstOrDefaultAsync(u => u.Id == 2);
            var instructor = await context.Instructors.FirstOrDefaultAsync(u => u.Id == 1);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            createdCode.Should().BeEquivalentTo(new GeneratedRegistrationCode
            {
                Id = 2,
                DrivingSchoolId = 1,
                DrivingSchoolEmail = "school@gmail.com",
                PersonType = "instructor",
                PersonId = 1,
                PersonEmail = "instructor@gmail.com",
            }, opt => opt.Excluding(x => x.GeneratedCode));
            instructor.Password.Should().BeEmpty();
        }
    }
}
