using WarehouseManagement.Core.DTO;

namespace WarehouseManagement.Core.Interfaces;

public interface IWarehouseService
{
    Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync();
    Task<WarehouseDto?> GetWarehouseByIdAsync(int id);
    Task<WarehouseDto> CreateWarehouseAsync(CreateWarehouseDto warehouseDto);
    Task UpdateWarehouseAsync(int id, UpdateWarehouseDto warehouseDto);
    Task DeleteWarehouseAsync(int id);
}