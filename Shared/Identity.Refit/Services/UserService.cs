using Base;
using Refit;
using System.Net;
using Identity.Sdk;

namespace Identity.Refit;

public class UserService (IUserApi _api) : IUserService
{
    public async Task<CreateUserResponse> CreateUser(CreateUserRequest request)
    {
        try
        {
            return await _api.CreateUser(request);
        }
        catch (ApiException ex)
        {
            HandleException(ex);
            throw;
        }
    }

    private void HandleException(ApiException ex)
    {
        if (ex.StatusCode == HttpStatusCode.NotFound)
            throw new NotFoundEx(ex.Message);

        if (ex.StatusCode == HttpStatusCode.BadRequest)
            throw new ValidationEx(ex.Message);

        throw new Ex(ex.Message);
    }
}