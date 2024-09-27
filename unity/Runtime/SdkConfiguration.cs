
using System;
using Dissonity.Models.Builders;

namespace Dissonity
{
    public interface ISdkConfiguration
    {
        string ClientId { get; }
        string[] OauthScopes { get; }
        string TokenRequestPath { get; }

        // Optional
        bool DisableConsoleLogOverride { get; }
        bool DisableDissonityInfoLogs { get; }
        MappingBuilder[] Mappings { get; }
        PatchUrlMappingsConfigBuilder PatchUrlMappingsConfig { get; }

        abstract Type GetRequestType();
        abstract Type GetResponseType();
    }

    public abstract class SdkConfiguration<TRequest, TResponse> : ISdkConfiguration
        where TRequest : ServerTokenRequest
        where TResponse : ServerTokenResponse
    {
        public abstract string ClientId { get; }
        public abstract string[] OauthScopes { get; }
        public abstract string TokenRequestPath { get; }

        // Optional
        public virtual bool DisableConsoleLogOverride { get; } = false;
        public virtual bool DisableDissonityInfoLogs { get; } = false;
        public virtual MappingBuilder[] Mappings { get; } = {};
        public virtual PatchUrlMappingsConfigBuilder PatchUrlMappingsConfig { get; } = new()
        {
            PatchFetch = true,
            PatchWebSocket = true,
            PatchXhr = true,
            PatchSrcAttributes = false
        };

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

    /// <summary>
    /// This class is used internally to access the configuration. <br/> <br/>
    /// You do not need to interact with this class directly.
    /// </summary>
    public class _UserData : ISdkConfiguration
    {
        public string ClientId { get; set; }
        public string[] OauthScopes { get; set; }
        public string TokenRequestPath { get; set; }
        public Type ServerTokenRequest { get; set; }
        public Type ServerTokenResponse { get; set; }

        // Optional
        public bool DisableConsoleLogOverride { get; set; }
        public bool DisableDissonityInfoLogs { get; set; }
        public MappingBuilder[] Mappings { get; set; }
        public PatchUrlMappingsConfigBuilder PatchUrlMappingsConfig { get; set; }

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
}