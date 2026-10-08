using ScoutAsset.Server.Application.Models.Categories;
using ScoutAsset.Server.Domain.Entities;

using ScoutAsset.Server.Application.Models.Resources;

namespace ScoutAsset.Server.Application.Services;

public interface ICategoryService
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<Category> CreateAsync(CreateCategoryRequest request);
    Task UpdateAsync(int id, CreateCategoryRequest request);
    Task DeleteAsync(int id);
    Task<ResourceImportResult> ImportCategoriesAsync(Stream stream, string fileName);
}
