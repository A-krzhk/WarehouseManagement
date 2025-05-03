using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Interfaces;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Infrastructure.Repositories;

public class LocationRepository : GenericRepository<Location>, ILocationRepository
{
    public LocationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<Location>> GetLocationsByWarehouseIdAsync(int warehouseId)
    {
        return await _dbContext.Set<Location>()
            .Where(l => l.WarehouseId == warehouseId)
            .Include(l => l.Warehouse)
            .ToListAsync();
    }

    public async Task<Location?> GetLocationWithProductLocationsAsync(int id)
    {
        return await _dbContext.Set<Location>()
            .Include(l => l.Warehouse)
            .Include(l => l.ProductLocations)
            .ThenInclude(pl => pl.Product)
            .FirstOrDefaultAsync(l => l.Id == id);
    }
}