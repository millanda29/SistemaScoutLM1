using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using ScoutAsset.Server.Application.Common.Interfaces;
using ScoutAsset.Server.Application.Models.Resources;
using ScoutAsset.Server.Application.Services;
using ScoutAsset.Server.Domain.Entities;
using ScoutAsset.Server.Domain.Enums;

namespace ScoutAsset.Server.Infrastructure.Services;

public class ResourceService : IResourceService
{
    private readonly IApplicationDbContext _context;

    public ResourceService(IApplicationDbContext context) => _context = context;

    public async Task<List<Resource>> GetAllAsync(string? search, int? categoryId, int? locationId, string? status)
    {
        var query = _context.Resources
            .Include(r => r.Category)
            .Include(r => r.Location)
            .Where(r => r.IsActive)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
            query = query.Where(r =>
                r.Code.Contains(search) ||
                r.Name.Contains(search) ||
                (r.SerialNumber != null && r.SerialNumber.Contains(search)));

        if (categoryId.HasValue)
            query = query.Where(r => r.CategoryId == categoryId);

        if (locationId.HasValue)
            query = query.Where(r => r.LocationId == locationId);

        if (!string.IsNullOrEmpty(status))
            query = query.Where(r => r.AdministrativeStatus == status);

        return await query.OrderBy(r => r.Code).ToListAsync();
    }

    public async Task<Resource?> GetByIdAsync(int id) =>
        await _context.Resources
            .Include(r => r.Category)
            .Include(r => r.Location)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<Resource> CreateAsync(CreateResourceRequest request, string currentUserId)
    {
        var category = await _context.Categories.FindAsync(request.CategoryId)
            ?? throw new InvalidOperationException("Categoría no encontrada");

        var code = $"{category.Prefix}-{category.NextNumber:D5}";
        category.NextNumber++;

        var resource = new Resource
        {
            Code = code,
            Name = request.Name,
            Description = request.Description,
            CategoryId = request.CategoryId,
            Brand = request.Brand,
            Model = request.Model,
            SerialNumber = request.SerialNumber,
            AcquisitionDate = request.AcquisitionDate,
            AcquisitionType = request.AcquisitionType ?? nameof(AcquisitionType.OTRO),
            AcquisitionCost = request.AcquisitionCost,
            AppraisedValue = request.AppraisedValue,
            PhysicalCondition = request.PhysicalCondition ?? nameof(PhysicalCondition.BUENO),
            AdministrativeStatus = nameof(AdministrativeStatus.DISPONIBLE),
            LocationId = request.LocationId,
            CurrentResponsibleId = request.CurrentResponsibleId,
            Observations = request.Observations,
            CreatedById = currentUserId
        };

        _context.Resources.Add(resource);
        await _context.SaveChangesAsync();

        if (!string.IsNullOrEmpty(request.CurrentResponsibleId))
        {
            _context.ResourceAssignments.Add(new ResourceAssignment
            {
                ResourceId = resource.Id,
                UserId = request.CurrentResponsibleId,
                AssignedById = currentUserId,
                AssignmentType = "CUSTODIA_PERMANENTE",
                AssignedAt = DateTime.UtcNow,
                IsActive = true
            });
        }

        var movement = new Movement
        {
            ResourceId = resource.Id,
            Type = nameof(MovementType.REGISTRO),
            Description = $"Registro inicial del recurso {code}",
            NewStatus = nameof(AdministrativeStatus.REGISTRADO),
            NewLocationId = request.LocationId,
            PerformedById = currentUserId,
            PerformedAt = DateTime.UtcNow
        };
        _context.Movements.Add(movement);
        await _context.SaveChangesAsync();

        return resource;
    }

