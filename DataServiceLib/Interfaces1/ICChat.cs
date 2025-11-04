using CoreLib.Dtos;
using CoreLib.Models;

namespace DataServiceLib.Interfaces1
{
    public interface ICChat
    {
        Task<CResponseMessage1> create_chat_group(CreateGroupChatDto createGroupChatDto);
        Task<CResponseMessage1> get_chat_between_users(GetChatBetweenUsers getChatBetweenUsers);
        Task<CResponseMessage1> get_group_chat_by_user(GetGroupChatbyuserDto getGroupChatbyuserDto);
        Task<CResponseMessage1> search_users_by_username(string username);
        Task<(bool success, string code, string message)> SaveMessageAsync(ChatMessageDto dto);
        Task<CResponseMessage1> get_all_users_except(string username);
        Task<CResponseMessage1> GetGroupsByUser(string username);
        Task<CResponseMessage1> AddUsertoGroup(addUserToGroup addUserToGroup);
        Task<CResponseMessage1> create_group_with_creator(createGroupwithCreator createGroupChatDto);
        Task<CResponseMessage1> get_messages_in_group(getMessagesinGroup request);
        Task<CResponseMessage1> get_group_members(GetGroupMembersRequest groupId);
        Task<CResponseMessage1> remove_user_from_group(addUserToGroup groupId);
        Task<CResponseMessage1> Delete_Message(string message_Id);
    }
}