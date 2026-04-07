using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Start_Drive.API.IntegrationTests.ControllersTests.ControllerTestFixture;
using Start_Drive.API.IntegrationTests.ControllersTests.FakePolicyUser;
using Start_Drive.API.Data;
using Start_Drive.API.ModelsDto;
using System.Security.Claims;
using System.Text.Json;
using Start_Drive.API.Models;

namespace Start_Drive.API.ControllersTests.ControllersTests
{
    public class SettingsControllerTests : IClassFixture<ControllerTestFixture>
    {
        private readonly ControllerTestFixture _fixture;

        public SettingsControllerTests(ControllerTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetOpenClosesObject_GetOpenCloses_OpenClosesDtoObjects()
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

            context.OpenCloses.AddRange(new OpenClose
            {
                Id = 1,
                DrivingSchoolId = 1,
                DayOfTheWeek = 2,
                IsOpen = true,
                FirstHour = 7,
                LastHour = 15
            });
            context.OpenCloses.AddRange(new OpenClose
            {
                Id = 2,
                DrivingSchoolId = 1,
                DayOfTheWeek = 3,
                IsOpen = true,
                FirstHour = 7,
                LastHour = 15
            });
            context.OpenCloses.AddRange(new OpenClose
            {
                Id = 3,
                DrivingSchoolId = 2,
                DayOfTheWeek = 1,
                IsOpen = true,
                FirstHour = 7,
                LastHour = 15
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();
            var response = await client.GetAsync("startDrive/stronaGlowna/ustawienia/1");
            var content = await response.Content.ReadAsStringAsync();
            var actualObjct = JsonSerializer.Deserialize<List<OpenCloseDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            actualObjct.Should().BeEquivalentTo(new List<OpenCloseDto>
            {
                new OpenCloseDto()
                {
                    Id = 1,
                    DrivingSchoolId = 1,
                    DayOfTheWeek = 2,
                    IsOpen = true,
                    FirstHour = 7,
                    LastHour = 15
                },
                new OpenCloseDto()
                {
                    Id = 2,
                    DrivingSchoolId = 1,
                    DayOfTheWeek = 3,
                    IsOpen = true,
                    FirstHour = 7,
                    LastHour = 15
                }
            });
        }

        [Fact]
        public async Task UpdateOpenCloseObject_UpdateOpenClose_ReturnUppdatedOpenCloseObject()
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
            context.OpenCloses.AddRange(new OpenClose
            {
                Id = 1,
                DrivingSchoolId = 1,
                DayOfTheWeek = 2,
                IsOpen = true,
                FirstHour = 7,
                LastHour = 15
            });
            context.OpenCloses.AddRange(new OpenClose
            {
                Id = 2,
                DrivingSchoolId = 1,
                DayOfTheWeek = 3,
                IsOpen = true,
                FirstHour = 7,
                LastHour = 15
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var uppdateObj = new List<OpenClose>
            {
                new OpenClose()
                {
                    Id = 2,
                    DrivingSchoolId = 1,
                    DayOfTheWeek = 3,
                    IsOpen = true,
                    FirstHour = 8,
                    LastHour = 16
                }
            };
            var response = await client.PutAsJsonAsync("startDrive/stronaGlowna/ustawienia/1", uppdateObj);

            context.ChangeTracker.Clear();

            //assert
            var updatedInstructor = await context.OpenCloses.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            updatedInstructor.Should().NotBeNull();
            updatedInstructor.Should().BeEquivalentTo(new OpenClose
            {
                Id = 2,
                DrivingSchoolId = 1,
                DayOfTheWeek = 3,
                IsOpen = true,
                FirstHour = 8,
                LastHour = 16
            });
        }

        [Fact]
        public async Task GetSingleClosesObject_GetSingleCloses_SingleClosesDtoObjects()
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

            context.SingleCloses.AddRange(new SingleClose
            {
                Id = 1,
                DrivingSchoolId = 1,
                DateCloseKey = "2026-5-1",
                OpenCloseValue = false
            });
            context.SingleCloses.AddRange(new SingleClose
            {
                Id = 2,
                DrivingSchoolId = 1,
                DateCloseKey = "2026-5-2",
                OpenCloseValue = false
            });
            context.SingleCloses.AddRange(new SingleClose
            {
                Id = 3,
                DrivingSchoolId = 2,
                DateCloseKey = "2026-5-7",
                OpenCloseValue = false
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();
            var response = await client.GetAsync("startDrive/stronaGlowna/ustawienia/pojedynczeWolne/1");
            var content = await response.Content.ReadAsStringAsync();
            var actualObjct = JsonSerializer.Deserialize<List<SingleCloseDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            actualObjct.Should().BeEquivalentTo(new List<SingleCloseDto>
            {
                new SingleCloseDto()
                {
                    Id = 1,
                    DrivingSchoolId = 1,
                    DateCloseKey = "2026-5-1",
                    OpenCloseValue = false
                },
                new SingleCloseDto()
                {
                    Id = 2,
                    DrivingSchoolId = 1,
                    DateCloseKey = "2026-5-2",
                    OpenCloseValue = false
                }
            });
        }

        [Fact]
        public async Task CreateSingleCloseObject_CreateSingleClose_AddedSingleClose()
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
            context.SingleCloses.AddRange(new SingleClose
            {
                Id = 1,
                DrivingSchoolId = 1,
                DateCloseKey = "2026-5-1",
                OpenCloseValue = false
            });
            context.SaveChanges();

            //act
            var client = factory.CreateClient();

            var createObj = new SingleClose
            {
                Id = 2,
                DrivingSchoolId = 1,
                DateCloseKey = "2026-5-2",
                OpenCloseValue = false
            };
            var response = await client.PostAsJsonAsync("startDrive/stronaGlowna/ustawienia/pojedynczeWolne/1", createObj);

            context.ChangeTracker.Clear();

            //assert
            var createdSingleClose = await context.SingleCloses.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            createdSingleClose.Should().NotBeNull();
            createdSingleClose.Should().BeEquivalentTo(new SingleClose
            {
                Id = 2,
                DrivingSchoolId = 1,
                DateCloseKey = "2026-5-2",
                OpenCloseValue = false
            });
        }

        [Fact]
        public async Task DeleteSingleCloseObject_DeleteSingleClose_ReturnNullObject()
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

            context.SingleCloses.AddRange(new SingleClose
            {
                Id = 1,
                DrivingSchoolId = 1,
                DateCloseKey = "2026-5-1",
                OpenCloseValue = false
            });
            context.SingleCloses.AddRange(new SingleClose
            {
                Id = 2,
                DrivingSchoolId = 1,
                DateCloseKey = "2026-5-2",
                OpenCloseValue = false
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var response = await client.DeleteAsync("startDrive/stronaGlowna/ustawienia/pojedynczeWolne/1/2");

            context.ChangeTracker.Clear();

            //assert
            var deletedSingleClose = await context.SingleCloses.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            deletedSingleClose.Should().BeNull();
        }

        [Fact]
        public async Task GetBreakBetweenRidersSettingsObject_GetBreakBetweenRidersSettings_GetBreakBetweenRidersValue()
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
                PhoneNumber = "000000000",
                BreakBetweenRides = 1.3
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();
            var response = await client.GetAsync("startDrive/stronaGlowna/ustawienia/czasMiedzyJazdami/1");
            var content = await response.Content.ReadAsStringAsync();
            var actualObjct = JsonSerializer.Deserialize<double>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            actualObjct.Should().Be(1.3);
        }

        [Fact]
        public async Task UpdateBreakBetweenRidesObject_UpdateBreakBetweenRides_ReturnUppdatedBreakBetweenRidesValue()
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
                PhoneNumber = "000000000",
                BreakBetweenRides = 1.3
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var uppdateObj = 1.4;
            var response = await client.PutAsJsonAsync("startDrive/stronaGlowna/ustawienia/czasMiedzyJazdami/1", uppdateObj);

            context.ChangeTracker.Clear();

            //assert
            var updatedBreakBetweenRides = await context.RegisterSchools.FirstOrDefaultAsync(u => u.Id == 1);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            updatedBreakBetweenRides.Should().NotBeNull();
            updatedBreakBetweenRides.BreakBetweenRides.Should().Be(1.4);
        }
    }
}
