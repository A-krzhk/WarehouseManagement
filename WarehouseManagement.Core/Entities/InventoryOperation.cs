namespace WarehouseManagement.Core.Entities;

public enum OperationType
{
    Incoming,
    Outgoing
}

public class InventoryOperation
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
    public OperationType Type { get; set; }
    public string UserId { get; set; } = string.Empty;
    public ApplicationUser? User { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public string? Notes { get; set; }
}