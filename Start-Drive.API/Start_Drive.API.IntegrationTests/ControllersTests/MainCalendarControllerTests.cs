using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Start_Drive.API.IntegrationTests.ControllersTests.ControllerTestFixture;
using Start_Drive.API.IntegrationTests.ControllersTests.FakePolicyUser;
using Start_Drive.API.Data;
using Start_Drive.API.Models;
using Start_Drive.API.ModelsDto;
using System.Security.Claims;
using System.Text.Json;

namespace Start_Drive.API.ControllersTests.ControllersTests
{
    public class MainCalendarControllerTests : IClassFixture<ControllerTestFixture>
    {
        private readonly ControllerTestFixture _fixture;

        public MainCalendarControllerTests(ControllerTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("school")]
        [InlineData("instructor")]
        public async Task GetInstructorsHoursDrivesObject_GetInstructorsHoursDrives_InstructorsHoursDrivesDtoObjects(string role)
        {
            //arrange
            _fixture.SetTestUser(new Claim("schoolId", "1"), new Claim("roleApp", role));

            var factory = _fixture.CreateFactoryWithUser();

            using var scope = factory.Services.CreateScope();
            var provider = scope.ServiceProvider.GetRequiredService<ITestClaimsProvider>() as TestClaimsProvider;
            provider.Claims = new List<Claim>
            {
                new Claim("schoolId", "1"), new Claim("roleApp", role)
            };

            var context = scope.ServiceProvider.GetRequiredService<StartDriveDbContext>();


            context.StudentsHourDrives.AddRange(new StudentsHourDrive
            {
                Id = 1,
                DrivingSchoolId = 1,
                StudentId = 1,
                InstructorId = 1,
                DateAddedDrive = new DateTime(2026, 4, 5),
                MainHourFrom = 14.15,
                MainHourTo = 16.15
            });
            context.StudentsHourDrives.AddRange(new StudentsHourDrive
            {
                Id = 2,
                DrivingSchoolId = 1,
                StudentId = 1,
                InstructorId = 1,
                DateAddedDrive = new DateTime(2026, 4, 7),
                MainHourFrom = 14.15,
                MainHourTo = 16.15
            });
            context.StudentsHourDrives.AddRange(new StudentsHourDrive
            {
                Id = 3,
                DrivingSchoolId = 2,
                StudentId = 1,
                InstructorId = 1,
                DateAddedDrive = new DateTime(2026, 4, 7),
                MainHourFrom = 14.15,
                MainHourTo = 16.15
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();
            var response = await client.GetAsync("startDrive/stronaGlowna/kalendarz/1");
            var content = await response.Content.ReadAsStringAsync();
            var actualObjct = JsonSerializer.Deserialize<List<StudentsHourDriveDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            actualObjct.Should().BeEquivalentTo(new List<StudentsHourDriveDto>
            {
                new StudentsHourDriveDto
                {
                    Id = 1,
                    DrivingSchoolId = 1,
                    StudentId = 1,
                    InstructorId = 1,
                    DateAddedDrive = new DateTime(2026, 4, 5),
                    MainHourFrom = 14.15,
                    MainHourTo = 16.15
                },
                new StudentsHourDriveDto
                {
                    Id = 2,
                    DrivingSchoolId = 1,
                    StudentId = 1,
                    InstructorId = 1,
                    DateAddedDrive = new DateTime(2026, 4, 7),
                    MainHourFrom = 14.15,
                    MainHourTo = 16.15
                }
            });
        }

        [Theory]
        [InlineData("school")]
        [InlineData("student")]
        public async Task GetDrivingHoursForStudentObject_GetDrivingHoursForStudent_DrivingHoursForStudentDtoObjects(string role)
        {
            //arrange
            _fixture.SetTestUser(new Claim("schoolId", "1"), new Claim("roleApp", role));

            var factory = _fixture.CreateFactoryWithUser();

            using var scope = factory.Services.CreateScope();
            var provider = scope.ServiceProvider.GetRequiredService<ITestClaimsProvider>() as TestClaimsProvider;
            provider.Claims = new List<Claim>
            {
                new Claim("schoolId", "1"), new Claim("roleApp", role)
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
            context.StudentsHourDrives.AddRange(new StudentsHourDrive
            {
                Id = 1,
                DrivingSchoolId = 1,
                StudentId = 1,
                InstructorId = 1,
                DateAddedDrive = new DateTime(2026, 4, 5),
                MainHourFrom = 14.15,
                MainHourTo = 16.15
            });
            context.StudentsHourDrives.AddRange(new StudentsHourDrive
            {
                Id = 2,
                DrivingSchoolId = 1,
                StudentId = 1,
                InstructorId = 1,
                DateAddedDrive = new DateTime(2026, 4, 7),
                MainHourFrom = 14.15,
                MainHourTo = 16.15
            });
            context.StudentsHourDrives.AddRange(new StudentsHourDrive
            {
                Id = 3,
                DrivingSchoolId = 1,
                StudentId = 2,
                InstructorId = 1,
                DateAddedDrive = new DateTime(2026, 4, 9),
                MainHourFrom = 14.15,
                MainHourTo = 16.15
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();
            var response = await client.GetAsync("startDrive/stronaGlowna/kalendarz/1/1");
            var content = await response.Content.ReadAsStringAsync();
            var actualObjct = JsonSerializer.Deserialize<List<StudentsHourDriveDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            actualObjct.Should().BeEquivalentTo(new List<StudentsHourDriveDto>
            {
                new StudentsHourDriveDto
                {
                    Id = 1,
                    DrivingSchoolId = 1,
                    StudentId = 1,
                    InstructorId = 1,
                    DateAddedDrive = new DateTime(2026, 4, 5),
                    MainHourFrom = 14.15,
                    MainHourTo = 16.15
                },
                new StudentsHourDriveDto
                {
                    Id = 2,
                    DrivingSchoolId = 1,
                    StudentId = 1,
                    InstructorId = 1,
                    DateAddedDrive = new DateTime(2026, 4, 7),
                    MainHourFrom = 14.15,
                    MainHourTo = 16.15
                }
            });
        }

        [Fact]
        public async Task CreateHoursDrivingStudentObject_AddHoursDrivingStudent_AddedHoursDrivingStudent()
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

            context.StudentsHourDrives.AddRange(new StudentsHourDrive
            {
                Id = 1,
                DrivingSchoolId = 1,
                StudentId = 1,
                InstructorId = 1,
                DateAddedDrive = new DateTime(2026, 4, 5),
                MainHourFrom = 14.15,
                MainHourTo = 16.15
            });
            context.SaveChanges();

            //act
            var client = factory.CreateClient();

            var createObj = new StudentsHourDrive
            {
                Id = 2,
                DrivingSchoolId = 1,
                StudentId = 1,
                InstructorId = 1,
                DateAddedDrive = new DateTime(2026, 4, 7),
                MainHourFrom = 14.15,
                MainHourTo = 16.15
            };
            var response = await client.PostAsJsonAsync("startDrive/stronaGlowna/kalendarz/1/1/2026-04-07", createObj);

            context.ChangeTracker.Clear();

            //assert
            var createdStudentsHourDrive = await context.StudentsHourDrives.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            createdStudentsHourDrive.Should().NotBeNull();
            createdStudentsHourDrive.Should().BeEquivalentTo(new StudentsHourDrive
            {
                Id = 2,
                DrivingSchoolId = 1,
                StudentId = 1,
                InstructorId = 1,
                DateAddedDrive = new DateTime(2026, 4, 7),
                MainHourFrom = 14.15,
                MainHourTo = 16.15
            });
        }

        [Fact]
        public async Task DeleteHoursDrivingObject_DeleteHoursDriving_ReturnNullObject()
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

            context.StudentsHourDrives.AddRange(new StudentsHourDrive
            {
                Id = 1,
                DrivingSchoolId = 1,
                StudentId = 1,
                InstructorId = 1,
                DateAddedDrive = new DateTime(2026, 4, 5),
                MainHourFrom = 14.15,
                MainHourTo = 16.15
            });
            context.StudentsHourDrives.AddRange(new StudentsHourDrive
            {
                Id = 2,
                DrivingSchoolId = 1,
                StudentId = 1,
                InstructorId = 1,
                DateAddedDrive = new DateTime(2026, 4, 7),
                MainHourFrom = 14.15,
                MainHourTo = 16.15
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var response = await client.DeleteAsync("startDrive/stronaGlowna/kalendarz/1/1/2");

            context.ChangeTracker.Clear();

            //assert
            var deletedStudentsHourDrive = await context.StudentsHourDrives.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            deletedStudentsHourDrive.Should().BeNull();
        }
    }
}
