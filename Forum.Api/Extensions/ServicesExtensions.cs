//using Amazon.S3;
//using Amazon.SimpleEmail;
//using Forum.Api.Filters;
//using Forum.Application.Aggregates;
//using Forum.Application.Interfaces;
//using Forum.Domain.Entities.User;
//using Forum.Domain.Models;
//using Forum.Infrastructure.Email;
//using Forum.Infrastructure.FileStorage;
//using Forum.Infrastructure.Identity;
//using Forum.Infrastructure.Notifications;
//using Forum.Infrastructure.Persistence;
//using Forum.Shared.EventBus;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using System.Text;

//namespace Forum.Api.Extensions
//{
//    public static class ServicesExtensions
//    {
//        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
//        {
//            services.AddControllers();
//            services.AddEndpointsApiExplorer();
//            services.AddSwaggerGen();
//            return services;
//        }

//        #region AWSServices
//        public static IServiceCollection AddAmazonWebServices(this IServiceCollection services, IConfiguration configuration)
//        {
//            services.AddDefaultAWSOptions(configuration.GetAWSOptions());
//            services.AddAWSService<IAmazonS3>();
//            services.AddScoped<IS3FileStorageService, S3FileStorageService>();
//            services.Configure<ApiCredentials>(configuration);
//            services.AddAWSService<IAmazonSimpleEmailService>();
//            services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
//            return services;
//        }
//        #endregion

//        #region EmailServices
//        public static IServiceCollection AddEmailServices(this IServiceCollection services)
//        {
//            services.AddTransient<IMailService, MailService>();
//            services.AddTransient<EmailNotificationService>();
//            return services;
//        }
//        #endregion

//        #region CorsServices
//        public static IServiceCollection AddCorsServices(this IServiceCollection services)
//        {
//            services.AddCors(options =>
//            {
//                options.AddPolicy("DefaultCorsPolicy", builder =>
//                {
//                    builder.AllowAnyOrigin()
//                           .AllowAnyMethod()
//                           .AllowAnyHeader();
//                });
//            });
//            return services;
//        }
//        #endregion

//        #region FilterServices
//        public static IServiceCollection AddFilterServices(this IServiceCollection services)
//        {
//            services.AddScoped<ModelValidationFilterAttribute>();
//            services.AddScoped<ExceptionHandlingFilterAttribute>();

//            services.AddControllers(options =>
//            {
//                options.Filters.Add<ModelValidationFilterAttribute>();
//                options.Filters.Add<ExceptionHandlingFilterAttribute>();
//            });

//            return services;
//        }
//        #endregion

//        #region JwtServices
//        public static IServiceCollection AddJwtServices(this IServiceCollection services, IConfiguration configuration)
//        {
//            var jwtSection = configuration.GetSection("Jwt");

//            services.AddScoped<TokenValidationParameters>(sp =>
//            {
//                var issuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"]));

//                return new TokenValidationParameters
//                {
//                    ValidateIssuer = true,
//                    ValidateAudience = true,
//                    ValidateLifetime = true,
//                    IssuerSigningKey = issuerSigningKey,
//                    ValidIssuer = configuration["Jwt:Issuer"],
//                    ValidAudience = configuration["Jwt:Audience"]
//                };
//            });

//            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//                    .AddJwtBearer(options =>
//                    {
//                        options.TokenValidationParameters = services.BuildServiceProvider().GetRequiredService<TokenValidationParameters>();
//                    });

//            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

//            return services;
//        }

//        #endregion

//        public static IServiceCollection AddScopedServices(this IServiceCollection services)
//        {
//            services.AddScoped<IIdentityService, IdentityService>();
//            services.AddScoped<IUserRepository, UserRepository>();
//            services.AddScoped<IProfileRepository, ProfileRepository>();

//            services.AddIdentity<User, IdentityRole>()
//                   .AddEntityFrameworkStores<MemberDbContext>()
//                   .AddDefaultTokenProviders();

//            services.AddScoped<IPasswordHasher<UserAggregate>, PasswordHasher<UserAggregate>>();

//            return services;
//        }

//        public static IServiceCollection AddEventBusServices(this IServiceCollection services)
//        {
//            services.AddSingleton<IEventBus, EventBus>();
//            return services;
//        }

//        public static IServiceCollection AddDatabaseServices(this IServiceCollection services, IConfiguration configuration)
//        {
//            var profileConnectionString = configuration.GetConnectionString("ProfileDbConnection");
//            var identityConnectionString = configuration.GetConnectionString("IdentityDbConnection");

//            if (string.IsNullOrEmpty(identityConnectionString) || string.IsNullOrEmpty(profileConnectionString))
//            {
//                throw new Exception("Database connection string is not found.");
//            }

//            services.AddDbContext<ProfileDbContext>(options =>
//                options.UseNpgsql(profileConnectionString));

//            services.AddDbContext<MemberDbContext>(options =>
//                options.UseNpgsql(identityConnectionString));

//            return services;
//        }
//    }
//}
