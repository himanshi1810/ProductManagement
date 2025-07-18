namespace ProductManagement.API.Models.DTOs
{
    public class ProductPriceDto
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool IsDefault { get; set; }
    }
}
