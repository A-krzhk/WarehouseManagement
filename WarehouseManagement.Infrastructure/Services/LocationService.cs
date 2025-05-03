using WarehouseManagement.Core.DTO;
using WarehouseManagement.Core.Entities;
using WarehouseManagement.Core.Interfaces;

namespace WarehouseManagement.Core.Services;

public class LocationService : ILocationService
{
    private readonly ILocationRepository _locationRepository;
    private readonly IWarehouseRepository _warehouseRepository;

    public LocationService(ILocationRepository locationRepository, IWarehouseRepository warehouseRepository)
    {
        _locationRepository = locationRepository;
        _warehouseRepository = warehouseRepository;
    }

    public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
    {
        var locations = await _locationRepository.GetAsync(
            includeString: "Warehouse"
        );

        return locations.Select(l => new LocationDto
        {
            Id = l.Id,
            Name = l.Name,
            Description = l.Description,
            LocationCode = l.LocationCode,
            WarehouseId = l.WarehouseId,
            WarehouseName = l.Warehouse?.Name ?? string.Empty
        });
    }

    public async Task<IEnumerable<LocationDto>> GetLocationsByWarehouseIdAsync(int warehouseId)
    {
        var locations = await _locationRepository.GetLocationsByWarehouseIdAsync(warehouseId);
        
        return locations.Select(l => new LocationDto
        {
            Id = l.Id,
            Name = l.Name,
            Description = l.Description,
            LocationCode = l.LocationCode,
            WarehouseId = l.WarehouseId,
            WarehouseName = l.Warehouse?.Name ?? string.Empty
        });
    }

    public async Task<LocationDto?> GetLocationByIdAsync(int id)
    {
        var location = await _locationRepository.GetAsync(
            predicate: l => l.Id == id,
            includeString: "Warehouse"
        );

        var locationEntity = location.FirstOrDefault();
        if (locationEntity == null)
            return null;

        return new LocationDto
        {
            Id = locationEntity.Id,
            Name = locationEntity.Name,
            Description = locationEntity.Description,
            LocationCode = locationEntity.LocationCode,
            WarehouseId = locationEntity.WarehouseId,
            WarehouseName = locationEntity.Warehouse?.Name ?? string.Empty
        };
    }

    public async Task<LocationDto> CreateLocationAsync(CreateLocationDto locationDto)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(locationDto.WarehouseId);
        if (warehouse == null)
            throw new KeyNotFoundException($"Warehouse with ID {locationDto.WarehouseId} not found.");

        var location = new Location
        {
            Name = locationDto.Name,
            Description = locationDto.Description,
            LocationCode = locationDto.LocationCode,
            WarehouseId = locationDto.WarehouseId
        };

        await _locationRepository.AddAsync(location);

        return new LocationDto
        {
            Id = location.Id,
            Name = location.Name,
            Description = location.Description,
            LocationCode = location.LocationCode,
            WarehouseId = location.WarehouseId,
            WarehouseName = warehouse.Name
        };
    }

    public async Task UpdateLocationAsync(int id, UpdateLocationDto locationDto)
    {
        var location = await _locationRepository.GetByIdAsync(id);
        if (location == null)
            throw new KeyNotFoundException($"Location with ID {id} not found.");
        
        if (location.WarehouseId != locationDto.WarehouseId)
        {
            var warehouse = await _warehouseRepository.GetByIdAsync(locationDto.WarehouseId);
            if (warehouse == null)
                throw new KeyNotFoundException($"Warehouse with ID {locationDto.WarehouseId} not found.");
        }

        location.Name = locationDto.Name;
        location.Description = locationDto.Description;
        location.LocationCode = locationDto.LocationCode;
        location.WarehouseId = locationDto.WarehouseId;

        await _locationRepository.UpdateAsync(location);
    }

    public async Task DeleteLocationAsync(int id)
    {
        var location = await _locationRepository.GetByIdAsync(id);
        if (location == null)
            throw new KeyNotFoundException($"Location with ID {id} not found.");

        await _locationRepository.DeleteAsync(location);
    }
}