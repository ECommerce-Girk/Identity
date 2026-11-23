using System.Threading.Tasks;
using Refit;
using Identity.Sdk;

namespace Identity.Refit;

public interface IAuthenticationApi
{
	[Post("api/login")]
	Task<CreateUserResponse> Login([Body] UserLoginRequest request);
}