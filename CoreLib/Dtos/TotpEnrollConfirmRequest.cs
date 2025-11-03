using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Dtos
{
    public class TotpEnrollConfirmRequest
    {
        public string username { get; set; }
        public string secretBase32 { get; set; }
        public string code { get; set; }
    }
}
