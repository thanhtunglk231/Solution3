using CommonLib.Handles;
using CommonLib.Helpers;
using CoreLib.Dtos;
using Newtonsoft.Json;
using WebBrowser.Models;
using WebBrowser.Services.ApiServices;
using WebBrowser.Services.Interfaces;

namespace WebBrowser.Services.Implements
{
    public class ChatService : IChatService
    {
        private readonly IHttpService _httpService;
        private readonly IDataConvertHelper _dataConvertHelper;
        private readonly IErrorHandler _errorHandler;

        public ChatService(IHttpService httpService, IDataConvertHelper dataConvertHelper, IErrorHandler errorHandler)
        {
            _httpService = httpService;
            _dataConvertHelper = dataConvertHelper;
            _errorHandler = errorHandler;
        }


        //public async Task<ApiResponse123> GetGroupUser(string username)
        //{
        //    try
        //    {
        //        _errorHandler.WriteStringToFuncion("ChatService", nameof(GetGroupUser));
        //        Console.WriteLine("===> DTO gửi đi: " + username);

        //        var response = await _httpService.PostAsync<ApiResponse123>("Chat/getGroupUser", username);
        //        Console.WriteLine("===> Kết quả trả về (ApiResponse): " + JsonConvert.SerializeObject(response));

        //        var datalist = new List<GetGroupUser>();

        //        if (response?.data != null)
        //        {
        //            var json = JsonConvert.SerializeObject(response.data);
        //            Console.WriteLine("===> Dữ liệu data trước khi deserialize: " + json);

        //            var wrapper = JsonConvert.DeserializeObject<TableWrapper<GetGroupUser>>(json);
        //            datalist = wrapper?.Table ?? new List<GetGroupUser>();

        //            Console.WriteLine("===> Số lượng tin nhắn nhận được: " + datalist.Count);
        //        }

        //        var resule = new ApiResponse123
        //        {
        //            code = response.code,
        //            message = response.message,
        //            success = response.success,
        //            data = datalist
        //        };
        //        Console.WriteLine("✅ Kết quả trả về (resule): " + JsonConvert.SerializeObject(resule));

        //        return resule;
        //    }
        //    catch (Exception ex)
        //    {
        //        _errorHandler.WriteToFile(ex);
        //        Console.WriteLine("===> Lỗi: " + ex.ToString());

        //        return new ApiResponse123
        //        {
        //            code = "500",
        //            message = "Lỗi gọi API: " + ex.Message,
        //            success = false,
        //            data = null
        //        };
        //    }
        //}

        public async Task<ApiResponse123> AddUserTogroup(addUserToGroup addUserToGroup)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("ChatService", nameof(AddUserTogroup));
                Console.WriteLine("===> DTO gửi đi: " + addUserToGroup);

                var response = await _httpService.PostAsync<ApiResponse123>("Chat/addUserToGroup", addUserToGroup);
                Console.WriteLine("===> Kết quả trả về (ApiResponse): " + JsonConvert.SerializeObject(response));

              


                var resule = new ApiResponse123
                {
                    code = response.code,
                    message = response.message,
                    success = response.success,
                   
                };
                Console.WriteLine("✅ Kết quả trả về (resule): " + JsonConvert.SerializeObject(resule));

                return resule;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                Console.WriteLine("===> Lỗi: " + ex.ToString());

