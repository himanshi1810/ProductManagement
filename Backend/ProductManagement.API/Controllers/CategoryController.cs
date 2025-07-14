using Microsoft.AspNetCore.Mvc;
using ProductManagement.API.Models;
using ProductManagement.API.Services.Interfaces;

namespace ProductManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Category category)
        {
            await _categoryService.AddCategoryAsync(category);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Category category)
        {
            var result = await _categoryService.UpdateCategoryAsync(category);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _categoryService.DeleteCategoryAsync(id);
            if (!result) return NotFound();
            return Ok();
        }
    }
}
