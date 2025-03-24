using Newtonsoft.Json;

namespace FantasyFootball.Shared
{
    public class YahooTokenResponse
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("token_type")]
        public string TokentType { get; set; }
        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
        [JsonProperty("xoauth_yahoo_guid")]
        public string XoauthYahooGuid { get; set; }
    }
}
