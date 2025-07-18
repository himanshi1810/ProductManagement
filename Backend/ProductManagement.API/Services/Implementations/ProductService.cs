using ProductManagement.API.Models;
using ProductManagement.API.Models.DTOs;
using ProductManagement.API.Repositories.UnitOfWork;
using ProductManagement.API.Services.Interfaces;

namespace ProductManagement.API.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _unitOfWork.Products.GetAllAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Products.GetByIdAsync(id);
        }

        public async Task AddAsync(Product product)
        {
            await _unitOfWork.Products.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            var existing = await _unitOfWork.Products.GetByIdAsync(product.ProductId);
            if (existing == null) return false;

            existing.Name = product.Name;
            existing.Tax = product.Tax;
            existing.CategoryId = product.CategoryId;

            _unitOfWork.Products.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var existing = await _unitOfWork.Products.GetByIdAsync(id);
            if (existing == null) return false;

            _unitOfWork.Products.Delete(existing);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<decimal?> GetPriceForTodayAsync(int productId)
        {
            var today = DateTime.UtcNow.Date;

            var prices = await _unitOfWork.ProductPrices.FindAsync(p =>
                p.ProductId == productId &&
                !p.IsDefault &&
                p.FromDate.HasValue &&
                p.ToDate.HasValue &&
                today >= p.FromDate.Value.Date &&
                today <= p.ToDate.Value.Date);

            var priceForToday = prices.FirstOrDefault();
            if (priceForToday != null)
                return priceForToday.Price;

            var defaultPrice = (await _unitOfWork.ProductPrices
                .FindAsync(p => p.ProductId == productId && p.IsDefault))
                .FirstOrDefault();

            return defaultPrice?.Price;
        }

        public async Task AddPriceAsync(ProductPriceDto priceDto)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(priceDto.ProductId);
            if (product == null)
                throw new Exception("Product not found");

            if (priceDto.IsDefault)
            {
                var defaultPrice = await _unitOfWork.ProductPrices
                    .FindAsync(p => p.ProductId == priceDto.ProductId && p.IsDefault);

                if (defaultPrice.Any())
                    throw new Exception("Default price already exists for this product.");

                var price = new ProductPrice
                {
                    ProductId = priceDto.ProductId,
                    Price = priceDto.Price,
                    IsDefault = true,
                    FromDate = null,
                    ToDate = null
                };

                await _unitOfWork.ProductPrices.AddAsync(price);
            }
            else
            {
                if (!priceDto.FromDate.HasValue || !priceDto.ToDate.HasValue)
                    throw new Exception("FromDate and ToDate must be provided for non-default price.");

                if (priceDto.FromDate >= priceDto.ToDate)
                    throw new Exception("FromDate must be earlier than ToDate.");

                var existingPrices = await _unitOfWork.ProductPrices
                    .FindAsync(p => p.ProductId == priceDto.ProductId && !p.IsDefault);

                bool isOverlapping = existingPrices.Any(p =>
                    (priceDto.FromDate <= p.ToDate) && (priceDto.ToDate >= p.FromDate));

                if (isOverlapping)
                    throw new Exception("A price already exists for this product in the given date range.");

                var price = new ProductPrice
                {
                    ProductId = priceDto.ProductId,
                    Price = priceDto.Price,
                    FromDate = priceDto.FromDate,
                    ToDate = priceDto.ToDate,
                    IsDefault = false
                };

                await _unitOfWork.ProductPrices.AddAsync(price);
            }

            await _unitOfWork.SaveChangesAsync();
        }

    }

}
