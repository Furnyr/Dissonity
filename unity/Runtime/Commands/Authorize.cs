using System;
using Dissonity.Commands.Responses;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    [Serializable]
    internal class Authorize : FrameCommand<AuthorizeResponse>
    {
        #nullable enable annotations

        internal override string Command => DiscordCommandType.Authorize;

        [JsonProperty("client_id")]
        public string ClientId { get; set; }
        
        [JsonProperty("scope", NullValueHandling = NullValueHandling.Ignore)]
        public string[] Scope { get; set; }
        
        [JsonProperty("response_type")]
        public string? ResponseType { get; set; }
        
        [JsonProperty("code_challenge")] 
        public string? CodeChallenge { get; set; }
        
        [JsonProperty("state")] 
        public string? State { get; set; }

        [JsonProperty("prompt")]
        public string? Prompt => "none";
     
        public Authorize(string clientId, string[] scope)
        {
            ClientId = clientId;
            Scope = scope;
        }
    }
}