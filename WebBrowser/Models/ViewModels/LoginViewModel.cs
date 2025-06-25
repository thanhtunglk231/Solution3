using CoreLib.Models;

namespace WebBrowser.Models.ViewModels
{
    public class LoginViewModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }
        public string? Manv { get; set; }
        public ApiResponse? Response { get; set; }
    }
}
