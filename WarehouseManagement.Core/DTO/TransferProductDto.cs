namespace WarehouseManagement.Core.DTO;

public class TransferProductDto
{
    public int ProductId { get; set; }
    public int SourceLocationId { get; set; }
    public int DestinationLocationId { get; set; }
    public int Quantity { get; set; }
}