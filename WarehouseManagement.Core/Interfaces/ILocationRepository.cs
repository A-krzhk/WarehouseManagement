using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.Interfaces;

public interface ILocationRepository : IGenericRepository<Location>
{
    Task<IEnumerable<Location>> GetLocationsByWarehouseIdAsync(int warehouseId);
    Task<Location?> GetLocationWithProductLocationsAsync(int id);
}