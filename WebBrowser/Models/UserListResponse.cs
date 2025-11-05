using CoreLib.Dtos;

namespace WebBrowser.Models
{
    public class UserListResponse
    {
        public string code { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public List<UserDto> data { get; set; }
    }
}
