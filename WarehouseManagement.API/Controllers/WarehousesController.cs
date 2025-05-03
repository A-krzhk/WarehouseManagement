using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.Constants;
using WarehouseManagement.Core.DTO;
using WarehouseManagement.Core.Interfaces;

namespace WarehouseManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;

    public WarehousesController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<WarehouseDto>>> GetWarehouses()
    {
        var warehouses = await _warehouseService.GetAllWarehousesAsync();
        return Ok(warehouses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<WarehouseDto>> GetWarehouse(int id)
    {
        var warehouse = await _warehouseService.GetWarehouseByIdAsync(id);
        if (warehouse == null)
            return NotFound();

        return Ok(warehouse);
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<ActionResult<WarehouseDto>> CreateWarehouse(CreateWarehouseDto warehouseDto)
    {
        var warehouse = await _warehouseService.CreateWarehouseAsync(warehouseDto);
        return CreatedAtAction(nameof(GetWarehouse), new { id = warehouse.Id }, warehouse);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<IActionResult> UpdateWarehouse(int id, UpdateWarehouseDto warehouseDto)
    {
        try
        {
            await _warehouseService.UpdateWarehouseAsync(id, warehouseDto);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteWarehouse(int id)
    {
        try
        {
            await _warehouseService.DeleteWarehouseAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}