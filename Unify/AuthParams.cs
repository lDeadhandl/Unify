using Newtonsoft.Json;

namespace Unify
{
    class AuthParams
    {
        public string AccessToken { get; set; }

        public string TokenType { get; set; }

        public int ExpiresIn { get; set; }

        public string RefreshToken { get; set; } //can be used when original token expires

        public string Scope { get; set; }

    }
}
