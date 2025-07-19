using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Dtos
{
    public class VerifyOtpRequest
    {
        public string email { get; set; }
        public string otp { get; set; }
    }
}
