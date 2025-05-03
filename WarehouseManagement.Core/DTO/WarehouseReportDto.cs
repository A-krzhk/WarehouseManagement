namespace WarehouseManagement.Core.DTO;

public class WarehouseReportDto
{
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public List<ProductReportItemDto> Products { get; set; } = new();
}

public class ProductReportItemDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int TotalQuantity { get; set; }
}