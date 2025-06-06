using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Models
{
    public class Department
    {
        public string id {  get; set; } 
        public string name { get; set; }
        public string captain { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
