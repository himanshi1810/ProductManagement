using Moq;
using ProductManagement.API.Models.DTOs;
using ProductManagement.API.Models;
using ProductManagement.API.Repositories.Interfaces;
using ProductManagement.API.Repositories.UnitOfWork;
using ProductManagement.API.Services.Implementations;
using System.Linq.Expressions;

namespace ProductManagement.Test.Services
{
    public class InvoiceServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IGenericRepository<Product>> _mockProductRepo;
        private readonly Mock<IGenericRepository<ProductPrice>> _mockPriceRepo;
        private readonly Mock<IGenericRepository<Invoice>> _mockInvoiceRepo;
        private readonly InvoiceService _invoiceService;

        public InvoiceServiceTests()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _mockProductRepo = new Mock<IGenericRepository<Product>>();
            _mockPriceRepo = new Mock<IGenericRepository<ProductPrice>>();
            _mockInvoiceRepo = new Mock<IGenericRepository<Invoice>>();

            _mockUow.Setup(u => u.Products).Returns(_mockProductRepo.Object);
            _mockUow.Setup(u => u.ProductPrices).Returns(_mockPriceRepo.Object);
            _mockUow.Setup(u => u.Invoices).Returns(_mockInvoiceRepo.Object);

            _invoiceService = new InvoiceService(_mockUow.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsInvoices()
        {
            var invoices = new List<Invoice> { new Invoice { InvoiceId = 1 }, new Invoice { InvoiceId = 2 } };

            _mockInvoiceRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(invoices);

            var result = await _invoiceService.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsInvoice_IfExists()
        {
            var invoice = new Invoice { InvoiceId = 1 };
            _mockInvoiceRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(invoice);

            var result = await _invoiceService.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(1, result.InvoiceId);
        }

        [Fact]
        public async Task CreateInvoiceAsync_ThrowsException_IfProductNotFound()
        {
            var request = new InvoiceRequestDto
            {
                CustomerId = 10,
                Items = new List<InvoiceItemDto>
                {   
                    new InvoiceItemDto { ProductId = 100, Quantity = 2 }
                }
            };

            _mockProductRepo.Setup(r => r.GetByIdAsync(100)).ReturnsAsync((Product)null);

            await Assert.ThrowsAsync<Exception>(() => _invoiceService.CreateInvoiceAsync(request));
        }

        [Fact]
        public async Task CreateInvoiceAsync_CreatesInvoiceCorrectly()
        {
            // Arrange
            var request = new InvoiceRequestDto
            {
                CustomerId = 1,
                Items = new List<InvoiceItemDto>
                {
                    new InvoiceItemDto { ProductId = 1, Quantity = 2 }
                }   
            };

            var today = DateTime.UtcNow.Date;

            var product = new Product { ProductId = 1, Name = "Pen", Tax = 10 };
            var priceList = new List<ProductPrice>
            {
                new ProductPrice { ProductId = 1, Price = 50, FromDate = today.AddDays(-1), ToDate = today.AddDays(1) }
            };

            _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            _mockPriceRepo.Setup(r =>
                r.FindAsync(It.IsAny<Expression<Func<ProductPrice, bool>>>())
            ).ReturnsAsync(priceList);

            // Act
            var result = await _invoiceService.CreateInvoiceAsync(request);

            // Assert
            _mockInvoiceRepo.Verify(r => r.AddAsync(It.IsAny<Invoice>()), Times.Once);
            _mockUow.Verify(u => u.SaveChangesAsync(), Times.Once);

            Assert.Equal(1, result.InvoiceDetails.Count);
            var detail = result.InvoiceDetails.First();

            Assert.Equal(50, detail.Rate);
            Assert.Equal(100, detail.SubTotal);
            Assert.Equal(10, detail.TaxAmount);
            Assert.Equal(110, detail.TotalAmount);
            Assert.Equal(110, result.Total);
        }
    }
}
