using Api.Models.Requests;
using Api.Models.Responses;
using Core.Users;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
public class UserController : ControllerBase
{
    [HttpPost("api/v1/users")]
    [ProducesResponseType(typeof(CreateUserResponse), 201)]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserRequest request,
        [FromServices] CreateUser cmd,
        CancellationToken ct)
    {
        var cmdInput = new CreateUserModel()
        {
            Email = request.Email,
            Name = request.Name,
            Password = request.Password
        };

        var cmdResult = await cmd.Execute(cmdInput, ct);

        var response = new CreateUserResponse(cmdResult.Id, cmdResult.Email);

        return Created("api/v1/users/" + cmdResult.Id, response);
    }
}
