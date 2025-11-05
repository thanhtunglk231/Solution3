using CommonLib.Handles;
using DataServiceLib.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WebApi.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class CustomAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly List<string> _requiredRoles;

        public bool View { get; set; }
        public bool Add { get; set; }
        public bool Edit { get; set; }
        public bool Delete { get; set; }

        public CustomAuthorizeAttribute(string requiredPermission)
        {
            _requiredRoles = requiredPermission
                .ToLower()
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            IErrorHandler? errorHandler = null;

            try
            {
                errorHandler = context.HttpContext.RequestServices.GetService(typeof(IErrorHandler)) as IErrorHandler;
                errorHandler?.WriteStringToFuncion("CustomAuthorizeAttribute", "OnAuthorization");

                var user = context.HttpContext.User;

                if (!user.Identity?.IsAuthenticated ?? true)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var username = user.Identity.Name?.ToLower();

                var permissionService = context.HttpContext.RequestServices.GetService(typeof(IPermissionService)) as IPermissionService;

                if (permissionService == null)
                {
                    context.Result = new JsonResult(new
                    {
                        code = "500",
                        message = "Lỗi hệ thống: không lấy được dịch vụ phân quyền",
                        success = false
                    })
                    {
                        StatusCode = 500
                    };
                    return;
                }

                var requiredPermissions = _requiredRoles
                    .SelectMany(role => new[]
                    {
                        (View, $"{role}:view"),
                        (Add, $"{role}:add"),
                        (Edit, $"{role}:edit"),
                        (Delete, $"{role}:delete")
                    })
                    .Where(p => p.Item1)
                    .Select(p => p.Item2)
                    .ToList();

                errorHandler?.WriteStringToFile("Danh sách quyền yêu cầu", requiredPermissions);

                var userPermissions = permissionService.GetAllPermissions(username).Result;
                errorHandler?.WriteStringToFile("Danh sách quyền của user", userPermissions);

                bool hasAnyPermission = false;

                foreach (var permission in requiredPermissions)
                {
                    var hasPermission = permissionService.HasPermission(username, permission).Result;

                    if (hasPermission)
                    {
                        hasAnyPermission = true;
                        break;
                    }
                }

                if (!hasAnyPermission)
                {
                    context.Result = new JsonResult(new
                    {
                        code = "403",
                        message = $"Không có quyền truy cập (cần 1 trong các quyền: {string.Join(", ", requiredPermissions)})",
                        success = false
                    })
                    {
                        StatusCode = 403
                    };
                }

            }
            catch (Exception ex)
            {
                errorHandler?.WriteToFile(ex);
                context.Result = new JsonResult(new
                {
                    code = "500",
                    message = "Lỗi khi kiểm tra phân quyền: " + ex.Message,
                    success = false
                })
                {
                    StatusCode = 500
                };
            }
        }
    }
}
