using CoreLib.Dtos;
using CoreLib.Models;
using System.Data;

namespace DataServiceLib.Interfaces1
{
    public interface ICAccountDataProvider
    {
        Task<CResponseMessage1> DeletePermission(UserPermissionDto permissionDto);
        Task<(DataSet data, CResponseMessage1 response)> getall();
        Task<(DataSet data, CResponseMessage1 response)> getall_userName();
        Task<CResponseMessage1> UpdatePermission(UserPermissionDto permissionDto);
    }
}