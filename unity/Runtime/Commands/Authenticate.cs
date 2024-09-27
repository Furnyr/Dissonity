using System;
using Dissonity.Commands.Responses;
using Newtonsoft.Json;

namespace Dissonity.Commands
{
    /// <summary>
    /// Old implementation, now the authentication is handled by the RpcBridge. <br/> <br/>
    /// Still keeping this file around just in case
    /// </summary>
    [Serializable]
    internal class Authenticate : FrameCommand<AuthenticateResponse>
    {
        internal override string Command => DiscordCommandType.Authenticate;
        
        [JsonProperty("access_token")] 
        public string AccessToken { get; set; }

        public Authenticate(string accessToken)
        {
            AccessToken = accessToken;
        }
    }
}