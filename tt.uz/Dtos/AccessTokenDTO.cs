using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tt.uz.Dtos
{
    public class AccessTokenDTO
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
    }
}
