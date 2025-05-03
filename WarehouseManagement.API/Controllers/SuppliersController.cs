using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.Core.DTO;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Interfaces;

namespace WarehouseManagement.API.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierRepository _supplierRepository;

    public SuppliersController(ISupplierRepository supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    [HttpPost]
    public async Task<IActionResult> CreateSupplier([FromBody] SupplierDTO supplierDto)
    {
        if (supplierDto == null)
        {
            return BadRequest("Supplier data is null.");
        }

        var supplier = new Supplier
        {
            Name = supplierDto.Name,
            ContactPerson = supplierDto.ContactPerson,
            Email = supplierDto.Email,
            Phone = supplierDto.Phone,
            Address = supplierDto.Address,
            Products = new List<Product>()
        };

        await _supplierRepository.AddAsync(supplier);
        return CreatedAtAction(nameof(GetSupplier), new { id = supplier.Id }, supplier);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSupplier(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);
        if (supplier == null)
        {
            return NotFound();
        }
        return Ok(supplier);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllSuppliers()
    {
        var suppliers = await _supplierRepository.GetAllAsync();
        return Ok(suppliers);
    }
}