using Core.Users;
using Core.ValueObjects;
using Microsoft.AspNetCore.Mvc;

namespace Api.Users;

[ApiController]
public class UserController : ControllerBase
{
    [HttpPost("api/v1/users")]
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

public record CreateUserRequest(string Name, Email Email, Password Password);
public record CreateUserResponse(Guid Id, Email Email);