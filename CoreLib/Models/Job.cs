using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Models
{
    public class Job
    {
        public string MAJOB { get; set; }
        public string TENJOB { get; set; }
        public decimal? MIN_SALARY { get; set; } 
        public decimal? MAX_SALARY { get;set; }
    }
}
