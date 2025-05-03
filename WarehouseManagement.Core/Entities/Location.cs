namespace WarehouseManagement.Core.Entities;

public class Location
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty; 

    public int WarehouseId { get; set; }

    public Warehouse Warehouse { get; set; } = null!;
    public ICollection<ProductLocation> ProductLocations { get; set; } = new List<ProductLocation>();
}