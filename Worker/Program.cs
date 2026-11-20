
using Base.Database;
using Base.Message;
using Infra.Database;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);
var services = builder.Services;
services.AddMessagePublisher<MessageRegistry>(builder.Configuration);

services.AddOutboxDbContext<IdentityDbContext>(builder.Configuration.GetConnectionString("Db")!);

var host = builder.Build();

host.Run();
