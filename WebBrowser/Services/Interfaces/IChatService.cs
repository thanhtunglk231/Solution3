using CoreLib.Dtos;
using WebBrowser.Models;

namespace WebBrowser.Services.Interfaces
{
    public interface IChatService
    {
        Task<ApiResponse123> CreateGroupChatAsync(CreateGroupChatDto dto);
        Task<ApiResponse123> GetChatBetweenUsersAsync(GetChatBetweenUsers dto);
        Task<ApiResponse123> GetGroupChatsByUserAsync(GetGroupChatbyuserDto dto);
        Task<ApiResponse123> SearchUsersByUsernameAsync(string username);
        Task<ApiResponse123> get_all_users_except(string username);
        Task<ApiResponse123> GetGroupUser(string username);
        Task<ApiResponse123> AddUserTogroup(addUserToGroup addUserToGroup);
        Task<ApiResponse123> createGroupwithCreator(createGroupwithCreator dto);
        Task<ApiResponse123> getMessagesinGroup(getMessagesinGroup username);
        Task<ApiResponse123> GetGroupMenbers(GetGroupMembersRequest username);
        Task<ApiResponse123> RemoveGroupMembers(addUserToGroup dto);
        Task<ApiResponse123> DeleteMessage(string MessageId);
    }
}