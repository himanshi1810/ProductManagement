namespace ProductManagement.API.Models
{
    public class Customer
    {
        public int CustomerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public ICollection<Invoice>? Invoices { get; set; }
    }
}
