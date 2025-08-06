using CommonLib.Handles;
using CommonLib.Helpers;
using CoreLib.Dtos;
using CoreLib.Models;
using DataServiceLib.Interfaces1;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ChatController : ControllerBase
    {
        private readonly ICChat _chat;
        private readonly IDataConvertHelper _dataConvertHelper;
        private readonly IErrorHandler _errorHandler;
        public ChatController(ICChat chat, IDataConvertHelper dataConvertHelper, IErrorHandler errorHandler)
        {
            _chat = chat;
            _dataConvertHelper = dataConvertHelper;
            this._errorHandler = errorHandler;
        }
        [HttpPost("getchatbetweenusers")]
        public async Task<IActionResult> GetChatBetweenUsers([FromBody] GetChatBetweenUsers getChatBetweenUsers)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("ChatController", "GetChatBetweenUsers");
                var response = await _chat.get_chat_between_users(getChatBetweenUsers);
                return Ok(response);
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
        [HttpGet("SearchUser")]
        public async Task<IActionResult> search_users_by_username([FromQuery]string username )
        {
            try
            {
                _errorHandler.WriteStringToFuncion("ChatController", "GetChatBetweenUsersGet");
                
                var response = await _chat.search_users_by_username(username);
                return Ok(response);
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

        [HttpGet("getallEx")]
        public async Task<IActionResult> get_all_users_except([FromQuery] string username)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("ChatController", "get_all_users_except");

                var response = await _chat.get_all_users_except(username);
                return Ok(response);
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

        [HttpGet("getGroupMenbers")]
        public async Task<IActionResult> getGroupMenbers([FromQuery] GetGroupMembersRequest username)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("ChatController", "getGroupMenbers");

                var response = await _chat.get_group_members(username);
                return Ok(response);
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

        [HttpPost("RemoveUserGroup")]
        public async Task<IActionResult> getGroupMenbers([FromBody] addUserToGroup username)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("ChatController", "getGroupMenbers");

                var response = await _chat.remove_user_from_group(username);
                return Ok(response);
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


        [HttpPost("CreateGroupChatDto")]
        public async Task<IActionResult> create_chat_group([FromBody]CreateGroupChatDto createGroupChatDto)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("ChatController", "Get");
               var response = await _chat.create_chat_group(createGroupChatDto);
                return Ok(response);
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
        [HttpPost("CreateGroupChatWithUser")]
        public async Task<IActionResult> CreateGroupChatWithUser([FromBody] createGroupwithCreator createGroupChatDto)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(ChatController), nameof(CreateGroupChatWithUser));

                var response = await _chat.create_group_with_creator(createGroupChatDto);

                return Ok(response);
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
        [HttpPost("getMessagesinGroup")]
        public async Task<IActionResult> getMessagesinGroup([FromBody] getMessagesinGroup createGroupChatDto)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(ChatController), nameof(CreateGroupChatWithUser));

                var response = await _chat.get_messages_in_group(createGroupChatDto);

                return Ok(response);
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
        [HttpPost("getGroupUser")]
        public async Task<IActionResult> get_group_chat_by_user([FromQuery]string username)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("ChatController", "get_group_chat_by_user");
                var response = await _chat.GetGroupsByUser(username);
                return Ok(response);
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
        [HttpPost("addUserToGroup")]
        public async Task<IActionResult> AddUserToGroup([FromBody] addUserToGroup request)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("ChatController", "AddUserToGroup");

                var result = await _chat.AddUsertoGroup(request);
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






        [HttpPost("GetGroupChatbyuserDto")]
        public async Task<IActionResult> create_chat_group([FromBody] GetGroupChatbyuserDto getGroupChatbyuserDto)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("ChatController", "Get");
                var response = await _chat.get_group_chat_by_user(getGroupChatbyuserDto);
                return Ok(response);
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

    }
}
