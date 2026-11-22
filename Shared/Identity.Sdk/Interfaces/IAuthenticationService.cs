using System.Threading.Tasks;

namespace Identity.Sdk;

public interface IAuthenticationService
{
	Task<CreateUserResponse> Login(UserLoginRequest request);
}