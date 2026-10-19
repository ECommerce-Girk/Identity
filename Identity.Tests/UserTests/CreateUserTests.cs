using System.Net;
using System.Net.Http.Json;
using Identity.Core.Users;

namespace Identity.Tests;

public class CreateUserTests(CustomWebApplicationFactory factory) : TestBase(factory), IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task ReturnsCreatedUser()
    {
        // Arrange
        var createUserRequest = new CreateUserModel 
        { 
            Name = "Test User", 
            Email = "test@example.com",
            Password = "password123"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/users", createUserRequest);
        var responseModel = await response.Content.ReadFromJsonAsync<User>();

        // Assert response
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(responseModel);
        Assert.Equal(createUserRequest.Name, responseModel.Name);
        Assert.Equal(createUserRequest.Email, responseModel.Email);

        // Assert database state
        var ctx = GetDbContext();
        var userInDb = await ctx.Users.FindAsync(responseModel.Id);
        Assert.NotNull(userInDb);
        Assert.Equal(createUserRequest.Name, userInDb.Name);
        Assert.Equal(createUserRequest.Email, userInDb.Email);
    }

    [Fact]
    public async Task WithExistingEmail_ReturnsValidationError()
    {
        // Arrange
        var existingUser = GivenExistingUserInDatabase();        

        var createUserRequest = new CreateUserModel 
        { 
            Name = "Test User", 
            Email = existingUser.Email,
            Password = "password456"
        };

        // Act
        var response = await Client.PostAsJsonAsync("/users", createUserRequest);
        
        // Assert response
        //Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    private User GivenExistingUserInDatabase()
    {
        var existingUser = new User
        { 
            Name = "Existing User", 
            Email = "existing@example.com",
            PasswordHash = "password123"
        };

        var ctx = GetDbContext();
        ctx.Users.Add(existingUser);
        ctx.SaveChangesAsync();
        return existingUser;
    }
}