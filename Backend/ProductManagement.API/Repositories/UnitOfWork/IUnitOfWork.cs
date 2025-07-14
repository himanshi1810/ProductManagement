using ProductManagement.API.Models;
using ProductManagement.API.Repositories.Interfaces;

namespace ProductManagement.API.Repositories.UnitOfWork
{
    public interface IUnitOfWork
    {
        IGenericRepository<Category> Categories { get; }
        IGenericRepository<Product> Products { get; }
        IGenericRepository<ProductPrice> ProductPrices { get; }
        IGenericRepository<Customer> Customers { get; }
        IGenericRepository<Invoice> Invoices { get; }
        IGenericRepository<InvoiceDetail> InvoiceDetails { get; }
        Task<int> SaveChangesAsync();
    }
}
