using Microsoft.AspNetCore.Identity;

namespace WarehouseManagement.Core.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public ICollection<InventoryOperation> InventoryOperations { get; set; } = new List<InventoryOperation>();
}