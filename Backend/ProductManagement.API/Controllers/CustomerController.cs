using Microsoft.AspNetCore.Mvc;
using ProductManagement.API.Models;
using ProductManagement.API.Services.Interfaces;

namespace ProductManagement.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var customers = await _customerService.GetAllAsync();
            return Ok(customers);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var customer = await _customerService.GetByIdAsync(id);
            if (customer == null)
                return NotFound();
            return Ok(customer);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            await _customerService.AddAsync(customer);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromBody] Customer customer)
        {
            var result = await _customerService.UpdateAsync(customer);
            if (!result) return NotFound();
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _customerService.DeleteAsync(id);
            if (!result) return NotFound();
            return Ok();
        }
    }
}
