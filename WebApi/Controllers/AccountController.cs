using CommonLib.Handles;
using CoreLib.Dtos;
using CoreLib.Models;
using DataServiceLib.Interfaces1;
using DataServiceLib.unuse.Implementations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApi.Attributes;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly ICAccountDataProvider _accountDataProvider;
        private readonly IErrorHandler _errorHandler;
        public AccountController(ICAccountDataProvider accountDataProvider, IErrorHandler errorHandler)
        {
            _accountDataProvider = accountDataProvider;
            _errorHandler = errorHandler;
        }
        [HttpDelete("deletePremis")]
        [CustomAuthorize("admin", Edit = true)]
        public async Task<IActionResult> Delete([FromBody] UserPermissionDto userPermissionDto)
        {
            try
            {
                var result = await _accountDataProvider.DeletePermission(userPermissionDto); 

                return Ok(result); 
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi server: " + ex.Message
                });
            }
        }


        [HttpPost("updatePermis")]
        [CustomAuthorize("admin", Edit = true)]
        public async Task<IActionResult> Update([FromBody] UserPermissionDto userPermissionDto)
        {
            try
            {
                var result = await _accountDataProvider.UpdatePermission(userPermissionDto); 

                return Ok(result);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return StatusCode(500, new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi server: " + ex.Message
                });
            }
        }



        [HttpGet("getall")]
        [CustomAuthorize("admin,user", View =true)]
        public async Task<IActionResult> getall()
        {
            var (data, res) = await _accountDataProvider.getall();
            return Ok(
                new
                {
                    data = data,
                    message= res.message,
                    code= res.code
                } 
                );

        }


        [HttpGet("getUser")]
        [CustomAuthorize("admin,user", View = true)]
        public async Task<IActionResult> getUer()
        {
            var (data, res) = await _accountDataProvider.getall_userName();
            return Ok(
                new
                {
                    data = data,
                    message = res.message,
                    code = res.code
                }
                );
        }
    }
}
