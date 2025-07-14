using Microsoft.AspNetCore.Mvc;
using ProductManagement.API.Models;
using ProductManagement.API.Models.DTOs;
using ProductManagement.API.Services.Interfaces;

namespace ProductManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductCreateDto dto)
        {
            var product = new Product
            {
                Name = dto.Name,
                Tax = dto.Tax,
                CategoryId = dto.CategoryId
            };

            await _productService.AddAsync(product);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Product product)
        {
            var result = await _productService.UpdateAsync(product);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _productService.DeleteAsync(id);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpGet("{id}/price-today")]
        public async Task<IActionResult> GetPriceForToday(int id)
        {
            var price = await _productService.GetPriceForTodayAsync(id);
            if (price == null) return NotFound("No price available for today.");
            return Ok(price);
        }

        [HttpPost("add-price")]
        public async Task<IActionResult> AddPrice([FromBody] ProductPriceDto priceDto)
        {
            try
            {
                await _productService.AddPriceAsync(priceDto);
                return Ok("Price added successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
