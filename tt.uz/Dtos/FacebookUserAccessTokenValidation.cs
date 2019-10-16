using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace tt.uz.Dtos
{
    public class FacebookUserAccessTokenValidation
    {
        [JsonProperty("data")]
        public Data Data { get; set;}
    }

    public class Data {
        [JsonProperty("app_id")]
        public string AppId { get; set; }
        [JsonProperty("type")]
        public string Type { get; set; }
        [JsonProperty("application")]
        public string Application { get; set; }
        [JsonProperty("data_access_expires_at")]
        public Int32 DataAccessExpiresAt { get; set; }
        [JsonProperty("error")]
        public Error Error { get; set; }
        [JsonProperty("expires_at")]
        public string ExpiresAt { get; set; }
        [JsonProperty("is_valid")]
        public bool IsValid { get; set; }
        [JsonProperty("scopes")]
        public List<string> Scopes { get; set; }
        [JsonProperty("user_id")]
        public string UserId { get; set; }
    }

    public class Error {
        [JsonProperty("code")]
        public int Code { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
        [JsonProperty("subcode")]
        public int Subcode { get; set; }
    }
}
