using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.Constants;
using WarehouseManagement.Core.DTO;
using WarehouseManagement.Core.Interfaces;

namespace WarehouseManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class InventoryOperationsController : ControllerBase
{
    private readonly IInventoryOperationService _operationService;

    public InventoryOperationsController(IInventoryOperationService operationService)
    {
        _operationService = operationService;
    }

    [HttpGet]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<ActionResult<IEnumerable<InventoryOperationDto>>> GetOperations()
    {
        var operations = await _operationService.GetAllOperationsAsync();
        return Ok(operations);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<InventoryOperationDto>> GetOperation(int id)
    {
        var operation = await _operationService.GetOperationByIdAsync(id);
        if (operation == null)
            return NotFound();

        return Ok(operation);
    }

    [HttpGet("product/{productId}")]
    public async Task<ActionResult<IEnumerable<InventoryOperationDto>>> GetOperationsByProduct(int productId)
    {
        var operations = await _operationService.GetOperationsByProductIdAsync(productId);
        return Ok(operations);
    }

    [HttpGet("user/{userId}")]
    [Authorize(Roles = $"{Roles.Admin},{Roles.Manager}")]
    public async Task<ActionResult<IEnumerable<InventoryOperationDto>>> GetOperationsByUser(string userId)
    {
        var operations = await _operationService.GetOperationsByUserIdAsync(userId);
        return Ok(operations);
    }

    [HttpGet("my-operations")]
    public async Task<ActionResult<IEnumerable<InventoryOperationDto>>> GetMyOperations()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        var operations = await _operationService.GetOperationsByUserIdAsync(userId);
        return Ok(operations);
    }

    [HttpPost]
    public async Task<ActionResult<InventoryOperationDto>> CreateOperation(CreateInventoryOperationDto operationDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized();

        try
        {
            var operation = await _operationService.CreateOperationAsync(operationDto, userId);
            return CreatedAtAction(nameof(GetOperation), new { id = operation.Id }, operation);
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