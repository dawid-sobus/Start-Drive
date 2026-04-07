using FluentAssertions;
using Start_Drive.API.IntegrationTests.ControllersTests.ControllerTestFixture;
using Start_Drive.API.Data;
using Start_Drive.API.Models;
using System.Text.Json;
using Start_Drive.API.Models.instructorModel;

namespace Start_Drive.API.ControllersTests.ControllersTests
{
    public class LoginControllerTests : IClassFixture<ControllerTestFixture>
    {
        private readonly ControllerTestFixture _fixture;

        public LoginControllerTests(ControllerTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task LoginSchoolToApp_CheckCorrectness_ReturnSchoolUserAndToken()
        {
            //arrange
            var factory = _fixture.CreateFactoryWithUser();

            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StartDriveDbContext>();

            context.RegisterSchools.AddRange(new RegisterSchool
            {
                Id = 1,
                Email = "school@gmail.com",
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword("1234", 13),
                Name = "Name",
                Address = "Address",
                City = "City",
                PhoneNumber = "000000000"
            });
            context.SaveChanges();

            //act
            var client = factory.CreateClient();

            var loginObj = new Login
            {
                Email = "school@gmail.com",
                Password = "1234",
                SchoolOrStudent = "school"
            };

            var response = await client.PostAsJsonAsync("startDrive/login", loginObj);
            var content = await response.Content.ReadAsStringAsync();
            var template = new
            {
                loggedSchoolData = new
                {
                    id = 0,
                    email = "",
                    password = "",
                    name = "",
                    address = "",
                    city = "",
                    phoneNumber = ""
                },
                sendToken = ""
            };
            var actualObjct = JsonSerializer.Deserialize(content, template.GetType());
            var token = actualObjct.GetType().GetProperty("sendToken").GetValue(actualObjct).ToString();

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            token.Should().NotBeEmpty();
            actualObjct.Should().NotBeNull();
            actualObjct.Should().BeEquivalentTo(new
            {
                loggedSchoolData = new
                {
                    id = 1,
                    email = "school@gmail.com",
                    name = "Name",
                    address = "Address",
                    city = "City",
                    phoneNumber = "000000000"
                },
                sendToken = ""
            }, opt => opt.Excluding(x => x.sendToken));
        }

        [Fact]
        public async Task LoginInstructorToApp_CheckCorrectness_ReturnInstructorUserAndToken()
        {
            //arrange
            var factory = _fixture.CreateFactoryWithUser();

            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StartDriveDbContext>();

            context.Instructors.AddRange(new Instructor
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "instructorName",
                Address = "Address",
                Email = "Instructor@gmail.com",
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword("12345", 13)
            });
            context.SaveChanges();

            //act
            var client = factory.CreateClient();

            var loginObj = new Login
            {
                Email = "Instructor@gmail.com",
                Password = "12345",
                SchoolOrStudent = "instructor"
            };

            var response = await client.PostAsJsonAsync("startDrive/login", loginObj);
            var content = await response.Content.ReadAsStringAsync();
            var template = new
            {
                loggedInstructorData = new
                {
                    id = 0,
                    drivingSchoolId = 0,
                    name = "",
                    address = "",
                    email = "",
                    password = ""
                },
                sendToken = ""
            };
            var actualObjct = JsonSerializer.Deserialize(content, template.GetType());
            var token = actualObjct.GetType().GetProperty("sendToken").GetValue(actualObjct).ToString();

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            token.Should().NotBeEmpty();
            actualObjct.Should().NotBeNull();
            actualObjct.Should().BeEquivalentTo(new
            {
                loggedInstructorData = new
                {
                    id = 1,
                    drivingSchoolId = 1,
                    name = "instructorName",
                    address = "Address",
                    email = "Instructor@gmail.com"
                },
                sendToken = ""
            }, opt => opt.Excluding(x => x.sendToken));
        }

        [Fact]
        public async Task LoginStudentToApp_CheckCorrectness_ReturnStudentUserAndToken()
        {
            //arrange
            var factory = _fixture.CreateFactoryWithUser();

            using var scope = factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<StartDriveDbContext>();

            context.Students.AddRange(new Student
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "Name",
                Address = "Addres",
                Email = "student@gmail.com",
                Password = BCrypt.Net.BCrypt.EnhancedHashPassword("12345", 13)
            });
            context.SaveChanges();

            //act
            var client = factory.CreateClient();

            var loginObj = new Login
            {
                Email = "student@gmail.com",
                Password = "12345",
                SchoolOrStudent = "student"
            };

            var response = await client.PostAsJsonAsync("startDrive/login", loginObj);
            var content = await response.Content.ReadAsStringAsync();
            var template = new
            {
                loggedStudentData = new
                {
                    id = 0,
                    drivingSchoolId = 0,
                    name = "",
                    address = "",
                    email = "",
                    password = ""
                },
                sendToken = ""
            };
            var actualObjct = JsonSerializer.Deserialize(content, template.GetType());
            var token = actualObjct.GetType().GetProperty("sendToken").GetValue(actualObjct).ToString();

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            token.Should().NotBeEmpty();
            actualObjct.Should().NotBeNull();
            actualObjct.Should().BeEquivalentTo(new
            {
                loggedStudentData = new
                {
                    id = 1,
                    drivingSchoolId = 1,
                    name = "Name",
                    address = "Addres",
                    email = "student@gmail.com"
                },
                sendToken = ""
            }, opt => opt.Excluding(x => x.sendToken));
        }
    }
}
