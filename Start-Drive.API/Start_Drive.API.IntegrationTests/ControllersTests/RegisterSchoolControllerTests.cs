using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using Start_Drive.API.IntegrationTests.ControllersTests.ControllerTestFixture;
using Start_Drive.API.IntegrationTests.ControllersTests.FakePolicyUser;
using Start_Drive.API.Data;
using Start_Drive.API.Models;
using System.Security.Claims;
using Start_Drive.API.ModelsDto;
using System.Text.Json;
using Start_Drive.API.Models.instructorModel;
using Start_Drive.API.Models.forumModel;

namespace Start_Drive.API.ControllersTests.ControllersTests
{
    public class RegisterSchoolControllerTests : IClassFixture<ControllerTestFixture>
    {
        private readonly ControllerTestFixture _fixture;

        public RegisterSchoolControllerTests(ControllerTestFixture fixture)
        {
            _fixture = fixture;
        }

        List<OpenClose> listReturn(int schoolId)
        {
            List<OpenClose> openCloses = new List<OpenClose>();

            for (int i = 0; i < 7; i++)
            {
                var valueId = i + 1;
                var openCloseObj = new OpenClose()
                {
                    Id = valueId,
                    DrivingSchoolId = schoolId,
                    DayOfTheWeek = i,
                    IsOpen = true,
                    FirstHour = 7,
                    LastHour = 15
                };
                openCloses.Add(openCloseObj);
            }

            return openCloses;
        }

        [Fact]
        public async Task CreateSchoolObject_CreateSchool_AddedSchool()
        {
            //arrange
            var factory = _fixture.CreateFactoryWithUser();

            using var scope = factory.Services.CreateScope();
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
                BreakBetweenRides = 0
            });
            context.SaveChanges();

            //act
            var client = factory.CreateClient();

            var createObj = new RegisterSchool
            {
                Id = 2,
                Email = "school2@gmail.com",
                Password = "34567",
                Name = "Name2",
                Address = "Address2",
                City = "City2",
                PhoneNumber = "000000000",
                BreakBetweenRides = 0
            };
            var response = await client.PostAsJsonAsync("startDrive/rejestracja", createObj);

            context.ChangeTracker.Clear();



            //assert
            var createdRegisterSchool = await context.RegisterSchools.FirstOrDefaultAsync(u => u.Id == 2);
            
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            createdRegisterSchool.Should().NotBeNull();
            createdRegisterSchool.Should().BeEquivalentTo(new RegisterSchool
            {
                Id = 2,
                Email = "school2@gmail.com",
                Name = "Name2",
                Address = "Address2",
                City = "City2",
                PhoneNumber = "000000000",
                BreakBetweenRides = 0.1,

            }, opt => opt.Excluding(x => x.Password));

            var createdADrivingSchool = await context.ADrivingSchools.FirstOrDefaultAsync(u => u.DrivingSchoolId == 2);
            createdADrivingSchool.Should().BeEquivalentTo(new ADrivingSchool()
            {
                Id = 1,
                DrivingSchoolId = 2,
                AboutText = "Dodaj opis szkole jazdy!"
            }, opt => opt.Excluding(x => x.DrivingSchool));

