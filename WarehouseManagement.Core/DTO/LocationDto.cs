namespace WarehouseManagement.Core.DTO;

public class LocationDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
}

public class CreateLocationDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
}

public class UpdateLocationDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string LocationCode { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
}