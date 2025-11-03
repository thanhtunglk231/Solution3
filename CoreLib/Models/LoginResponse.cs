using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Models
{
    public class LoginResponse
    {
        public string? Code { get; set; }
        public string? Message { get; set; }
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Username { get; set; }
        public string? Role { get; set; }
        public string? Manv { get; set; }
        public string email { get; set; }

        public bool? mfaRequired { get; set; }
        public string[] methods { get; set; }
    }
}
