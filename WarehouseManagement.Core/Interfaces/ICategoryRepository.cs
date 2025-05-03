using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.Interfaces;

public interface ICategoryRepository
{
    Task AddAsync(Category category);
    Task<Category> GetByIdAsync(int id);
    Task<IEnumerable<Category>> GetAllAsync();
}