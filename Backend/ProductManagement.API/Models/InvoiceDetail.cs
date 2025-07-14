namespace ProductManagement.API.Models
{
    public class InvoiceDetail
    {
        public int InvoiceDetailId { get; set; }
        public int InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}