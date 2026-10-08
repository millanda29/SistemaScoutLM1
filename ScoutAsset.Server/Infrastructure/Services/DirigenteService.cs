using ExcelDataReader;
using Microsoft.EntityFrameworkCore;
using ScoutAsset.Server.Application.Common.Interfaces;
using ScoutAsset.Server.Application.Models.Dirigentes;
using ScoutAsset.Server.Application.Models.Resources;
using ScoutAsset.Server.Application.Services;
using ScoutAsset.Server.Domain.Entities;

namespace ScoutAsset.Server.Infrastructure.Services;

public class DirigenteService : IDirigenteService
{
    private readonly IApplicationDbContext _context;

    public DirigenteService(IApplicationDbContext context) => _context = context;

    public async Task<List<Dirigente>> GetAllAsync() =>
        await _context.Dirigentes.Where(d => d.IsActive).OrderBy(d => d.Apellidos).ThenBy(d => d.Nombres).ToListAsync();

    public async Task<Dirigente?> GetByIdAsync(int id) =>
        await _context.Dirigentes.FindAsync(id);

    public async Task<Dirigente> CreateAsync(CreateDirigenteRequest request)
    {
        var dirigente = new Dirigente
        {
            CI = request.CI,
            Nombres = request.Nombres,
            Apellidos = request.Apellidos,
            Correo = request.Correo,
            Telefono = request.Telefono,
            HabilitadoParaCustodio = request.HabilitadoParaCustodio
        };

        _context.Dirigentes.Add(dirigente);
        await _context.SaveChangesAsync();
        return dirigente;
    }

    public async Task UpdateAsync(int id, CreateDirigenteRequest request)
    {
        var dirigente = await _context.Dirigentes.FindAsync(id)
            ?? throw new KeyNotFoundException("Dirigente no encontrado");

        dirigente.CI = request.CI;
        dirigente.Nombres = request.Nombres;
        dirigente.Apellidos = request.Apellidos;
        dirigente.Correo = request.Correo;
        dirigente.Telefono = request.Telefono;
        dirigente.HabilitadoParaCustodio = request.HabilitadoParaCustodio;
        dirigente.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var dirigente = await _context.Dirigentes.FindAsync(id)
            ?? throw new KeyNotFoundException("Dirigente no encontrado");

        dirigente.IsActive = false;
        dirigente.UpdatedAt = DateTime.UtcNow;

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

    public async Task<ResourceImportResult> ImportDirigentesAsync(Stream stream, string fileName)
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

        for (int i = 0; i < table.Rows.Count; i++)
        {
            var row = table.Rows[i];
            int rowNum = i + 2;

            try
            {
                string ci = GetColumnValue(row, "Cédula", "Cedula", "CI");
                string nombres = GetColumnValue(row, "Nombres", "Nombre");
                string apellidos = GetColumnValue(row, "Apellidos", "Apellido");
                string correo = GetColumnValue(row, "Correo", "Email", "Correo Electrónico");
                string telefono = GetColumnValue(row, "Teléfono", "Telefono", "Phone");
                string custodioStr = GetColumnValue(row, "Custodio", "HabilitadoCustodio", "Habilitado");

                if (string.IsNullOrWhiteSpace(nombres))
                {
                    result.Errors.Add($"Fila {rowNum}: El nombre es obligatorio.");
                    result.ErrorCount++;
                    continue;
                }

                bool habilitadoCustodio = true;
                if (!string.IsNullOrWhiteSpace(custodioStr))
                {
                    var val = custodioStr.Trim().ToLower();
                    if (val == "no" || val == "false" || val == "0")
                        habilitadoCustodio = false;
                }

                // Create Dirigente record
                var request = new CreateDirigenteRequest(ci, nombres, apellidos, correo, telefono, habilitadoCustodio);
                await CreateAsync(request);

                result.SuccessCount++;
                result.SuccessMessages.Add($"Dirigente '{nombres} {apellidos}' creado correctamente.");
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
}
