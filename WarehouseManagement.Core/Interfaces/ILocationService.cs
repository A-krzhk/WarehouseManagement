using WarehouseManagement.Core.DTO;

namespace WarehouseManagement.Core.Interfaces;

public interface ILocationService
{
    Task<IEnumerable<LocationDto>> GetAllLocationsAsync();
    Task<IEnumerable<LocationDto>> GetLocationsByWarehouseIdAsync(int warehouseId);
    Task<LocationDto?> GetLocationByIdAsync(int id);
    Task<LocationDto> CreateLocationAsync(CreateLocationDto locationDto);
    Task UpdateLocationAsync(int id, UpdateLocationDto locationDto);
    Task DeleteLocationAsync(int id);
}