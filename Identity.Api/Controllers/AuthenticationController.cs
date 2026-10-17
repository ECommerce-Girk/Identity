using Base.Exceptions;
using Identity.Core.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Controllers;

public class AuthenticationController : ControllerBase
{
    [HttpPost("/login")]
    public async Task<IActionResult> Login(
        [FromServices] Login cmd,
        [FromBody] LoginModel request,
        CancellationToken ct)
    {
        try
        {
            var token = await cmd.Execute(request, ct);
            return Ok(new { Token = token });
        }
        catch (ValidationEx ex) when (ex.Message == "Invalid credentials.")
        {
            return Unauthorized();
        }
        catch (NotFoundEx ex) when (ex.Message == "Invalid credentials.")
        {
            return Unauthorized();
        }
    }
}