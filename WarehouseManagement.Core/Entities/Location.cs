namespace WarehouseManagement.Core.Entities;

public class Location
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; }
    public string Shelf { get; set; } = string.Empty;
    public string Room { get; set; } = string.Empty;
}