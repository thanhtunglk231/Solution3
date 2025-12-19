using System.Threading.Tasks;

namespace WebBrowser.Services.Interfaces
{
    public interface IMyProfileService
    {
        Task<ApiResponse<MyProfileDto>?> GetMe();
        Task<ApiResponse<object>?> UpdateMe(UpdateMyProfileRequest model);
        Task<ApiResponse<object>?> DeleteMe();
        void UseToken(string token);
    }

    public class UpdateMyProfileRequest
    {
        public string? Email { get; set; }
        public string? Sdt { get; set; }
        public string? Manv { get; set; }
    }

    // DTO đúng theo JSON API trả về
    public class MyProfileDto
    {
        public string? username { get; set; }
        public string? role { get; set; }
        public string? manv { get; set; }
        public string? email { get; set; }
        public string? sdt { get; set; }

        public string? totP_SECRET_BASE32 { get; set; }
        public int iS_TOTP_ENABLED { get; set; }
        public System.DateTime? totP_ENROLLED_AT { get; set; }
        public System.DateTime? totP_LAST_VERIFIED_AT { get; set; }

        public int mfA_FAIL_COUNT { get; set; }
        public System.DateTime? mfA_LOCKED_UNTIL { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool success { get; set; }
        public string? code { get; set; }
        public string? message { get; set; }
        public T[]? data { get; set; }
    }
}
