namespace WarehouseManagement.Core.DTO;
public class ProductLocationInfo
{
    public int LocationId { get; set; }
    public string LocationCode { get; set; } = string.Empty;
    public string LocationName { get; set; } = string.Empty;
    public int WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
public class ProductDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public int SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public List<ProductLocationInfo> Locations { get; set; } = new List<ProductLocationInfo>();
}

public class CreateProductDto
{
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public string Barcode { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
}

public class UpdateProductDto
{
    public string Name { get; set; } = string.Empty;
    public string Barcode { get; set; } = string.Empty;
    public int CategoryId { get; set; }
    public int SupplierId { get; set; }
}