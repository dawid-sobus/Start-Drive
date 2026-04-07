using Start_Drive.API.IntegrationTests.ControllersTests.ControllerTestFixture;
using Start_Drive.API.IntegrationTests.ControllersTests.FakePolicyUser;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Start_Drive.API.Data;
using Start_Drive.API.ModelsDto;
using System.Security.Claims;
using System.Text.Json;
using Start_Drive.API.Models.instructorModel;
using Start_Drive.API.Models;
using Start_Drive.API.ModelsDto.instructorModelDto;

namespace Start_Drive.API.ControllersTests.ControllersTests
{
    public class InstructorControllerTests : IClassFixture<ControllerTestFixture>
    {
        private readonly ControllerTestFixture _fixture;

        public InstructorControllerTests(ControllerTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetInstructorsObjects_GetInstructorsAll_InstructorsDtoObjects()
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

            context.Instructors.AddRange(new Instructor
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "instructorName",
                Address = "Address",
                Email = "Instructor@gmail.com",
                Password = "12345"
            });
            context.Instructors.AddRange(new Instructor
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "instructor2Name",
                Address = "Address",
                Email = "Instructor2@gmail.com",
                Password = "67890"
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();
            var response = await client.GetAsync("startDrive/stronaGlowna/instruktorzy/1");
            var content = await response.Content.ReadAsStringAsync();
            var actualObjct = JsonSerializer.Deserialize<List<InstructorDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            actualObjct.Should().BeEquivalentTo(new List<InstructorDto>
            {
                new InstructorDto()
                {
                    Id = 1,
                    DrivingSchoolId = 1,
                    Name = "instructorName",
                    Address = "Address",
                    Email = "Instructor@gmail.com",
                    Password = "12345",
                    StudentsHourDrivesSchool = new List<StudentsHourDriveDto>(),
                    InstructorDaysOff = new List<InstructorAbsenceDto>()
                },
                new InstructorDto()
                {
                    Id = 2,
                    DrivingSchoolId = 1,
                    Name = "instructor2Name",
                    Address = "Address",
                    Email = "Instructor2@gmail.com",
                    Password = "67890",
                    StudentsHourDrivesSchool = new List<StudentsHourDriveDto>(),
                    InstructorDaysOff = new List<InstructorAbsenceDto>()
                }
            });
        }

        [Fact]
        public async Task GetInstructorObject_GetInstructor_InstructorDtoObject()
        {
            //arrange
            _fixture.SetTestUser(new Claim("schoolId", "1"), new Claim("roleApp", "instructor"),
            new Claim("emailAddress", "Instructor@gmail.com"), new Claim("instructorId", "1"));

            var factory = _fixture.CreateFactoryWithUser();

            using var scope = factory.Services.CreateScope();
            var provider = scope.ServiceProvider.GetRequiredService<ITestClaimsProvider>() as TestClaimsProvider;
            provider.Claims = new List<Claim>
            {
                new Claim("schoolId", "1"), new Claim("roleApp", "instructor"),
                new Claim("emailAddress", "Instructor@gmail.com"), new Claim("instructorId", "1")
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
                Name = "instructorName",
                Address = "Address",
                Email = "Instructor@gmail.com",
                Password = "12345",
                StudentsHourDrivesSchool = new List<StudentsHourDrive>()
                {
                    new StudentsHourDrive()
                    {
                        Id = 1,
                        DrivingSchoolId = 1,
                        StudentId = 1,
                        InstructorId = 1,
                        DateAddedDrive = new DateTime(2026, 4, 5),
                        MainHourFrom = 14.15,
                        MainHourTo = 16.15
                    }
                },
                InstructorDaysOff = new List<InstructorAbsence>()
                {
                    new InstructorAbsence()
                    {
                        Id = 1,
                        IdDrivingSchool = 1,
                        InstructorId = 1,
                        DateAbsenceKey = "2026.5.5",
                        ReasonAbsenceValue = "Some reason absence"
                    }
                }
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();
            var response = await client.GetAsync("startDrive/stronaGlowna/instruktorzy/1/1");
            var content = await response.Content.ReadAsStringAsync();
            var actualObjct = JsonSerializer.Deserialize<InstructorDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            actualObjct.Should().BeEquivalentTo(new InstructorDto
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "instructorName",
                Address = "Address",
                Email = "Instructor@gmail.com",
                Password = "12345",
                StudentsHourDrivesSchool = new List<StudentsHourDriveDto>()
                {
                    new StudentsHourDriveDto()
                    {
                        Id = 1,
                        DrivingSchoolId = 1,
                        StudentId = 1,
                        InstructorId = 1,
                        DateAddedDrive = new DateTime(2026, 4, 5),
                        MainHourFrom = 14.15,
                        MainHourTo = 16.15
                    }
                },
                InstructorDaysOff = new List<InstructorAbsenceDto>()
                {
                    new InstructorAbsenceDto()
                    {
                        Id = 1,
                        IdDrivingSchool = 1,
                        InstructorId = 1,
                        DateAbsenceKey = "2026.5.5",
                        ReasonAbsenceValue = "Some reason absence"
                    }
                }
            });
        }

