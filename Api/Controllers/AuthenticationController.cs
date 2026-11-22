using Api.Models.Requests;
using Api.Models.Responses;
using Base;
using Core.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class AuthenticationController : ControllerBase
{
    [HttpPost("api/login")]
    [ProducesResponseType(typeof(CreateUserResponse), 201)]
    public async Task<IActionResult> Login(
        [FromServices] UserLogin cmd,
        [FromBody] UserLoginRequest request,
        CancellationToken ct)
    {
        try
        {
            var cmdInput = new UserLoginModel(request.Email, request.Password);
            var token = await cmd.Execute(cmdInput, ct);
            var response = new UserLoginResponse(token);
            return Ok(response);
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