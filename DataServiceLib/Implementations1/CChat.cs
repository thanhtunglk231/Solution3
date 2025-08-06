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
using System.Text;
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

                // Gọi stored procedure
                var parameters = new OracleParameter[] { p_group_id, o_cursor, o_code, o_message };

                var dataset = _dataProvider.GetDatasetFromSP("sp_get_group_members", parameters, _connectString);

                // Tạo kết quả trả về
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

                // Gọi stored procedure
                var parameters = new OracleParameter[] { p_group_id, p_username, o_code, o_message };

                var dataset = _dataProvider.GetDatasetFromSP("sp_remove_user_from_group", parameters, _connectString);

                // Tạo kết quả trả về
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

                // Khai báo tham số đầu vào
                var p_group_id = new OracleParameter("p_group_id", OracleDbType.Varchar2)
                {
                    Direction = ParameterDirection.Input,
                    Value = request.groupId
                };

                // Khai báo các tham số đầu ra
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

                // Gọi stored procedure
                var parameters = new OracleParameter[] { p_group_id, o_cursor, o_code, o_message };

                var dataset = _dataProvider.GetDatasetFromSP("sp_get_messages_in_group", parameters, _connectString);

                // Tạo kết quả trả về
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
                var o_group_id = new OracleParameter("p_username", OracleDbType.Varchar2) { Value=addUserToGroup.username };
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
            catch(Exception ex)
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
            catch(Exception ex)
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
        public async Task<(bool success, string code, string message)> SaveMessageAsync(ChatMessageDto dto)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(CChat), nameof(SaveMessageAsync));

                using var conn = new OracleConnection(_connectString);
                using var cmd = new OracleCommand("sp_add_chat_message", conn)
                {
                    CommandType = CommandType.StoredProcedure
                };

                cmd.Parameters.Add("p_sender_username", OracleDbType.Varchar2).Value = dto.SenderUsername;
                cmd.Parameters.Add("p_receiver_username", OracleDbType.Varchar2).Value = (object?)dto.ReceiverUsername ?? DBNull.Value;
                cmd.Parameters.Add("p_group_id", OracleDbType.Varchar2).Value = (object?)dto.GroupId ?? DBNull.Value;
                cmd.Parameters.Add("p_message_text", OracleDbType.Varchar2).Value = dto.MessageText;

                var o_code = new OracleParameter("o_code", OracleDbType.Varchar2, 10)
                {
                    Direction = ParameterDirection.Output
                };
                var o_message = new OracleParameter("o_message", OracleDbType.Varchar2, 4000)
                {
                    Direction = ParameterDirection.Output
                };

                cmd.Parameters.Add(o_code);
                cmd.Parameters.Add(o_message);

                await conn.OpenAsync();
                await cmd.ExecuteNonQueryAsync();

                string code = o_code.Value?.ToString() ?? "500";
                string message = o_message.Value?.ToString() ?? "Không lấy được phản hồi";

                return (code == "200", code, message);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return (false, "500", "Lỗi khi gửi tin nhắn: " + ex.Message);
            }
        }

    }
}
