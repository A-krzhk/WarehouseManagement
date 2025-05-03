using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.Interfaces;

public interface ISupplierRepository
{
    Task AddAsync(Supplier supplier);
    Task<Supplier> GetByIdAsync(int id);
    Task<IEnumerable<Supplier>> GetAllAsync();
}