using ProductManagement.API.Models;

namespace ProductManagement.API.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task AddCategoryAsync(Category category);
        Task<bool> UpdateCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