                return new ApiResponse123
                {
                    code = "500",
                    message = "Lỗi gọi API: " + ex.Message,
                    success = false,
                 
                };
            }
        }

        public async Task<ApiResponse123> GetGroupUser(string username)
        {
            try
            {
                Console.WriteLine("🔍 [ChatService] => Bắt đầu gọi GetGroupUser với username: " + username);

                string url = $"Chat/getGroupUser?username={username}";
                Console.WriteLine("🌐 [ChatService] => Gọi API POST tới URL: " + url);

                var response = await _httpService.PostAsync<ApiResponse123>(url,null);

                Console.WriteLine("📥 [ChatService] => Nhận response: " + JsonConvert.SerializeObject(response));

                var datalist = new List<GroupChat>();

                if (response?.data != null)
                {
                    var json = JsonConvert.SerializeObject(response.data);
                    Console.WriteLine("📦 [ChatService] => Dữ liệu thô: " + json);

                    var wrapper = JsonConvert.DeserializeObject<TableWrapper<GroupChat>>(json);
                    datalist = wrapper?.Table ?? new List<GroupChat>();

                    Console.WriteLine("✅ [ChatService] => Số nhóm nhận được: " + datalist.Count);
                }
                else
                {
                    Console.WriteLine("⚠️ [ChatService] => Không có dữ liệu trong response.");
                }

                return new ApiResponse123
                {
                    code = response?.code ?? "500",
                    message = response?.message ?? "Không có phản hồi từ server",
                    success = response?.success ?? false,
                    data = datalist
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ [ChatService] => Lỗi xảy ra khi gọi GetGroupUser: " + ex.Message);
                return new ApiResponse123
                {
                    code = "500",
                    message = "Lỗi gọi API: " + ex.Message,
                    success = false,
                    data = null
                };
            }
        }
        public async Task<ApiResponse123> getMessagesinGroup(getMessagesinGroup username)
        {
            try
            {
                Console.WriteLine("🔍 [ChatService] => Bắt đầu gọi GetGroupUser với username: " + username);

                string url = $"Chat/getMessagesinGroup";
                Console.WriteLine("🌐 [ChatService] => Gọi API POST tới URL: " + url);

                var response = await _httpService.PostAsync<ApiResponse123>(url, username);

                Console.WriteLine("📥 [ChatService] => Nhận response: " + JsonConvert.SerializeObject(response));

                var datalist = new List<GroupMessageModel>();

                if (response?.data != null)
                {
                    var json = JsonConvert.SerializeObject(response.data);
                    Console.WriteLine("📦 [ChatService] => Dữ liệu thô: " + json);

                    var wrapper = JsonConvert.DeserializeObject<TableWrapper<GroupMessageModel>>(json);
                    datalist = wrapper?.Table ?? new List<GroupMessageModel>();

                    Console.WriteLine("✅ [ChatService] => Số nhóm nhận được: " + datalist.Count);
                }
                else
                {
                    Console.WriteLine("⚠️ [ChatService] => Không có dữ liệu trong response.");
                }

                var result= new ApiResponse123
                {
                    code = response?.code ?? "500",
                    message = response?.message ?? "Không có phản hồi từ server",
                    success = response?.success ?? false,
                    data = datalist
                };
                Console.WriteLine("🎯 [ChatService] => Kết quả trả về:\n" + JsonConvert.SerializeObject(result, Formatting.Indented));

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("❌ [ChatService] => Lỗi xảy ra khi gọi GetGroupUser: " + ex.Message);
                return new ApiResponse123
                {
                    code = "500",
                    message = "Lỗi gọi API: " + ex.Message,
                    success = false,
                    data = null
                };
            }
        }



        public async Task<ApiResponse123> GetChatBetweenUsersAsync(GetChatBetweenUsers dto)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("ChatService", nameof(GetChatBetweenUsersAsync));
                Console.WriteLine("===> DTO gửi đi: " + JsonConvert.SerializeObject(dto));

                var response = await _httpService.PostAsync<ApiResponse123>("Chat/getchatbetweenusers", dto);
                Console.WriteLine("===> Kết quả trả về (ApiResponse): " + JsonConvert.SerializeObject(response));

                var datalist = new List<MessageDto>();

                if (response?.data != null)
                {
                    var json = JsonConvert.SerializeObject(response.data);
                    Console.WriteLine("===> Dữ liệu data trước khi deserialize: " + json);

                    var wrapper = JsonConvert.DeserializeObject<TableWrapper<MessageDto>>(json);
                    datalist = wrapper?.Table ?? new List<MessageDto>();

                    Console.WriteLine("===> Số lượng tin nhắn nhận được: " + datalist.Count);
                }

                var resule = new ApiResponse123
                {
                    code = response.code,
                    message = response.message,
                    success = response.success,
                    data = datalist ?? new List<MessageDto>()
                };
                Console.WriteLine("✅ Kết quả trả về (resule): " + JsonConvert.SerializeObject(resule));

                return resule ?? new ApiResponse123() ;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                Console.WriteLine("===> Lỗi: " + ex.ToString());

                return new ApiResponse123
                {
                    code = "500",
                    message = "Lỗi gọi API: " + ex.Message,
                    success = false,
                    data = null
                };
            }
        }

        public async Task<ApiResponse123> GetGroupMenbers(GetGroupMembersRequest username)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("ChatService", nameof(GetGroupMenbers));

                if (string.IsNullOrWhiteSpace(username?.groupId))
                    throw new ArgumentException("GroupId không được để trống");

                string url = $"Chat/getGroupMenbers?groupId={Uri.EscapeDataString(username.groupId)}";

                var response = await _httpService.GetAsync<ApiResponse123>(url);
                var datalist = new List<GroupMemberDto>();

                if (response?.data != null)
                {
                    var json = JsonConvert.SerializeObject(response.data);
                    var wrapper = JsonConvert.DeserializeObject<TableWrapper<GroupMemberDto>>(json);
                    datalist = wrapper?.Table ?? new List<GroupMemberDto>();
                }

                var result = new ApiResponse123
                {
                    code = response.code,
                    message = response.message,
                    success = response.success,
                    data = datalist
                };

                Console.WriteLine("🎯 [ChatService] => Kết quả trả về:\n" + JsonConvert.SerializeObject(result, Formatting.Indented));
                return result;
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new ApiResponse123
                {
                    code = "500",
                    message = "Lỗi gọi API: " + ex.Message,
                    success = false,
                    data = null
                };
            }
        }



        public async Task<ApiResponse123> get_all_users_except(string username)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("ChatService", nameof(SearchUsersByUsernameAsync));
                string url = $"Chat/getallEx?username={Uri.EscapeDataString(username)}";
                var response = await _httpService.GetAsync<ApiResponse123>(url);
                var datalist = new List<UserModles>();

                if (response?.data != null)
                {
                    var json = JsonConvert.SerializeObject(response.data);
                    var wrapper = JsonConvert.DeserializeObject<TableWrapper<UserModles>>(json); // sửa đúng kiểu
                    datalist = wrapper?.Table ?? new List<UserModles>();
                }
                return new ApiResponse123
                {
                    code = response.code,
                    message = response.message,
                    success = response.success,
                    data = datalist
                };
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new ApiResponse123
                {
                    code = "500",
                    message = "Lỗi gọi API: " + ex.Message,
                    success = false,
                    data = null
                };
            }
        }
        public async Task<ApiResponse123> SearchUsersByUsernameAsync(string username)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("ChatService", nameof(SearchUsersByUsernameAsync));
                string url = $"Chat/getchatbetweenusers?username={Uri.EscapeDataString(username)}";
                var response = await _httpService.GetAsync<ApiResponse123>(url);
                var datalist = new List<UserModles>();

                if (response?.data != null)
                {
                    var json = JsonConvert.SerializeObject(response.data);
                    var wrapper = JsonConvert.DeserializeObject<TableWrapper<UserModles>>(json);
                    datalist = wrapper?.Table ?? new List<UserModles>();
                }

                return new ApiResponse123
                {
                    code = response.code,
                    message = response.message,
                    success = response.success,
                    data = datalist
                };
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new ApiResponse123
                {
                    code = "500",
                    message = "Lỗi gọi API: " + ex.Message,
                    success = false,
                    data = null
                };
            }
        }
        public async Task<ApiResponse123> DeleteMessage(string MessageId)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("ChatService", nameof(CreateGroupChatAsync));
                return await _httpService.PostAsync<ApiResponse123>("Chat/DeleteMessage", MessageId);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new ApiResponse123
                {
                    code = "500",
                    message = "Lỗi gọi API: " + ex.Message,
                    success = false,
                    data = null
                };
            }
        }
        public async Task<ApiResponse123> RemoveGroupMembers(addUserToGroup dto)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("ChatService", nameof(CreateGroupChatAsync));
                return await _httpService.PostAsync<ApiResponse123>("Chat/RemoveUserGroup", dto);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new ApiResponse123
                {
                    code = "500",
                    message = "Lỗi gọi API: " + ex.Message,
                    success = false,
                    data = null
                };
            }
        }


        // 3. Gọi POST: /api/Chat/CreateGroupChatDto
        public async Task<ApiResponse123> CreateGroupChatAsync(CreateGroupChatDto dto)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("ChatService", nameof(CreateGroupChatAsync));
                return await _httpService.PostAsync<ApiResponse123>("Chat/CreateGroupChatDto", dto);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new ApiResponse123
                {
                    code = "500",
                    message = "Lỗi gọi API: " + ex.Message,
                    success = false,
                    data = null
                };
            }
        }

        public async Task<ApiResponse123> createGroupwithCreator(createGroupwithCreator dto)
        {
            try
            {
                _errorHandler.WriteStringToFuncion(nameof(ChatService), nameof(createGroupwithCreator));

                return await _httpService.PostAsync<ApiResponse123>("Chat/CreateGroupChatWithUser", dto);
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new ApiResponse123
                {
                    code = "500",
                    message = "Lỗi gọi API: " + ex.Message,
                    success = false,
                    data = null
                };
            }
        }


        // 4. Gọi POST: /api/Chat/GetGroupChatbyuserDto
        public async Task<ApiResponse123> GetGroupChatsByUserAsync(GetGroupChatbyuserDto dto)
        {
            try
            {
                _errorHandler.WriteStringToFuncion("ChatService", nameof(GetGroupChatsByUserAsync));
                var response = await _httpService.PostAsync<ApiResponse123>("Chat/GetGroupChatbyuserDto", dto);
                var datalist = new List<GroupChatDto>();

                if (response?.data != null)
                {
                    var json = JsonConvert.SerializeObject(response.data);
                    var wrapper = JsonConvert.DeserializeObject<TableWrapper<GroupChatDto>>(json);
                    datalist = wrapper?.Table ?? new List<GroupChatDto>();
                }

                return new ApiResponse123
                {
                    code = response.code,
                    message = response.message,
                    success = response.success,
                    data = datalist
                };
            }
            catch (Exception ex)
            {
                _errorHandler.WriteToFile(ex);
                return new ApiResponse123
                {
                    code = "500",
                    message = "Lỗi gọi API: " + ex.Message,
                    success = false,
                    data = null
                };
            }
        }

    }
}
