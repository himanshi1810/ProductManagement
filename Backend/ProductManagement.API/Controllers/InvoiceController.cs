using Microsoft.AspNetCore.Mvc;
using ProductManagement.API.Models.DTOs;
using ProductManagement.API.Services.Interfaces;

namespace ProductManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoiceController : ControllerBase
    {
        private readonly IInvoiceService _invoiceService;

        public InvoiceController(IInvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var invoices = await _invoiceService.GetAllAsync();
            return Ok(invoices);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null)
                return NotFound();
            return Ok(invoice);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] InvoiceRequestDto request)
        {
            var invoice = await _invoiceService.CreateInvoiceAsync(request);
            return Ok(invoice);
        }
    }
}
