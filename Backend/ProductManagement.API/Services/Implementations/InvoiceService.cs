using ProductManagement.API.Models.DTOs;
using ProductManagement.API.Models;
using ProductManagement.API.Repositories.UnitOfWork;
using ProductManagement.API.Services.Interfaces;

namespace ProductManagement.API.Services.Implementations
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _unitOfWork.Invoices.GetAllAsync();
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Invoices.GetByIdAsync(id);
        }

        public async Task<Invoice> CreateInvoiceAsync(InvoiceRequestDto request)
        {
            var invoice = new Invoice
            {
                InvoiceDate = DateTime.UtcNow,
                CustomerId = request.CustomerId,
                InvoiceDetails = new List<InvoiceDetail>()
            };

            decimal grandTotal = 0;
            var today = DateTime.UtcNow.Date;

            foreach (var item in request.Items)
            {
                var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new Exception($"Product with ID {item.ProductId} not found");

                var price = (await _unitOfWork.ProductPrices.FindAsync(p =>
                    p.ProductId == item.ProductId &&
                    today >= p.FromDate.Date && today <= p.ToDate.Date))
                    .FirstOrDefault()?.Price ?? 0;

                var qty = item.Quantity;
                var subTotal = price * qty;
                var taxAmt = subTotal * (product.Tax / 100);
                var totalAmt = subTotal + taxAmt;

                invoice.InvoiceDetails.Add(new InvoiceDetail
                {
                    ProductId = item.ProductId,
                    Quantity = qty,
                    Rate = price,
                    SubTotal = subTotal,
                    TaxAmount = taxAmt,
                    TotalAmount = totalAmt
                });

                grandTotal += totalAmt;
            }

            invoice.Total = grandTotal;

            await _unitOfWork.Invoices.AddAsync(invoice);
            await _unitOfWork.SaveChangesAsync();

            return invoice;
        }
    }
}
