using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Security.Claims;
using System.Text;
using TravelingApp.Domain.Entities;
using TravelingApp.Infraestructure;
using TravelingApp.Infraestructure.Context;
using TravelingApp.Application.Abstractions;
using TravelingApp.Application.Configuration;
using TravelingApp.Application.Behaviors;
using TravelingApp.Application.Extensions;
using FluentValidation;
using TravelingApp.Application.Features.Account.Commands.Login;
using TravelingApp.Application.Features.Users.Queries.ListUsers;

namespace TravelingApp.API
{
    public static class DependencyContainer
    {
        public static void RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtOptions>(configuration.GetSection(JwtOptions.SectionName));

            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("RedisDbConnection") ?? "localhost:6379";
                options.InstanceName = $"TravelingApp.Cache:{configuration["ASPNETCORE_ENVIRONMENT"]}:";
            });

            services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.SectionName));

            services.AddHttpContextAccessor();
            services.AddControllers();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "TravelingApp API",
                    Version = "v1"
                });

                options.TagActionsBy(apiDescription =>
                {
                    if (apiDescription.ActionDescriptor is not ControllerActionDescriptor controller)
                        return [apiDescription.GroupName ?? "Default"];

                    var name = controller.ControllerTypeInfo.Name;
                    const string suffix = "Controller";
                    return [name.EndsWith(suffix, StringComparison.Ordinal) ? name[..^suffix.Length] : name];
                });

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Introduce el token JWT.\nEjemplo: abcdef123456"
                });

                options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
                {
                    { new OpenApiSecuritySchemeReference("Bearer", document), [] }
                });
            });

            services.RegisterDbServices(configuration);

            services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<TravelingAppDbContext>());

            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins", builder =>
                {
                    // TODO: Restrict origins for production
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
             .AddJwtBearer(options =>
             {
                 var jwt = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>()!;

                 options.TokenValidationParameters = new TokenValidationParameters
                 {
                     ValidateIssuer = true,
                     ValidateAudience = true,
                     ValidateLifetime = true,
                     ValidateIssuerSigningKey = true,
                     ValidIssuer = jwt.Issuer,
                     ValidAudience = jwt.Audience,
                     IssuerSigningKey = new SymmetricSecurityKey(
                         Encoding.UTF8.GetBytes(jwt.Key!)
                     ),
                     ClockSkew = TimeSpan.Zero,
                     RoleClaimType = ClaimTypes.Role
                 };

                 options.Events = new JwtBearerEvents
                 {
                     OnMessageReceived = context =>
                     {
                         var token = context.Request.Cookies["AuthToken"];
                         if (!string.IsNullOrEmpty(token))
                         {
                             context.Token = token;
                         }

                         return Task.CompletedTask;
                     }
                 };
             });

            services.AddIdentityCore<User>(opt =>
            {
                opt.SignIn.RequireConfirmedAccount = false;
                opt.Password.RequireDigit = true;
                opt.Password.RequiredLength = 8;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = true;
                opt.Password.RequireLowercase = true;
            })
            .AddRoles<IdentityRole>()
            .AddSignInManager<SignInManager<User>>()
            .AddEntityFrameworkStores<TravelingAppDbContext>()
            .AddDefaultTokenProviders();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly);
            });

            services.AddAutoMapper(cfg => cfg.AddMaps(typeof(ListUsersQueryHandler).Assembly));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
            services.AddValidatorsFromAssembly(typeof(LoginCommand).Assembly);

            services.AddAttributedServices("TravelingApp");
        }
    }
}
