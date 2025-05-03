using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.Constants;
using WarehouseManagement.Core.DTO;
using WarehouseManagement.Core.Interfaces;

namespace WarehouseManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class LocationsController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationsController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<LocationDto>>> GetLocations()
    {
        var locations = await _locationService.GetAllLocationsAsync();
        return Ok(locations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<LocationDto>> GetLocation(int id)
    {
        var location = await _locationService.GetLocationByIdAsync(id);
        if (location == null)
            return NotFound();

        return Ok(location);
    }

    [HttpGet("warehouse/{warehouseId}")]
    public async Task<ActionResult<IEnumerable<LocationDto>>> GetLocationsByWarehouse(int warehouseId)
    {
        var locations = await _locationService.GetLocationsByWarehouseIdAsync(warehouseId);
        return Ok(locations);
    }

    [HttpPost]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<ActionResult<LocationDto>> CreateLocation(CreateLocationDto locationDto)
    {
        try
        {
            var location = await _locationService.CreateLocationAsync(locationDto);
            return CreatedAtAction(nameof(GetLocation), new { id = location.Id }, location);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<IActionResult> UpdateLocation(int id, UpdateLocationDto locationDto)
    {
        try
        {
            await _locationService.UpdateLocationAsync(id, locationDto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Admin)]
    public async Task<IActionResult> DeleteLocation(int id)
    {
        try
        {
            await _locationService.DeleteLocationAsync(id);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}