namespace ProductManagement.API.Models.DTOs
{
    public class InvoiceRequestDto
    {
        public int CustomerId { get; set; }
        public List<InvoiceItemDto> Items { get; set; }
    }
}
