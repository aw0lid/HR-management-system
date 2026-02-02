using Application.Write.CommandHandlers;
using Application.Write.Commands;
using Microsoft.AspNetCore.Mvc;
using Read.Providers;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserHandler _handler;
        private readonly UserProvider _provider;


        public UserController(UserHandler handler, UserProvider provider)
        {
            _handler = handler;
            _provider = provider;
        }


        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserAddCommand command)
        {
            if(command == default) return BadRequest("BadRequest");
           var result = await _handler.AddHandle(command);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);
               
            return Ok();
        }

        [HttpPut("{id:int}/update")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateCommand command)
        {
            if(command == default || id <= 0) return BadRequest("BadRequest");
            var result = await _handler.UpdateHandle(id, command);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);

            return Ok(result.Value);
        }

        [HttpPut("{id:int}/Active")]
        public async Task<IActionResult> Activate(int id)
        {
            if(id <= 0) return BadRequest("BadRequest");
            var result = await _handler.ActivationHandle(id, true);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);

            return Ok(result.Value);
        }

        [HttpPut("{id:int}/Freeze")]
        public async Task<IActionResult> Freeze(int id)
        {
            if(id <= 0) return BadRequest("BadRequest");
            var result = await _handler.ActivationHandle(id, false);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);

            return Ok(result.Value);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            if (command == null) return BadRequest("Invalid login request");
            
            var result = await _handler.LoginHandle(command);
            
            if (!result.IsSuccess) return Helpers.Result(result.Error!);

            return Ok(result.Value);
        }




        [HttpGet("Id/{Id}/User")]
        public async Task<IActionResult> GetUserById(int Id)
        {
            if(Id <= 0) return BadRequest("BadRequest");
            var result = await _provider.GetUserById(Id);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);

            return Ok(result.Value);
        }

        [HttpGet("Code/{Code}/User")]
        public async Task<IActionResult> GetUserByCode(string Code)
        {
            if(string.IsNullOrWhiteSpace(Code)) return BadRequest("BadRequest");
            var result = await _provider.GetUserByCode(Code);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);

            return Ok(result.Value);
        }


        [HttpGet("Username/{username}/User")]
        public async Task<IActionResult> GetUserByUserName(string username)
        {
            if(string.IsNullOrWhiteSpace(username)) return BadRequest("BadRequest");
            var result = await _provider.GetUserByUserName(username);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);

            return Ok(result.Value);
        }

        [HttpGet("NationalNumber/{nationalNumber}/User")]
        public async Task<IActionResult> GetUserByNationalNumber(string nationalNumber)
        {
            if(string.IsNullOrWhiteSpace(nationalNumber)) return BadRequest("BadRequest");
            var result = await _provider.GetUserByNationalNumber(nationalNumber);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);

            return Ok(result.Value);
        }


        [HttpGet("Users/")]
        public async Task<IActionResult> GetAllUsers(int page, int size)
        {
            if(page <= 0 || size <= 0) return BadRequest("BadRequest");
            var result = await _provider.GetAllUsers(page, size);
            if (!result.IsSuccess) return Helpers.Result(result.Error!);

            return Ok(result.Value);
        }
    }
}