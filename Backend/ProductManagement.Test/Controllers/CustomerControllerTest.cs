using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagement.API.Controllers;
using ProductManagement.API.Models;
using ProductManagement.API.Services.Interfaces;

namespace ProductManagement.Test.Controllers
{
    public class CustomerControllerTest
    {
        private readonly Mock<ICustomerService> _mockCustomerService;
        private readonly CustomerController _customerController;
        public CustomerControllerTest()
        {
            _mockCustomerService = new Mock<ICustomerService>();
            _customerController = new CustomerController(_mockCustomerService.Object);
        }

        [Fact]
        public async Task Get_ReturnsOkResult_WithCustomers()
        {
            var customers = new List<Customer>
            {
                new Customer { CustomerId = 1, Name = "Alice" },
                new Customer { CustomerId = 2, Name = "Bob" }
            };
            _mockCustomerService.Setup(s => s.GetAllAsync()).ReturnsAsync(customers);

            var result = await _customerController.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCustomers = Assert.IsAssignableFrom<IEnumerable<Customer>>(okResult.Value);
            Assert.Equal(2, returnedCustomers.Count());
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WithCustomer()
        {
            var customer = new Customer { CustomerId = 1, Name = "Charlie" };
            _mockCustomerService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(customer);

            var result = await _customerController.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCustomer = Assert.IsType<Customer>(okResult.Value);
            Assert.Equal("Charlie", returnedCustomer.Name);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenCustomerDoesNotExist()
        {
            _mockCustomerService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync((Customer)null);

            var result = await _customerController.Get(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_IfMissing()
        {
            _mockCustomerService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Customer)null);

            var result = await _customerController.Get(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Post_ReturnsOk()
        {
            var customer = new Customer { Name = "David", Email = "david@gmail.com" };

            var result = await _customerController.Post(customer);

            _mockCustomerService.Verify(s => s.AddAsync(customer), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Put_ReturnsOk_WhenCustomerUpdated()
        {
            var customer = new Customer { CustomerId = 1, Name = "Eve", Email = "eve@gmail.com" };
            _mockCustomerService.Setup(s => s.UpdateAsync(customer)).ReturnsAsync(true);

            var result = await _customerController.Put(customer);

            _mockCustomerService.Verify(s => s.UpdateAsync(customer), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Put_ReturnsNotFound_WhenCustomerNotFound()
        {
            var customer = new Customer { CustomerId = 99, Name = "Frank", Email = "frank@gmail.com" };
            _mockCustomerService.Setup(s => s.UpdateAsync(customer)).ReturnsAsync(false);

            var result = await _customerController.Put(customer);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsOk_WhenCustomerDeleted()
        {
            _mockCustomerService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _customerController.Delete(1);

            _mockCustomerService.Verify(s => s.DeleteAsync(1), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_WhenCustomerNotFound()
        {
            _mockCustomerService.Setup(s => s.DeleteAsync(99)).ReturnsAsync(false);

            var result = await _customerController.Delete(99);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
