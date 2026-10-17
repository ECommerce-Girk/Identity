using System.Net;
using System.Net.Http.Json;
using Identity.Core.Authentication;
using Identity.Core.Users;

namespace Identity.Tests;

public class AuthenticationTests(CustomWebApplicationFactory factory) : TestBase(factory)
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOK()
    {
        // Arrange
        var user = new CreateUserModel
        { 
            Name = "Test User", 
            Email = "test@example.com",
            Password = "password123"
        };

        await Client.PostAsJsonAsync("/users", user);

        var loginRequest = new LoginModel(user.Email, user.Password);

        // Act
        var response = await Client.PostAsJsonAsync("/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ReturnsUnauthorized()
    {
        // Arrange
        var user = new User
        { 
            Name = "Test User", 
            Email = "test@example.com",
            PasswordHash = "password123"
        };

        await Client.PostAsJsonAsync("/users", user);

        var loginRequest = new LoginModel(user.Email, "wrongpassword");

        // Act
        var response = await Client.PostAsJsonAsync("/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithNonExistentUser_ReturnsUnauthorized()
    {
        //Arrange
        var loginRequest = new LoginModel("nonexistent@example.com", "password123");

        // Act
        var response = await Client.PostAsJsonAsync("/login", loginRequest);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}