using AutoMapper;
using Start_Drive.API.Data;
using Start_Drive.API.Models;
using Start_Drive.API.ModelsDto;

namespace Start_Drive.API.Services
{
    public interface IRegisterSchoolService
    {
        public int CreateDrivingSchool(RegisterSchool reg);
        public int CreateSchoolAccount(RegisterSchool register);
        public int aDrivingSchoolInitialValue(int schoolId);
        public int OpenCloseInitialValue(int schoolId);
        public RegisterSchoolDto GetSchoolInformation(int schoolId);
        public void UpdateSchool(int schoolId, RegisterSchool editDataSchool);
        public void DeleteSchool(int schoolId);

        //----------------------------------------------------------- Register Instructor -------------------------------------------------------
        public int CreateInstructorAccount(RegisterStudentInstructor registerInst);

        //----------------------------------------------------------- Register Student -------------------------------------------------------
        public int CreateStudentAccount(RegisterStudentInstructor registerStud);
    }
    public class RegisterSchoolService : IRegisterSchoolService
    {
        private readonly StartDriveDbContext _context;
        private readonly IMapper _mapper;
        public RegisterSchoolService(StartDriveDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public int CreateDrivingSchool(RegisterSchool reg)
        {
            var schoolId = CreateSchoolAccount(reg);
            var aDrivingId = aDrivingSchoolInitialValue(schoolId);
            var openCloseFunction = OpenCloseInitialValue(schoolId);

            return schoolId;
        }

        public int CreateSchoolAccount(RegisterSchool register)
        {
            var hashPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(register.Password, 13);

            var school = register;
            school.Password = hashPassword;
            school.BreakBetweenRides = 0.1;

            _context.Add(school);
            _context.SaveChanges();

            return school.Id;
        }

        public int aDrivingSchoolInitialValue(int schoolId)
        {
            var aboutDrivingSchool = new ADrivingSchool()
            { 
                DrivingSchoolId = schoolId,
                AboutText = "Dodaj opis szkole jazdy!" 
            };

            aboutDrivingSchool.DrivingSchoolId = schoolId;

            _context.Add(aboutDrivingSchool);
            _context.SaveChanges();

            return aboutDrivingSchool.Id;
        }

        public int OpenCloseInitialValue(int schoolId)
        {
            for (int i = 0; i < 7; i++)
            {
                var openCloseObj = new OpenClose()
                {
                    DrivingSchoolId = schoolId,
                    DayOfTheWeek = i,
                    IsOpen = true,
                    FirstHour = 7,
                    LastHour = 15
                };

                openCloseObj.DrivingSchoolId = schoolId;
                openCloseObj.DayOfTheWeek = i;
                openCloseObj.IsOpen = true;
                openCloseObj.FirstHour = 7;
                openCloseObj.LastHour = 15;


                _context.Add(openCloseObj);
            }
            _context.SaveChanges();

            return schoolId;
        }

        public RegisterSchoolDto GetSchoolInformation(int schoolId)
        {
            var schoolInfo = _context.RegisterSchools.First(r => r.Id == schoolId);

            if(schoolInfo != null)
            {
                var schoolInfoDto = _mapper.Map<RegisterSchoolDto>(schoolInfo);
                return schoolInfoDto;
            }
            else
            {
                return null;
            }
        }

        public void UpdateSchool(int schoolId, RegisterSchool editDataSchool)
        {
            var school = _context.RegisterSchools.FirstOrDefault(r => r.Id == schoolId);

            if (school is null)
            {
                throw new DirectoryNotFoundException("School not found");
            }

            school.Name = editDataSchool.Name;
            school.Address = editDataSchool.Address;
            school.City = editDataSchool.City;
            school.PhoneNumber = editDataSchool.PhoneNumber;
            school.AccountNumber = editDataSchool.AccountNumber;
            school.Description = editDataSchool.Description;
            school.Email = editDataSchool.Email;

            if (editDataSchool.Password != "")
            {
                var hashPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(editDataSchool.Password, 13);
                school.Password = hashPassword;
            }

            _context.SaveChanges();
        }

        public void DeleteSchool(int schoolId)
        {
            var schoolD = _context.RegisterSchools.FirstOrDefault(r => r.Id == schoolId);

            if (schoolD is null)
            {
                throw new DirectoryNotFoundException("School not found");
            }

            _context.RegisterSchools.Remove(schoolD);
            _context.SaveChanges();
        }


        //----------------------------------------------------------- Register Instructor -------------------------------------------------------

        public int CreateInstructorAccount(RegisterStudentInstructor registerInst)
        {
            var school = _context.RegisterSchools.FirstOrDefault(r => r.Email == registerInst.DrivingSchoolEmail);
            if(school is null)
            {
                return -1;
            }

            var instructor = _context.Instructors.FirstOrDefault(i => i.DrivingSchoolId == school.Id && i.Email == registerInst.PersonEmail);
            if(instructor is null)
            {
                return -2;
            }

            var registerCode = _context.GeneratedRegistrationCodes.FirstOrDefault(g => g.DrivingSchoolId == school.Id && g.PersonId == instructor.Id);
            if(registerCode is null)
            {
                return -3;
            }

            if (BCrypt.Net.BCrypt.EnhancedVerify(registerInst.GeneratedCode, registerCode.GeneratedCode))
            {
                var hashPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(registerInst.Password, 13);
                instructor.Password = hashPassword;
                _context.SaveChanges();
                _context.GeneratedRegistrationCodes.Remove(registerCode);
                _context.SaveChanges();
                return school.Id;
            }
            else
            {
                return -4;
            }
        }


        //----------------------------------------------------------- Register Student -------------------------------------------------------

        public int CreateStudentAccount(RegisterStudentInstructor registerStud)
        {
            var school = _context.RegisterSchools.FirstOrDefault(r => r.Email == registerStud.DrivingSchoolEmail);
            if (school is null)
            {
                return -1;
            }

            var student = _context.Students.FirstOrDefault(i => i.DrivingSchoolId == school.Id && i.Email == registerStud.PersonEmail);
            if (student is null)
            {
                return -2;
            }

            var registerCode = _context.GeneratedRegistrationCodes.FirstOrDefault(g => g.DrivingSchoolId == school.Id && g.PersonId == student.Id);
            if (registerCode is null)
            {
                return -3;
            }

            if (BCrypt.Net.BCrypt.EnhancedVerify(registerStud.GeneratedCode, registerCode.GeneratedCode))
            {
                var hashPassword = BCrypt.Net.BCrypt.EnhancedHashPassword(registerStud.Password, 13);
                student.Password = hashPassword;
                _context.SaveChanges();
                _context.GeneratedRegistrationCodes.Remove(registerCode);
                _context.SaveChanges();
                return school.Id;
            }
            else
            {
                return -4;
            }
        }
    }
}
