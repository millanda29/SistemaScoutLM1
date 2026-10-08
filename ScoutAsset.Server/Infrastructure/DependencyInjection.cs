using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ScoutAsset.Server.Application.Common.Interfaces;
using ScoutAsset.Server.Application.Services;
using ScoutAsset.Server.Infrastructure.Identity;
using ScoutAsset.Server.Infrastructure.Persistence;
using ScoutAsset.Server.Infrastructure.Services;
using System.Text;

namespace ScoutAsset.Server.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName)));

        services.AddScoped<IApplicationDbContext>(sp =>
            sp.GetRequiredService<AppDbContext>());

        services.AddIdentityCore<IdentityUser>(options =>
        {
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<AppDbContext>()
        .AddDefaultTokenProviders()
        .AddSignInManager();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!))
                };
            });

        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IResourceService, ResourceService>();
        services.AddScoped<ILoanService, LoanService>();
        services.AddScoped<ILossService, LossService>();
        services.AddScoped<IMaintenanceService, MaintenanceService>();
        services.AddScoped<IRetirementService, RetirementService>();
        services.AddScoped<IPhysicalInventoryService, PhysicalInventoryService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDashboardService, DashboardService>();
        services.AddScoped<IEmailSender, SmtpEmailSender>();
        services.AddScoped<IDirigenteService, DirigenteService>();

        return services;
    }
}
