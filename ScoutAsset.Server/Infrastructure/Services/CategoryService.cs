using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using ScoutAsset.Server.Application.Common.Interfaces;
using ScoutAsset.Server.Application.Models.Categories;
using ScoutAsset.Server.Application.Models.Resources;
using ScoutAsset.Server.Application.Services;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Infrastructure.Services;

public class CategoryService : ICategoryService
{
    private readonly IApplicationDbContext _context;

    public CategoryService(IApplicationDbContext context) => _context = context;

    public async Task<List<Category>> GetAllAsync() =>
        await _context.Categories.Where(c => c.IsActive).OrderBy(c => c.Name).ToListAsync();

    public async Task<Category?> GetByIdAsync(int id) =>
        await _context.Categories.FindAsync(id);

    public async Task<Category> CreateAsync(CreateCategoryRequest request)
    {
        var category = new Category
        {
            Name = request.Name,
            Description = request.Description,
            Prefix = request.Prefix.ToUpper()
        };
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task UpdateAsync(int id, CreateCategoryRequest request)
    {
        var category = await _context.Categories.FindAsync(id)
            ?? throw new KeyNotFoundException("Categoría no encontrada");
        category.Name = request.Name;
        category.Description = request.Description;
        category.Prefix = request.Prefix.ToUpper();
        category.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id)
            ?? throw new KeyNotFoundException("Categoría no encontrada");
        var hasResources = await _context.Resources.AnyAsync(r => r.CategoryId == id);
        if (hasResources)
            throw new InvalidOperationException("No se puede eliminar una categoría con recursos asociados");
        category.IsActive = false;
        category.UpdatedAt = DateTime.UtcNow;
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

    public async Task<ResourceImportResult> ImportCategoriesAsync(Stream stream, string fileName)
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

        var existing = await _context.Categories.ToListAsync();

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

        int colName = FindCol("nombre", "name", "categoría", "categoria");
        int colPrefix = FindCol("prefijo", "prefix", "código", "codigo");
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
                result.Errors.Add($"Fila {rowNum}: El nombre de la categoría es obligatorio.");
                continue;
            }

            if (existing.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                result.ErrorCount++;
                result.Errors.Add($"Fila {rowNum}: La categoría '{name}' ya existe.");
                continue;
            }

            var prefix = GetVal(colPrefix).ToUpper();
            if (string.IsNullOrWhiteSpace(prefix))
            {
                prefix = name.Length >= 3 ? name.Substring(0, 3).ToUpper() : name.ToUpper();
            }

            int counter = 1;
            var basePrefix = prefix;
            while (existing.Any(c => c.Prefix.Equals(prefix, StringComparison.OrdinalIgnoreCase)))
            {
                prefix = $"{basePrefix[..Math.Min(2, basePrefix.Length)]}{counter}";
                counter++;
            }

            var category = new Category
            {
                Name = name,
                Prefix = prefix,
                Description = GetVal(colDesc),
                NextNumber = 1,
                IsActive = true
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
            existing.Add(category);

            result.SuccessCount++;
            result.SuccessMessages.Add($"Categoría '{name}' ({prefix}) creada correctamente.");
        }

        return result;
    }
}
