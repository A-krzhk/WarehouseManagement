using WarehouseManagement.Core.DTO;

namespace WarehouseManagement.Core.Interfaces;

public interface IProductService
{
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int id);
    Task<ProductDto> CreateProductAsync(CreateProductDto productDto);
    Task UpdateProductAsync(int id, UpdateProductDto productDto);
    Task DeleteProductAsync(int id);
    Task<bool> ProductExistsAsync(int id);
}