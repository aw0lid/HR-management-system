using Application.Read.Providers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Application.Read.ViewModels;
using Microsoft.AspNetCore.RateLimiting;


namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [EnableRateLimiting("global-user-policy")]
    [ApiController]
    public class OrgnizrionsController : ControllerBase
    {
        private readonly DepartmentProvider _departmentProvider;
        private readonly JobTitleLevelProvider _jobTitleLevelProvider;
        private readonly NationalityProvider _nationalityProvider;
        private readonly JobGradeProvider _jobGradeProvider;
        private readonly JobTitleProvider _jobTitleProvider;

        public OrgnizrionsController(
            DepartmentProvider departmentProvider,
            JobTitleLevelProvider jobTitleLevelProvider,
            NationalityProvider nationalityProvider,
            JobGradeProvider jobGradeProvider,
            JobTitleProvider jobTitleProvider)
        {
            _departmentProvider = departmentProvider;
            _jobTitleLevelProvider = jobTitleLevelProvider;
            _nationalityProvider = nationalityProvider;
            _jobGradeProvider = jobGradeProvider;
            _jobTitleProvider = jobTitleProvider;
        }

        #region --- Get All Endpoints ---

        
        [HttpGet("departments")]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<DepartmentView>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDepartments()
        {
            var result = await _departmentProvider.GetAll();
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }

        [HttpGet("job-title-levels")]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<JobTitleLevelView>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetJobTitleLevels()
        {
            var result = await _jobTitleLevelProvider.GetAll();
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }

        [HttpGet("nationalities")]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<NationalityView>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetNationalities()
        {
            var result = await _nationalityProvider.GetAll();
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }

        [HttpGet("job-grades")]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<JobGradeView>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetJobGrades()
        {
            var result = await _jobGradeProvider.GetAll();
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }

        [HttpGet("job-titles")]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<JobTitleView>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetJobTitles()
        {
            var result = await _jobTitleProvider.GetAll();
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }
        #endregion
    }
}