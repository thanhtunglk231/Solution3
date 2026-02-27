using CoreLib.Dtos;
using CoreLib.Models;

namespace WebBrowser.Services.Interfaces
{
    public interface IAccountService
    {
        Task<List<PermissionDto>> getall();
        Task<List<UserPermissionDto>> getUSer();
        Task<CResponseMessage1> UpdatePermission(UserPermissionDto userPermissionDto);
        Task<CResponseMessage1> deletePermission(UserPermissionDto userPermissionDto);
    }
}