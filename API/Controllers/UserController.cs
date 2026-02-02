using Application.Write.CommandHandlers;
using Application.Write.Commands;
using Microsoft.AspNetCore.Mvc;
using Read.Providers;
using Microsoft.AspNetCore.Authorization;
using Domain.Entites;
using SharedKernal;
using Read.ViewModels;
using Microsoft.AspNetCore.RateLimiting;


namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [EnableRateLimiting("global-user-policy")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserHandler _handler;
        private readonly UserProvider _provider;


        public UsersController(UserHandler handler, UserProvider provider)
        {
            _handler = handler;
            _provider = provider;
        }



        private async Task<IActionResult> GetUser(Func<string, Task<Result<UserView>>> getAction, string Param)
        {
            if (string.IsNullOrWhiteSpace(Param)) return BadRequest("BadRequest");

            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userPolicy = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? null;
            bool isAdmin = userPolicy == enRole.SystemAdmin.ToString();

            var result = await getAction(Param); 
            var targetUser = result.Value;

            if (isAdmin || (targetUser?.Id == currentUserId))
                return targetUser == default ? Helpers.Result(result.Error!) : Ok(targetUser);
            
            return Forbid();
        }



        [HttpPost("/Create/User")]
        [Authorize(Roles = "SystemAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateUser([FromBody] UserAddCommand command)
        {
            if(command == default) return BadRequest("BadRequest");
            var result = await _handler.AddHandle(command);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);
               
            return Ok(result.Value);
        }

        [HttpPut("/Change-username")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeUserName([FromBody] UserUpdateCommand command)
        {
            if(command == default || string.IsNullOrWhiteSpace(command.userName)) return BadRequest("BadRequest");
            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            
            if(currentUserId <= 0) return BadRequest();

            var result = await _handler.ChangeUsernameHandle(command.userName, currentUserId);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);

            return Ok(result.Value);
        }



        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            if(command == default) return BadRequest("Invalid Request");
            if(command.NewPassword == command.OldPassword) return BadRequest("New and Old are same");
            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if(currentUserId <= 0) return BadRequest("Invalid Request");
            

            var Result = await _handler.ChangePasswordHandle(command, currentUserId);
            return Result.IsSuccess ? Ok(Result.Value) : Helpers.Result(Result.Error!);
        }

        

        [HttpPut("{id:int}/Active")]
        [Authorize(Roles = "SystemAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Activate(int id)
        {
            if(id <= 0) return BadRequest("BadRequest");
            var result = await _handler.ActivateHandle(id);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);

            return Ok(result.Value);
        }



        [HttpPut("{id:int}/Freeze")]
        [Authorize(Roles = "SystemAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Freeze(int id)
        {
            if(id <= 0) return BadRequest("BadRequest");
            var result = await _handler.FreezeHandle(id);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);

            return Ok(result.Value);
        }



        

        [HttpPut("{id:int}/ChangeToHRManager")]
        [Authorize(Roles = "SystemAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ChangeToHRManager(int id)
        {
            if(id <= 0) return BadRequest("BadRequest");
            var result = await _handler.ChangeToEmployeeManagmentHandle(id);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);

            return Ok(result.Value);
        }






        [HttpGet("Id/{Id}/User")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserById(int Id)
        {
            if (Id <= 0) return BadRequest("BadRequest");

            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            var userPolicy = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value ?? null;
            bool isAdmin = userPolicy == enRole.SystemAdmin.ToString();

            if (!isAdmin && currentUserId != Id) return Forbid();
           
            var result = await _provider.GetUserById(Id);
            return result.IsSuccess ? Ok(result.Value) : Helpers.Result(result.Error!); 
        }

        [HttpGet("Code/{Code}/User")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserByCode(string Code)
            => await GetUser(_provider.GetUserByCode, Code);


        [HttpGet("Username/{username}/User")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserByUserName(string username)
            => await GetUser(_provider.GetUserByUserName, username);
        

        
        [HttpGet("NationalNumber/{nationalNumber}/User")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserView))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserByNationalNumber(string nationalNumber)
            => await GetUser(_provider.GetUserByNationalNumber, nationalNumber);
       


        [HttpGet]
        [Authorize(Roles = "SystemAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UserView>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllUsers(int page = 1, int size = 10)
        {
            if(page <= 0 || size <= 0) return BadRequest("BadRequest");
            var result = await _provider.GetAllUsers(page, size);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);

            return Ok(result.Value);
        }
    }
}