using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Interfaces;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Infrastructure.Repositories;

public class ProductLocationRepository : GenericRepository<ProductLocation>, IProductLocationRepository
{
    public ProductLocationRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<IEnumerable<ProductLocation>> GetProductLocationsByProductIdAsync(int productId)
    {
        return await _dbContext.Set<ProductLocation>()
            .Where(pl => pl.ProductId == productId)
            .Include(pl => pl.Location)
            .ThenInclude(l => l.Warehouse)
            .Include(pl => pl.Product)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProductLocation>> GetProductLocationsByLocationIdAsync(int locationId)
    {
        return await _dbContext.Set<ProductLocation>()
            .Where(pl => pl.LocationId == locationId)
            .Include(pl => pl.Location)
            .ThenInclude(l => l.Warehouse)
            .Include(pl => pl.Product)
            .ToListAsync();
    }

    public async Task<ProductLocation?> GetProductLocationByProductAndLocationIdAsync(int productId, int locationId)
    {
        return await _dbContext.Set<ProductLocation>()
            .Where(pl => pl.ProductId == productId && pl.LocationId == locationId)
            .Include(pl => pl.Location)
            .Include(pl => pl.Product)
            .FirstOrDefaultAsync();
    }
}