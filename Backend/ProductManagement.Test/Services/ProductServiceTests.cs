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
        public async Task GetPriceForTodayAsync_ReturnsDefaultPrice_IfNoDateBasedPrice()
        {
            var today = DateTime.UtcNow.Date;

            _mockPriceRepo.SetupSequence(r => r.FindAsync(It.IsAny<Expression<Func<ProductPrice, bool>>>()))
                .ReturnsAsync(new List<ProductPrice>()) 
                .ReturnsAsync(new List<ProductPrice>
                {
            new ProductPrice
            {
                ProductId = 1,
                Price = 200,
                IsDefault = true
            }
                }); 

            var result = await _productService.GetPriceForTodayAsync(1);

            Assert.Equal(200, result);
        }

        [Fact]
        public async Task GetPriceForTodayAsync_ReturnsNull_IfNoPriceFound()
        {
            _mockPriceRepo.SetupSequence(r => r.FindAsync(It.IsAny<Expression<Func<ProductPrice, bool>>>()))
                .ReturnsAsync(new List<ProductPrice>()) // No date-based
                .ReturnsAsync(new List<ProductPrice>()); // No default

            var result = await _productService.GetPriceForTodayAsync(1);

            Assert.Null(result);
        }

        [Fact]
        public async Task AddPriceAsync_ThrowsIfDefaultPriceAlreadyExists()
        {
            var product = new Product { ProductId = 1 };
            _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            var existingDefault = new ProductPrice { ProductId = 1, IsDefault = true };

            _mockPriceRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ProductPrice, bool>>>()))
                .ReturnsAsync(new List<ProductPrice> { existingDefault });

            var dto = new ProductPriceDto
            {
                ProductId = 1,
                Price = 100,
                IsDefault = true
            };

            var ex = await Assert.ThrowsAsync<Exception>(() => _productService.AddPriceAsync(dto));
            Assert.Equal("Default price already exists for this product.", ex.Message);
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
        public async Task AddPriceAsync_AddsPrice_IfValidData()
        {
            var product = new Product { ProductId = 1 };
            _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            _mockPriceRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ProductPrice, bool>>>()))
                .ReturnsAsync(new List<ProductPrice>()); 

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

        [Fact]
        public async Task AddPriceAsync_AddsDefaultPrice_IfNoneExists()
        {
            var product = new Product { ProductId = 1 };
            _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            _mockPriceRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ProductPrice, bool>>>()))
                .ReturnsAsync(new List<ProductPrice>()); // No existing default

            var dto = new ProductPriceDto
            {
                ProductId = 1,
                Price = 250,
                IsDefault = true
            };

            await _productService.AddPriceAsync(dto);

            _mockPriceRepo.Verify(p => p.AddAsync(It.Is<ProductPrice>(x =>
                x.IsDefault && x.Price == 250 && x.FromDate == null && x.ToDate == null)), Times.Once);

            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddPriceAsync_ThrowsIfDatesMissing_ForNonDefaultPrice()
        {
            var product = new Product { ProductId = 1 };
            _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            var dto = new ProductPriceDto
            {
                ProductId = 1,
                Price = 100,
                IsDefault = false,
                FromDate = null,
                ToDate = null
            };

            var ex = await Assert.ThrowsAsync<Exception>(() => _productService.AddPriceAsync(dto));
            Assert.Equal("FromDate and ToDate must be provided for non-default price.", ex.Message);
        }

        [Fact]
        public async Task AddPriceAsync_ThrowsIfOverlappingPriceExists()
        {
            var product = new Product { ProductId = 1 };
            _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);

            var existingPrice = new ProductPrice
            {
                ProductId = 1,
                FromDate = DateTime.UtcNow.AddDays(1),
                ToDate = DateTime.UtcNow.AddDays(10)
            };

            _mockPriceRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ProductPrice, bool>>>()))
                .ReturnsAsync(new List<ProductPrice> { existingPrice });

            var dto = new ProductPriceDto
            {
                ProductId = 1,
                Price = 120,
                FromDate = DateTime.UtcNow.AddDays(5), 
                ToDate = DateTime.UtcNow.AddDays(15)
            };

            var ex = await Assert.ThrowsAsync<Exception>(() => _productService.AddPriceAsync(dto));
            Assert.Equal("A price already exists for this product in the given date range.", ex.Message);
        }

        [Fact]
        public async Task AddPriceAsync_ThrowsIfFromDateAfterToDate()
        {
            var product = new Product { ProductId = 1 };
            _mockProductRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(product);
            _mockPriceRepo.Setup(r => r.FindAsync(It.IsAny<Expression<Func<ProductPrice, bool>>>()))
                .ReturnsAsync(new List<ProductPrice>()); 

            var dto = new ProductPriceDto
            {
                ProductId = 1,
                Price = 120,
                FromDate = DateTime.UtcNow.AddDays(10),
                ToDate = DateTime.UtcNow.AddDays(5) 
            };

            var ex = await Assert.ThrowsAsync<Exception>(() => _productService.AddPriceAsync(dto));
            Assert.Equal("FromDate must be earlier than ToDate.", ex.Message);
        }

    }
}
