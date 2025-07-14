using Moq;
using ProductManagement.API.Models.DTOs;
using ProductManagement.API.Models;
using ProductManagement.API.Repositories.Interfaces;
using ProductManagement.API.Repositories.UnitOfWork;
using ProductManagement.API.Services.Implementations;
using System.Linq.Expressions;

namespace ProductManagement.Test.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Product>> _mockProductRepo;
        private readonly Mock<IGenericRepository<ProductPrice>> _mockPriceRepo;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockProductRepo = new Mock<IGenericRepository<Product>>();
            _mockPriceRepo = new Mock<IGenericRepository<ProductPrice>>();

            _mockUnitOfWork.Setup(u => u.Products).Returns(_mockProductRepo.Object);
            _mockUnitOfWork.Setup(u => u.ProductPrices).Returns(_mockPriceRepo.Object);

            _productService = new ProductService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllProducts()
        {
            var products = new List<Product>
        {
            new Product { ProductId = 1, Name = "Pen" },
            new Product { ProductId = 2, Name = "Pencil" }
        };

            _mockProductRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

            var result = await _productService.GetAllAsync();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllAsync_ReturnsEmptyList_IfNoProductsExist()
        {
            _mockProductRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(new List<Product>());

            var result = await _productService.GetAllAsync();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsProduct_IfExists()
        {
            var product = new Product { ProductId = 1, Name = "Marker" };
            _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            var result = await _productService.GetByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Marker", result.Name);
        }

        [Fact]
        public async Task AddAsync_AddsProduct_AndSaves()
        {
            var product = new Product { ProductId = 3, Name = "Eraser" };

            await _productService.AddAsync(product);

            _mockProductRepo.Verify(r => r.AddAsync(product), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ReturnsFalse_IfProductNotFound()
        {
            _mockProductRepo.Setup(r => r.GetByIdAsync(10)).ReturnsAsync((Product)null);

            var result = await _productService.UpdateAsync(new Product { ProductId = 10 });

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateAsync_UpdatesProduct_IfExists()
        {
            var product = new Product { ProductId = 1, Name = "Old" };
            _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            var updated = new Product { ProductId = 1, Name = "New", Tax = 18, CategoryId = 2 };

            var result = await _productService.UpdateAsync(updated);

            Assert.True(result);
            _mockProductRepo.Verify(r => r.Update(It.Is<Product>(p => p.Name == "New")), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsFalse_IfNotFound()
        {
            _mockProductRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync((Product)null);

            var result = await _productService.DeleteAsync(5);

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_DeletesProduct_IfFound()
        {
            var product = new Product { ProductId = 1 };
            _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            var result = await _productService.DeleteAsync(1);

            Assert.True(result);
            _mockProductRepo.Verify(r => r.Delete(product), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetPriceForTodayAsync_ReturnsPrice_IfMatchExists()
        {
            var today = DateTime.UtcNow.Date;
            var prices = new List<ProductPrice>
        {
            new ProductPrice
            {
                ProductId = 1,
                Price = 150,
                FromDate = today.AddDays(-1),
                ToDate = today.AddDays(1)
            }
        };

            _mockPriceRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ProductPrice, bool>>>()))
                          .ReturnsAsync(prices);

            var result = await _productService.GetPriceForTodayAsync(1);

            Assert.Equal(150, result);
        }

        [Fact]
        public async Task AddPriceAsync_ThrowsException_IfProductNotFound()
        {
            _mockProductRepo.Setup(r => r.GetByIdAsync(100)).ReturnsAsync((Product)null);

            var dto = new ProductPriceDto
            {
                ProductId = 100,
                Price = 99,
                FromDate = DateTime.UtcNow,
                ToDate = DateTime.UtcNow.AddDays(7)
            };

            await Assert.ThrowsAsync<Exception>(() => _productService.AddPriceAsync(dto));
        }

        [Fact]
        public async Task AddPriceAsync_AddsPrice_IfProductExists()
        {
            var product = new Product { ProductId = 1 };
            _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            var dto = new ProductPriceDto
            {
                ProductId = 1,
                Price = 99,
                FromDate = DateTime.UtcNow,
                ToDate = DateTime.UtcNow.AddDays(7)
            };

            await _productService.AddPriceAsync(dto);

            _mockPriceRepo.Verify(p => p.AddAsync(It.Is<ProductPrice>(x => x.Price == 99)), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
