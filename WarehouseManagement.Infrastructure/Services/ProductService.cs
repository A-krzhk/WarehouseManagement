using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Core.DTO;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Interfaces;
using WarehouseManagement.Infrastructure.Data;

namespace WarehouseManagement.Core.Services;
public class ProductService : IProductService
{
    private readonly IGenericRepository<Product> _productRepository;
    private readonly IGenericRepository<Category> _categoryRepository;
    private readonly IGenericRepository<Supplier> _supplierRepository;
    private readonly ApplicationDbContext _dbContext;
    private readonly IProductLocationRepository _productLocationRepository;

    public ProductService(
        IGenericRepository<Product> productRepository,
        IGenericRepository<Category> categoryRepository,
        IGenericRepository<Supplier> supplierRepository,
        ApplicationDbContext dbContext,
        IProductLocationRepository productLocationRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _supplierRepository = supplierRepository;
        _dbContext = dbContext;
        _productLocationRepository = productLocationRepository;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var products = await _dbContext.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .AsNoTracking()
            .ToListAsync();

        var productDtos = new List<ProductDto>();
        
        foreach (var product in products)
        {
            var productLocations = await _productLocationRepository.GetProductLocationsByProductIdAsync(product.Id);
            
            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Quantity = product.Quantity,
                Barcode = product.Barcode,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name ?? string.Empty,
                SupplierId = product.SupplierId,
                SupplierName = product.Supplier?.Name ?? string.Empty,
                Locations = productLocations.Select(pl => new ProductLocationInfo
                {
                    LocationId = pl.LocationId,
                    LocationCode = pl.Location?.LocationCode ?? string.Empty,
                    LocationName = pl.Location?.Name ?? string.Empty,
                    WarehouseId = pl.Location?.WarehouseId ?? 0,
                    WarehouseName = pl.Location?.Warehouse?.Name ?? string.Empty,
                    Quantity = pl.Quantity
                }).ToList()
            };
            
            productDtos.Add(productDto);
        }
        
        return productDtos;
    }

    public async Task<ProductDto?> GetProductByIdAsync(int id)
    {
        var product = await _dbContext.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (product == null)
            return null;

        var productLocations = await _productLocationRepository.GetProductLocationsByProductIdAsync(id);
        
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Quantity = product.Quantity,
            Barcode = product.Barcode,
            CategoryId = product.CategoryId,
            CategoryName = product.Category?.Name ?? string.Empty,
            SupplierId = product.SupplierId,
            SupplierName = product.Supplier?.Name ?? string.Empty,
            Locations = productLocations.Select(pl => new ProductLocationInfo
            {
                LocationId = pl.LocationId,
                LocationCode = pl.Location?.LocationCode ?? string.Empty,
                LocationName = pl.Location?.Name ?? string.Empty,
                WarehouseId = pl.Location?.WarehouseId ?? 0,
                WarehouseName = pl.Location?.Warehouse?.Name ?? string.Empty,
                Quantity = pl.Quantity
            }).ToList()
        };
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto productDto)
    {
        var product = new Product
        {
            Name = productDto.Name,
            Quantity = productDto.Quantity,
            Barcode = productDto.Barcode,
            CategoryId = productDto.CategoryId,
            SupplierId = productDto.SupplierId
        };

        await _productRepository.AddAsync(product);

        var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
        var supplier = await _supplierRepository.GetByIdAsync(product.SupplierId);

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Quantity = product.Quantity,
            Barcode = product.Barcode,
            CategoryId = product.CategoryId,
            CategoryName = category?.Name ?? string.Empty,
            SupplierId = product.SupplierId,
            SupplierName = supplier?.Name ?? string.Empty,
            Locations = new List<ProductLocationInfo>()
        };
    }

    public async Task UpdateProductAsync(int id, UpdateProductDto productDto)
    {
        var product = await _dbContext.Products.FindAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found.");

        product.Name = productDto.Name;
        product.Barcode = productDto.Barcode;
        product.CategoryId = productDto.CategoryId;
        product.SupplierId = productDto.SupplierId;

        await _productRepository.UpdateAsync(product);
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _dbContext.Products.FindAsync(id);
        if (product == null)
            throw new KeyNotFoundException($"Product with ID {id} not found.");

        await _productRepository.DeleteAsync(product);
    }

    public async Task<bool> ProductExistsAsync(int id)
    {
        var product = await _dbContext.Products.FindAsync(id);
        return product != null;
    }
}