using System.Linq.Expressions;
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
        // Verify product exists
        var product = await _productRepository.GetByIdAsync(productLocationDto.ProductId);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {productLocationDto.ProductId} not found.");

        // Verify location exists
        var location = await _locationRepository.GetAsync(
            predicate: l => l.Id == productLocationDto.LocationId,
            includeString: "Warehouse"
        );
        var locationEntity = location.FirstOrDefault();
        if (locationEntity == null)
            throw new KeyNotFoundException($"Location with ID {productLocationDto.LocationId} not found.");

        // Check if product already exists at this location
        var existingProductLocation = await _productLocationRepository.GetProductLocationByProductAndLocationIdAsync(
            productLocationDto.ProductId, productLocationDto.LocationId);
            
        if (existingProductLocation != null)
        {
            // Update quantity instead of creating new record
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
        // Get source product location
        var sourceProductLocation = await _productLocationRepository.GetProductLocationByProductAndLocationIdAsync(
            productId, sourceLocationId);
            
        if (sourceProductLocation == null)
            throw new KeyNotFoundException($"Product with ID {productId} not found at source location {sourceLocationId}.");
            
        if (sourceProductLocation.Quantity < quantity)
            throw new InvalidOperationException($"Not enough quantity available at source location. Available: {sourceProductLocation.Quantity}, Requested: {quantity}");

        // Get or create destination product location
        var destinationProductLocation = await _productLocationRepository.GetProductLocationByProductAndLocationIdAsync(
            productId, destinationLocationId);
            
        if (destinationProductLocation == null)
        {
            // Create new product location at destination
            destinationProductLocation = new ProductLocation
            {
                ProductId = productId,
                LocationId = destinationLocationId,
                Quantity = 0
            };
            await _productLocationRepository.AddAsync(destinationProductLocation);
        }

        // Update quantities
        sourceProductLocation.Quantity -= quantity;
        destinationProductLocation.Quantity += quantity;

        // Save changes
        await _productLocationRepository.UpdateAsync(sourceProductLocation);
        await _productLocationRepository.UpdateAsync(destinationProductLocation);

        // If source location now has zero quantity, consider removing the record
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
}