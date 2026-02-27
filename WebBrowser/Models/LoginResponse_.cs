namespace WebBrowser.Models
{
    public class LoginResponse_
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }

        // để nhận token khi verify xong hoặc login không bật MFA
        public string token { get; set; }
        public string username { get; set; }
        public string role { get; set; }
        public string manv { get; set; }
        public string email { get; set; }

        // để nhận tín hiệu yêu cầu MFA
        public bool? mfaRequired { get; set; }
        public string[] methods { get; set; }
    }
}
