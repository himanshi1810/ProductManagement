namespace ProductManagement.API.Models
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
        public decimal Total { get; set; }
        public ICollection<InvoiceDetail>? InvoiceDetails { get; set; }
    }
}