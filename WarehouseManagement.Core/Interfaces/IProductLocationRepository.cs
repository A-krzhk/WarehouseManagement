using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.Interfaces;

public interface IProductLocationRepository : IGenericRepository<ProductLocation>
{
    Task<IEnumerable<ProductLocation>> GetProductLocationsByProductIdAsync(int productId);
    Task<IEnumerable<ProductLocation>> GetProductLocationsByLocationIdAsync(int locationId);
    Task<ProductLocation?> GetProductLocationByProductAndLocationIdAsync(int productId, int locationId);
}