using Microsoft.AspNetCore.Mvc;
using Application.Write.Commands;
using Application.Write.CommandHandlers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Identity.UI.Services;
using Application.Write.Contracts;



namespace API.Controllers
{
    [AllowAnonymous]
    [EnableRateLimiting("auth-policy")]
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private AuthHandler _authhandler;

        public AuthController(AuthHandler authHandler, IEmailService emailService)
        {
            _authhandler = authHandler;
        }

        
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            if(command == default || 
            string.IsNullOrWhiteSpace(command.userName) ||
            string.IsNullOrWhiteSpace(command.password))

            return BadRequest("Invalid Request");

           
            var LoginHandleResult = await _authhandler.LoginHandle(command);
            return !LoginHandleResult.IsSuccess ? BadRequest(LoginHandleResult.Error) :
                                                  Ok(LoginHandleResult.Value);   
        }


        [HttpPost("Refresh")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RefreshToken([FromBody] string OldRefreshToken)
        {
            if(string.IsNullOrWhiteSpace(OldRefreshToken)) return BadRequest("Invalid Request");
            
            var RefreshResult = await _authhandler.RefreshUserTokenHandle(OldRefreshToken);
            return !RefreshResult.IsSuccess ? BadRequest("Invalid Request") :
                                              Ok(RefreshResult.Value);
        }


        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(string))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("user/forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] string username)
        {
            if(string.IsNullOrWhiteSpace(username)) return BadRequest("Invalid Request");
            var Result = await _authhandler.ForgotPasswordHandle(username);

            return Result.IsSuccess ? Ok(Result.Value) : Helpers.Result(Result.Error!);
        }

        
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("user/reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            if(command == default) return BadRequest("Invalid Request");
            var Result = await _authhandler.ResetPasswordHandle(command);

            return Result.IsSuccess ? Ok(Result.Value) : Helpers.Result(Result.Error!);
        }

        

        [HttpPost("activate-user")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActivateNewUser([FromBody] ActivateUserCommand command)
        {
            if(command == default) return BadRequest("Invalid Request");
            var Result = await _authhandler.ActivateNewUserHandle(command);
            return Result.IsSuccess ? Ok(Result.Value) : Helpers.Result(Result.Error!);
        }

        [HttpPost("Logout")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Logout([FromBody] string refToken)
        {
            if(refToken == default) return Ok();
            var Result = await _authhandler.LogoutHandle(refToken);
            return Result.IsSuccess ? Ok(Result.Value) : Ok();
        }
    }
}