using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Start_Drive.API.Data;
using Start_Drive.API.IntegrationTests.ControllersTests.ControllerTestFixture;
using Start_Drive.API.IntegrationTests.ControllersTests.FakePolicyUser;
using Start_Drive.API.Models;
using Start_Drive.API.ModelsDto;
using System.Security.Claims;
using System.Text.Json;

namespace Start_Drive.API.ControllerTests.ControllersTests
{
    public class CourseRoundControllerTests : IClassFixture<ControllerTestFixture>
    {
        private readonly ControllerTestFixture _fixture;

        public CourseRoundControllerTests(ControllerTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Theory]
        [InlineData("school")]
        [InlineData("instructor")]
        public async Task GetCourseRoundsObject_GetCourseRounds_CourseRoundsDtoObjects(string role)
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


            context.CourseRounds.AddRange(new CourseRoundsModel
            { 
                Id = 1,
                DrivingSchoolId = 1, 
                Name = "Round 1", 
                From = new DateTime(2026, 3, 3), 
                To = new DateTime(2026, 5, 5)
            });
            context.CourseRounds.AddRange(new CourseRoundsModel
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "Round 2",
                From = new DateTime(2026, 5, 6),
                To = new DateTime(2026, 7, 7)
            });
            context.CourseRounds.AddRange(new CourseRoundsModel
            {
                Id = 3,
                DrivingSchoolId = 2,
                Name = "Round 3",
                From = new DateTime(2026, 5, 6),
                To = new DateTime(2026, 7, 7)
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();
            var response = await client.GetAsync("startDrive/stronaGlowna/turyKursow/1");
            var content = await response.Content.ReadAsStringAsync();
            var actualObjct = JsonSerializer.Deserialize<List<CourseRoundsModelDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            actualObjct.Should().BeEquivalentTo(new List<CourseRoundsModelDto>
            {
                new CourseRoundsModelDto
                {
                    Id = 1,
                    DrivingSchoolId = 1,
                    Name = "Round 1",
                    From = new DateTime(2026, 3, 3),
                    To = new DateTime(2026, 5, 5),
                    StudentsThisRoundCourse = new List<CourseRoundsStudentsIdDto>()
                },
                new CourseRoundsModelDto
                {
                    Id = 2,
                    DrivingSchoolId = 1,
                    Name = "Round 2",
                    From = new DateTime(2026, 5, 6),
                    To = new DateTime(2026, 7, 7),
                    StudentsThisRoundCourse = new List<CourseRoundsStudentsIdDto>()
                }
            });
        }

        [Fact]
        public async Task CreateCourseRoundObject_CreateCourseRound_ReturnCourseRoundObject()
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
            context.CourseRounds.AddRange(new CourseRoundsModel
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "Round 1",
                From = new DateTime(2026, 3, 3),
                To = new DateTime(2026, 5, 5)
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var createObj = new CourseRoundsModel 
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "Round 2",
                From = new DateTime(2026, 5, 6),
                To = new DateTime(2026, 7, 7)
            };
            var response = await client.PostAsJsonAsync("startDrive/stronaGlowna/turyKursow/1", createObj);

            context.ChangeTracker.Clear();

            //assert
            var createCourseRounds = await context.CourseRounds.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            createCourseRounds.Should().NotBeNull();
            createCourseRounds.Should().BeEquivalentTo(new CourseRoundsModel
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "Round 2",
                From = new DateTime(2026, 5, 6),
                To = new DateTime(2026, 7, 7)
            });
        }

        [Fact]
        public async Task UppdateCourseRoundObject_UpdateCourseRound_ReturnUppdatedCourseRoundObject()
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

            context.CourseRounds.AddRange(new CourseRoundsModel
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "Round 1",
                From = new DateTime(2026, 3, 3),
                To = new DateTime(2026, 5, 5)
            });
            context.CourseRounds.AddRange(new CourseRoundsModel
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "Round 2",
                From = new DateTime(2026, 5, 6),
                To = new DateTime(2026, 7, 7)
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var uppdateObj = new CourseRoundsModel
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "RoundUppdated 1",
                From = new DateTime(2026, 3, 5),
                To = new DateTime(2026, 5, 6)
            };
            var response = await client.PutAsJsonAsync("startDrive/stronaGlowna/turyKursow/1/1", uppdateObj);

            context.ChangeTracker.Clear();

            //assert
            var updateCourseRoundsModel = await context.CourseRounds.FirstOrDefaultAsync(u => u.Id == 1);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            updateCourseRoundsModel.Should().NotBeNull();
            updateCourseRoundsModel.Should().BeEquivalentTo(new CourseRoundsModel
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "RoundUppdated 1",
                From = new DateTime(2026, 3, 5),
                To = new DateTime(2026, 5, 6)
            });
        }

        [Fact]
        public async Task DeleteCourseRoundObject_DeleteCourseRound_ReturnNullObject()
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
            context.CourseRounds.AddRange(new CourseRoundsModel
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "Round 1",
                From = new DateTime(2026, 3, 3),
                To = new DateTime(2026, 5, 5)
            });
            context.CourseRounds.AddRange(new CourseRoundsModel
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "Round 2",
                From = new DateTime(2026, 5, 6),
                To = new DateTime(2026, 7, 7)
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var response = await client.DeleteAsync("startDrive/stronaGlowna/turyKursow/1/1");

            context.ChangeTracker.Clear();

            //assert
            var deletedCourseRounds = await context.CourseRounds.FirstOrDefaultAsync(u => u.Id == 1);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            deletedCourseRounds.Should().BeNull();
        }

        [Fact]
        public async Task AddStudent_AddStudentToCourseRound_ReturnAddedStudentId()
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
            context.CourseRoundsStudentsIds.AddRange(new CourseRoundsStudentsId
            {
                Id = 1,
                IdDrivingSchool = 1,
                CourseRoundsModelId = 1,
                CourseRoundStudentId = 1
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var createObj = new CourseRoundsStudentsId
            {
                Id = 2,
                IdDrivingSchool = 1,
                CourseRoundStudentId = 2
            };
            var response = await client.PostAsJsonAsync("startDrive/stronaGlowna/turyKursow/dodajKursanta/1/1", createObj);

            context.ChangeTracker.Clear();
            
            //assert
            var createCourseRoundsStudentsId = await context.CourseRoundsStudentsIds.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            createCourseRoundsStudentsId.Should().NotBeNull();
            createCourseRoundsStudentsId.Should().BeEquivalentTo(new CourseRoundsStudentsId
            {
                Id = 2,
                IdDrivingSchool = 1,
                CourseRoundsModelId = 1,
                CourseRoundStudentId = 2
            });
        }

        [Fact]
        public async Task DeleteStudent_DeleteStudentFromCourseRound_ReturnNullObject()
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

            context.CourseRoundsStudentsIds.AddRange(new CourseRoundsStudentsId
            {
                Id = 1,
                IdDrivingSchool = 1,
                CourseRoundsModelId = 1,
                CourseRoundStudentId = 1
            });
            context.CourseRoundsStudentsIds.AddRange(new CourseRoundsStudentsId
            {
                Id = 2,
                IdDrivingSchool = 1,
                CourseRoundsModelId = 1,
                CourseRoundStudentId = 2
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var response = await client.DeleteAsync("startDrive/stronaGlowna/turyKursow/1/1/2");

            context.ChangeTracker.Clear();

            //assert
            var deletedCourseRounds = await context.CourseRoundsStudentsIds.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            deletedCourseRounds.Should().BeNull();
        }
    }
}
