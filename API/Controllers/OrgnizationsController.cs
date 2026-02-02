using Application.Read.Providers;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrgnizrionsController : ControllerBase
    {
        private readonly DepartmentProvider _departmentProvider;
        private readonly JobTitleLevelProvider _jobTitleLevelProvider;
        private readonly NationalityProvider _nationalityProvider;
        private readonly JobGradeProvider _jobGradeProvider;
        private readonly JobTitleProvider _jobTitleProvider;
        private readonly PermissionProvider _permissionProvider;

        public OrgnizrionsController(
            DepartmentProvider departmentProvider,
            JobTitleLevelProvider jobTitleLevelProvider,
            NationalityProvider nationalityProvider,
            JobGradeProvider jobGradeProvider,
            JobTitleProvider jobTitleProvider,
            PermissionProvider permissionProvider)
        {
            _departmentProvider = departmentProvider;
            _jobTitleLevelProvider = jobTitleLevelProvider;
            _nationalityProvider = nationalityProvider;
            _jobGradeProvider = jobGradeProvider;
            _jobTitleProvider = jobTitleProvider;
            _permissionProvider = permissionProvider;
        }

        #region --- Get All Endpoints ---

        [HttpGet("departments")]
        public async Task<IActionResult> GetDepartments()
        {
            var result = await _departmentProvider.GetAll();
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }

        [HttpGet("job-title-levels")]
        public async Task<IActionResult> GetJobTitleLevels()
        {
            var result = await _jobTitleLevelProvider.GetAll();
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }

        [HttpGet("nationalities")]
        public async Task<IActionResult> GetNationalities()
        {
            var result = await _nationalityProvider.GetAll();
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }

        [HttpGet("job-grades")]
        public async Task<IActionResult> GetJobGrades()
        {
            var result = await _jobGradeProvider.GetAll();
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }

        [HttpGet("job-titles")]
        public async Task<IActionResult> GetJobTitles()
        {
            var result = await _jobTitleProvider.GetAll();
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }

        [HttpGet("permissions")]
        public async Task<IActionResult> GetPermissions()
        {
            var result = await _permissionProvider.GetAll();
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }
        #endregion
    }
}