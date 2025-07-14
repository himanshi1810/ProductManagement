namespace ProductManagement.API.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; }
        public ICollection<ProductPrice>? ProductPrices { get; set; }
        public decimal Tax { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; } = null;
    }
}
