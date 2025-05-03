namespace WarehouseManagement.Core.Entities;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public int SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public ICollection<InventoryOperation> InventoryOperations { get; set; } = new List<InventoryOperation>();
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; }
    public int LocationId { get; set; }
    public Location Location { get; set; }
}