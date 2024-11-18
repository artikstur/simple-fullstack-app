using API.Extensions;
using Application.Interfaces.Auth;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Services;
using Core.Enums;
using Infrastructure.Auth;
using Microsoft.EntityFrameworkCore;
using Persistence;
using Persistence.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

builder.Configuration
    .AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true);

var configuration = builder.Configuration;
var services = builder.Services;

services.AddControllers();
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();

services.Configure<JwtOptions>(configuration.GetSection(nameof(JwtOptions)));
services.Configure<AuthorizationOptions>(configuration.GetSection(nameof(AuthorizationOptions)));
services.AddApiAuthentication(configuration);

services.AddDbContext<MedDbContext>(options =>
{
    options.UseNpgsql(configuration.GetConnectionString(nameof(MedDbContext)));
});

services.AddAuthorizationPolicy("RequireAdmin", new[] { Permission.Delete });

services.AddScoped<IJwtProvider, JwtProvider>();
services.AddScoped<IPasswordHasher, PasswordHasher>();
services.AddScoped<IUsersRepository, UsersRepository>();

services.AddScoped<IUsersService, UsersService>();
services.AddScoped<ErrorResponseFactory>();
services.AddAutoMapper(typeof(DataBaseMappings));

services.AddValidators();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

public partial class Program { }