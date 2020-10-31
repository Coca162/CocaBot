using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CocaBot
{
    public struct ConfigJson
    {
        [JsonProperty("token")]
        public string Token { get; private set; }
        [JsonProperty("prefix")]
        public string Prefix { get; private set; }
        [JsonProperty("server_id")]
        public ulong ServerID { get; private set; }
        [JsonProperty("welcome_id")]    
        public ulong WelcomeID { get; private set; }
        [JsonProperty("district_name")]
        public string DistrictName { get; private set; }
        [JsonProperty("citizen_id")]
        public ulong CitizenID { get; private set; }
        [JsonProperty("non-citizen_id")]
        public ulong NonCitizenID { get; private set; }
        [JsonProperty("senate_id")]
        public ulong SenateID { get; private set; }
    }
}