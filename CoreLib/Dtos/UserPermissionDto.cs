using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoreLib.Dtos
{
    public class UserPermissionDto
    {
        [JsonProperty("Permission_Code")]
        public string Permission_Code { get; set; }

        [JsonProperty("username")]
        public string username { get; set; }
    }
}
