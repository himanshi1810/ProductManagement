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

        public async Task<IEnumerable<InvoiceDetail>> GetInvoiceDetail()
        {
            return await _unitOfWork.InvoiceDetails.GetAllAsync();
        }

        public async Task<InvoiceDetail?> GetByIdInvoiceDetail(int id)
        {
            return await _unitOfWork.InvoiceDetails.GetByIdAsync(id);
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
                              !p.IsDefault &&
                              p.FromDate != null &&
                              p.ToDate != null &&
                              today >= p.FromDate.Value.Date &&
                              today <= p.ToDate.Value.Date))
                              .FirstOrDefault()?.Price;

                if (price == null || price == 0)
                {
                    price = (await _unitOfWork.ProductPrices
                             .FindAsync(p => p.ProductId == item.ProductId && p.IsDefault))
                             .FirstOrDefault()?.Price ?? 0;
                }

                var qty = item.Quantity;
                var subTotal = price * qty;
                var taxAmt = subTotal * (product.Tax / 100);
                var totalAmt = subTotal + taxAmt;

                invoice.InvoiceDetails.Add(new InvoiceDetail
                {
                    ProductId = item.ProductId,
                    Quantity = qty,
                    Rate = (decimal)price,
                    SubTotal = (decimal)subTotal,
                    TaxAmount = (decimal)taxAmt,
                    TotalAmount = (decimal)totalAmt
                });

                grandTotal += (decimal)totalAmt;
            }

            invoice.Total = grandTotal;

            await _unitOfWork.Invoices.AddAsync(invoice);
            await _unitOfWork.SaveChangesAsync();

            return invoice;
        }

    }
}
