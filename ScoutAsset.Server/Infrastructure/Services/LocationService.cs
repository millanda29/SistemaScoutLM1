using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using ScoutAsset.Server.Application.Common.Interfaces;
using ScoutAsset.Server.Application.Models.Locations;
using ScoutAsset.Server.Application.Models.Resources;
using ScoutAsset.Server.Application.Services;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Infrastructure.Services;

public class LocationService : ILocationService
{
    private readonly IApplicationDbContext _context;

    public LocationService(IApplicationDbContext context) => _context = context;

    public async Task<List<Location>> GetAllAsync() =>
        await _context.Locations.Where(l => l.IsActive).OrderBy(l => l.Name).ToListAsync();

    public async Task<Location?> GetByIdAsync(int id) =>
        await _context.Locations.FindAsync(id);

    public async Task<Location> CreateAsync(CreateLocationRequest request)
    {
        var location = new Location { Name = request.Name, Description = request.Description };
        _context.Locations.Add(location);
        await _context.SaveChangesAsync();
        return location;
    }

    public async Task UpdateAsync(int id, CreateLocationRequest request)
    {
        var location = await _context.Locations.FindAsync(id)
            ?? throw new KeyNotFoundException("Ubicación no encontrada");
        location.Name = request.Name;
        location.Description = request.Description;
        location.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var location = await _context.Locations.FindAsync(id)
            ?? throw new KeyNotFoundException("Ubicación no encontrada");
        var hasResources = await _context.Resources.AnyAsync(r => r.LocationId == id);
        if (hasResources)
            throw new InvalidOperationException("No se puede eliminar una ubicación con recursos asociados");
        location.IsActive = false;
        location.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    private static IExcelDataReader CreateDataReader(Stream stream, string fileName)
    {
        if (fileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
        {
            return ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration
            {
                FallbackEncoding = System.Text.Encoding.UTF8
            });
        }

        try
        {
            return ExcelReaderFactory.CreateReader(stream);
        }
        catch (ExcelDataReader.Exceptions.HeaderException)
        {
            if (stream.CanSeek) stream.Position = 0;
            return ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration
            {
                FallbackEncoding = System.Text.Encoding.UTF8
            });
        }
    }

    public async Task<ScoutAsset.Server.Application.Models.Resources.ResourceImportResult> ImportLocationsAsync(Stream stream, string fileName)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        var result = new ScoutAsset.Server.Application.Models.Resources.ResourceImportResult();
        using var reader = CreateDataReader(stream, fileName);

        var dataSet = reader.AsDataSet(new ExcelDataReader.ExcelDataSetConfiguration
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

        if (table.Rows.Count == 0)
        {
            result.Errors.Add("El archivo está vacío.");
            return result;
        }

        var existing = await _context.Locations.ToListAsync();

        int FindCol(params string[] possibleNames)
        {
            for (int i = 0; i < table.Columns.Count; i++)
            {
                var colName = table.Columns[i].ColumnName?.Trim().ToLowerInvariant();
                if (colName != null && possibleNames.Any(p => colName.Equals(p.ToLowerInvariant(), StringComparison.OrdinalIgnoreCase) || colName.Contains(p.ToLowerInvariant())))
                    return i;
            }
            return -1;
        }

        int colName = FindCol("nombre", "name", "ubicación", "ubicacion");
        int colDesc = FindCol("descripción", "descripcion", "description", "detalle");

        int rowNum = 1;
        foreach (System.Data.DataRow row in table.Rows)
        {
            rowNum++;
            string GetVal(int col) => col >= 0 && !row.IsNull(col) ? row[col]?.ToString()?.Trim() ?? "" : "";

            var name = GetVal(colName);
            if (string.IsNullOrWhiteSpace(name))
            {
                result.ErrorCount++;
                result.Errors.Add($"Fila {rowNum}: El nombre de la ubicación es obligatorio.");
                continue;
            }

            if (existing.Any(l => l.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                result.ErrorCount++;
                result.Errors.Add($"Fila {rowNum}: La ubicación '{name}' ya existe.");
                continue;
            }

            var location = new Location
            {
                Name = name,
                Description = GetVal(colDesc),
                IsActive = true
            };

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            existing.Add(location);

            result.SuccessCount++;
            result.SuccessMessages.Add($"Ubicación '{name}' creada correctamente.");
        }

        return result;
    }
}
