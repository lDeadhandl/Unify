using Newtonsoft.Json;

namespace Unify
{
    class AuthParams
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; } //can be used when original token expires

        [JsonProperty("scope")]
        public string Scope { get; set; }

    }
}
