using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagement.API.Controllers;
using ProductManagement.API.Models.DTOs;
using ProductManagement.API.Models;
using ProductManagement.API.Services.Interfaces;

namespace ProductManagement.Test.Controllers
{
    public class ProductControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _controller = new ProductController(_mockService.Object);
        }

        [Fact]
        public async Task Get_ReturnsOk_WithProducts()
        {
            var products = new List<Product> { new Product { ProductId = 1, Name = "Pen" } };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(products);

            var result = await _controller.Get();

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsAssignableFrom<IEnumerable<Product>>(ok.Value);
            Assert.Single(value);
        }

        [Fact]
        public async Task GetById_ReturnsOk_IfProductExists()
        {
            var product = new Product { ProductId = 1, Name = "Marker" };
            _mockService.Setup(s => s.GetByIdAsync(1)).ReturnsAsync(product);

            var result = await _controller.Get(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<Product>(ok.Value);
            Assert.Equal("Marker", value.Name);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_IfNotExists()
        {
            _mockService.Setup(s => s.GetByIdAsync(99)).ReturnsAsync((Product)null);

            var result = await _controller.Get(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Post_AddsProductAndReturnsOk()
        {
            var dto = new ProductCreateDto
            {
                Name = "New Pen",
                Tax = 5,
                CategoryId = 1
            };

            var result = await _controller.Post(dto);

            _mockService.Verify(s => s.AddAsync(It.Is<Product>(
                p => p.Name == dto.Name && p.Tax == dto.Tax && p.CategoryId == dto.CategoryId)), Times.Once);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Put_UpdatesProduct_IfExists()
        {
            var product = new Product { ProductId = 1, Name = "Pen" };
            _mockService.Setup(s => s.UpdateAsync(product)).ReturnsAsync(true);

            var result = await _controller.Put(product);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Put_ReturnsNotFound_IfNotExists()
        {
            var product = new Product { ProductId = 99 };
            _mockService.Setup(s => s.UpdateAsync(product)).ReturnsAsync(false);

            var result = await _controller.Put(product);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_DeletesProduct_IfExists()
        {
            _mockService.Setup(s => s.DeleteAsync(1)).ReturnsAsync(true);

            var result = await _controller.Delete(1);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_IfNotExists()
        {
            _mockService.Setup(s => s.DeleteAsync(5)).ReturnsAsync(false);

            var result = await _controller.Delete(5);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetPriceForToday_ReturnsOk_IfExists()
        {
            _mockService.Setup(s => s.GetPriceForTodayAsync(1)).ReturnsAsync(150);

            var result = await _controller.GetPriceForToday(1);

            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(150m, ok.Value);
        }

        [Fact]
        public async Task GetPriceForToday_ReturnsNotFound_IfNull()
        {
            _mockService.Setup(s => s.GetPriceForTodayAsync(1)).ReturnsAsync((decimal?)null);

            var result = await _controller.GetPriceForToday(1);

            var notFound = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("No price available for today.", notFound.Value);
        }

        [Fact]
        public async Task AddPrice_ReturnsOk_IfSuccess()
        {
            var dto = new ProductPriceDto
            {
                ProductId = 1,
                Price = 99,
                FromDate = DateTime.UtcNow,
                ToDate = DateTime.UtcNow.AddDays(3)
            };

            var result = await _controller.AddPrice(dto);

            _mockService.Verify(s => s.AddPriceAsync(dto), Times.Once);
            var ok = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Price added successfully.", ok.Value);
        }

        [Fact]
        public async Task AddPrice_ReturnsBadRequest_IfThrows()
        {
            var dto = new ProductPriceDto { ProductId = 999 };

            _mockService.Setup(s => s.AddPriceAsync(dto)).ThrowsAsync(new Exception("Product not found"));

            var result = await _controller.AddPrice(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Product not found", badRequest.Value);
        }
    }
}