    public async Task UpdateAsync(int id, UpdateResourceRequest request)
    {
        var resource = await _context.Resources.FindAsync(id)
            ?? throw new KeyNotFoundException("Recurso no encontrado");

        // Check if custodian changed
        if (request.CurrentResponsibleId != resource.CurrentResponsibleId)
        {
            var oldAssignments = await _context.ResourceAssignments
                .Where(a => a.ResourceId == id && a.IsActive)
                .ToListAsync();

            foreach (var old in oldAssignments)
            {
                old.IsActive = false;
                old.UnassignedAt = DateTime.UtcNow;
            }

            if (!string.IsNullOrEmpty(request.CurrentResponsibleId))
            {
                _context.ResourceAssignments.Add(new ResourceAssignment
                {
                    ResourceId = id,
                    UserId = request.CurrentResponsibleId,
                    AssignedById = "system",
                    AssignmentType = "CUSTODIA_PERMANENTE",
                    AssignedAt = DateTime.UtcNow,
                    IsActive = true
                });
            }

            resource.CurrentResponsibleId = request.CurrentResponsibleId;
        }

        resource.Name = request.Name;
        resource.Description = request.Description;
        resource.CategoryId = request.CategoryId;
        resource.LocationId = request.LocationId;
        resource.Brand = request.Brand;
        resource.Model = request.Model;
        resource.SerialNumber = request.SerialNumber;
        resource.AcquisitionDate = request.AcquisitionDate;
        resource.AcquisitionType = request.AcquisitionType;
        resource.AcquisitionCost = request.AcquisitionCost;
        resource.AppraisedValue = request.AppraisedValue;
        resource.PhysicalCondition = request.PhysicalCondition;
        resource.Observations = request.Observations;
        resource.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task ChangeLocationAsync(int id, ChangeLocationRequest request, string currentUserId)
    {
        var resource = await _context.Resources.FindAsync(id)
            ?? throw new KeyNotFoundException("Recurso no encontrado");

        var previousLocationId = resource.LocationId;
        resource.LocationId = request.LocationId;
        resource.UpdatedAt = DateTime.UtcNow;

        _context.Movements.Add(new Movement
        {
            ResourceId = id,
            Type = nameof(MovementType.TRASLADO),
            Description = "Traslado de ubicación",
            PreviousLocationId = previousLocationId,
            NewLocationId = request.LocationId,
            PerformedById = currentUserId,
            PerformedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    public async Task ChangeResponsibleAsync(int id, ChangeResponsibleRequest request, string currentUserId)
    {
        var resource = await _context.Resources.FindAsync(id)
            ?? throw new KeyNotFoundException("Recurso no encontrado");

        var previousResponsible = resource.CurrentResponsibleId;
        resource.CurrentResponsibleId = request.ResponsibleId;
        resource.UpdatedAt = DateTime.UtcNow;

        if (resource.AdministrativeStatus == nameof(AdministrativeStatus.REGISTRADO) ||
            resource.AdministrativeStatus == nameof(AdministrativeStatus.DISPONIBLE))
            resource.AdministrativeStatus = nameof(AdministrativeStatus.ASIGNADO);

        _context.ResourceAssignments.Add(new ResourceAssignment
        {
            ResourceId = id,
            UserId = request.ResponsibleId,
            AssignedById = currentUserId,
            AssignmentType = "CUSTODIO"
        });

        _context.Movements.Add(new Movement
        {
            ResourceId = id,
            Type = nameof(MovementType.ASIGNACION),
            Description = "Asignación de responsable",
            PreviousResponsibleId = previousResponsible,
            NewResponsibleId = request.ResponsibleId,
            PerformedById = currentUserId,
            PerformedAt = DateTime.UtcNow
        });

        await _context.SaveChangesAsync();
    }

    public async Task<List<Movement>> GetMovementsAsync(int id) =>
        await _context.Movements
            .Where(m => m.ResourceId == id)
            .OrderByDescending(m => m.PerformedAt)
            .ToListAsync();

    public async Task DeleteAsync(int id)
    {
        var resource = await _context.Resources.FindAsync(id)
            ?? throw new KeyNotFoundException("Recurso no encontrado");
        resource.IsActive = false;
        resource.UpdatedAt = DateTime.UtcNow;
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

    public async Task<ResourceImportResult> ImportResourcesAsync(Stream stream, string fileName, string currentUserId)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

        var result = new ResourceImportResult();
        using var reader = CreateDataReader(stream, fileName);

        var dataSet = reader.AsDataSet(new ExcelDataSetConfiguration
        {
            ConfigureDataTable = _ => new ExcelDataTableConfiguration
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

        // Cache existing categories & locations
        var categories = await _context.Categories.ToListAsync();
        var locations = await _context.Locations.ToListAsync();
        var defaultCategory = categories.FirstOrDefault() ?? new Category { Name = "General", Prefix = "GEN", NextNumber = 1 };
        var defaultLocation = locations.FirstOrDefault() ?? new Location { Name = "Bodega Principal" };

        if (_context.Categories.Count() == 0)
        {
            _context.Categories.Add(defaultCategory);
            await _context.SaveChangesAsync();
            categories.Add(defaultCategory);
        }

        if (_context.Locations.Count() == 0)
        {
            _context.Locations.Add(defaultLocation);
            await _context.SaveChangesAsync();
            locations.Add(defaultLocation);
        }

        // Helper to find column index by list of headers
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

        int colCode = FindCol("código", "codigo", "code");
        int colName = FindCol("nombre", "name", "recurso", "bienes");
        int colDesc = FindCol("descripción", "descripcion", "description", "detalle");
        int colCat = FindCol("categoría", "categoria", "category");
        int colLoc = FindCol("ubicación", "ubicacion", "location");
        int colBrand = FindCol("marca", "brand");
        int colModel = FindCol("modelo", "model");
        int colSerial = FindCol("serie", "nroserie", "serialnumber");
        int colCost = FindCol("costo", "valor", "price", "acquisitioncost");
        int colAcqType = FindCol("adquisición", "adquisicion", "acquisitiontype", "tipo");
        int colCondition = FindCol("estado físico", "estadofisico", "condicion", "condition");

        int rowNum = 1;
        foreach (System.Data.DataRow row in table.Rows)
        {
            rowNum++;
            string GetVal(int col) => col >= 0 && !row.IsNull(col) ? row[col]?.ToString()?.Trim() ?? "" : "";

            var name = GetVal(colName);
            if (string.IsNullOrWhiteSpace(name))
            {
                result.ErrorCount++;
                result.Errors.Add($"Fila {rowNum}: El nombre del recurso es obligatorio.");
                continue;
            }

            var codeStr = GetVal(colCode);
            var categoryStr = GetVal(colCat);
            var locationStr = GetVal(colLoc);

            // Find or create category
            Category cat = defaultCategory;
            if (!string.IsNullOrWhiteSpace(categoryStr))
            {
                cat = categories.FirstOrDefault(c => c.Name.Equals(categoryStr, StringComparison.OrdinalIgnoreCase) || c.Prefix.Equals(categoryStr, StringComparison.OrdinalIgnoreCase))!;
                if (cat == null)
                {
                    var prefix = categoryStr.Length >= 3 ? categoryStr.Substring(0, 3).ToUpper() : categoryStr.ToUpper();
                    cat = new Category { Name = categoryStr, Prefix = prefix, NextNumber = 1 };
                    _context.Categories.Add(cat);
                    await _context.SaveChangesAsync();
                    categories.Add(cat);
                }
            }

            // Find or create location
            Location loc = defaultLocation;
            if (!string.IsNullOrWhiteSpace(locationStr))
            {
                loc = locations.FirstOrDefault(l => l.Name.Equals(locationStr, StringComparison.OrdinalIgnoreCase))!;
                if (loc == null)
                {
                    loc = new Location { Name = locationStr, Description = "Creado automáticamente vía importación masiva" };
                    _context.Locations.Add(loc);
                    await _context.SaveChangesAsync();
                    locations.Add(loc);
                }
            }

            // Generate or use code
            string code;
            if (!string.IsNullOrWhiteSpace(codeStr) && !await _context.Resources.AnyAsync(r => r.Code == codeStr))
            {
                code = codeStr;
            }
            else
            {
                code = $"{cat.Prefix}-{cat.NextNumber:D5}";
                cat.NextNumber++;
            }

            decimal? cost = null;
            var costStr = GetVal(colCost);
            if (!string.IsNullOrWhiteSpace(costStr) && decimal.TryParse(costStr.Replace("$", "").Trim(), out var parsedCost))
            {
                cost = parsedCost;
            }

            var condition = GetVal(colCondition).ToUpper();
            if (condition != "REGULAR" && condition != "DANIADO" && condition != "DAÑADO")
                condition = "BUENO";

            var acqType = GetVal(colAcqType).ToUpper();
            if (acqType != "DONACION" && acqType != "DONACIÓN" && acqType != "TRANSFERENCIA")
                acqType = "COMPRA";

            var resource = new Resource
            {
                Code = code,
                Name = name,
                Description = GetVal(colDesc),
                CategoryId = cat.Id,
                LocationId = loc.Id,
                Brand = GetVal(colBrand),
                Model = GetVal(colModel),
                SerialNumber = GetVal(colSerial),
                AcquisitionType = acqType,
                AcquisitionCost = cost,
                PhysicalCondition = condition,
                AdministrativeStatus = nameof(AdministrativeStatus.DISPONIBLE),
                CreatedById = currentUserId
            };

            _context.Resources.Add(resource);
            await _context.SaveChangesAsync();

            _context.Movements.Add(new Movement
            {
                ResourceId = resource.Id,
                Type = nameof(MovementType.REGISTRO),
                Description = $"Importación masiva Excel ({fileName})",
                NewStatus = nameof(AdministrativeStatus.REGISTRADO),
                NewLocationId = loc.Id,
                PerformedById = currentUserId,
                PerformedAt = DateTime.UtcNow
            });
            await _context.SaveChangesAsync();

            result.SuccessCount++;
            result.SuccessMessages.Add($"Recurso '{code} - {name}' importado correctamente.");
        }

        return result;
    }
}
