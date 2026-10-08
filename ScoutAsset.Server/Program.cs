using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScoutAsset.Server.Application;
using ScoutAsset.Server.Application.Common.Interfaces;
using ScoutAsset.Server.Domain.Entities;
using ScoutAsset.Server.Infrastructure;
using ScoutAsset.Server.Infrastructure.Persistence;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddHttpContextAccessor();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("https://localhost:49698")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

var app = builder.Build();

app.UseDefaultFiles();
app.MapStaticAssets();
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("/index.html");

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.EnsureCreated();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    var roles = new[] { "ADMIN", "SUPERINTENDENTE", "JEFE_GRUPO", "CUSTODIO", "SOLICITANTE", "AUXILIAR" };
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }

    var adminUser = builder.Configuration["AdminSeed:UserName"] ?? "admin";
    var adminEmail = builder.Configuration["AdminSeed:Email"] ?? "admin@scoutinventory.com";
    var adminPassword = builder.Configuration["AdminSeed:Password"] ?? "Admin123!";

    if (await userManager.FindByNameAsync(adminUser) == null)
    {
        var admin = new IdentityUser { UserName = adminUser, Email = adminEmail };
        await userManager.CreateAsync(admin, adminPassword);
        await userManager.AddToRoleAsync(admin, "ADMIN");

        var profile = new UserProfile
        {
            UserId = admin.Id,
            CI = "1700000001",
            Nombres = "Administrador",
            Apellidos = "del Sistema",
            Telefono = "0999999999",
            IsActive = true
        };
        dbContext.UserProfiles.Add(profile);
        await dbContext.SaveChangesAsync();
    }

    if (!await dbContext.Categories.AnyAsync())
    {
        dbContext.Categories.AddRange(
            new Category { Name = "Mobiliario", Prefix = "MOB", Description = "Muebles y enseres" },
            new Category { Name = "Tecnología", Prefix = "TEC", Description = "Equipos tecnológicos" },
            new Category { Name = "Campamento", Prefix = "CAM", Description = "Equipo de campamento" },
            new Category { Name = "Material Scout", Prefix = "SCOUT", Description = "Material scout" },
            new Category { Name = "Documentación", Prefix = "DOC", Description = "Documentos institucionales" },
            new Category { Name = "Limpieza", Prefix = "LIM", Description = "Artículos de limpieza" },
            new Category { Name = "Recreación", Prefix = "REC", Description = "Artículos de recreación" }
        );
        await dbContext.SaveChangesAsync();
    }

    if (!await dbContext.Locations.AnyAsync())
    {
        dbContext.Locations.AddRange(
            new Location { Name = "Oficina Principal", Description = "Oficina institucional" },
            new Location { Name = "Bodega", Description = "Bodega de almacenamiento" },
            new Location { Name = "Sala de Reuniones", Description = "Sala de reuniones" }
        );
        await dbContext.SaveChangesAsync();
    }
}

app.Run();
