using ProductManagement.API.Models;
using ProductManagement.API.Repositories.Implementations;
using ProductManagement.API.Repositories.Interfaces;

namespace ProductManagement.API.Repositories.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public IGenericRepository<Category> Categories => new GenericRepository<Category>(_context);
        public IGenericRepository<Product> Products => new GenericRepository<Product>(_context);
        public IGenericRepository<ProductPrice> ProductPrices => new GenericRepository<ProductPrice>(_context);
        public IGenericRepository<Customer> Customers => new GenericRepository<Customer>(_context);
        public IGenericRepository<Invoice> Invoices => new GenericRepository<Invoice>(_context);
        public IGenericRepository<InvoiceDetail> InvoiceDetails => new GenericRepository<InvoiceDetail>(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
