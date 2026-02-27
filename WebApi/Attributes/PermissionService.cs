using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using DataServiceLib.Interfaces;
using CommonLib.Handles;
using System.Collections.Generic;
using System.Linq;

namespace DataServiceLib.Implementations
{
    public class PermissionService : IPermissionService
    {
        private readonly string _connectionString;
        private readonly IErrorHandler _errorHandler;

        public PermissionService(IConfiguration configuration, IErrorHandler errorHandler)
        {
            _connectionString = configuration.GetConnectionString("OracleDb");
            _errorHandler = errorHandler;
        }

        public async Task<List<string>> GetAllPermissions(string username)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("PermissionService", "GetAllPermissions");

                await using var conn = new OracleConnection(_connectionString);
                await conn.OpenAsync();

                string sql = @"
                    SELECT TRIM(LOWER(PERMISSION_CODE))
                    FROM USER_PERMISSIONS
                    WHERE TRIM(LOWER(USERNAME)) = :username";

                var parameters = new { username = username.ToLower().Trim() };
                _errorHandler.WriteStringToFile("SQL:GetAllPermissions", parameters);

                var permissions = (await conn.QueryAsync<string>(sql, parameters)).ToList();

                _errorHandler.WriteStringToFile("Danh sách quyền của user", permissions);
                return permissions;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                throw;
            }
        }

        public async Task<bool> HasPermission(string username, string permissionCode)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("PermissionService", "HasPermission");

                await using var conn = new OracleConnection(_connectionString);
                await conn.OpenAsync();

                string sql = @"
                    SELECT COUNT(*)
                    FROM USER_PERMISSIONS
                    WHERE TRIM(LOWER(USERNAME)) = :username
                      AND TRIM(LOWER(PERMISSION_CODE)) = :permissionCode";

                var parameters = new
                {
                    username = username.ToLower().Trim(),
                    permissionCode = permissionCode.ToLower().Trim()
                };

                _errorHandler.WriteStringToFile("SQL:HasPermission", parameters);

                var count = await conn.ExecuteScalarAsync<decimal>(sql, parameters);

                return count > 0;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                throw;
            }
        }
    }
}
