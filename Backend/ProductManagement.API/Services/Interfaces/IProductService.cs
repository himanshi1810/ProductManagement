using ProductManagement.API.Models;
using ProductManagement.API.Models.DTOs;

namespace ProductManagement.API.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetByIdAsync(int id);
        Task AddPriceAsync(ProductPriceDto priceDto);
        Task<decimal?> GetPriceForTodayAsync(int productId); 
        Task AddAsync(Product product);
        Task<bool> UpdateAsync(Product product);
        Task<bool> DeleteAsync(int id);
    }
}
