using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties; 
using System.Linq.Expressions;
using iText.Kernel.Geom;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using OfficeOpenXml;
using WarehouseManagement.Core.DTO;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Interfaces;

namespace WarehouseManagement.Core.Services;

public class ProductLocationService : IProductLocationService
{
    private readonly IProductLocationRepository _productLocationRepository;
    private readonly IGenericRepository<Product> _productRepository;
    private readonly ILocationRepository _locationRepository;

    public ProductLocationService(
        IProductLocationRepository productLocationRepository,
        IGenericRepository<Product> productRepository,
        ILocationRepository locationRepository)
    {
        _productLocationRepository = productLocationRepository;
        _productRepository = productRepository;
        _locationRepository = locationRepository;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }
    public async Task<string> GenerateWarehouseReportAsync()
    {
        var includes = new List<Expression<Func<ProductLocation, object>>>
        {
            pl => pl.Product,
            pl => pl.Location.Warehouse
        };

        var productLocations = await _productLocationRepository.GetAsync(
            predicate: null,
            orderBy: null,
            includes: includes,
            disableTracking: true);

        // Группировка по складам и товарам
        var reportData = productLocations
            .GroupBy(pl => pl.Location.Warehouse)
            .Select(g => new WarehouseReportDto
            {
                WarehouseId = g.Key.Id,
                WarehouseName = g.Key.Name,
                Products = g
                    .GroupBy(pl => pl.Product)
                    .Select(pg => new ProductReportItemDto
                    {
                        ProductId = pg.Key.Id,
                        ProductName = pg.Key.Name,
                        TotalQuantity = pg.Sum(pl => pl.Quantity)
                    }).ToList()
            }).ToList();

        // Генерация XLSX
        using var package = new ExcelPackage();
        var worksheet = package.Workbook.Worksheets.Add("Warehouse Report");
        worksheet.Cells[1, 1].Value = "Warehouse ID";
        worksheet.Cells[1, 2].Value = "Warehouse Name";
        worksheet.Cells[1, 3].Value = "Product ID";
        worksheet.Cells[1, 4].Value = "Product Name";
        worksheet.Cells[1, 5].Value = "Total Quantity";

        int row = 2;
        foreach (var warehouse in reportData)
        {
            foreach (var product in warehouse.Products)
            {
                worksheet.Cells[row, 1].Value = warehouse.WarehouseId;
                worksheet.Cells[row, 2].Value = warehouse.WarehouseName;
                worksheet.Cells[row, 3].Value = product.ProductId;
                worksheet.Cells[row, 4].Value = product.ProductName;
                worksheet.Cells[row, 5].Value = product.TotalQuantity;
                row++;
            }
        }

        worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

        // Преобразование в байты и Base64
        var fileBytes = await package.GetAsByteArrayAsync();
        return Convert.ToBase64String(fileBytes);
    }
    public async Task<IEnumerable<ProductLocationDto>> GetAllProductLocationsAsync()
    {
        var includes = new List<Expression<Func<ProductLocation, object>>>
        {
            pl => pl.Product,
            pl => pl.Location.Warehouse
        };

        var productLocations = await _productLocationRepository.GetAsync(
            predicate: null,
            orderBy: null,
            includes: includes,
            disableTracking: true);

        return productLocations.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductLocationDto>> GetProductLocationsByProductIdAsync(int productId)
    {
        var productLocations = await _productLocationRepository.GetProductLocationsByProductIdAsync(productId);
        return productLocations.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductLocationDto>> GetProductLocationsByLocationIdAsync(int locationId)
    {
        var productLocations = await _productLocationRepository.GetProductLocationsByLocationIdAsync(locationId);
        return productLocations.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductLocationDto>> GetProductLocationsByWarehouseIdAsync(int warehouseId)
    {
        var locations = await _locationRepository.GetLocationsByWarehouseIdAsync(warehouseId);
        var locationIds = locations.Select(l => l.Id).ToList();
        
        var productLocations = new List<ProductLocation>();
        foreach (var locationId in locationIds)
        {
            var locProducts = await _productLocationRepository.GetProductLocationsByLocationIdAsync(locationId);
            productLocations.AddRange(locProducts);
        }
        
        return productLocations.Select(MapToDto);
    }
    
    public async Task<ProductLocationDto?> GetProductLocationByIdAsync(int id)
    {
        var includes = new List<Expression<Func<ProductLocation, object>>>
        {
            pl => pl.Product,
            pl => pl.Location.Warehouse
        };

        var productLocations = await _productLocationRepository.GetAsync(
            predicate: pl => pl.Id == id,
            orderBy: null,
            includes: includes,
            disableTracking: true);

        var entity = productLocations.FirstOrDefault();
        return entity != null ? MapToDto(entity) : null;
    }

    public async Task<ProductLocationDto> CreateProductLocationAsync(CreateProductLocationDto productLocationDto)
    {
        var product = await _productRepository.GetByIdAsync(productLocationDto.ProductId);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productLocationDto.ProductId} not found.");
        
        var location = await _locationRepository.GetAsync(
            predicate: l => l.Id == productLocationDto.LocationId,
            includeString: "Warehouse"
        );
        var locationEntity = location.FirstOrDefault();
        if (locationEntity == null)
            throw new KeyNotFoundException($"Location with ID {productLocationDto.LocationId} not found.");
        
        var existingProductLocation = await _productLocationRepository.GetProductLocationByProductAndLocationIdAsync(
            productLocationDto.ProductId, productLocationDto.LocationId);
            
        if (existingProductLocation != null)
        {
            existingProductLocation.Quantity += productLocationDto.Quantity;
            await _productLocationRepository.UpdateAsync(existingProductLocation);
            return MapToDto(existingProductLocation);
        }

        var productLocation = new ProductLocation
        {
            ProductId = productLocationDto.ProductId,
            LocationId = productLocationDto.LocationId,
            Quantity = productLocationDto.Quantity
        };

        await _productLocationRepository.AddAsync(productLocation);

        productLocation.Product = product;
        productLocation.Location = locationEntity;

        return MapToDto(productLocation);
    }

    public async Task UpdateProductLocationAsync(int id, UpdateProductLocationDto productLocationDto)
    {
        var productLocation = await _productLocationRepository.GetByIdAsync(id);
        if (productLocation == null)
            throw new KeyNotFoundException($"Product location with ID {id} not found.");

        productLocation.Quantity = productLocationDto.Quantity;

        await _productLocationRepository.UpdateAsync(productLocation);
    }

    public async Task DeleteProductLocationAsync(int id)
    {
        var productLocation = await _productLocationRepository.GetByIdAsync(id);
        if (productLocation == null)
            throw new KeyNotFoundException($"Product location with ID {id} not found.");

        await _productLocationRepository.DeleteAsync(productLocation);
    }

    public async Task TransferProductAsync(int productId, int sourceLocationId, int destinationLocationId, int quantity)
    {
        var sourceProductLocation = await _productLocationRepository.GetProductLocationByProductAndLocationIdAsync(
            productId, sourceLocationId);
            
        if (sourceProductLocation == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found at source location {sourceLocationId}.");
            
        if (sourceProductLocation.Quantity < quantity)
            throw new InvalidOperationException($"Not enough quantity available at source location. Available: {sourceProductLocation.Quantity}, Requested: {quantity}");
        
        var destinationProductLocation = await _productLocationRepository.GetProductLocationByProductAndLocationIdAsync(
            productId, destinationLocationId);
            
        if (destinationProductLocation == null)
        {
            destinationProductLocation = new ProductLocation
            {
                ProductId = productId,
                LocationId = destinationLocationId,
                Quantity = 0
            };
            await _productLocationRepository.AddAsync(destinationProductLocation);
        }
        
        sourceProductLocation.Quantity -= quantity;
        destinationProductLocation.Quantity += quantity;
        
        await _productLocationRepository.UpdateAsync(sourceProductLocation);
        await _productLocationRepository.UpdateAsync(destinationProductLocation);
        
        if (sourceProductLocation.Quantity == 0)
        {
            await _productLocationRepository.DeleteAsync(sourceProductLocation);
        }
    }

    private ProductLocationDto MapToDto(ProductLocation productLocation)
    {
        return new ProductLocationDto
        {
            Id = productLocation.Id,
            ProductId = productLocation.ProductId,
            ProductName = productLocation.Product?.Name ?? string.Empty,
            LocationId = productLocation.LocationId,
            LocationCode = productLocation.Location?.LocationCode ?? string.Empty,
            LocationName = productLocation.Location?.Name ?? string.Empty,
            WarehouseId = productLocation.Location?.WarehouseId ?? 0,
            WarehouseName = productLocation.Location?.Warehouse?.Name ?? string.Empty,
            Quantity = productLocation.Quantity
        };
    }

    public async Task<string> GenerateLabelAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found.");

        var labelData = new LabelDto
        {
            ProductId = productId,
            ProductName = product.Name,
            Quantity = 0,
            LocationCode = "-",
            WarehouseName = "-"
        };
        
        var includes = new List<Expression<Func<ProductLocation, object>>>
        {
            pl => pl.Location.Warehouse
        };

        var productLocations = await _productLocationRepository.GetAsync(
            predicate: pl => pl.ProductId == productId,
            orderBy: null,
            includes: includes,
            disableTracking: true);

        var productLocation = productLocations.FirstOrDefault();
        
        if (productLocation != null)
        {
            labelData.Quantity = productLocation.Quantity;
            labelData.LocationCode = productLocation.Location?.LocationCode ?? "-";
            labelData.WarehouseName = productLocation.Location?.Warehouse?.Name ?? "-";
        }

        using var memoryStream = new MemoryStream();
        using var pdfWriter = new iText.Kernel.Pdf.PdfWriter(memoryStream);

        var pageSize = new PageSize(80 * 2.83464567f, 40 * 2.83464567f);
        using var pdfDocument = new iText.Kernel.Pdf.PdfDocument(pdfWriter);
        pdfDocument.SetDefaultPageSize(pageSize);
        var document = new iText.Layout.Document(pdfDocument);
        document.SetMargins(5, 5, 5, 5); 
        
        document.Add(new iText.Layout.Element.Paragraph("\n").SetFontSize(2)); 


        var table = new iText.Layout.Element.Table(iText.Layout.Properties.UnitValue.CreatePercentArray(new float[] { 40, 60 }))
            .UseAllAvailableWidth()
            .SetFontSize(6);
        
        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("ID:")).SetBold());
        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(labelData.ProductId.ToString())));

        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("Product Name:")).SetBold());
        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(labelData.ProductName)));

        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("Quantity:")).SetBold());
        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(labelData.Quantity.ToString())));

        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("Location:")).SetBold());
        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(labelData.LocationCode)));

        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph("Warehouse:")).SetBold());
        table.AddCell(new iText.Layout.Element.Cell().Add(new iText.Layout.Element.Paragraph(labelData.WarehouseName)));

        document.Add(table);

        document.Close();

        var pdfBytes = memoryStream.ToArray();
        return Convert.ToBase64String(pdfBytes);
    }
    
}