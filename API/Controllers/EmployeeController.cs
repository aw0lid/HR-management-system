using Application.Write.CommandHandlers;
using Application.Write.Commands;
using Microsoft.AspNetCore.Mvc;
using Read.Providers;
using Microsoft.AspNetCore.Authorization;
using Domain.Entites;
using Read.ViewModels;
using SharedKernal;
using Microsoft.AspNetCore.RateLimiting;


namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [EnableRateLimiting("global-user-policy")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly EmployeeHandler _handler;
        private readonly EmployeeProvider _provider;
        private readonly EmployeeContactsHandler _employeeContactsHandler;

        public EmployeesController(
            EmployeeHandler handler, 
            EmployeeContactsHandler employeeContactsHandler, 
            EmployeeProvider provider)
        {
            _handler = handler;
            _provider = provider;
            _employeeContactsHandler = employeeContactsHandler;
        }



        private async Task<IActionResult> GetEmployee<T>(Func<int, Task<Result<T>>> getAction, int targetId) where T : IEmployeeView
        {
            if (targetId <= 0) return BadRequest("Invalid Request");

            var currentEmpId = int.Parse(User.FindFirst("empId")?.Value ?? "0");
            var userPolicy = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? null;
            bool isHRManager = userPolicy == enRole.EmployeeManagement.ToString();

            if (!isHRManager && currentEmpId != targetId) return Forbid();

            var result = await getAction(targetId);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);

            return Ok(result.Value);
        }

        private async Task<IActionResult> GetEmployee<T>(Func<string, Task<Result<T>>> getAction, string param) where T : IEmployeeView
        {
            if (string.IsNullOrWhiteSpace(param)) return BadRequest("Invalid Request");

            var currentEmpId = int.Parse(User.FindFirst("empId")?.Value ?? "0");
            var userPolicy = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? null;
            bool isHRManager = userPolicy == enRole.EmployeeManagement.ToString();

            var GetResult = await getAction(param);
            if (!isHRManager && currentEmpId != GetResult.Value?.Id) return Forbid();

            if (!GetResult.IsSuccess) return Helpers.Result(GetResult.Error!);
            return Ok(GetResult.Value);
        }




        #region --- Employee Management (Commands) ---

        [HttpPost("Create")]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeAddCommand command)
        {
            if(command == null) return BadRequest("BadRequest");

           var result = await _handler.AddHandle(command);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);
               
            return Ok();
        }


        
        [HttpPut("{id:int}/personal-info")]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePersonalInfo(int id, [FromBody] EmployeePersonalInfoUpdateCommand command)
        {
            if(command == default || id <= 0) return BadRequest("BadRequest");

            var result = await _handler.UpdatePersonalInfoHandle(id, command);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);

            return Ok(result.Value);
        }

        [HttpPut("{id:int}/work-info")]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateWorkInfo(int id, [FromBody] EmployeeWorkInfoUpdateCommand command)
        {
            if(command == default || id <= 0 || (!command.DepartmentId.HasValue && !command.JobTitleLevelId.HasValue && !command.ManagerId.HasValue)) return BadRequest("BadRequest");
             var result = await _handler.UpdateWorkInfoHandle(id, command);
             if (!result.IsSuccess) return Helpers.Result(result.Error!);
             return Ok(result.Value);
        }

        #endregion

        #region --- Contact Details (Phones) ---

        [HttpPost("{employeeId:int}/phones")]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddPhone(int employeeId, [FromQuery] string phoneNumber, [FromQuery] bool primary)
        {
            if(employeeId <= 0 || string.IsNullOrWhiteSpace(phoneNumber)) return BadRequest("BadRequest");
           var result = await _employeeContactsHandler.AddPhoneHandle(employeeId, phoneNumber, primary);
                if (!result.IsSuccess) return Helpers.Result(result.Error!);
                return Ok(result.Value);
        }

        [HttpPut("{employeeId:int}/phones/{id:int}")]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdatePhone(int employeeId, int id, [FromQuery] string newPhoneNumber)
        {
            if(employeeId <= 0 || id <= 0 || string.IsNullOrWhiteSpace(newPhoneNumber)) return BadRequest("BadRequest");
            var result = await _employeeContactsHandler.UpdatePhoneHandle(id, employeeId, newPhoneNumber);
                if (!result.IsSuccess) return Helpers.Result(result.Error!);
                return Ok(result.Value);
        }

        [HttpPatch("{employeeId:int}/phones/{id:int}/primary")]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetPhoneAsPrimary(int employeeId, int id)
        {
             if(employeeId <= 0 || id <= 0) return BadRequest("BadRequest");
             var result = await _employeeContactsHandler.SetPrimaryPhoneHandle(id, employeeId);
                if (!result.IsSuccess) return Helpers.Result(result.Error!);
                return Ok(result.Value);
        }

        [HttpPatch("{employeeId:int}/phones/{id:int}/freeze")]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FreezePhone(int employeeId, int id)
        {
            if(employeeId <= 0 || id <= 0) return BadRequest("BadRequest");
           var result = await _employeeContactsHandler.FreezePhoneHandle(id, employeeId);
                if (!result.IsSuccess) return Helpers.Result(result.Error!);
                return Ok(result.Value);
        }

        #endregion

        #region --- Contact Details (Emails) ---

        [HttpPost("{employeeId:int}/emails")]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddEmail(int employeeId, [FromQuery] string emailAddress, [FromQuery] bool primary)
        {
            if(employeeId <= 0 || string.IsNullOrWhiteSpace(emailAddress)) return BadRequest("BadRequest");
             var result = await _employeeContactsHandler.AddEmailHandle(employeeId, emailAddress, primary);
                if (!result.IsSuccess) return Helpers.Result(result.Error!);
                return Ok(result.Value);
        }

        [HttpPut("{employeeId:int}/emails/{id:int}")]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateEmail(int employeeId, int id, [FromQuery] string newEmailAddress)
        {
            if(employeeId <= 0 || id <= 0 || string.IsNullOrWhiteSpace(newEmailAddress)) return BadRequest("BadRequest");
            var result = await _employeeContactsHandler.UpdateEmailHandle(id, employeeId, newEmailAddress);
                if (!result.IsSuccess) return Helpers.Result(result.Error!);
                return Ok(result.Value);
        }

        [HttpPatch("{employeeId:int}/emails/{id:int}/primary")]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SetEmailAsPrimary(int employeeId, int id)
        {
            if(employeeId <= 0 || id <= 0) return BadRequest("BadRequest");
           var result = await _employeeContactsHandler.SetPrimaryEmailHandle(id, employeeId);
                if (!result.IsSuccess) return Helpers.Result(result.Error!);
                return Ok(result.Value);
        }

        [HttpPatch("{employeeId:int}/emails/{id:int}/freeze")]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FreezeEmail(int employeeId, int id)
        {
            if(employeeId <= 0 || id <= 0) return BadRequest("BadRequest");
            var result = await _employeeContactsHandler.FreezeEmailHandle(id, employeeId);
                if (!result.IsSuccess) return Helpers.Result(result.Error!);
                return Ok(result.Value);
        }

        #endregion

        #region --- Information Queries (Read) ---

        [HttpGet("{id:int}")]
        [ActionName(nameof(GetPersonalInfo))]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeePersonalInfoView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPersonalInfo(int id)
            => await GetEmployee(_provider.GetPersonalInfoById, id);

        [HttpGet("personal-info/paged")]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EmployeePersonalInfoView>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPersonalInfoPaged([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if(page <= 0 || size <= 0) return BadRequest("BadRequest");
            var result = await _provider.GetAllPersonalInfo(page, size);
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }
        
        
        [HttpGet("personal-info/by-code/{code}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeePersonalInfoView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPersonalInfoByCode(string code)
            => await GetEmployee(_provider.GetPersonalInfoByCode, code);


        [HttpGet("personal-info/by-NationalNumber/{NationalNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeePersonalInfoView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPersonalInfoByNationalNumber(string NationalNumber)
            => await GetEmployee(_provider.GetPersonalInfoByNationalNumber, NationalNumber);


        [HttpGet("{id:int}/work-info")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeeWorkInfoView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetWorkInfo(int id)
            => await GetEmployee(_provider.GetWorkInfoById, id);
        

        [HttpGet("work-info/by-code/{code}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeeWorkInfoView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetWorkInfoByCode(string code)
            => await GetEmployee(_provider.GetWorkInfoByCode, code);
        


        [HttpGet("work-info/by-NationalNumber/{NationalNumber}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeeWorkInfoView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetWorkInfoByNationalNumber(string NationalNumber)
            => await GetEmployee(_provider.GetWorkInfoByNationalNumber, NationalNumber);





        [HttpGet("work-info/paged")]
        [Authorize(Roles = "HRManagement")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EmployeeWorkInfoView>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetWorkInfoPaged([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if(page <= 0 || size <= 0) return BadRequest("BadRequest");
            var result = await _provider.GetAllWorkInfo(page, size);
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }

        [HttpGet("{id:int}/full-profile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EmployeeFullProfileView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFullProfile(int id)
            => await GetEmployee(_provider.GetFullProfileById, id);

        #endregion
    }
}