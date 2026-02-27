using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Dtos
{
    public class PermissionDto
    {

        [JsonProperty("Permission_Code")]
        public string Permission_Code { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }
        
    }
}