        [Fact]
        public async Task CreateInstructorObject_CreateInstructor_AddedInstructor()
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
                Name = "instructorName",
                Address = "Address",
                Email = "Instructor@gmail.com",
                Password = "12345"
            });
            context.SaveChanges();

            //act
            var client = factory.CreateClient();

            var createObj = new Instructor
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "instructor2Name",
                Address = "Address",
                Email = "Instructor2@gmail.com",
            };
            var response = await client.PostAsJsonAsync("startDrive/stronaGlowna/instruktorzy/1", createObj);

            context.ChangeTracker.Clear();

            //assert
            var createdInstructor = await context.Instructors.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            createdInstructor.Should().NotBeNull();
            createdInstructor.Should().BeEquivalentTo(new Instructor
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "instructor2Name",
                Address = "Address",
                Email = "Instructor2@gmail.com",
                Password = ""
            });
        }

        [Fact]
        public async Task UpdateInstructorObject_UpdateInstructor_ReturnUppdatedInstructorObject()
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

            context.Instructors.AddRange(new Instructor
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "instructorName",
                Address = "Address",
                Email = "Instructor@gmail.com",
                Password = "12345"
            });
            context.Instructors.AddRange(new Instructor
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "instructor2Name",
                Address = "Address",
                Email = "Instructor2@gmail.com",
                Password = "67890"
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var uppdateObj = new Instructor
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "instructorNameUppdated",
                Address = "AddressUppdated",
                Email = "InstructorUppdated@gmail.com"
            };
            var response = await client.PutAsJsonAsync("startDrive/stronaGlowna/instruktorzy/1/1", uppdateObj);

            context.ChangeTracker.Clear();

            //assert
            var updatedInstructor = await context.Instructors.FirstOrDefaultAsync(u => u.Id == 1);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            updatedInstructor.Should().NotBeNull();
            updatedInstructor.Should().BeEquivalentTo(new Instructor
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "instructorNameUppdated",
                Address = "AddressUppdated",
                Email = "InstructorUppdated@gmail.com",
                Password = "12345"
            });
        }

        [Fact]
        public async Task DeleteInstructorObject_DeleteInstructor_ReturnNullObject()
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

            context.Instructors.AddRange(new Instructor
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "instructorName",
                Address = "Address",
                Email = "Instructor@gmail.com",
                Password = "12345"
            });
            context.Instructors.AddRange(new Instructor
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "instructor2Name",
                Address = "Address",
                Email = "Instructor2@gmail.com",
                Password = "67890"
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var response = await client.DeleteAsync("startDrive/stronaGlowna/instruktorzy/1/2");

            context.ChangeTracker.Clear();

            //assert
            var deletedInstructor = await context.Instructors.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            deletedInstructor.Should().BeNull();
        }

        //--------------------------------------- Calendar Absence tests ------------------------------------------------------

        [Fact]
        public async Task CreateAbsenceObject_CreateAbsence_AddedAbsence()
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

            context.InstructorAbsences.AddRange(new InstructorAbsence
            {
                Id = 1,
                IdDrivingSchool = 1,
                InstructorId = 1,
                DateAbsenceKey = "2026.5.5 - 2026.5.10",
                ReasonAbsenceValue = "some reason absence1"
            });
            context.InstructorAbsences.AddRange(new InstructorAbsence
            {
                Id = 2,
                IdDrivingSchool = 1,
                InstructorId = 1,
                DateAbsenceKey = "2026.5.12 - 2026.5.13",
                ReasonAbsenceValue = "some reason absence2"
            });
            context.SaveChanges();

            //act
            var client = factory.CreateClient();

            var createObj = new List<InstructorAbsence>
            {
                new InstructorAbsence()
                {
                    Id = 3,
                    IdDrivingSchool = 1,
                    InstructorId = 1,
                    DateAbsenceKey = "2026.5.15 - 2026.5.16",
                    ReasonAbsenceValue = "some reason absence3"
                }
            };
            var response = await client.PostAsJsonAsync("startDrive/stronaGlowna/instruktorzy/nieobecnosc/1/1", createObj);

            context.ChangeTracker.Clear();

            //assert
            var createdInstructor = await context.InstructorAbsences.FirstOrDefaultAsync(u => u.Id == 3);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            createdInstructor.Should().NotBeNull();
            createdInstructor.Should().BeEquivalentTo(new InstructorAbsence
            {
                Id = 3,
                IdDrivingSchool = 1,
                InstructorId = 1,
                DateAbsenceKey = "2026.5.15 - 2026.5.16",
                ReasonAbsenceValue = "some reason absence3"
            });
        }

        [Fact]
        public async Task DeleteAbsenceObject_DeleteAbsence_ReturnNullObject()
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

            context.InstructorAbsences.AddRange(new InstructorAbsence
            {
                Id = 1,
                IdDrivingSchool = 1,
                InstructorId = 1,
                DateAbsenceKey = "2026.5.5 - 2026.5.10",
                ReasonAbsenceValue = "some reason absence1"
            });
            context.InstructorAbsences.AddRange(new InstructorAbsence
            {
                Id = 2,
                IdDrivingSchool = 1,
                InstructorId = 1,
                DateAbsenceKey = "2026.5.12 - 2026.5.13",
                ReasonAbsenceValue = "some reason absence2"
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var deleteObj = new List<int>
            {
                1, 2
            };
            var response = await client.PutAsJsonAsync("startDrive/stronaGlowna/instruktorzy/nieobecnosc/1/1", deleteObj);

            context.ChangeTracker.Clear();

            //assert
            var deletedAbsence1 = await context.InstructorAbsences.FirstOrDefaultAsync(u => u.Id == 1);
            var deletedAbsence2 = await context.InstructorAbsences.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            deletedAbsence1.Should().BeNull();
            deletedAbsence2.Should().BeNull();
        }

        //----------------------------------------------------------- InstructorPage ---------------------------------------------------

        [Fact]
        public async Task DeleteInstructorForInstructorPageObject_DeleteInstructorForInstructorPage_ReturnNullObject()
        {
            //arrange
            _fixture.SetTestUser(new Claim("schoolId", "1"), new Claim("roleApp", "instructor"),
            new Claim("emailAddress", "Instructor@gmail.com"), new Claim("instructorId", "1"));


            var factory = _fixture.CreateFactoryWithUser();

            using var scope = factory.Services.CreateScope();
            var provider = scope.ServiceProvider.GetRequiredService<ITestClaimsProvider>() as TestClaimsProvider;
            provider.Claims = new List<Claim>
            {
                new Claim("schoolId", "1"), new Claim("roleApp", "instructor"),
                new Claim("emailAddress", "Instructor@gmail.com"), new Claim("instructorId", "1")
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
                Name = "instructorName",
                Address = "Address",
                Email = "Instructor@gmail.com",
                Password = "12345"
            });
            context.Instructors.AddRange(new Instructor
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "instructor2Name",
                Address = "Address",
                Email = "Instructor2@gmail.com",
                Password = "67890"
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var response = await client.DeleteAsync("startDrive/stronaGlowna/instruktorzy/kontoInstruktora/1/1");

            context.ChangeTracker.Clear();

            //assert
            var deletedInstructor = await context.Instructors.FirstOrDefaultAsync(u => u.Id == 1);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            deletedInstructor.Password.Should().BeEmpty();
        }
    }
}
