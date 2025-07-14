using Moq;
using ProductManagement.API.Models;
using ProductManagement.API.Repositories.Interfaces;
using ProductManagement.API.Repositories.UnitOfWork;
using ProductManagement.API.Services.Implementations;

namespace ProductManagement.Test.Services
{
    public class CustomerServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Customer>> _mockCustomerRepo;
        private readonly CustomerService _customerService;

        public CustomerServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCustomerRepo = new Mock<IGenericRepository<Customer>>();

            _mockUnitOfWork.Setup(u => u.Customers).Returns(_mockCustomerRepo.Object);

            _customerService = new CustomerService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllCustomers()
        {
            var customers = new List<Customer>
            {
                new Customer { CustomerId = 1, Name = "Alice" },
                new Customer { CustomerId = 2, Name = "Bob" }
            };

            _mockCustomerRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(customers);

            var result = await _customerService.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCustomer_IfExists()
        {
            var customer = new Customer { CustomerId = 1, Name = "Charlie" };

            _mockCustomerRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);

            var result = await _customerService.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Charlie", result.Name);
        }

        [Fact]
        public async Task AddAsync_AddsCustomer_AndSaves()
        {
            var customer = new Customer { CustomerId = 3, Name = "David" };

            await _customerService.AddAsync(customer);

            _mockCustomerRepo.Verify(r => r.AddAsync(customer), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsFalse_IfCustomerNotFound()
        {
            _mockCustomerRepo.Setup(r => r.GetByIdAsync(99)).ReturnsAsync((Customer?)null);

            var result = await _customerService.UpdateAsync(new Customer { CustomerId = 99 });

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesAndSaves_IfCustomerExists()
        {
            var existing = new Customer { CustomerId = 1, Name = "Old", Email = "old@example.com" };

            _mockCustomerRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

            var updated = new Customer { CustomerId = 1, Name = "New", Email = "new@example.com" };

            var result = await _customerService.UpdateAsync(updated);

            Assert.True(result);
            Assert.Equal("New", existing.Name);
            Assert.Equal("new@example.com", existing.Email);
            _mockCustomerRepo.Verify(r => r.Update(existing), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_IfCustomerNotFound()
        {
            _mockCustomerRepo.Setup(r => r.GetByIdAsync(404)).ReturnsAsync((Customer?)null);

            var result = await _customerService.DeleteAsync(404);

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_DeletesAndSaves_IfCustomerExists()
        {
            var customer = new Customer { CustomerId = 1, Name = "DeleteMe" };

            _mockCustomerRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(customer);

            var result = await _customerService.DeleteAsync(1);

            Assert.True(result);
            _mockCustomerRepo.Verify(r => r.Delete(customer), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
