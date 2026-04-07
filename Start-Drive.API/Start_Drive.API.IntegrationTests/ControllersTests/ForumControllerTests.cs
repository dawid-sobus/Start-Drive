using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Start_Drive.API.Data;
using Start_Drive.API.IntegrationTests.ControllersTests.ControllerTestFixture;
using Start_Drive.API.IntegrationTests.ControllersTests.FakePolicyUser;
using Start_Drive.API.Models;
using Start_Drive.API.Models.forumModel;
using Start_Drive.API.ModelsDto.forumModelDto;
using System.Security.Claims;
using System.Text.Json;

namespace Start_Drive.API.ControllerTests.ControllersTests
{
    public class ForumControllerTests : IClassFixture<ControllerTestFixture>
    {
        private readonly ControllerTestFixture _fixture;

        public ForumControllerTests(ControllerTestFixture fixture)
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
            context.Questionss.AddRange(new Questions
            {
                Id = 1,
                DrivingSchoolId = 1,
                PersonId = 1,
                AskedQuestion = "student",
                DateAdded = new DateTime(2026, 3, 3),
                QuestionText = "Some question student",
                Answer = new List<Answers>()
            });
            context.SaveChanges();

            //act
            var client = factory.CreateClient();

            var createObj = new Questions
            {
                Id = 2,
                DrivingSchoolId = 1,
                PersonId = 1,
                AskedQuestion = "school",
                DateAdded = new DateTime(2026, 4, 4),
                QuestionText = "Some question school",
                Answer = new List<Answers>()
            };
            var response = await client.PostAsJsonAsync("startDrive/stronaGlowna/forum/1", createObj);

            context.ChangeTracker.Clear();

            //assert
            var createdQuestion = await context.Questionss.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            createdQuestion.Should().NotBeNull();
            createdQuestion.Should().BeEquivalentTo(new Questions
            {
                Id = 2,
                DrivingSchoolId = 1,
                PersonId = 1,
                AskedQuestion = "school",
                DateAdded = new DateTime(2026, 4, 4),
                QuestionText = "Some question school",
                Answer = new List<Answers>()
            });
        }

        [Fact]
        public async Task GetQuestionsAnswersObject_GetQuestionsAnswers_QuestionsAnswersDtoObjects()
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

