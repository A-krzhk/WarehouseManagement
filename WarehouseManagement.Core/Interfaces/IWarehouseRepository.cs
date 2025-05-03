using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.Interfaces;

public interface IWarehouseRepository : IGenericRepository<Warehouse>
{
    Task<Warehouse?> GetWarehouseWithLocationsAsync(int id);
}