using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Start_Drive.API.Controllers;
using Start_Drive.API.Models;
using Start_Drive.API.Models.instructorModel;
using Start_Drive.API.Services;


namespace Start_Drive.API.UnitTests
{
    public class LoginControllerTests
    {
        [Fact]
        public void CheckCorrectness_CheckSchoolLogsIn_ReturnSchoolAndToken()
        {
            //arrange
            var mockService = new Mock<ILoginService>();
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            mockService.Setup(s => s.CheckLoginSchool(It.IsAny<Login>())).Returns(new RegisterSchool { Email = "school@gmail.com", Password = "1234", Name = "Name", Address = "Address", PhoneNumber = "000000000" });

            var controller = new LoginController(mockService.Object, configuration);

            //act
            var result = controller.CheckCorrectness(new Login { Email = "school@gmail.com", Password = "1234", SchoolOrStudent = "school" });

            var value = result.Should().BeOfType<OkObjectResult>().Subject.Value;

            var valueAsString = value.ToString();
            var splitValue = valueAsString.Split(" = ");

            //assert
            value.Should().BeEquivalentTo(new
            {
                loggedSchoolData = new RegisterSchool { Email = "school@gmail.com", Password = "1234", Name = "Name", Address = "Address", PhoneNumber = "000000000" },
                sendToken = "token"
            }, opt => opt.Excluding(x => x.sendToken));

            splitValue[2].Length.Should().BeGreaterThan(2);
        }

        [Fact]
        public void CheckCorrectness_CheckInstructorLogsIn_ReturnInstructorAndToken()
        {
            //arrange
            var mockService = new Mock<ILoginService>();
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            mockService.Setup(s => s.CheckLoginInstructor(It.IsAny<Login>())).Returns(new Instructor { Email = "instructor@gmail.com", Password = "1234", Name = "Name", Address = "Address", PhoneNumber = "000000000" });

            var controller = new LoginController(mockService.Object, configuration);

            //act
            var result = controller.CheckCorrectness(new Login { Email = "instructor@gmail.com", Password = "1234", SchoolOrStudent = "instructor" });

            var value = result.Should().BeOfType<OkObjectResult>().Subject.Value;

            var valueAsString = value.ToString();
            var splitValue = valueAsString.Split(" = ");

            //assert
            value.Should().BeEquivalentTo(new
            {
                loggedInstructorData = new Instructor { Email = "instructor@gmail.com", Password = "1234", Name = "Name", Address = "Address", PhoneNumber = "000000000" },
                sendToken = "token"
            }, opt => opt.Excluding(x => x.sendToken));

            splitValue[2].Length.Should().BeGreaterThan(2);
        }

        [Fact]
        public void CheckCorrectness_CheckStudentLogsIn_ReturnStudentAndToken()
        {
            //arrange
            var mockService = new Mock<ILoginService>();
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            mockService.Setup(s => s.CheckLoginStudent(It.IsAny<Login>())).Returns(new Student { Email = "student@gmail.com", Password = "1234", Name = "Name", Address = "Address", PhoneNumber = "000000000" });

            var controller = new LoginController(mockService.Object, configuration);

            //act
            var result = controller.CheckCorrectness(new Login { Email = "student@gmail.com", Password = "1234", SchoolOrStudent = "student" });

            var value = result.Should().BeOfType<OkObjectResult>().Subject.Value;

            var valueAsString = value.ToString();
            var splitValue = valueAsString.Split(" = ");

            //assert
            value.Should().BeEquivalentTo(new
            {
                loggedStudentData = new Student { Email = "student@gmail.com", Password = "1234", Name = "Name", Address = "Address", PhoneNumber = "000000000" },
                sendToken = "token"
            }, opt => opt.Excluding(x => x.sendToken));

            splitValue[2].Length.Should().BeGreaterThan(2);
        }

        [Fact]
        public void CheckCorrectness_CheckSchoolNoUserIncorrectData_ReturnNull()
        {
            //arrange
            var mockService = new Mock<ILoginService>();
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            mockService.Setup(s => s.CheckLoginSchool(It.IsAny<Login>())).Returns((RegisterSchool)null);

            var controller = new LoginController(mockService.Object, configuration);

            //act
            var result = controller.CheckCorrectness(new Login { Email = "school@gmail.com", Password = "1634", SchoolOrStudent = "school" });

            var value = result.Should().BeOfType<OkObjectResult>().Subject.Value;

            //assert
            value.Should().BeEquivalentTo(new
            {
                loggedSchoolData = (RegisterSchool)null,
                sendToken = ""
            });
        }

        [Fact]
        public void CheckCorrectness_CheckInstructorNoUserIncorrectData_ReturnNull()
        {
            //arrange
            var mockService = new Mock<ILoginService>();
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            mockService.Setup(s => s.CheckLoginInstructor(It.IsAny<Login>())).Returns((Instructor)null);

            var controller = new LoginController(mockService.Object, configuration);

            //act
            var result = controller.CheckCorrectness(new Login { Email = "instructor@gmail.com", Password = "1264", SchoolOrStudent = "instructor" });

            var value = result.Should().BeOfType<OkObjectResult>().Subject.Value;

            //assert
            value.Should().BeEquivalentTo(new
            {
                loggedInstructorData = (Instructor)null,
                sendToken = ""
            });
        }

        [Fact]
        public void CheckCorrectness_CheckStudentNoUserIncorrectData_ReturnNull()
        {
            //arrange
            var mockService = new Mock<ILoginService>();
            IConfiguration configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();

            mockService.Setup(s => s.CheckLoginStudent(It.IsAny<Login>())).Returns((Student)null);

            var controller = new LoginController(mockService.Object, configuration);

            //act
            var result = controller.CheckCorrectness(new Login { Email = "student@gmail.com", Password = "1234", SchoolOrStudent = "student" });

            var value = result.Should().BeOfType<OkObjectResult>().Subject.Value;

            //assert
            value.Should().BeEquivalentTo(new
            {
                loggedStudentData = (Student)null,
                sendToken = ""
            });
        }
    }
}
