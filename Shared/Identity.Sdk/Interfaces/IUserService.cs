using System.Threading.Tasks;

namespace Identity.Sdk;

public interface IUserService
{
	Task<CreateUserResponse> CreateUser(CreateUserRequest request);
}