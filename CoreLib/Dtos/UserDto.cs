using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Dtos
{
    public class UserDto
    {
        public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public string? ROLE { get; set; }
        public string? MANV { get; set; }
    }
}
