using Moq;
using ProductManagement.API.Models;
using ProductManagement.API.Repositories.Interfaces;
using ProductManagement.API.Repositories.UnitOfWork;
using ProductManagement.API.Services.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductManagement.Test.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IGenericRepository<Category>> _mockCategoryRepo;
        private readonly CategoryService _categoryService;

        public CategoryServiceTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockCategoryRepo = new Mock<IGenericRepository<Category>>();

            _mockUnitOfWork.Setup(u => u.Categories).Returns(_mockCategoryRepo.Object);

            _categoryService = new CategoryService(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task GetAllCategoriesAsync_ReturnsAllCategories()
        {
            var categories = new List<Category>
        {
            new Category { CategoryId = 1, Name = "Stationery" },
            new Category { CategoryId = 2, Name = "Electronics" }
        };

            _mockCategoryRepo.Setup(r => r.GetAllAsync()).ReturnsAsync(categories);

            var result = await _categoryService.GetAllCategoriesAsync();

            Assert.Equal(2, result.Count());
            Assert.Contains(result, c => c.Name == "Stationery");
        }

        [Fact]
        public async Task GetCategoryByIdAsync_ReturnsCategory_IfFound()
        {
            var category = new Category { CategoryId = 1, Name = "Books" };

            _mockCategoryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);

            var result = await _categoryService.GetCategoryByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal("Books", result.Name);
        }

        [Fact]
        public async Task AddCategoryAsync_AddsCategory_AndSaves()
        {
            var newCategory = new Category { CategoryId = 3, Name = "Furniture" };

            await _categoryService.AddCategoryAsync(newCategory);

            _mockCategoryRepo.Verify(r => r.AddAsync(newCategory), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateCategoryAsync_ReturnsFalse_IfNotExists()
        {
            _mockCategoryRepo.Setup(r => r.GetByIdAsync(100)).ReturnsAsync((Category)null);

            var result = await _categoryService.UpdateCategoryAsync(new Category { CategoryId = 100 });

            Assert.False(result);
        }

        [Fact]
        public async Task UpdateCategoryAsync_UpdatesAndSaves_IfExists()
        {
            var existing = new Category { CategoryId = 1, Name = "Old Name" };

            _mockCategoryRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(existing);

            var updated = new Category { CategoryId = 1, Name = "Updated Name" };

            var result = await _categoryService.UpdateCategoryAsync(updated);

            Assert.True(result);
            _mockCategoryRepo.Verify(r => r.Update(It.Is<Category>(c => c.Name == "Updated Name")), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryAsync_ReturnsFalse_IfNotExists()
        {
            _mockCategoryRepo.Setup(r => r.GetByIdAsync(55)).ReturnsAsync((Category)null);

            var result = await _categoryService.DeleteCategoryAsync(55);

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteCategoryAsync_Deletes_IfExists()
        {
            var category = new Category { CategoryId = 5, Name = "Toys" };

            _mockCategoryRepo.Setup(r => r.GetByIdAsync(5)).ReturnsAsync(category);

            var result = await _categoryService.DeleteCategoryAsync(5);

            Assert.True(result);
            _mockCategoryRepo.Verify(r => r.Delete(category), Times.Once);
            _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        }
    }
}
