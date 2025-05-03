namespace WarehouseManagement.Core.Entities;

public class ProductLocation
{
    public int Id { get; set; }
    
    public int ProductId { get; set; }
    public int LocationId { get; set; }

    public int Quantity { get; set; }

    public Product Product { get; set; } = null!;
    public Location Location { get; set; } = null!;
}