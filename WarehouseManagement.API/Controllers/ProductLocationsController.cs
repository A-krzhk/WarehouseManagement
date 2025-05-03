using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.Constants;
using WarehouseManagement.Core.DTO;
using WarehouseManagement.Core.Interfaces;

namespace WarehouseManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ProductLocationsController : ControllerBase
{
    private readonly IProductLocationService _productLocationService;

    public ProductLocationsController(IProductLocationService productLocationService)
    {
        _productLocationService = productLocationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductLocationDto>>> GetProductLocations()
    {
        var productLocations = await _productLocationService.GetAllProductLocationsAsync();
        return Ok(productLocations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductLocationDto>> GetProductLocation(int id)
    {
        var productLocation = await _productLocationService.GetProductLocationByIdAsync(id);
        if (productLocation == null)
            return NotFound();

        return Ok(productLocation);
    }

    [HttpGet("product/{productId}")]
    public async Task<ActionResult<IEnumerable<ProductLocationDto>>> GetProductLocationsByProduct(int productId)
    {
        var productLocations = await _productLocationService.GetProductLocationsByProductIdAsync(productId);
        return Ok(productLocations);
    }

    [HttpGet("location/{locationId}")]
    public async Task<ActionResult<IEnumerable<ProductLocationDto>>> GetProductLocationsByLocation(int locationId)
    {
        var productLocations = await _productLocationService.GetProductLocationsByLocationIdAsync(locationId);
        return Ok(productLocations);
    }

    [HttpGet("warehouse/{warehouseId}")]
    public async Task<ActionResult<IEnumerable<ProductLocationDto>>> GetProductLocationsByWarehouse(int warehouseId)
    {
        var productLocations = await _productLocationService.GetProductLocationsByWarehouseIdAsync(warehouseId);
        return Ok(productLocations);
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<ActionResult<ProductLocationDto>> CreateProductLocation(CreateProductLocationDto productLocationDto)
    {
        try
        {
            var productLocation = await _productLocationService.CreateProductLocationAsync(productLocationDto);
            return CreatedAtAction(nameof(GetProductLocation), new { id = productLocation.Id }, productLocation);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<IActionResult> UpdateProductLocation(int id, UpdateProductLocationDto productLocationDto)
    {
        try
        {
            await _productLocationService.UpdateProductLocationAsync(id, productLocationDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<IActionResult> DeleteProductLocation(int id)
    {
        try
        {
            await _productLocationService.DeleteProductLocationAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost("transfer")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<IActionResult> TransferProduct([FromBody] TransferProductDto transferDto)
    {
        try
        {
            await _productLocationService.TransferProductAsync(
                transferDto.ProductId,
                transferDto.SourceLocationId,
                transferDto.DestinationLocationId,
                transferDto.Quantity);
                
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}