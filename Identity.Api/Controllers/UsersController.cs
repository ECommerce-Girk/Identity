using Identity.Core.Users;
using Microsoft.AspNetCore.Mvc;

namespace Identity.Api.Users;

public class UserController : ControllerBase
{
    [HttpPost("/users")]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserModel request,
        [FromServices] CreateUser cmd,
        CancellationToken ct)
    {
        var record = await cmd.Execute(request, ct);
        return Created("/users/" + record.Id, record);
    }
}