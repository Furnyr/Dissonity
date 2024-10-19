using Dissonity.Commands.Responses;
using Dissonity.Events;
using UnityEngine;

namespace Dissonity.Models.Mock
{
    public class MockMultiEvent : MultiEvent
    {
        new public ReadyEventData ReadyData { get; set; } = new()
        {
            Version = 1,
            Config = new()
            {
                CdnHost = "cdn_host",
                ApiEndpoint = "api_endpoint",
                Environment = "environment"
            }
        };

        new public AuthorizeData AuthorizeData { get; set; } = new()
        {
            Code = "authorize_code"
        };

        new public AuthenticateData AuthenticateData { get; set; } = new()
        {
            AccessToken = "access_token",
            User = new()
            {
                Id = 9123456780,
                Username = "mock",
                Avatar = "4v4t43h4sh",
                GlobalName = "Mock name",
                AccentColor = 0,
                AvatarDecoration = new()
                {
                    Asset = "4v4t43h4sh",
                    SkuId = 123456789
                },
                PublicFlags = 0,
            },
            Scopes = DissonityConfigAttribute.GetUserConfig().OauthScopes,
            Expires = "20xx-04-15T15:50+00Z",
            Application = new()
            {
                Name = "application_name",
                Description = "application_description",
                Icon = "application_icon",
                Id = 123456789,
                RpcOrigins = "rpc_origins"
            }
        };

        new public object ServerResponse = null;

        private void SetMockUser()
        {
            var mock = GameObject.FindObjectOfType<DiscordMock>();

            AuthenticateData.User.Id = mock._currentPlayer.Participant.Id;
            AuthenticateData.User.Username = mock._currentPlayer.Participant.Username;
            AuthenticateData.User.Avatar = mock._currentPlayer.Participant.Avatar;
            AuthenticateData.User.GlobalName = mock._currentPlayer.Participant.GlobalName;
            AuthenticateData.User.AccentColor = mock._currentPlayer.Participant.AccentColor;
            AuthenticateData.User.AvatarDecoration = mock._currentPlayer.Participant.AvatarDecoration;
            AuthenticateData.User.PublicFlags = mock._currentPlayer.Participant.Flags;
        }

        public MultiEvent ToMultiEvent()
        {
            SetMockUser();
            
            return new MultiEvent()
            {
                ReadyData = ReadyData,
                AuthorizeData = AuthorizeData,
                AuthenticateData = AuthenticateData
            };
        }
    }
}