namespace DataServiceLib.Interfaces
{
    public interface IPermissionService
    {
        Task<bool> HasPermission(string username, string permissionCode);
        Task<List<string>> GetAllPermissions(string username);
    }
}