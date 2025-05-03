namespace WarehouseManagement.Core.DTO;

public class ProductLocationDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class CreateProductLocationDto
{
    public int ProductId { get; set; }
    public int LocationId { get; set; }
    public int Quantity { get; set; }
}

public class UpdateProductLocationDto
{
    public int Quantity { get; set; }
}