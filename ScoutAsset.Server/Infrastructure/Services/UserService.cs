using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScoutAsset.Server.Application.Common.Interfaces;
using ScoutAsset.Server.Application.Models.Users;
using ScoutAsset.Server.Application.Services;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IApplicationDbContext _context;
    private readonly IEmailSender _emailSender;

    public UserService(
        UserManager<IdentityUser> userManager, 
        RoleManager<IdentityRole> roleManager,
        IApplicationDbContext context,
        IEmailSender emailSender)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _emailSender = emailSender;
    }

    public async Task<List<UserDetailDto>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        var result = new List<UserDetailDto>();

        foreach (var user in users)
        {
            var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
            var roles = (await _userManager.GetRolesAsync(user)).ToList();

            result.Add(new UserDetailDto(
                user.Id,
                user.UserName ?? string.Empty,
                user.Email ?? string.Empty,
                profile?.CI ?? string.Empty,
                profile?.Nombres ?? string.Empty,
                profile?.Apellidos ?? string.Empty,
                profile?.Telefono ?? string.Empty,
                roles
            ));
        }

        return result.OrderBy(u => u.UserName).ToList();
    }

    public async Task<UserDetailDto?> GetByIdAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return null;

        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == user.Id);
        var roles = (await _userManager.GetRolesAsync(user)).ToList();

        return new UserDetailDto(
            user.Id,
            user.UserName ?? string.Empty,
            user.Email ?? string.Empty,
            profile?.CI ?? string.Empty,
            profile?.Nombres ?? string.Empty,
            profile?.Apellidos ?? string.Empty,
            profile?.Telefono ?? string.Empty,
            roles
        );
    }

    public async Task<IdentityUser> CreateAsync(CreateUserRequest request)
    {
        if (await _userManager.FindByNameAsync(request.UserName) != null)
            throw new InvalidOperationException("El nombre de usuario ya existe");

        if (!string.IsNullOrEmpty(request.Email) && await _userManager.FindByEmailAsync(request.Email) != null)
            throw new InvalidOperationException("El correo electrónico ya está registrado");

        var user = new IdentityUser
        {
            UserName = request.UserName,
            Email = request.Email,
            EmailConfirmed = true,
            PhoneNumber = request.Telefono
        };

        // Auto-generate temporary password if not provided
        var password = request.Password;
        if (string.IsNullOrEmpty(password))
        {
            password = GenerateRandomPassword();
        }

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

        if (!string.IsNullOrEmpty(request.Role) && await _roleManager.RoleExistsAsync(request.Role))
            await _userManager.AddToRoleAsync(user, request.Role);

        // Save extended UserProfile
        var profile = new UserProfile
        {
            UserId = user.Id,
            CI = request.CI,
            Nombres = request.Nombres,
            Apellidos = request.Apellidos,
            Telefono = request.Telefono,
            IsActive = true
        };
        _context.UserProfiles.Add(profile);
        await _context.SaveChangesAsync();

        // Send confirmation email with access details (Set Password / Temporary Password)
        var subject = "Bienvenido a ScoutAsset - Credenciales de Acceso";
        var body = EmailTemplates.GetWelcomeEmail(request.Nombres, request.UserName, user.Email!, password);

        await _emailSender.SendEmailAsync(user.Email!, subject, body);

        return user;
    }

    public async Task UpdateAsync(string id, UpdateUserRequest request)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new KeyNotFoundException("Usuario no encontrado");

        user.Email = request.Email ?? user.Email;
        user.UserName = request.UserName ?? user.UserName;

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

        if (request.Roles != null)
        {
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);
            foreach (var role in request.Roles)
            {
                if (await _roleManager.RoleExistsAsync(role))
                    await _userManager.AddToRoleAsync(user, role);
            }
        }
    }

    public async Task DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id)
            ?? throw new KeyNotFoundException("Usuario no encontrado");

        if (user.UserName == "admin")
            throw new InvalidOperationException("No se puede eliminar el usuario administrador");

        // Delete linked profile if exists
        var profile = await _context.UserProfiles.FirstOrDefaultAsync(p => p.UserId == id);
        if (profile != null)
        {
            _context.UserProfiles.Remove(profile);
        }

        var result = await _userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));

        await _context.SaveChangesAsync();
    }

    private static ExcelDataReader.IExcelDataReader CreateDataReader(Stream stream, string fileName)
    {
        if (fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return ExcelDataReader.ExcelReaderFactory.CreateCsvReader(stream, new ExcelDataReader.ExcelReaderConfiguration
            {
                FallbackEncoding = System.Text.Encoding.UTF8
            });
        }

        try
        {
            return ExcelDataReader.ExcelReaderFactory.CreateReader(stream);
        }
        catch (ExcelDataReader.Exceptions.HeaderException)
        {
            if (stream.CanSeek) stream.Position = 0;
            return ExcelDataReader.ExcelReaderFactory.CreateCsvReader(stream, new ExcelDataReader.ExcelReaderConfiguration
            {
                FallbackEncoding = System.Text.Encoding.UTF8
            });
        }
    }

    public async Task<ScoutAsset.Server.Application.Models.Resources.ResourceImportResult> ImportUsersAsync(Stream stream, string fileName)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        var result = new ScoutAsset.Server.Application.Models.Resources.ResourceImportResult();
        using var reader = CreateDataReader(stream, fileName);

        var dataSet = ExcelDataReader.ExcelDataReaderExtensions.AsDataSet(reader, new ExcelDataReader.ExcelDataSetConfiguration
        {
            ConfigureDataTable = _ => new ExcelDataReader.ExcelDataTableConfiguration
            {
                UseHeaderRow = true
            }
        });

        if (dataSet.Tables.Count == 0)
        {
            result.Errors.Add("El archivo no contiene hojas válidas.");
            return result;
        }

        var table = dataSet.Tables[0];
        result.TotalRows = table.Rows.Count;

        for (int i = 0; i < table.Rows.Count; i++)
        {
            var row = table.Rows[i];
            int rowNum = i + 2;

            try
            {
                string ci = GetColumnValue(row, "Cédula", "Cedula", "CI");
                string nombres = GetColumnValue(row, "Nombres", "Nombre");
                string apellidos = GetColumnValue(row, "Apellidos", "Apellido");
                string userName = GetColumnValue(row, "Usuario", "UserName", "Username");
                string email = GetColumnValue(row, "Correo", "Email", "Correo Electrónico");
                string telefono = GetColumnValue(row, "Teléfono", "Telefono", "Phone");
                string role = GetColumnValue(row, "Rol", "Role");

                if (string.IsNullOrWhiteSpace(nombres))
                {
                    result.Errors.Add($"Fila {rowNum}: El nombre es obligatorio.");
                    result.ErrorCount++;
                    continue;
                }

                if (string.IsNullOrWhiteSpace(email))
                {
                    result.Errors.Add($"Fila {rowNum}: El correo electrónico es obligatorio.");
                    result.ErrorCount++;
                    continue;
                }

                // If username is not provided, generate from nombres and apellidos
                if (string.IsNullOrWhiteSpace(userName))
                {
                    var cleanFirstName = nombres.Trim().Split(' ')[0].ToLower();
                    var cleanLastName = string.IsNullOrWhiteSpace(apellidos) ? "" : apellidos.Trim().Split(' ')[0].ToLower();
                    userName = $"{cleanFirstName}.{cleanLastName}".TrimEnd('.');
                }

                // Default role to DIRIGENTE if empty
                if (string.IsNullOrWhiteSpace(role))
                {
                    role = "DIRIGENTE";
                }
                else
                {
                    role = role.Trim().ToUpper();
                    if (role != "ADMIN" && role != "SUPERINTENDENTE" && role != "DIRIGENTE")
                        role = "DIRIGENTE";
                }

                // Check if user or email already exists
                if (await _userManager.FindByNameAsync(userName) != null)
                {
                    result.Errors.Add($"Fila {rowNum}: El usuario '{userName}' ya existe.");
                    result.ErrorCount++;
                    continue;
                }

                if (await _userManager.FindByEmailAsync(email) != null)
                {
                    result.Errors.Add($"Fila {rowNum}: El correo '{email}' ya se encuentra registrado.");
                    result.ErrorCount++;
                    continue;
                }

                var createRequest = new CreateUserRequest(userName, email, null, role, ci, nombres, apellidos, telefono);

                await CreateAsync(createRequest);

                result.SuccessCount++;
                result.SuccessMessages.Add($"Dirigente/Usuario '{nombres} {apellidos}' ({userName}) creado correctamente.");
            }
            catch (Exception ex)
            {
                result.ErrorCount++;
                result.Errors.Add($"Fila {rowNum}: Error inesperado - {ex.Message}");
            }
        }

        return result;
    }

    private static string GetColumnValue(System.Data.DataRow row, params string[] possibleColumnNames)
    {
        foreach (var colName in possibleColumnNames)
        {
            if (row.Table.Columns.Contains(colName) && row[colName] != DBNull.Value)
            {
                return row[colName]?.ToString()?.Trim() ?? string.Empty;
            }
        }
        return string.Empty;
    }

    private string GenerateRandomPassword()
    {
        const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$";
        return new string(Enumerable.Repeat(chars, 10)
            .Select(s => s[Random.Shared.Next(s.Length)]).ToArray());
    }
}
