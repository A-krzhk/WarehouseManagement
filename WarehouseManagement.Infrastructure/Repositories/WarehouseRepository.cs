using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Interfaces;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Infrastructure.Repositories;

public class WarehouseRepository : GenericRepository<Warehouse>, IWarehouseRepository
{
    public WarehouseRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Warehouse?> GetWarehouseWithLocationsAsync(int id)
    {
        return await _dbContext.Set<Warehouse>()
            .Include(w => w.Locations)
            .FirstOrDefaultAsync(w => w.Id == id);
    }
}