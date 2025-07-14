using ProductManagement.API.Models;
using ProductManagement.API.Repositories.UnitOfWork;
using ProductManagement.API.Services.Interfaces;

namespace ProductManagement.API.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _unitOfWork.Categories.GetAllAsync();
        }
        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _unitOfWork.Categories.GetByIdAsync(id);
        }
        public async Task AddCategoryAsync(Category category)
        {
            await _unitOfWork.Categories.AddAsync(category);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task<bool> UpdateCategoryAsync(Category category)
        {
            var existing = await _unitOfWork.Categories.GetByIdAsync(category.CategoryId);
            if (existing == null) return false;

            existing.Name = category.Name;
            _unitOfWork.Categories.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteCategoryAsync(int id)
        {
            var category = await _unitOfWork.Categories.GetByIdAsync(id);
            if (category == null) return false;

            _unitOfWork.Categories.Delete(category);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
