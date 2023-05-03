using Application.Shared.Pipes;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Inventory.Application.Infrastructure.Authentication.Requirements;
using Persistence.GenericRepository.Repository;
using System.Text;
using Utils.Time;
using Persistence.GenericRepository.UnitOfWork;
using Inventory.Application.Interfaces.Persistence;
using Inventory.Application.Interfaces.Service;
using Inventory.Application.Shared.Permissions;
using Inventory.Application.Domain.Identity;
using Inventory.Application.Infrastructure.Service;
using Inventory.Application.Infrastructure.Authentication;
using Inventory.Application;
using Inventory.Application.Infrastructure.Persistence;
using Inventory.Application.Shared;
using Inventory.Application.Interfaces.Authentication;

namespace Inventory.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("ApplicationConnection");
            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(connectionString));
            services.AddScoped<DbContext, ApplicationDbContext>();
            services.Configure<DapperConfig>(options => options.ConnectionString = connectionString);
            services.AddScoped<IDapperContext, DapperContext>();
            services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }

        public static void AddPipes(this IServiceCollection services)
        {
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledExceptionBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        }

        public static void AddServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IPermissionsService, PermissionsService>();
            services.AddTransient<IPathComposer, PathComposer>();
            services.AddTransient<IUriComposer, UriComposer>();

            services.Configure<ApplicationUri>(configuration.GetSection("ApplicationUri"));

        }

        public static void AddUtils(this IServiceCollection services)
        {
            services.AddTransient<IDateTimeProvider, DateTimeProvider>();
            services.AddMediatR(typeof(IApplicationLayer).Assembly);
            services.AddAutoMapper(typeof(IApplicationLayer).Assembly);
            services.AddValidatorsFromAssembly(typeof(IApplicationLayer).Assembly);
        }

        public static void AddAuth(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthenticatedUserService, AuthenticatedUserService>();
            services.AddScoped<IAuthorizationHandler, PermissionRequirementHandler>();
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = true;
                options.Password.RequireNonAlphanumeric = false;
            })
            .AddDefaultUI()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();


            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    ValidIssuer = configuration["JWTSettings:Issuer"],
                    ValidAudience = configuration["JWTSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWTSettings:Key"]))
                };
            });
        }

        public static void AddAuthrorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                var modules = PermissionsFactory.CreateModulesWithPermissions();
                foreach (var module in modules)
                {
                    foreach (var permission in module.Permissions)
                    {
                        var policy = permission.Id;
                        options.AddPolicy(policy, options =>
                        {
                            options.RequireAuthenticatedUser();
                            options.Requirements.Add(new PermissionRequirement(policy));
                        });
                    }
                }
            });
        }
    }
}
