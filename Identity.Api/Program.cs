
using System.Text;
using Base;
using Base.Database;
using Base.Message;
using Base.Message.Publisher;
using Identity.Api;
using Identity.Core.Abstractions;
using Identity.Core.Authentication;
using Identity.Core.Users;
using Identity.Infra.Authentication;
using Identity.Infra.Database;
using Identity.Infra.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

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


services.AddSingleton<IMessageRegistry, MessageRegistry>();
services.AddDbContext<IdentityDbContext>(o => o.UseNpgsql(builder.Configuration.GetConnectionString("Db")));
services.AddScoped<IUoW, IdentityDbContext>();
services.AddScoped<ITransactionalUoW, IdentityDbContext>();

services.AddScoped<UserLogin>();
services.AddScoped<ITokenProvider, TokenProvider>();

services.AddScoped<CreateUser>();
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IPasswordHasher, PasswordHasher>();

services.Configure<BrokerSettings>(builder.Configuration.GetSection("BrokerSettings"));
services.AddHostedService<MessagePublisher<IdentityDbContext>>();

var app = builder.Build();

app.UseExceptionHandler();
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithTitle("Identity API");
});
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();