using Core.Users;
using Microsoft.AspNetCore.Mvc;

namespace Api.Users;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateUser(
        [FromBody] CreateUserModel request,
        [FromServices] CreateUser cmd,
        CancellationToken ct)
    {
        var record = await cmd.Execute(request, ct);
        return Created("/users/" + record.Id, record);
    }
}