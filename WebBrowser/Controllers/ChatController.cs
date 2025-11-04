using CommonLib.Handles;
using CommonLib.Helpers;
using CoreLib.Dtos;
using Microsoft.AspNetCore.Mvc;
using WebBrowser.Models;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Controllers
{
    public class ChatController : BaseController
    {
        private readonly IChatService _chatService;
        private readonly IErrorHandler _errorHandler;
        private readonly IDataConvertHelper _dataConvertHelper;

        public ChatController(
            IChatService chatService,
            IErrorHandler errorHandler,
            IDataConvertHelper dataConvertHelper,
            IConfiguration config
        ) : base(config)
        {
            _chatService = chatService;
            _errorHandler = errorHandler;
            _dataConvertHelper = dataConvertHelper;
        }

        public async Task<IActionResult> Index()
        {
            var username = HttpContext.Session.GetString("Username");
            ViewBag.Username = username;

            var token = GetJwtTokenOrRedirect(out IActionResult? redirectResult);
            if (token == null)
                return redirectResult!;

            return View();
        }

        public IActionResult GlobalChatWidget()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username") ?? "anonymous";
            return PartialView("_GlobalChatWidget");
        }
        public IActionResult Widget()
        {
            ViewBag.Username = HttpContext.Session.GetString("Username") ?? "anonymous";
            return View();
        }
        public async Task<IActionResult> GetGroupUser()
        {
            var token = GetJwtTokenOrRedirect(out IActionResult? redirectResult);
            if (token == null)
                return redirectResult!;
            var username = HttpContext.Session.GetString("Username");
            var result = await _chatService.GetGroupUser(username);
            return Json(result);
        }

        public async Task<IActionResult> GetGroupMembers([FromBody] GetGroupMembersRequest request)
        {
            var token = GetJwtTokenOrRedirect(out IActionResult? redirectResult);
            if (token == null)
                return redirectResult!;

            var result = await _chatService.GetGroupMenbers(request);
            return Json(result);
        }
        public async Task<IActionResult> RemoveGroupMembers([FromBody] addUserToGroup groupId)
        {
            var result = await _chatService.RemoveGroupMembers(groupId);
            return Json(result);
        }
        public async Task<IActionResult> addUsertoGroup([FromBody]addUserToGroup addUserToGroup)
        {
            
            var result = await _chatService.AddUserTogroup(addUserToGroup);
            return Json(result);
        }

        public async Task<IActionResult> GetChatBetweenUsers([FromBody]GetChatBetweenUsers dto)
        {
            var token = GetJwtTokenOrRedirect(out IActionResult? redirectResult);
            if (token == null)
                return redirectResult!;
            var result = await _chatService.GetChatBetweenUsersAsync(dto);
            return Json(result);
        }
        //public async Task<IActionResult> getGroupUser()
        //{
        //    var username = HttpContext.Session.GetString("Username");

        //    var result = await _chatService.GetGroupUser(username);
        //    return Json(result);
        //}

        public async Task<IActionResult> getAllEx()
        {
            var token = GetJwtTokenOrRedirect(out IActionResult? redirectResult);
            if (token == null)
                return redirectResult!;
            var username = HttpContext.Session.GetString("Username");
            var result = await _chatService.get_all_users_except(username);
            return Json(result);
        }
     
        public async Task<IActionResult> CreateGroupChat([FromBody] CreateGroupChatDto dto)
        {
            var result = await _chatService.CreateGroupChatAsync(dto);
            return Json(result);
        }
        public async Task<IActionResult> createGroupwithCreator([FromBody] createGroupwithCreator dto)
        {
            var result = await _chatService.createGroupwithCreator(dto);
            return Json(result);
        }
        public async Task<IActionResult> getMessagesinGroup([FromBody] getMessagesinGroup dto)
        {
            var result = await _chatService.getMessagesinGroup(dto);
            return Json(result);
        }
        public async Task<IActionResult> DeleteMessage([FromBody] string MessageId)
        {
            var result = await _chatService.DeleteMessage(MessageId);
            return Json(result);
        }
        public async Task<IActionResult> GetGroupChatsByUser([FromBody] GetGroupChatbyuserDto dto)
        {
            var result = await _chatService.GetGroupChatsByUserAsync(dto);
            return Json(result);
        }
    }
}
