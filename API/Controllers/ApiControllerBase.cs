using Microsoft.AspNetCore.Mvc;
using SharedKernal;

public static class Helpers
{
    
    public static IActionResult Result(Error error)
    {
        return error.Type switch
        {
            enErrorType.Validation => new BadRequestObjectResult(error),
            enErrorType.NotFound => new NotFoundObjectResult(error),
            enErrorType.Conflict => new ConflictObjectResult(error),
            enErrorType.Failure => new BadRequestObjectResult(error),
            _ => new ObjectResult(new { Message = "Unexpected Error" })
        };
    }
}