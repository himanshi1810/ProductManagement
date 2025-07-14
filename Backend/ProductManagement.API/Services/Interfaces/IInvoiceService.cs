using ProductManagement.API.Models.DTOs;
using ProductManagement.API.Models;

namespace ProductManagement.API.Services.Interfaces
{
    public interface IInvoiceService
    {
        Task<Invoice> CreateInvoiceAsync(InvoiceRequestDto request);
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task<Invoice?> GetByIdAsync(int id);
    }
}
