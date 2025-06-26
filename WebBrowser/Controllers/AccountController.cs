    using CommonLib.Handles;
    using CommonLib.Helpers;
    using CoreLib.Dtos;
    using Microsoft.AspNetCore.Mvc;
using WebBrowser.Services.Implements;
    using WebBrowser.Services.Interfaces;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;
namespace WebBrowser.Controllers
    {
        public class AccountController : BaseController
        {
            private readonly IAccountService _accountService;
            private readonly IErrorHandler _errorHandler;
            private readonly IDataConvertHelper _dataConvertHelper;

            public AccountController(IAccountService accountService, IConfiguration configuration,
                                  IErrorHandler errorHandler, IDataConvertHelper dataConvertHelper)
                : base(configuration)
            {
                _accountService = accountService;
                _errorHandler = errorHandler;
                _dataConvertHelper = dataConvertHelper;
            }

            public IActionResult Index()
            {
                var token = GetJwtTokenOrRedirect(out IActionResult? redirectResult);
                if (token == null)
                    return redirectResult!;
                return View();
            }


         
            public async Task<IActionResult> getAll()
            {
                try
                {
                    var result = await _accountService.getall(); // Gọi sang service
                    return Json(new
                    {
                        code =200,
                        message = "Lấy danh sách quyền thành công.",
                        data = result
                    });
                }
                catch (Exception ex)
                {
                    _errorHandler.WriteToFile(ex);
                    return Json(new
                    {
                        code = 500,
                        message = "Đã xảy ra lỗi.",
                        data = new List<object>()
                    });
                }
            }
           
            public async Task<IActionResult> getUser()
            {
                try
                {
                    var result = await _accountService.getUSer();
                    return Json(new
                    {
                        code = 200,
                        message = "Lấy danh sách quyền thành công.",
                        data = result
                    });
                }
                catch (Exception ex)
                {
                    _errorHandler.WriteToFile(ex);
                    return Json(new
                    {
                        code = 500,
                        message = "Đã xảy ra lỗi.",
                        data = new List<object>()
                    });
                }
            }
       
        public async Task<IActionResult> deleteUserPermission([FromBody] UserPermissionDto userPermissionDto)
        {
            try
            {
                _errorHandler.WriteStringToFile("DTO in Controller (AFTER [FromBody])", userPermissionDto);
                _errorHandler.WriteStringToFuncion("AccountController", nameof(updateUserPermission));
                _errorHandler.WriteStringToFile("DTO in Controller", userPermissionDto);

                var result = await _accountService.deletePermission(userPermissionDto);


                return Json(result); // Nếu là object thì ok
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return Json(new
                {
                    code = 500,
                    message = "Đã xảy ra lỗi.",
                    data = new List<object>()
                });
            }
        }

       
            public async Task<IActionResult> updateUserPermission([FromBody] UserPermissionDto userPermissionDto)
            {
                try
                {
                    _errorHandler.WriteStringToFile("DTO in Controller (AFTER [FromBody])", userPermissionDto);
                    _errorHandler.WriteStringToFuncion("AccountController", nameof(updateUserPermission));
                    _errorHandler.WriteStringToFile("DTO in Controller", userPermissionDto);

                    var result = await _accountService.UpdatePermission(userPermissionDto);

                 
                    return Json(result); // Nếu là object thì ok
                }
                catch (Exception ex)
                {
                    _errorHandler.WriteToFile(ex);
                    return Json(new
                    {
                        code = 500,
                        message = "Đã xảy ra lỗi.",
                        data = new List<object>()
                    });
                }
            }



}
    }