            var opencloseList = listReturn(2);
            var createdOpenClose = await context.OpenCloses.Where(u => u.DrivingSchoolId == 2).ToListAsync();
            createdOpenClose.Should().BeEquivalentTo(opencloseList, opt => opt.Excluding(x => x.DrivingSchool));

        }

        [Fact]
        public async Task GetInformationSchoolObject_GetInformationSchool_RegisterSchoolDtoObjects()
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
                BreakBetweenRides = 0
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();
            var response = await client.GetAsync("startDrive/rejestracja/1");
            var content = await response.Content.ReadAsStringAsync();
            var actualObjct = JsonSerializer.Deserialize<RegisterSchoolDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            actualObjct.Should().BeEquivalentTo(new RegisterSchoolDto()
            {
                Id = 1,
                Email = "school@gmail.com",
                Password = "1234",
                Name = "Name",
                Address = "Address",
                City = "City",
                PhoneNumber = "000000000",
                BreakBetweenRides = 0,
                Instructors = new List<Instructor>(),
                Students = new List<Student>(),
                CourseRounds = new List<CourseRoundsModel>(),
                Forum = new List<Questions>(),
                InfoForMembers = new List<Information>(),
                OpenCloseSchool = new List<OpenClose>(),
                ClosedSingleDays = new List<SingleClose>(),
                DrivingLessonsStudents = new List<StudentsHourDrive>(),
                GeneratedCodes = new List<GeneratedRegistrationCode>()
            });
        }

        [Fact]
        public async Task UpdateSchoolObject_UpdateSchool_ReturnUppdatedRegisterSchoolObject()
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
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var uppdateObj = new RegisterSchool
            {
                Id = 1,
                Email = "Uppdatedschool@gmail.com",
                Password = "1234",
                Name = "UppdatedName",
                Address = "UppdatedAddress",
                City = "UppdatedCity",
                PhoneNumber = "111111111"
            };
            var response = await client.PutAsJsonAsync("startDrive/rejestracja/1", uppdateObj);

            context.ChangeTracker.Clear();

            //assert
            var updatedRegisterSchool = await context.RegisterSchools.FirstOrDefaultAsync(u => u.Id == 1);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            updatedRegisterSchool.Should().NotBeNull();
            updatedRegisterSchool.Should().BeEquivalentTo(new RegisterSchool
            {
                Id = 1,
                Email = "Uppdatedschool@gmail.com",
                Name = "UppdatedName",
                Address = "UppdatedAddress",
                City = "UppdatedCity",
                PhoneNumber = "111111111"
            }, opt => opt.Excluding(x => x.Password));
        }

        [Fact]
        public async Task DeleteSchoolObject_DeleteSchool_ReturnNullObject()
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
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var response = await client.DeleteAsync("startDrive/rejestracja/1");

            context.ChangeTracker.Clear();

            //assert
            var deletedRegisterSchool = await context.RegisterSchools.FirstOrDefaultAsync(u => u.Id == 1);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            deletedRegisterSchool.Should().BeNull();
        }

        //----------------------------------------------------------- Register Instructor -------------------------------------------------------

        [Fact]
        public async Task CreateInstructorAccountObject_CreateInstructorAccount_CreatedInstructorAccount()
        {
            //arrange
            var factory = _fixture.CreateFactoryWithUser();

            using var scope = factory.Services.CreateScope();
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
                BreakBetweenRides = 0
            });
            context.Instructors.AddRange(new Instructor
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "instructorName",
                Address = "Address",
                Email = "Instructor@gmail.com",
                Password = ""
            });
            context.GeneratedRegistrationCodes.AddRange(new GeneratedRegistrationCode
            {
                Id = 1,
                DrivingSchoolId = 1,
                DrivingSchoolEmail = "school@gmail.com",
                PersonType = "instructor",
                PersonId = 1,
                PersonEmail = "Instructor@gmail.com",
                GeneratedCode = BCrypt.Net.BCrypt.EnhancedHashPassword("qwertyuiopasdfghjklzxcvbnm", 13)
            });
            context.SaveChanges();

            //act
            var client = factory.CreateClient();

            var createObj = new RegisterStudentInstructor
            {
                GeneratedCode = "qwertyuiopasdfghjklzxcvbnm",
                DrivingSchoolEmail = "school@gmail.com",
                PersonEmail = "Instructor@gmail.com",
                Password = "12345"
            };

            var response = await client.PostAsJsonAsync("startDrive/rejestracja/instruktor", createObj);

            context.ChangeTracker.Clear();

            //assert
            var createdInstructorAccount = await context.Instructors.FirstOrDefaultAsync(u => u.Id == 1);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            createdInstructorAccount.Should().NotBeNull();
            createdInstructorAccount.Password.Should().NotBeEmpty();
        }

        //----------------------------------------------------------- Register Student -------------------------------------------------------

        [Fact]
        public async Task CreateStudentAccountObject_CreateStudentAccount_CreatedStudentAccount()
        {
            //arrange
            var factory = _fixture.CreateFactoryWithUser();

            using var scope = factory.Services.CreateScope();
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
                BreakBetweenRides = 0
            });
            context.Students.AddRange(new Student
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "Name",
                Address = "Addres",
                Email = "student@gmail.com",
                Password = ""
            });
            context.GeneratedRegistrationCodes.AddRange(new GeneratedRegistrationCode
            {
                Id = 1,
                DrivingSchoolId = 1,
                DrivingSchoolEmail = "school@gmail.com",
                PersonType = "student",
                PersonId = 1,
                PersonEmail = "student@gmail.com",
                GeneratedCode = BCrypt.Net.BCrypt.EnhancedHashPassword("qwertyuiopasdfghjklzxcvbnm", 13)
            });
            context.SaveChanges();

            //act
            var client = factory.CreateClient();

            var createObj = new RegisterStudentInstructor
            {
                GeneratedCode = "qwertyuiopasdfghjklzxcvbnm",
                DrivingSchoolEmail = "school@gmail.com",
                PersonEmail = "student@gmail.com",
                Password = "12345"
            };

            var response = await client.PostAsJsonAsync("startDrive/rejestracja/kursant", createObj);

            context.ChangeTracker.Clear();

            //assert
            var createdStudentAccount = await context.Students.FirstOrDefaultAsync(u => u.Id == 1);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            createdStudentAccount.Should().NotBeNull();
            createdStudentAccount.Password.Should().NotBeEmpty();
        }
    }
}
