using Application.Write.CommandHandlers;
using Application.Write.Commands;
using Microsoft.AspNetCore.Mvc;
using Read.Providers;

namespace API.Controllers
{
    [Route("api/[controller]")]
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

        #region --- Employee Management (Commands) ---

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] EmployeeAddCommand command)
        {
            if(command == null) return BadRequest("BadRequest");

           var result = await _handler.AddHandle(command);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);
               
            return Ok();
        }

        [HttpPut("{id:int}/personal-info")]
        public async Task<IActionResult> UpdatePersonalInfo(int id, [FromBody] EmployeePersonalInfoUpdateCommand command)
        {
            if(command == default || id <= 0) return BadRequest("BadRequest");

            var result = await _handler.UpdatePersonalInfoHandle(id, command);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);

            return Ok(result.Value);
        }

        [HttpPut("{id:int}/work-info")]
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
        public async Task<IActionResult> AddPhone(int employeeId, [FromQuery] string phoneNumber, [FromQuery] bool primary)
        {
            if(employeeId <= 0 || string.IsNullOrWhiteSpace(phoneNumber)) return BadRequest("BadRequest");
           var result = await _employeeContactsHandler.AddPhoneHandle(employeeId, phoneNumber, primary);
                if (!result.IsSuccess) return Helpers.Result(result.Error!);
                return Ok(result.Value);
        }

        [HttpPut("{employeeId:int}/phones/{id:int}")]
        public async Task<IActionResult> UpdatePhone(int employeeId, int id, [FromQuery] string newPhoneNumber)
        {
            if(employeeId <= 0 || id <= 0 || string.IsNullOrWhiteSpace(newPhoneNumber)) return BadRequest("BadRequest");
            var result = await _employeeContactsHandler.UpdatePhoneHandle(id, employeeId, newPhoneNumber);
                if (!result.IsSuccess) return Helpers.Result(result.Error!);
                return Ok(result.Value);
        }

        [HttpPatch("{employeeId:int}/phones/{id:int}/primary")]
        public async Task<IActionResult> SetPhoneAsPrimary(int employeeId, int id)
        {
             if(employeeId <= 0 || id <= 0) return BadRequest("BadRequest");
             var result = await _employeeContactsHandler.SetPrimaryPhoneHandle(id, employeeId);
                if (!result.IsSuccess) return Helpers.Result(result.Error!);
                return Ok(result.Value);
        }

        [HttpPatch("{employeeId:int}/phones/{id:int}/freeze")]
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
        public async Task<IActionResult> AddEmail(int employeeId, [FromQuery] string emailAddress, [FromQuery] bool primary)
        {
            if(employeeId <= 0 || string.IsNullOrWhiteSpace(emailAddress)) return BadRequest("BadRequest");
             var result = await _employeeContactsHandler.AddEmailHandle(employeeId, emailAddress, primary);
                if (!result.IsSuccess) return Helpers.Result(result.Error!);
                return Ok(result.Value);
        }

        [HttpPut("{employeeId:int}/emails/{id:int}")]
        public async Task<IActionResult> UpdateEmail(int employeeId, int id, [FromQuery] string newEmailAddress)
        {
            if(employeeId <= 0 || id <= 0 || string.IsNullOrWhiteSpace(newEmailAddress)) return BadRequest("BadRequest");
            var result = await _employeeContactsHandler.UpdateEmailHandle(id, employeeId, newEmailAddress);
                if (!result.IsSuccess) return Helpers.Result(result.Error!);
                return Ok(result.Value);
        }

        [HttpPatch("{employeeId:int}/emails/{id:int}/primary")]
        public async Task<IActionResult> SetEmailAsPrimary(int employeeId, int id)
        {
            if(employeeId <= 0 || id <= 0) return BadRequest("BadRequest");
           var result = await _employeeContactsHandler.SetPrimaryEmailHandle(id, employeeId);
                if (!result.IsSuccess) return Helpers.Result(result.Error!);
                return Ok(result.Value);
        }

        [HttpPatch("{employeeId:int}/emails/{id:int}/freeze")]
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
        public async Task<IActionResult> GetPersonalInfo(int id)
        {
            if(id <= 0) return BadRequest("BadRequest");
            var result = await _provider.GetPersonalInfoById(id);
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }

        [HttpGet("personal-info/paged")]
        public async Task<IActionResult> GetPersonalInfoPaged([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if(page <= 0 || size <= 0) return BadRequest("BadRequest");
            var result = await _provider.GetAllPersonalInfo(page, size);
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }

        [HttpGet("personal-info/by-code/{code}")]
        public async Task<IActionResult> GetPersonalInfoByCode(string code)
        {
            if(string.IsNullOrWhiteSpace(code)) return BadRequest("BadRequest");
            var result = await _provider.GetPersonalInfoByCode(code);
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }


        [HttpGet("personal-info/by-NationalNumber/{NationalNumber}")]
        public async Task<IActionResult> GetPersonalInfoByNationalNumber(string NationalNumber)
        {
            if(string.IsNullOrWhiteSpace(NationalNumber)) return BadRequest("BadRequest");
            var result = await _provider.GetPersonalInfoByNationalNumber(NationalNumber);
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }


        [HttpGet("{id:int}/work-info")]
        public async Task<IActionResult> GetWorkInfo(int id)
        {
            if(id <= 0) return BadRequest("BadRequest");
            var result = await _provider.GetWorkInfoById(id);
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }

        [HttpGet("work-info/by-code/{code}")]
        public async Task<IActionResult> GetWorkInfoByCode(string code)
        {
            if(string.IsNullOrWhiteSpace(code)) return BadRequest("BadRequest");
            var result = await _provider.GetWorkInfoByCode(code);
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }


        [HttpGet("work-info/by-NationalNumber/{NationalNumber}")]
        public async Task<IActionResult> GetWorkInfoByNationalNumber(string NationalNumber)
        {
            if(string.IsNullOrWhiteSpace(NationalNumber)) return BadRequest("BadRequest");
            var result = await _provider.GetWorkInfoByNationalNumber(NationalNumber);
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }





        [HttpGet("work-info/paged")]
        public async Task<IActionResult> GetWorkInfoPaged([FromQuery] int page = 1, [FromQuery] int size = 10)
        {
            if(page <= 0 || size <= 0) return BadRequest("BadRequest");
            var result = await _provider.GetAllWorkInfo(page, size);
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }

        [HttpGet("{id:int}/full-profile")]
        public async Task<IActionResult> GetFullProfile(int id)
        {
            if(id <= 0) return BadRequest("BadRequest");
            var result = await _provider.GetFullProfileById(id);
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!);
        }

        #endregion
    }
}