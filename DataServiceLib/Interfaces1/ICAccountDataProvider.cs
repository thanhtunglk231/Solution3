using CoreLib.Dtos;
using CoreLib.Models;
using System.Data;

namespace DataServiceLib.Interfaces1
{
    public interface ICAccountDataProvider
    {
        (DataSet data, CResponseMessage1 response) getall();
        (DataSet data, CResponseMessage1 response) getall_userName();
        Task<CResponseMessage1> UpdatePermission(UserPermissionDto permissionDto);
        Task<CResponseMessage1> DeletePermission(UserPermissionDto permissionDto);
    }
}