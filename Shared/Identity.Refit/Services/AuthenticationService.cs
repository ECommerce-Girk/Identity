using Base;
using Refit;
using System.Net;
using Identity.Sdk;

namespace Identity.Refit;

public class AuthenticationService (IAuthenticationApi _api) : IAuthenticationService
{
    public async Task<CreateUserResponse> Login(UserLoginRequest request)
    {
        try
        {
            return await _api.Login(request);
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