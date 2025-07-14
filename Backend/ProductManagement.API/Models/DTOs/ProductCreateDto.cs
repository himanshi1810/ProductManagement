namespace ProductManagement.API.Models.DTOs
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public decimal Tax { get; set; }
        public int CategoryId { get; set; }
    }
}
