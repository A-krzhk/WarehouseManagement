using WarehouseManagement.Core.DTO;

namespace WarehouseManagement.Core.Interfaces;

public interface IProductLocationService
{
    Task<IEnumerable<ProductLocationDto>> GetAllProductLocationsAsync();
    Task<IEnumerable<ProductLocationDto>> GetProductLocationsByProductIdAsync(int productId);
    Task<IEnumerable<ProductLocationDto>> GetProductLocationsByLocationIdAsync(int locationId);
    Task<IEnumerable<ProductLocationDto>> GetProductLocationsByWarehouseIdAsync(int warehouseId);
    Task<ProductLocationDto?> GetProductLocationByIdAsync(int id);
    Task<ProductLocationDto> CreateProductLocationAsync(CreateProductLocationDto productLocationDto);
    Task UpdateProductLocationAsync(int id, UpdateProductLocationDto productLocationDto);
    Task DeleteProductLocationAsync(int id);
    Task TransferProductAsync(int productId, int sourceLocationId, int destinationLocationId, int quantity);
    Task<string> GenerateWarehouseReportAsync();
    Task<string> GenerateLabelAsync(int productId);
}