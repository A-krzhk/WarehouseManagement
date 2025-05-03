using WarehouseManagement.Core.DTO;

namespace WarehouseManagement.Core.Interfaces;

public interface IInventoryOperationService
{
    Task<IEnumerable<InventoryOperationDto>> GetAllOperationsAsync();
    Task<InventoryOperationDto?> GetOperationByIdAsync(int id);
    Task<InventoryOperationDto> CreateOperationAsync(CreateInventoryOperationDto operationDto, string userId);
    Task<IEnumerable<InventoryOperationDto>> GetOperationsByProductIdAsync(int productId);
    Task<IEnumerable<InventoryOperationDto>> GetOperationsByUserIdAsync(string userId);
}