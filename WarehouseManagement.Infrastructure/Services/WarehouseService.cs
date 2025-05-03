using WarehouseManagement.Core.DTO;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Interfaces;

namespace WarehouseManagement.Core.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseRepository _warehouseRepository;

    public WarehouseService(IWarehouseRepository warehouseRepository)
    {
        _warehouseRepository = warehouseRepository;
    }

    public async Task<IEnumerable<WarehouseDto>> GetAllWarehousesAsync()
    {
        var warehouses = await _warehouseRepository.GetAllAsync();
        return warehouses.Select(w => new WarehouseDto
        {
            Id = w.Id,
            Name = w.Name,
            Address = w.Address,
            Description = w.Description
        });
    }

    public async Task<WarehouseDto?> GetWarehouseByIdAsync(int id)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(id);
        if (warehouse == null)
            return null;

        return new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Address = warehouse.Address,
            Description = warehouse.Description
        };
    }

    public async Task<WarehouseDto> CreateWarehouseAsync(CreateWarehouseDto warehouseDto)
    {
        var warehouse = new Warehouse
        {
            Name = warehouseDto.Name,
            Address = warehouseDto.Address,
            Description = warehouseDto.Description
        };

        await _warehouseRepository.AddAsync(warehouse);

        return new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Address = warehouse.Address,
            Description = warehouse.Description
        };
    }

    public async Task UpdateWarehouseAsync(int id, UpdateWarehouseDto warehouseDto)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(id);
        if (warehouse == null)
            throw new KeyNotFoundException($"Warehouse with ID {id} not found.");

        warehouse.Name = warehouseDto.Name;
        warehouse.Address = warehouseDto.Address;
        warehouse.Description = warehouseDto.Description;

        await _warehouseRepository.UpdateAsync(warehouse);
    }

    public async Task DeleteWarehouseAsync(int id)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(id);
        if (warehouse == null)
            throw new KeyNotFoundException($"Warehouse with ID {id} not found.");

        await _warehouseRepository.DeleteAsync(warehouse);
    }
}