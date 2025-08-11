using CommonLib.Handles;
using CommonLib.Interfaces;
using CoreLib.Dtos;
using CoreLib.Models;
using DataServiceLib.Interfaces1;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace DataServiceLib.Implementations1
{
    public class CChat : ICChat
    {
        private readonly ICBaseDataProvider1 _dataProvider;
        private readonly string _connectString;
        private readonly IErrorHandler _errorHandler;
        private readonly IRedisService _redisService;

        public CChat(
            ICBaseDataProvider1 cBaseDataProvider1,
            IConfiguration configuration,
            IErrorHandler errorHandler,
            IRedisService redisService)
        {
            _dataProvider = cBaseDataProvider1;
            _connectString = configuration.GetConnectionString("OracleDb");
            _errorHandler = errorHandler;
            _redisService = redisService;
        }

        public async Task<CResponseMessage1> get_group_members(GetGroupMembersRequest groupId)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CChat), nameof(get_messages_in_group));

                var p_group_id = new OracleParameter("p_group_id", OracleDbType.Varchar2)
                {
                    Direction = ParameterDirection.Input,
                    Value = groupId.groupId
                };

                var o_cursor = new OracleParameter("o_cursor", OracleDbType.RefCursor)
                {
                    Direction = ParameterDirection.Output
                };

                var o_code = new OracleParameter("o_code", OracleDbType.Varchar2, 10)
                {
                    Direction = ParameterDirection.Output
                };

                var o_message = new OracleParameter("o_message", OracleDbType.Varchar2, 4000)
                {
                    Direction = ParameterDirection.Output
                };

                var parameters = new OracleParameter[] { p_group_id, o_cursor, o_code, o_message };

                var dataset = _dataProvider.GetDatasetFromSP("sp_get_group_members", parameters, _connectString);

                var response = new CResponseMessage1
                {
                    Data = dataset,
                    code = o_code.Value?.ToString() ?? "400",
                    message = o_message.Value?.ToString() ?? "Không lấy được phản hồi",
                    Success = (o_code.Value?.ToString() == "200")
                };

                return response;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi server: " + ex.Message
                };
            }
        }

        public async Task<CResponseMessage1> remove_user_from_group(addUserToGroup groupId)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CChat), nameof(get_messages_in_group));

                var p_group_id = new OracleParameter("p_group_id", OracleDbType.Varchar2)
                {
                    Direction = ParameterDirection.Input,
                    Value = groupId.groupId
                };
                var p_username = new OracleParameter("p_username", OracleDbType.Varchar2)
                {
                    Direction = ParameterDirection.Input,
                    Value = groupId.username
                };

                var o_code = new OracleParameter("o_code", OracleDbType.Varchar2, 10)
                {
                    Direction = ParameterDirection.Output
                };

                var o_message = new OracleParameter("o_message", OracleDbType.Varchar2, 4000)
                {
                    Direction = ParameterDirection.Output
                };

                var parameters = new OracleParameter[] { p_group_id, p_username, o_code, o_message };

                var dataset = _dataProvider.GetDatasetFromSP("sp_remove_user_from_group", parameters, _connectString);

                var response = new CResponseMessage1
                {
                    Data = dataset,
                    code = o_code.Value?.ToString() ?? "400",
                    message = o_message.Value?.ToString() ?? "Không lấy được phản hồi",
                    Success = (o_code.Value?.ToString() == "200")
                };

                return response;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi server: " + ex.Message
                };
            }
        }

        public async Task<CResponseMessage1> get_messages_in_group(getMessagesinGroup request)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CChat), nameof(get_messages_in_group));

                var p_group_id = new OracleParameter("p_group_id", OracleDbType.Varchar2)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.groupId
                };

                var o_cursor = new OracleParameter("o_cursor", OracleDbType.RefCursor)
                {
                    Direction = ParameterDirection.Output
                };

                var o_code = new OracleParameter("o_code", OracleDbType.Varchar2, 10)
                {
                    Direction = ParameterDirection.Output
                };

                var o_message = new OracleParameter("o_message", OracleDbType.Varchar2, 4000)
                {
                    Direction = ParameterDirection.Output
                };

                var parameters = new OracleParameter[] { p_group_id, o_cursor, o_code, o_message };

                var dataset = _dataProvider.GetDatasetFromSP("sp_get_messages_in_group", parameters, _connectString);

                var response = new CResponseMessage1
                {
                    Data = dataset,
                    code = o_code.Value?.ToString() ?? "400",
                    message = o_message.Value?.ToString() ?? "Không lấy được phản hồi",
                    Success = (o_code.Value?.ToString() == "200")
                };

                return response;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi server: " + ex.Message
                };
            }
        }

        public async Task<CResponseMessage1> Delete_Message(string message_Id)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CChat), nameof(Delete_Message));

                var p_message_id = new OracleParameter("p_message_id ", OracleDbType.Varchar2)
                {
                    Direction = ParameterDirection.Input,
                    Value = message_Id
                };

              

                var o_code = new OracleParameter("o_code", OracleDbType.Varchar2, 10)
                {
                    Direction = ParameterDirection.Output
                };

                var o_message = new OracleParameter("o_message", OracleDbType.Varchar2, 4000)
                {
                    Direction = ParameterDirection.Output
                };

                var parameters = new OracleParameter[] { p_message_id, o_code, o_message };

                var dataset = _dataProvider.GetDatasetFromSP("sp_delete_chat_message", parameters, _connectString);

                var response = new CResponseMessage1
                {
                    
                    code = o_code.Value?.ToString() ?? "400",
                    message = o_message.Value?.ToString() ?? "Không lấy được phản hồi",
                    Success = (o_code.Value?.ToString() == "200")
                };

                return response;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi server: " + ex.Message
                };
            }
        }

        public async Task<CResponseMessage1> get_chat_between_users(GetChatBetweenUsers getChatBetweenUsers)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CChat), nameof(get_chat_between_users));

                var p_user1 = new OracleParameter("p_user1", OracleDbType.Varchar2) { Value = getChatBetweenUsers.usename1 };
                var p_user2 = new OracleParameter("p_user2", OracleDbType.Varchar2) { Value = getChatBetweenUsers.usename2 };
                var o_cursor = new OracleParameter("o_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output };
                var o_code = new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output };
                var o_message = new OracleParameter("o_message", OracleDbType.Varchar2, 4000) { Direction = ParameterDirection.Output };

                var para = new OracleParameter[] { p_user1, p_user2, o_cursor, o_code, o_message };

                var dataset = _dataProvider.GetDatasetFromSP("sp_get_chat_between_users", para, _connectString);

                var response = new CResponseMessage1
                {
                    Data = dataset,
                    code = o_code.Value?.ToString() ?? "400",
                    message = o_message.Value?.ToString() ?? "Không lấy được phản hồi",
                    Success = (o_code.Value?.ToString() == "200")
                };

                return response;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi server: " + ex.Message
                };
            }
        }

        public async Task<CResponseMessage1> search_users_by_username(string username)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CChat), nameof(search_users_by_username));

                var p_username = new OracleParameter("p_username_pattern", OracleDbType.Varchar2) { Value = username };
                var o_cursor = new OracleParameter("o_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output };
                var o_code = new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output };
                var o_message = new OracleParameter("o_message", OracleDbType.Varchar2, 4000) { Direction = ParameterDirection.Output };

                var para = new OracleParameter[] { p_username, o_cursor, o_code, o_message };

                var dataset = _dataProvider.GetDatasetFromSP("sp_search_users_by_username", para, _connectString);

                var response = new CResponseMessage1
                {
                    Data = dataset,
                    code = o_code.Value?.ToString() ?? "400",
                    message = o_message.Value?.ToString() ?? "Không lấy được phản hồi",
                    Success = (o_code.Value?.ToString() == "200")
                };
                return response;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi server: " + ex.Message
                };
            }
        }

        public async Task<CResponseMessage1> get_group_chat_by_user(GetGroupChatbyuserDto getGroupChatbyuserDto)
        {
            try
            {
                _errorHandler.WriteStringToFile(nameof(CChat), nameof(get_group_chat_by_user));

                var p_username = new OracleParameter("p_username", OracleDbType.Varchar2) { Value = getGroupChatbyuserDto.username };
                var p_group_id = new OracleParameter("p_group_id", OracleDbType.Varchar2) { Value = getGroupChatbyuserDto.GroupID };
                var o_cursor = new OracleParameter("o_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output };
                var o_code = new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output };
                var o_message = new OracleParameter("o_message", OracleDbType.Varchar2, 4000) { Direction = ParameterDirection.Output };

                var para = new OracleParameter[] { p_username, p_group_id, o_cursor, o_code, o_message };

                var dataset = _dataProvider.GetDatasetFromSP("sp_get_group_chat_by_user", para, _connectString);

                var response = new CResponseMessage1
                {
                    Data = dataset,
                    code = o_code.Value?.ToString() ?? "400",
                    message = o_message.Value?.ToString() ?? "Không lấy được phản hồi",
                    Success = (o_code.Value?.ToString() == "200")
                };
                return response;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi server: " + ex.Message
                };
            }
        }

        public async Task<CResponseMessage1> AddUsertoGroup(addUserToGroup addUserToGroup)
        {
            try
            {
                _errorHandler.WriteStringToFile(nameof(CChat), nameof(create_chat_group));

                var p_group_name = new OracleParameter("p_group_id", OracleDbType.Varchar2) { Value = addUserToGroup.groupId };
                var o_group_id = new OracleParameter("p_username", OracleDbType.Varchar2) { Value = addUserToGroup.username };
                var o_code = new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output };
                var o_message = new OracleParameter("o_message", OracleDbType.Varchar2, 4000) { Direction = ParameterDirection.Output };

                var para = new OracleParameter[] { p_group_name, o_group_id, o_code, o_message };

                var dataset = _dataProvider.GetDatasetFromSP("sp_add_user_to_group", para, _connectString);

                var response = new CResponseMessage1
                {
                    Data = dataset,
                    code = o_code.Value?.ToString() ?? "400",
                    message = o_message.Value?.ToString() ?? "Không lấy được phản hồi",
                    Success = (o_code.Value?.ToString() == "200")
                };
                return response;

            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi server: " + ex.Message
                };
            }
        }

        public async Task<CResponseMessage1> create_group_with_creator(createGroupwithCreator createGroupChatDto)
        {
            try
            {
                _errorHandler.WriteStringToFile(nameof(CChat), nameof(create_group_with_creator));

                var p_group_name = new OracleParameter("p_group_name", OracleDbType.Varchar2) { Value = createGroupChatDto.groupName };
                var p_creator_username = new OracleParameter("p_creator_username", OracleDbType.Varchar2) { Value = createGroupChatDto.username };
                var o_group_id = new OracleParameter("o_group_id", OracleDbType.Varchar2, 100) { Direction = ParameterDirection.Output };
                var o_code = new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output };
                var o_message = new OracleParameter("o_message", OracleDbType.Varchar2, 4000) { Direction = ParameterDirection.Output };

                var para = new OracleParameter[]
                {
                    p_group_name,
                    p_creator_username,
                    o_group_id,
                    o_code,
                    o_message
                };

                var dataset = _dataProvider.GetDatasetFromSP("sp_create_group_with_creator", para, _connectString);

                var response = new CResponseMessage1
                {
                    Data = dataset,
                    code = o_code.Value?.ToString() ?? "400",
                    message = o_message.Value?.ToString() ?? "Không lấy được phản hồi",
                    Success = (o_code.Value?.ToString() == "200")
                };

                return response;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi server: " + ex.Message
                };
            }
        }

        public async Task<CResponseMessage1> create_chat_group(CreateGroupChatDto createGroupChatDto)
        {
            try
            {
                _errorHandler.WriteStringToFile(nameof(CChat), nameof(create_chat_group));

                var p_group_name = new OracleParameter("p_group_name", OracleDbType.Varchar2) { Value = createGroupChatDto.groupName };
                var o_group_id = new OracleParameter("o_group_id", OracleDbType.Varchar2) { Direction = ParameterDirection.Output };
                var o_code = new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output };
                var o_message = new OracleParameter("o_message", OracleDbType.Varchar2, 4000) { Direction = ParameterDirection.Output };

                var para = new OracleParameter[] { p_group_name, o_group_id, o_code, o_message };

                var dataset = _dataProvider.GetDatasetFromSP("sp_create_chat_group", para, _connectString);

                var response = new CResponseMessage1
                {
                    Data = dataset,
                    code = o_code.Value?.ToString() ?? "400",
                    message = o_message.Value?.ToString() ?? "Không lấy được phản hồi",
                    Success = (o_code.Value?.ToString() == "200")
                };
                return response;

            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi server: " + ex.Message
                };
            }
        }

        public async Task<CResponseMessage1> get_all_users_except(string username)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CChat), nameof(get_all_users_except));

                var p_current_username = new OracleParameter("p_current_username", OracleDbType.Varchar2) { Value = username };
                var o_cursor = new OracleParameter("o_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output };
                var o_code = new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output };
                var o_message = new OracleParameter("o_message", OracleDbType.Varchar2, 4000) { Direction = ParameterDirection.Output };

                var para = new OracleParameter[] { p_current_username, o_cursor, o_code, o_message };

                var dataset = _dataProvider.GetDatasetFromSP("sp_get_all_users_except_current", para, _connectString);

                var response = new CResponseMessage1
                {
                    Data = dataset,
                    code = o_code.Value?.ToString() ?? "400",
                    message = o_message.Value?.ToString() ?? "Không lấy được phản hồi",
                    Success = (o_code.Value?.ToString() == "200")
                };
                return response;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi server: " + ex.Message
                };

            }
        }

        public async Task<CResponseMessage1> GetGroupsByUser(string username)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CChat), nameof(GetGroupsByUser));

                var p_current_username = new OracleParameter("p_current_username", OracleDbType.Varchar2) { Value = username };
                var o_cursor = new OracleParameter("o_cursor", OracleDbType.RefCursor) { Direction = ParameterDirection.Output };
                var o_code = new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output };
                var o_message = new OracleParameter("o_message", OracleDbType.Varchar2, 4000) { Direction = ParameterDirection.Output };

                var para = new OracleParameter[] { p_current_username, o_cursor, o_code, o_message };

                var dataset = _dataProvider.GetDatasetFromSP("sp_get_groups_by_user", para, _connectString);

                var response = new CResponseMessage1
                {
                    Data = dataset,
                    code = o_code.Value?.ToString() ?? "400",
                    message = o_message.Value?.ToString() ?? "Không lấy được phản hồi",
                    Success = (o_code.Value?.ToString() == "200")
                };
                return response;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new CResponseMessage1
                {
                    Success = false,
                    code = "500",
                    message = "Lỗi server: " + ex.Message
                };

            }
        }

        /// <summary>
        /// Gọi sp_add_chat_message nhiều lần tương ứng số lượng URL.
        /// - Nếu có property AttachmentUrls (IEnumerable&lt;string&gt;): lặp theo danh sách này.
        /// - Nếu không có, dùng AttachmentUrl đơn lẻ.
        /// - Nếu không có URL nào: gọi 1 lần với p_url = NULL.
        /// </summary>
        public async Task<(bool success, string code, string message)> SaveMessageAsync(ChatMessageDto dto)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CChat), nameof(SaveMessageAsync));

                // Gom danh sách URL một cách an toàn (hỗ trợ cả AttachmentUrl & AttachmentUrls nếu có)
                var urls = new List<string>();

                if (!string.IsNullOrWhiteSpace(dto?.AttachmentUrl))
                    urls.Add(dto.AttachmentUrl);

                // Dùng reflection để không phụ thuộc compile nếu DTO chưa có AttachmentUrls
                var prop = dto?.GetType().GetProperty("AttachmentUrls");
                if (prop != null)
                {
                    var enumerable = prop.GetValue(dto) as System.Collections.IEnumerable;
                    if (enumerable != null)
                    {
                        foreach (var item in enumerable)
                        {
                            var s = item?.ToString();
                            if (!string.IsNullOrWhiteSpace(s)) urls.Add(s);
                        }
                    }
                }

                bool allSuccess = true;
                string lastCode = "200";
                string lastMessage = "OK";

                using (var conn = new OracleConnection(_connectString))
                using (var cmd = new OracleCommand("sp_add_chat_message", conn) { CommandType = CommandType.StoredProcedure })
                {
                    // Khai báo tham số 1 lần
                    var pSender = cmd.Parameters.Add("p_sender_username", OracleDbType.Varchar2);
                    var pReceiver = cmd.Parameters.Add("p_receiver_username", OracleDbType.Varchar2);
                    var pGroupId = cmd.Parameters.Add("p_group_id", OracleDbType.Varchar2);
                    var pText = cmd.Parameters.Add("p_message_text", OracleDbType.Varchar2);
                    var pUrl = cmd.Parameters.Add("p_url", OracleDbType.Varchar2);

                    var oCode = new OracleParameter("o_code", OracleDbType.Varchar2, 10) { Direction = ParameterDirection.Output };
                    var oMessage = new OracleParameter("o_message", OracleDbType.Varchar2, 4000) { Direction = ParameterDirection.Output };
                    cmd.Parameters.Add(oCode);
                    cmd.Parameters.Add(oMessage);

                    await conn.OpenAsync();

                    // Hàm local để set & gọi
                    async Task<(bool ok, string code, string msg)> ExecOnceAsync(string urlOrNull)
                    {
                        pSender.Value = dto?.SenderUsername ?? (object)DBNull.Value;
                        pReceiver.Value = string.IsNullOrWhiteSpace(dto?.ReceiverUsername) ? (object)DBNull.Value : dto.ReceiverUsername;
                        pGroupId.Value = string.IsNullOrWhiteSpace(dto?.GroupId) ? (object)DBNull.Value : dto.GroupId;
                        pText.Value = dto?.MessageText ?? string.Empty;
                        pUrl.Value = string.IsNullOrWhiteSpace(urlOrNull) ? (object)DBNull.Value : urlOrNull;

                        // reset output buffer (Oracle driver sẽ set lại khi exec)
                        oCode.Value = DBNull.Value;
                        oMessage.Value = DBNull.Value;

                        await cmd.ExecuteNonQueryAsync();

                        var code = oCode.Value?.ToString() ?? "500";
                        var msg = oMessage.Value?.ToString() ?? "Không lấy được phản hồi";
                        return (code == "200", code, msg);
                    }

                    if (urls.Count == 0)
                    {
                        // Không có URL -> gọi 1 lần (url = null)
                        var (ok, code, msg) = await ExecOnceAsync(null);
                        allSuccess = ok;
                        lastCode = code;
                        lastMessage = msg;
                    }
                    else
                    {
                        // Có nhiều URL -> gọi lặp, mỗi lần 1 URL
                        foreach (var u in urls)
                        {
                            var (ok, code, msg) = await ExecOnceAsync(u);
                            if (!ok) allSuccess = false;
                            lastCode = code;
                            lastMessage = msg;
                        }
                    }
                }

                return (allSuccess, lastCode, lastMessage);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return (false, "500", "Lỗi khi gửi tin nhắn: " + ex.Message);
            }
        }
    }
}
