using System.Net;
using System.Net.Http.Json;
using Identity.Core.Authentication;
using Identity.Core.Users;

namespace Identity.Tests;

public class AuthTests(CustomWebApplicationFactory factory) : TestBase(factory)
{
    [Fact]
    public async Task Login_WithValidCredentials_ReturnsOK()
    {
        // This test just verifies the endpoint exists and handles valid requests correctly
        // The actual token generation is tested through integration tests
        
        // Arrange - create a user
        var user = new User
        { 
            Name = "Test User", 
            Email = "test@example.com",
            PasswordHash = "password123"
        };

        var ctx = GetDbContext();
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        var loginRequest = new LoginModel(user.Email, user.PasswordHash);

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

        var ctx = GetDbContext();
        ctx.Users.Add(user);
        await ctx.SaveChangesAsync();

        // Act
            var loginRequest = new LoginModel(user.Email, "wrongpassword");

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