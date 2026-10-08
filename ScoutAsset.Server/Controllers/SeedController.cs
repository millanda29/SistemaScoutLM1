using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScoutAsset.Server.Application.Common.Interfaces;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SeedController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public SeedController(
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IApplicationDbContext context,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _configuration = configuration;
    }

    [HttpPost]
    public async Task<IActionResult> Seed()
    {
        // Roles
        var roles = new[] { "ADMIN", "SUPERINTENDENTE", "JEFE_GRUPO", "CUSTODIO", "SOLICITANTE", "AUXILIAR" };
        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
                await _roleManager.CreateAsync(new IdentityRole(role));
        }

        // Admin user
        var adminUser = _configuration["AdminSeed:UserName"] ?? "admin";
        var adminEmail = _configuration["AdminSeed:Email"] ?? "admin@scoutinventory.com";
        var adminPassword = _configuration["AdminSeed:Password"] ?? "Admin123!";

        var admin = await _userManager.FindByNameAsync(adminUser);
        if (admin == null)
        {
            admin = new IdentityUser
            {
                UserName = adminUser,
                Email = adminEmail
            };
            await _userManager.CreateAsync(admin, adminPassword);
            await _userManager.AddToRoleAsync(admin, "ADMIN");
        }

        // Default categories
        if (!await _context.Categories.AnyAsync())
        {
            var categories = new[]
            {
                new Category { Name = "Mobiliario", Prefix = "MOB", Description = "Muebles y enseres" },
                new Category { Name = "Tecnología", Prefix = "TEC", Description = "Equipos tecnológicos" },
                new Category { Name = "Campamento", Prefix = "CAM", Description = "Equipo de campamento" },
                new Category { Name = "Material Scout", Prefix = "SCOUT", Description = "Material scout" },
                new Category { Name = "Documentación", Prefix = "DOC", Description = "Documentos institucionales" },
                new Category { Name = "Limpieza", Prefix = "LIM", Description = "Artículos de limpieza" },
                new Category { Name = "Recreación", Prefix = "REC", Description = "Artículos de recreación" },
            };

            _context.Categories.AddRange(categories);
            await _context.SaveChangesAsync();
        }

        // Default locations
        if (!await _context.Locations.AnyAsync())
        {
            var locations = new[]
            {
                new Location { Name = "Oficina Principal", Description = "Oficina institucional" },
                new Location { Name = "Bodega", Description = "Bodega de almacenamiento" },
                new Location { Name = "Sala de Reuniones", Description = "Sala de reuniones" },
            };

            _context.Locations.AddRange(locations);
            await _context.SaveChangesAsync();
        }

        return Ok(new { message = "Datos iniciales creados exitosamente" });
    }
}
