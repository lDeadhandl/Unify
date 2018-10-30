using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Unify
{
    class AuthParams
    {
        //[JsonProperty("access_token")]
        public string access_token { get; set; }

        //[JsonProperty("token_type")]
        public string token_type { get; set; }

        //[JsonProperty("expires_in")]
        public int expires_in { get; set; }

        //[JsonProperty("refresh_token")]
        public string refresh_token { get; set; } //can be used when original token expires

        //[JsonProperty("scope")]
        public string scope { get; set; }

    }
}
