
using System.Text;
using Base;
using Base.Database;
using Core.Abstractions;
using Core.Authentication;
using Core.Users;
using Infra.Authentication;
using Infra.Database;
using Infra.Database.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Infra;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;


builder.Services.AddControllers().AddJsonOptions(opts => opts.JsonSerializerOptions.AddValueObjectsJsonConverters());
builder.Services.AddOpenApi(opts =>
{
    opts.AddSchemaTransformer<TextSchemaTransformer>();
});
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


services.AddOutboxDbContext<IdentityDbContext>(builder.Configuration.GetConnectionString("Db")!);

services.AddScoped<UserLogin>();
services.AddScoped<ITokenProvider, TokenProvider>();

services.AddScoped<CreateUser>();
services.AddScoped<IUserRepository, UserRepository>();
services.AddScoped<IPasswordHasher, PasswordHasher>();

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

public sealed class TextSchemaTransformer : IOpenApiSchemaTransformer
{
    public Task TransformAsync(
        OpenApiSchema schema,
        OpenApiSchemaTransformerContext context,
        CancellationToken cancellationToken)
    {
        if (!typeof(Text).IsAssignableFrom(context.JsonTypeInfo.Type))
            return Task.CompletedTask;

        schema.Type = JsonSchemaType.String;
        schema.Properties?.Clear();
        schema.Required?.Clear();

        return Task.CompletedTask;
    }
}