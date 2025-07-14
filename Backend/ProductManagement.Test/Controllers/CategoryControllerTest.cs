using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductManagement.API.Controllers;
using ProductManagement.API.Models;
using ProductManagement.API.Services.Interfaces;

namespace ProductManagement.Test.Controllers
{
    public class CategoryControllerTest
    {
        private readonly Mock<ICategoryService> _mockCategoryService;
        private readonly CategoryController _categoryController;

        public CategoryControllerTest()
        {
            _mockCategoryService = new Mock<ICategoryService>();
            _categoryController = new CategoryController(_mockCategoryService.Object);
        }

        [Fact]
        public async Task Get_ReturnsOkResult_WithCategories()
        {
            var categories = new List<Category>
            {
                new Category { CategoryId = 1, Name = "Stationery" },
                new Category { CategoryId = 2, Name = "Electronics" }
            };
            _mockCategoryService.Setup(s => s.GetAllCategoriesAsync()).ReturnsAsync(categories);

            var result = await _categoryController.Get();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCategories = Assert.IsAssignableFrom<IEnumerable<Category>>(okResult.Value);
            Assert.Equal(2, returnedCategories.Count());
        }

        [Fact]
        public async Task GetById_ReturnsOkResult_WithCategory()
        {
            var category = new Category { CategoryId = 1, Name = "Books" };
            _mockCategoryService.Setup(s => s.GetCategoryByIdAsync(1)).ReturnsAsync(category);

            var result = await _categoryController.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCategory = Assert.IsType<Category>(okResult.Value);
            Assert.Equal("Books", returnedCategory.Name);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenCategoryDoesNotExist()
        {
            var category = new Category { CategoryId = 1, Name = "Tech" };

            _mockCategoryService.Setup(s => s.GetCategoryByIdAsync(1)).ReturnsAsync(category);

            var result = await _categoryController.Get(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returned = Assert.IsType<Category>(okResult.Value);
            Assert.Equal("Tech", returned.Name);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_IfMissing()
        {
            _mockCategoryService.Setup(s => s.GetCategoryByIdAsync(99)).ReturnsAsync((Category)null);

            var result = await _categoryController.Get(99);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Post_ReturnsOk()
        {
            var category = new Category { Name = "Gadgets" };

            var result = await _categoryController.Post(category);

            _mockCategoryService.Verify(s => s.AddCategoryAsync(category), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Put_ReturnsOk_IfUpdated()
        {
            var category = new Category { CategoryId = 1, Name = "Updated" };

            _mockCategoryService.Setup(s => s.UpdateCategoryAsync(category)).ReturnsAsync(true);

            var result = await _categoryController.Put(category);

            _mockCategoryService.Verify(s => s.UpdateCategoryAsync(category), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Put_ReturnsNotFound_IfNotFound()
        {
            var category = new Category { CategoryId = 1, Name = "Updated" };

            _mockCategoryService.Setup(s => s.UpdateCategoryAsync(category)).ReturnsAsync(false);

            var result = await _categoryController.Put(category);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsOk_IfDeleted()
        {
            _mockCategoryService.Setup(s => s.DeleteCategoryAsync(1)).ReturnsAsync(true);

            var result = await _categoryController.Delete(1);

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task Delete_ReturnsNotFound_IfMissing()
        {
            _mockCategoryService.Setup(s => s.DeleteCategoryAsync(100)).ReturnsAsync(false);

            var result = await _categoryController.Delete(100);

            Assert.IsType<NotFoundResult>(result);
        }
    }
}
