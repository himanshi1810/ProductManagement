using ProductManagement.API.Models;
using ProductManagement.API.Repositories.UnitOfWork;
using ProductManagement.API.Services.Interfaces;

namespace ProductManagement.API.Services.Implementations
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Customer>> GetAllAsync()
        {
            return await _unitOfWork.Customers.GetAllAsync();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Customers.GetByIdAsync(id);
        }

        public async Task AddAsync(Customer customer)
        {
            await _unitOfWork.Customers.AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Customer customer)
        {
            var existing = await _unitOfWork.Customers.GetByIdAsync(customer.CustomerId);
            if (existing == null) return false;

            existing.Name = customer.Name;
            existing.Email = customer.Email;

            _unitOfWork.Customers.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _unitOfWork.Customers.GetByIdAsync(id);
            if (existing == null) return false;

            _unitOfWork.Customers.Delete(existing);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }
    }
}
