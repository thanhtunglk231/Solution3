using CommonLib.Handles;
using CommonLib.Helpers;
using CoreLib.config;
using CoreLib.Dtos;
using CoreLib.Models;
using WebBrowser.Services.ApiServices;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Services.Implements
{
    public class AccountService : IAccountService
    {

        private readonly IHttpService _httpService;
        private readonly IDataConvertHelper _dataConvertHelper;
        private readonly IErrorHandler _errorHandler;

        public AccountService(IHttpService httpService, IDataConvertHelper dataConvertHelper, IErrorHandler errorHandler)
        {
            _httpService = httpService;
            _dataConvertHelper = dataConvertHelper;
            _errorHandler = errorHandler;
        }

        public async Task<List<PermissionDto>> getall()
        {
            try
            {
                _errorHandler.WriteStringToFuncion("AccountService", nameof(getall));
                return await _httpService.GetTableFromCResponseAsync<PermissionDto>(ApiRouter.UserPermissions);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new List<PermissionDto>();
            }
        }
        public async Task<CResponseMessage1> UpdatePermission(UserPermissionDto userPermissionDto)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("AccountService", nameof(UpdatePermission));

                return await _httpService.PostResponseAsync("Account/updatePermis", userPermissionDto);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi hệ thống: " + ex.Message
                };
            }
        }
        public async Task<CResponseMessage1> deletePermission(UserPermissionDto userPermissionDto)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("AccountService", nameof(UpdatePermission));

                return await _httpService.DeleteWithBodyResponseAsync("Account/deletePremis", userPermissionDto);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi hệ thống: " + ex.Message
                };
            }
        }



        public async Task<List<UserPermissionDto>> getUSer()
        {
            try
            {
                _errorHandler.WriteStringToFuncion("AccountService", nameof(getUSer));

                var result = await _httpService.GetTableFromCResponseAsync<UserPermissionDto>("Account/getUser");

                
                return result;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new List<UserPermissionDto>();
            }
        }


    }
}
