using CoreLib.Dtos;

namespace WebBrowser.Models.ViewModels
{
    public class UserListViewModel
    {
        public List<UserDto> Users { get; set; } = new();
        public ApiResponse Response { get; set; } = new();
    }
}
