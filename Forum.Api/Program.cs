#region Using statements
using Amazon.S3;
using Amazon.SimpleEmail;
using FluentValidation;
using Forum.Api.Filters;
using Forum.Api.Middlewares;
using Forum.Application.AutoMapper;
using Forum.Application.CommandHandlers.IdentityHandlers;
using Forum.Application.CommandHandlers.ProfileHandlers;
using Forum.Application.Identity;
using Forum.Application.Interfaces;
using Forum.Application.Repositories;
using Forum.Domain.Entities.FeedEntities;
using Forum.Domain.Entities.User;
using Forum.Domain.Models;
using Forum.Domain.Validators.FeedValidators;
using Forum.Infrastructure.Configuration;
using Forum.Infrastructure.DbContext;
using Forum.Infrastructure.Email;
using Forum.Infrastructure.EventHandlers;
using Forum.Infrastructure.FileStorage;
using Forum.Infrastructure.Identity;
using Forum.Infrastructure.Logging;
using Forum.Infrastructure.Migrations;
using Forum.Infrastructure.Notifications;
using Forum.Infrastructure.Persistence;
using Forum.Shared.EventBus;
using Forum.Shared.Events;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using StackExchange.Redis;
using System.Text;
#endregion

#region Program.cs

var builder = WebApplication.CreateBuilder(args);
Serilogger.ConfigureLogging();

builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonS3>();
builder.Services.AddScoped<IS3FileStorageService, S3FileStorageService>();
builder.Services.Configure<ApiCredentials>(builder.Configuration);

builder.Services.AddAWSService<IAmazonSimpleEmailService>();
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));

builder.Services.AddSingleton<IEventBus, EventBus>();

builder.Services.AddTransient<IMailService, MailService>();
builder.Services.AddTransient<EmailNotificationService>();

builder.Services.AddTransient<UserForgotPasswordEventHandler>();
builder.Services.AddTransient<UserRegisterEventHandler>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddOpenApi();

builder.Services.AddScoped<ModelValidationFilterAttribute>();
builder.Services.AddScoped<ExceptionHandlingFilterAttribute>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

builder.Services.AddSignalR();

builder.Services.AddScoped<JwtTokenValidationFilter>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IFeedRepository, FeedRepository>();
builder.Services.AddScoped<UserActivityService>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<IValidator<CategoryEntity>, CategoryValidator>();
builder.Services.AddAuthorization();
builder.Services.AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<MemberDbContext>()
        .AddDefaultTokenProviders();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateUserCommandHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ResetPasswordCommandHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ForgotPasswordCommandHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UpdateProfileCommandHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LoginCommandHandler).Assembly));

builder.Services.AddScoped<IPasswordHasher<UserAggregate>, PasswordHasher<UserAggregate>>();

builder.Services.AddSingleton<IJwtBlacklistService, JwtBlacklistService>();

builder.Services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
{
    try
    {
        return ConnectionMultiplexer.Connect("localhost:6379");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Redis baðlantýsý kurulamadý: {ex.Message}");
        return null;
    }
});

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "localhost:6379";
    options.InstanceName = "ForumApp";
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ModelValidationFilterAttribute>();
    options.Filters.Add<ExceptionHandlingFilterAttribute>();
});

builder.Host.ConfigureAppConfiguration((context, configurationBuilder) =>
{
    configurationBuilder.AddAmazonSecretsManager("eu-north-1", "prod/Forum/AppSettings");
});

var profileConnectionString = builder.Configuration.GetConnectionString("ProfileDbConnection");
var identityConnectionString = builder.Configuration.GetConnectionString("IdentityDbConnection");

if (string.IsNullOrEmpty(identityConnectionString) || string.IsNullOrEmpty(profileConnectionString))
{
    throw new Exception("Database connection string is not found.");
}

builder.Services.AddDbContext<ProfileDbContext>(options =>
    options.UseNpgsql(profileConnectionString));

builder.Services.AddDbContext<MemberDbContext>(options =>
    options.UseNpgsql(identityConnectionString));

builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDbSettings"));

builder.Services.AddSingleton<FeedDbContext>();

var jwtSection = builder.Configuration.GetSection("Jwt");
var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            IssuerSigningKey = issuerSigningKey,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"]
        };
    });

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

var app = builder.Build();

var eventBus = app.Services.GetRequiredService<IEventBus>();
var userForgotPasswordEventHandler = app.Services.GetRequiredService<UserForgotPasswordEventHandler>();
var userRegisteredEventHandler = app.Services.GetRequiredService<UserRegisterEventHandler>();

eventBus.Subscribe<UserForgotPasswordEvent>(userForgotPasswordEventHandler.HandleAsync);
eventBus.Subscribe<UserRegisteredEvent>(userRegisteredEventHandler.HandleAsync);

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowAllOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var mongoDbSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
var mongoDatabase = mongoClient.GetDatabase(mongoDbSettings.DatabaseName);
var migration = new Migration(mongoDatabase);
await migration.MigrateAsync();

app.Run();

#endregion