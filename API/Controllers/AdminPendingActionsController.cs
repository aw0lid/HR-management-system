using Application.Write.Commands;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Read.Providers;
using Application.Write.CommandHandlers;


namespace API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [EnableRateLimiting("global-user-policy")]
    [ApiController]
    public class AdminPendingActionsController : ControllerBase
    {
        private readonly PendingActionshandler _pendingActionHandler;
        private readonly UserProvider _userProvider;



        public AdminPendingActionsController(PendingActionshandler pendingActionHandler, UserProvider userProvider)
        {
            _pendingActionHandler = pendingActionHandler;
            _userProvider = userProvider;
        }




        [Authorize(Roles = "SystemAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpGet("/PendingActions")]
        public async Task<IActionResult> GetPendingActions()
        {
            var pendingActions = await _userProvider.GetPendingAdminsActions();
            if(!pendingActions.IsSuccess) return NotFound(pendingActions.Error);
            
            return Ok(pendingActions.Value);
        }




        [Authorize(Roles = "SystemAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("/Freeze-Request")]
        public async Task<IActionResult> FreezeAdminRequest([FromBody] AdminFreezeCommand command)
        {
            if(command == default) return BadRequest("Invalid Request");
            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if(currentUserId <= 0) return BadRequest("Invalid Request");
            
            var result = await _pendingActionHandler.FreezAdminRequestHandle(command, currentUserId);
            if(!result.IsSuccess) return BadRequest(result.Error);
            return Ok(true);
        }




        [Authorize(Roles = "SystemAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPost("/Reactivate-Request")]
        public async Task<IActionResult> ReactivateAdminRequest([FromBody] AdminReactivateCommand command)
        {
            if(command == default) return BadRequest("Invalid Request");
            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if(currentUserId <= 0) return BadRequest("Invalid Request");
            var result = await _pendingActionHandler.ReavtivateAdminRequestHandle(command, currentUserId);
            if(!result.IsSuccess) return BadRequest(result.Error);
            return Ok(true);
        }


        [Authorize(Roles = "SystemAdmin")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [HttpPut("/Process-Action")]
        public async Task<IActionResult> ProcessAdminAction([FromBody] ResponseAdminActionCommand command)
        {
            if(command == default) return BadRequest("Invalid Request");
            var currentUserId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
            if(currentUserId <= 0) return BadRequest("Invalid Request");
            
            var result = await _pendingActionHandler.ResponseAdminActionHandle(command, currentUserId);
            if(!result.IsSuccess) return BadRequest(result.Error);
            return Ok(true);
        }
    }
}