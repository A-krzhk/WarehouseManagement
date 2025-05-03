namespace WarehouseManagement.Core.Constants;

public static class Roles
{
    public const string Admin = "Admin";
    public const string Manager = "Manager";
    public const string WarehouseWorker = "WarehouseWorker";
        
    public static readonly string[] AllRoles = new[] { Admin, Manager, WarehouseWorker };
}