
using System;
using Dissonity.Models.Builders;

namespace Dissonity
{
    /// @cond
    public interface ISdkConfiguration
    {
        long ClientId { get; }
        string[] OauthScopes { get; }
        string TokenRequestPath { get; }

        // Optional
        bool DisableConsoleLogOverride { get; }
        bool DisableDissonityInfoLogs { get; }
        MappingBuilder[] Mappings { get; }
        PatchUrlMappingsConfigBuilder PatchUrlMappingsConfig { get; }
        bool SynchronizeUser { get; }
        bool SynchronizeGuildMemberRpc { get; }
        ScreenResolution DesktopResolution { get; } 
        ScreenResolution MobileResolution { get; } 
        ScreenResolution BrowserResolution { get; } 

        // Types
        abstract Type GetRequestType();
        abstract Type GetResponseType();
    }
    /// @endcond

    public abstract class SdkConfiguration<TRequest, TResponse> : ISdkConfiguration
        where TRequest : ServerTokenRequest
        where TResponse : ServerTokenResponse
    {
        /// <summary>
        /// Your application id. <br/> <br/>
        /// https://discord.com/developers/applications
        /// </summary>
        public abstract long ClientId { get; }

        /// <summary>
        /// The OAuth2 scopes your app needs. <br/> <br/> https://discord.com/developers/docs/topics/oauth2
        /// </summary>
        public abstract string[] OauthScopes { get; }

        /// <summary>
        /// Path where your backend exchanges an authorization code for an access token.
        /// </summary>
        public abstract string TokenRequestPath { get; }


        //# OPTIONAL - - - - -
        /// <summary>
        /// https://discord.com/developers/docs/activities/development-guides#disabling-logging <br/> <br/>
        /// Defaults to true.
        /// </summary>
        public virtual bool DisableConsoleLogOverride { get; } = true;

        /// <summary>
        /// Disable information logs. <br/> <br/>
        /// It's recommended to keep logs enabled during testing.
        /// </summary>
        public virtual bool DisableDissonityInfoLogs { get; } = false;

        /// <summary>
        /// Mappings to patch before initialization. <br/> <br/>
        /// https://discord.com/developers/docs/activities/development-guides#using-external-resources
        /// </summary>
        public virtual MappingBuilder[] Mappings { get; } = {};

        /// <summary>
        /// Patch url mappings configuration. <br/> <br/>
        /// https://discord.com/developers/docs/activities/development-guides#using-external-resources
        /// </summary>
        public virtual PatchUrlMappingsConfigBuilder PatchUrlMappingsConfig { get; } = new()
        {
            PatchFetch = true,
            PatchWebSocket = true,
            PatchXhr = true,
            PatchSrcAttributes = false
        };

        /// <summary>
        /// Keep <c> Api.SyncedUser </c> up to date by automatically subscribing to <i> CurrentUserUpdate </i>.
        /// </summary>
        public virtual bool SynchronizeUser { get; } = false;

        /// <summary>
        /// Keep <c> Api.SyncedGuildMemberRpc </c> up to date by automatically subscribing to <i> CurrentGuildMemberUpdate </i>.
        /// </summary>
        public virtual bool SynchronizeGuildMemberRpc { get; } = false;

        /// <summary>
        /// How app resolution will be handled on desktop. <br/> <br/>
        /// Defaults to <c> ScreenResolution.Max </c>
        /// </summary>
        public virtual ScreenResolution DesktopResolution { get; } = ScreenResolution.Max;

        /// <summary>
        /// How app resolution will be handled on mobile. <br/> <br/>
        /// Defaults to <c> ScreenResolution.Max </c>
        /// </summary>
        public virtual ScreenResolution MobileResolution { get; } = ScreenResolution.Max;

        /// <summary>
        /// How app resolution will be handled in a browser outside of Discord. <br/> <br/>
        /// Defaults to <c> ScreenResolution.Dynamic </c>
        /// </summary>
        public virtual ScreenResolution BrowserResolution { get; } = ScreenResolution.Dynamic;


        //# USED INTERNALLY - - - - -
        // Used later to serialize data returned by BridgeLib
        public Type GetRequestType()
        {
            return typeof(TRequest);
        }

        public Type GetResponseType()
        {
            return typeof(TResponse);
        }
    }

    /// @cond
    /// <summary>
    /// This class is used internally to access the configuration. <br/> <br/>
    /// You do not need to interact with this class directly.
    /// </summary>
    public class _UserData : ISdkConfiguration
    {
        public long ClientId { get; set; }
        public string[] OauthScopes { get; set; }
        public string TokenRequestPath { get; set; }
        public Type ServerTokenRequest { get; set; }
        public Type ServerTokenResponse { get; set; }

        // Optional
        public bool DisableConsoleLogOverride { get; set; }
        public bool DisableDissonityInfoLogs { get; set; }
        public bool SynchronizeUser { get; set; }
        public bool SynchronizeGuildMemberRpc { get; set; }
        public MappingBuilder[] Mappings { get; set; }
        public PatchUrlMappingsConfigBuilder PatchUrlMappingsConfig { get; set; }
        public ScreenResolution DesktopResolution { get; set; } 
        public ScreenResolution MobileResolution { get; set; } 
        public ScreenResolution BrowserResolution { get; set; } 

        // Used later to serialize data returned by BridgeLib
        public Type GetRequestType()
        {
            return ServerTokenRequest;
        }

        public Type GetResponseType()
        {
            return ServerTokenResponse;
        }
    }
    /// @endcond
}