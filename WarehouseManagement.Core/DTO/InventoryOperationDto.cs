using WarehouseManagement.Core.Entities;

namespace WarehouseManagement.Core.DTO;

public class InventoryOperationDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public OperationType Type { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string? Notes { get; set; }
}

public class CreateInventoryOperationDto
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public OperationType Type { get; set; }
    public string? Notes { get; set; }
}