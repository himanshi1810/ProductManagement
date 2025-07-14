using ProductManagement.API.Models;

namespace ProductManagement.API.Services.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> GetAllAsync();
        Task<Customer?> GetByIdAsync(int id);
        Task AddAsync(Customer customer);
        Task<bool> UpdateAsync(Customer customer);
        Task<bool> DeleteAsync(int id);
    }
}
