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
    public class StudentControllerTests : IClassFixture<ControllerTestFixture>
    {
        private readonly ControllerTestFixture _fixture;

        public StudentControllerTests(ControllerTestFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetStudentsAllObject_GetStudentsAll_StudentDtoObjects()
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

            context.Students.AddRange(new Student
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "Name1",
                Address = "Addres1",
                Email = "student1@gmail.com",
                Password = "12345"
            });
            context.Students.AddRange(new Student
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "Name2",
                Address = "Addres3",
                Email = "student3@gmail.com",
                Password = "23456"
            });
            context.Students.AddRange(new Student
            {
                Id = 3,
                DrivingSchoolId = 2,
                Name = "Name4",
                Address = "Addres4",
                Email = "student4@gmail.com",
                Password = "45678"
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();
            var response = await client.GetAsync("startDrive/stronaGlowna/kursanci/1");
            var content = await response.Content.ReadAsStringAsync();
            var actualObjct = JsonSerializer.Deserialize<List<StudentDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            actualObjct.Should().BeEquivalentTo(new List<StudentDto>
            {
                new StudentDto()
                {
                    Id = 1,
                    DrivingSchoolId = 1,
                    Name = "Name1",
                    Address = "Addres1",
                    Email = "student1@gmail.com",
                    Password = "12345"
                },
                new StudentDto()
                {
                    Id = 2,
                    DrivingSchoolId = 1,
                    Name = "Name2",
                    Address = "Addres3",
                    Email = "student3@gmail.com",
                    Password = "23456"
                }
            });
        }

        [Fact]
        public async Task GetStudentObject_GetStudent_StudentDtoObjects()
        {
            //arrange
            _fixture.SetTestUser(new Claim("schoolId", "1"), new Claim("roleApp", "student"),
            new Claim("emailAddress", "student1@gmail.com"), new Claim("studentId", "1"));

            var factory = _fixture.CreateFactoryWithUser();

            using var scope = factory.Services.CreateScope();
            var provider = scope.ServiceProvider.GetRequiredService<ITestClaimsProvider>() as TestClaimsProvider;
            provider.Claims = new List<Claim>
            {
                new Claim("schoolId", "1"), new Claim("roleApp", "student"),
                new Claim("emailAddress", "student1@gmail.com"), new Claim("studentId", "1")
            };

            var context = scope.ServiceProvider.GetRequiredService<StartDriveDbContext>();

            context.Students.AddRange(new Student
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "Name1",
                Address = "Addres1",
                Email = "student1@gmail.com",
                Password = "12345"
            });
            context.Students.AddRange(new Student
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "Name2",
                Address = "Addres2",
                Email = "student2@gmail.com",
                Password = "23456"
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();
            var response = await client.GetAsync("startDrive/stronaGlowna/kursanci/1/1");
            var content = await response.Content.ReadAsStringAsync();
            var actualObjct = JsonSerializer.Deserialize<StudentDto>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            actualObjct.Should().BeEquivalentTo(new StudentDto()
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "Name1",
                Address = "Addres1",
                Email = "student1@gmail.com",
                Password = "12345"
            });
        }

        [Fact]
        public async Task CreateStudentObject_CreateStudent_AddedStudent()
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
            context.Students.AddRange(new Student
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "Name1",
                Address = "Addres1",
                Email = "student1@gmail.com",
                Password = "12345"
            });
            context.SaveChanges();

            //act
            var client = factory.CreateClient();

            var createObj = new Student
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "Name2",
                Address = "Addres2",
                Email = "student2@gmail.com",
                Password = ""
            };
            var response = await client.PostAsJsonAsync("startDrive/stronaGlowna/kursanci/1", createObj);

            context.ChangeTracker.Clear();

            //assert
            var createdStudent = await context.Students.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            createdStudent.Should().NotBeNull();
            createdStudent.Should().BeEquivalentTo(new Student
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "Name2",
                Address = "Addres2",
                Email = "student2@gmail.com",
                Password = ""
            });
        }

        [Fact]
        public async Task UpdateStudentObject_UpdateStudent_ReturnUppdatedStudentObject()
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

            context.Students.AddRange(new Student
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "Name1",
                Address = "Addres1",
                Email = "student1@gmail.com",
                Password = "12345"
            });
            context.Students.AddRange(new Student
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "Name2",
                Address = "Addres2",
                Email = "student2@gmail.com",
                Password = "23456"
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var uppdateObj = new Student()
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "UppdatedName2",
                Address = "UppdatedAddres2",
                Email = "Uppdatedstudent2@gmail.com",
                Password = "23456"
            };
            var response = await client.PutAsJsonAsync("startDrive/stronaGlowna/kursanci/1/2", uppdateObj);

            context.ChangeTracker.Clear();

            //assert
            var updatedStudent = await context.Students.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            updatedStudent.Should().NotBeNull();
            updatedStudent.Should().BeEquivalentTo(new Student
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "UppdatedName2",
                Address = "UppdatedAddres2",
                Email = "Uppdatedstudent2@gmail.com",
                Password = "23456"
            });
        }

        [Fact]
        public async Task DeleteStudentObject_DeleteStudent_ReturnNullObject()
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

            context.Students.AddRange(new Student
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "Name1",
                Address = "Addres1",
                Email = "student1@gmail.com",
                Password = "12345"
            });
            context.Students.AddRange(new Student
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "Name2",
                Address = "Addres2",
                Email = "student2@gmail.com",
                Password = "23456"
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var response = await client.DeleteAsync("startDrive/stronaGlowna/kursanci/1/2");

            context.ChangeTracker.Clear();

            //assert
            var deletedStudent = await context.Students.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            deletedStudent.Should().BeNull();
        }

        //----------------------------------------------------------- StudentPage ---------------------------------------------------

        [Fact]
        public async Task DeleteStudentForStudentPageObject_DeleteStudentForStudentPage_ReturnNullObject()
        {
            //arrange
            _fixture.SetTestUser(new Claim("schoolId", "1"), new Claim("roleApp", "student"),
            new Claim("emailAddress", "student1@gmail.com"), new Claim("studentId", "1"));


            var factory = _fixture.CreateFactoryWithUser();

            using var scope = factory.Services.CreateScope();
            var provider = scope.ServiceProvider.GetRequiredService<ITestClaimsProvider>() as TestClaimsProvider;
            provider.Claims = new List<Claim>
            {
                new Claim("schoolId", "1"), new Claim("roleApp", "student"),
                new Claim("emailAddress", "student1@gmail.com"), new Claim("studentId", "1")
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
            context.Students.AddRange(new Student
            {
                Id = 1,
                DrivingSchoolId = 1,
                Name = "Name1",
                Address = "Addres1",
                Email = "student1@gmail.com",
                Password = "12345"
            });
            context.Students.AddRange(new Student
            {
                Id = 2,
                DrivingSchoolId = 1,
                Name = "Name2",
                Address = "Addres2",
                Email = "student2@gmail.com",
                Password = "23456"
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var response = await client.DeleteAsync("startDrive/stronaGlowna/kursanci/kontoKursanta/1/1");

            context.ChangeTracker.Clear();

            //assert
            var deletedStudent = await context.Students.FirstOrDefaultAsync(u => u.Id == 1);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            deletedStudent.Password.Should().BeEmpty();
        }
    }
}
