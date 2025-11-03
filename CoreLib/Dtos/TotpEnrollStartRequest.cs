using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Dtos
{
    public class TotpEnrollStartRequest
    {
        public string username { get; set; }
        public string issuer { get; set; } = "MyWebApp"; // Tên app hiển thị trong Google Authenticator
    }
}