            context.Questionss.AddRange(new Questions
            {
                Id = 1,
                DrivingSchoolId = 2,
                PersonId = 1,
                AskedQuestion = "student",
                DateAdded = new DateTime(2026, 3, 3),
                QuestionText = "Some question student",
                Answer = new List<Answers>()
            });
            context.Questionss.AddRange(new Questions
            {
                Id = 2,
                DrivingSchoolId = 1,
                PersonId = 1,
                AskedQuestion = "school",
                DateAdded = new DateTime(2026, 3, 3),
                QuestionText = "Some question student",
                Answer = new List<Answers>()
                {
                    new Answers
                    {
                        Id = 1,
                        DrivingSchoolId = 1,
                        QuestionsId = 2,
                        PersonId = 1,
                        WhoReplied = "student",
                        DateAdded = new DateTime(2026, 3, 3),
                        AnswerText = "Some answer student",
                    }
                }
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();
            var response = await client.GetAsync("startDrive/stronaGlowna/forum/1");
            var content = await response.Content.ReadAsStringAsync();
            var actualObjct = JsonSerializer.Deserialize<List<QuestionsDto>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            //assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            content.Should().NotBeNullOrEmpty();
            actualObjct.Should().BeEquivalentTo(new List<QuestionsDto>
            {
                new QuestionsDto()
                {
                    Id = 2,
                    DrivingSchoolId = 1,
                    PersonId = 1,
                    AskedQuestion = "school",
                    DateAdded = new DateTime(2026, 3, 3),
                    QuestionText = "Some question student",
                    Answer = new List<AnswersDto>()
                    {
                        new AnswersDto
                        {
                            Id = 1,
                            DrivingSchoolId = 1,
                            QuestionsId = 2,
                            PersonId = 1,
                            WhoReplied = "student",
                            DateAdded = new DateTime(2026, 3, 3),
                            AnswerText = "Some answer student",
                        }
                    }
                }
                
            });
        }

        [Fact]
        public async Task CreateAnswerObject_CreateAnswer_AddedAnswer()
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
            context.Questionss.AddRange(new Questions
            {
                Id = 1,
                DrivingSchoolId = 1,
                PersonId = 1,
                AskedQuestion = "student",
                DateAdded = new DateTime(2026, 3, 3),
                QuestionText = "Some question student",
                Answer = new List<Answers>()
            });
            context.Answerss.AddRange(new Answers
            {
                Id = 1,
                DrivingSchoolId = 1,
                QuestionsId = 1,
                PersonId = 1,
                WhoReplied = "instructor",
                DateAdded = new DateTime(2026, 3, 5),
                AnswerText = "Some answer instructor"
            });
            context.SaveChanges();

            //act
            var client = factory.CreateClient();

            var createObj = new Answers
            {
                Id = 2,
                DrivingSchoolId = 1,
                QuestionsId = 1,
                PersonId = 1,
                WhoReplied = "school",
                DateAdded = new DateTime(2026, 3, 6),
                AnswerText = "Some answer school"
            };
            var response = await client.PostAsJsonAsync("startDrive/stronaGlowna/forum/1/1", createObj);

            context.ChangeTracker.Clear();

            //assert
            var createdAnswer = await context.Answerss.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            createdAnswer.Should().NotBeNull();
            createdAnswer.Should().BeEquivalentTo(new Answers
            {
                Id = 2,
                DrivingSchoolId = 1,
                QuestionsId = 1,
                PersonId = 1,
                WhoReplied = "school",
                DateAdded = new DateTime(2026, 3, 6),
                AnswerText = "Some answer school"
            });
        }

        [Fact]
        public async Task DeleteQuestionObject_DeleteQuestion_ReturnNullObject()
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

            context.Questionss.AddRange(new Questions
            {
                Id = 1,
                DrivingSchoolId = 1,
                PersonId = 1,
                AskedQuestion = "student",
                DateAdded = new DateTime(2026, 3, 3),
                QuestionText = "Some question student",
                Answer = new List<Answers>()
            });
            context.Questionss.AddRange(new Questions
            {
                Id = 2,
                DrivingSchoolId = 1,
                PersonId = 1,
                AskedQuestion = "school",
                DateAdded = new DateTime(2026, 3, 4),
                QuestionText = "Some question school",
                Answer = new List<Answers>()
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var response = await client.DeleteAsync("startDrive/stronaGlowna/forum/1/2");

            context.ChangeTracker.Clear();

            //assert
            var deletedQuestion = await context.Questionss.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            deletedQuestion.Should().BeNull();
        }

        [Fact]
        public async Task DeleteAnswerObject_DeleteAnswer_ReturnNullObject()
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

            context.Answerss.AddRange(new Answers
            {
                Id = 1,
                DrivingSchoolId = 1,
                QuestionsId = 1,
                PersonId = 1,
                WhoReplied = "instructor",
                DateAdded = new DateTime(2026, 3, 6),
                AnswerText = "Some answer instructor"
            });
            context.Answerss.AddRange(new Answers
            {
                Id = 2,
                DrivingSchoolId = 1,
                QuestionsId = 1,
                PersonId = 1,
                WhoReplied = "school",
                DateAdded = new DateTime(2026, 3, 7),
                AnswerText = "Some answer school"
            });
            context.SaveChanges();


            //act
            var client = factory.CreateClient();

            var response = await client.DeleteAsync("startDrive/stronaGlowna/forum/1/1/2");

            context.ChangeTracker.Clear();

            //assert
            var deletedAnswer = await context.Answerss.FirstOrDefaultAsync(u => u.Id == 2);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
            deletedAnswer.Should().BeNull();
        }
    }
}
