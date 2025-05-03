namespace WarehouseManagement.Core.Entities;

public class Warehouse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public ICollection<Location> Locations { get; set; } = new List<Location>();
}