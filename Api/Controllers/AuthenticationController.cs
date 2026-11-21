using Base;
using Core.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class AuthenticationController : ControllerBase
{
    [HttpPost("api/login")]
    public async Task<IActionResult> Login(
        [FromServices] UserLogin cmd,
        [FromBody] UserLoginModel request,
        CancellationToken ct)
    {
        try
        {
            var token = await cmd.Execute(request, ct);
            return Ok(new { Token = token });
        }
        catch (NotFoundEx)
        {
            return Unauthorized();
        }   
        catch (NotAllowedEx)
        {
            return Unauthorized();
        }
    }
}