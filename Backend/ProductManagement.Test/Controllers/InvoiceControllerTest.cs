
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagement.API.Controllers;
using ProductManagement.API.Models.DTOs;
using ProductManagement.API.Models;
using ProductManagement.API.Services.Interfaces;

namespace ProductManagement.Test.Controllers
{
    public class InvoiceControllerTests
    {
        private readonly Mock<IInvoiceService> _mockService;
        private readonly InvoiceController _controller;

        public InvoiceControllerTests()
        {
            _mockService = new Mock<IInvoiceService>();
            _controller = new InvoiceController(_mockService.Object);
        }

        [Fact]
        public async Task Get_ReturnsAllInvoices()
        {
            var invoices = new List<Invoice> { new Invoice { InvoiceId = 1 }, new Invoice { InvoiceId = 2 } };

            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(invoices);

            var result = await _controller.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnInvoices = Assert.IsAssignableFrom<IEnumerable<Invoice>>(okResult.Value);

            Assert.Equal(2, returnInvoices.Count());
        }

        [Fact]
        public async Task GetById_ReturnsInvoice_IfExists()
        {
            var invoice = new Invoice { InvoiceId = 1 };

            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(invoice);

            var result = await _controller.Get(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<Invoice>(ok.Value);
            Assert.Equal(1, returned.InvoiceId);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_IfNotExists()
        {
            _mockService.Setup(s => s.GetByIdAsync(10)).ReturnsAsync((Invoice)null);

            var result = await _controller.Get(10);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Post_CreatesInvoiceAndReturnsIt()
        {
            var request = new InvoiceRequestDto
            {
                CustomerId = 1,
                Items = new List<InvoiceItemDto>
                {
                    new InvoiceItemDto { ProductId = 1, Quantity = 2 }
                }
            };

            var invoice = new Invoice
            {
                InvoiceId = 1,
                CustomerId = 1,
                Total = 200,
                InvoiceDetails = new List<InvoiceDetail>
                {
                    new InvoiceDetail { ProductId = 1, Quantity = 2, Rate = 100 }
                }
            };

            _mockService.Setup(s => s.CreateInvoiceAsync(request)).ReturnsAsync(invoice);

            var result = await _controller.Post(request);

            var ok = Assert.IsType<OkObjectResult>(result);
            var createdInvoice = Assert.IsType<Invoice>(ok.Value);
            Assert.Equal(1, createdInvoice.InvoiceId);
            Assert.Equal(200, createdInvoice.Total);
        }
    }
}
