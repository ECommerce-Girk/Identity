using System.Threading.Tasks;
using Refit;
using Identity.Sdk;

namespace Identity.Refit;

public interface IUserApi
{
	[Post("api/v1/users")]
	Task<CreateUserResponse> CreateUser([Body] CreateUserRequest request);
}