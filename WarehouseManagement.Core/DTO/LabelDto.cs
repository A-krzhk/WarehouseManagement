namespace WarehouseManagement.Core.DTO;

public class LabelDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
}