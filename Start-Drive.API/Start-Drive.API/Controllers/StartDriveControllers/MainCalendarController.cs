using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Start_Drive.API.Identity;
using Start_Drive.API.Models;
using Start_Drive.API.ModelsDto;
using Start_Drive.API.Services.StartDriveServices;
using System.Security.Claims;

namespace Start_Drive.API.Controllers.StartDriveControllers
{
    [ApiController]
    [Route("startDrive/stronaGlowna/kalendarz")]
    public class MainCalendarController : ControllerBase
    {
        private readonly IMainCalendarServices _mainCalendarServices;
        public MainCalendarController(IMainCalendarServices mainCalendarServices)
        {
            _mainCalendarServices = mainCalendarServices;
        }


        [Authorize]
        [RequiresClaim(IdentityData.AdminUserClaimName, "school", "instructor")]
        [HttpGet("{schoolId}")]
        public ActionResult<IEnumerable<StudentsHourDriveDto>> GetInstructorsHoursDrives([FromRoute] int schoolId)
        {
            var cheskUser = User.FindFirstValue("schoolId");
            if (int.Parse(cheskUser) == schoolId)
            {
                var allDrivingSchools = _mainCalendarServices.GetDrivingHoursCalendar(schoolId);
                return Ok(allDrivingSchools);
            }
            else
            {
                return new ForbidResult();
            }
        }

        [Authorize]
        [RequiresClaim(IdentityData.AdminUserClaimName, "school", "student")]
        [HttpGet("{schoolId}/{studentId}")]
        public ActionResult<IEnumerable<StudentsHourDriveDto>> GetDrivingHoursForStudent([FromRoute] int schoolId, [FromRoute] int studentId)
        {
            var cheskUser = User.FindFirstValue("schoolId");
            if (int.Parse(cheskUser) == schoolId)
            {
                var drivingHours = _mainCalendarServices.GetDrivingHoursForStudent(schoolId, studentId);
                return Ok(drivingHours);
            }
            else
            {
                return new ForbidResult();
            }
        }

        [Authorize]
        [RequiresClaim(IdentityData.AdminUserClaimName, "school")]
        [HttpPost("{schoolId}/{instructorId}/{dateAddedDriveString}")]
        public ActionResult AddHoursDrivingStudent([FromRoute] int schoolId, [FromRoute] int instructorId, [FromRoute] string dateAddedDriveString,
            [FromBody] StudentsHourDrive studentHourDrive)
        {
            var cheskUser = User.FindFirstValue("schoolId");
            if (int.Parse(cheskUser) == schoolId)
            {
                var hourDriveStudentId = _mainCalendarServices.AddHourDriveStudent(instructorId, schoolId, dateAddedDriveString, studentHourDrive);
                return Ok(hourDriveStudentId);
            }
            else
            {
                return new ForbidResult();
            }
        }

        [Authorize]
        [RequiresClaim(IdentityData.AdminUserClaimName, "school")]
        [HttpDelete("{schoolId}/{instructorId}/{hourDriveStudentId}")]
        public ActionResult DeleteHoursDriving([FromRoute] int schoolId, [FromRoute] int hourDriveStudentId)
        {
            var cheskUser = User.FindFirstValue("schoolId");
            if (int.Parse(cheskUser) == schoolId)
            {
                _mainCalendarServices.DeleteHourDrive(hourDriveStudentId);
                return Ok();
            }
            else
            {
                return new ForbidResult();
            }
        }
    }
}
