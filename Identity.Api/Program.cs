
using System.Text;
using Base;
using Identity.Core.Abstractions;
using Identity.Core.Authentication;
using Identity.Core.Users;
using Identity.Infra.Authentication;
using Identity.Infra.Database;
using Identity.Infra.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;

services.AddControllers();
services.AddOpenApi();
services.AddProblemDetails();
//services.AddExceptionHandler<CustomExceptionHandler>();
services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"]!))
        };
    });


services.AddDbContext<IdentityDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("Db")));
services.AddScoped<IUnitOfWork, IdentityDbContext>();

services.AddScoped<Login>();
services.AddScoped<ITokenProvider, TokenProvider>();

services.AddScoped<CreateUser>();
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IPasswordHasher, PasswordHasher>();

var app = builder.Build();

app.UseExceptionHandler();
app.MapOpenApi();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();