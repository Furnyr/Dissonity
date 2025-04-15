
using System;
using System.Linq;
using Dissonity.Models.Builders;

namespace Dissonity
{
    /// <summary>
    /// Use this attribute on a class that inherits from SdkConfiguration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DissonityConfigAttribute : Attribute
    {
        /// @cond
        public static I_UserData _rawOverrideConfiguration = null;
        /// @endcond

        public static I_UserData GetUserConfig()
        {
            if (_rawOverrideConfiguration != null) return _rawOverrideConfiguration;

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            var found = assemblies
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => type.IsDefined(typeof(DissonityConfigAttribute), false));

            var type = found.FirstOrDefault();

            //? No classes with the attribute
            if (type == null)
            {
                throw new Exception("[Dissonity] No class with the DissonityConfigAttribute found. You can create a config file with (Right-click > Create > Dissonity > Configuration)");
            }

            var instance = Activator.CreateInstance(type);
            
            //? Not inherited
            if (instance is not ISdkConfiguration)
            {
                throw new Exception("[Dissonity] The class with the DissonityConfigAttribute must inherit from SdkConfiguration");
            }

            //? More than one
            if (found.Count() > 1)
            {
                Utils.DissonityLogWarning("More than one classes with the DissonityConfigAttribute found. This can produce unexpected behaviors.");
            }

            //\ Get fields
            long clientId = ((ISdkConfiguration) instance).ClientId;
            bool disableLogOverride = ((ISdkConfiguration) instance).DisableConsoleLogOverride;
            string[] oauthScopes = ((ISdkConfiguration) instance).OauthScopes;
            string tokenRequestPath = ((ISdkConfiguration) instance).TokenRequestPath;
            Type requestType = ((ISdkConfiguration) instance).GetRequestType();
            Type responseType = ((ISdkConfiguration) instance).GetResponseType();
            bool disableDissonityInfoLogs = ((ISdkConfiguration) instance).DisableDissonityInfoLogs;
            MappingBuilder[] mappings = ((ISdkConfiguration) instance).Mappings;
            PatchUrlMappingsConfigBuilder patchConfig = ((ISdkConfiguration) instance).PatchUrlMappingsConfig;
            ScreenResolution desktopResolution = ((ISdkConfiguration) instance).DesktopResolution;
            ScreenResolution mobileResolution = ((ISdkConfiguration) instance).MobileResolution;
            ScreenResolution webResolution = ((ISdkConfiguration) instance).BrowserResolution;
            bool synchronizeUser = ((ISdkConfiguration) instance).SynchronizeUser;
            bool synchronizeGuildMemberRpc = ((ISdkConfiguration) instance).SynchronizeGuildMemberRpc;

            // Handle token request path
            tokenRequestPath = tokenRequestPath.StartsWith("/")
                ? tokenRequestPath
                : $"/{tokenRequestPath}";

            var data = new I_UserData() {
                ClientId = clientId,
                DisableConsoleLogOverride = disableLogOverride,
                OauthScopes = oauthScopes,
                TokenRequestPath = tokenRequestPath,
                ServerTokenRequest = requestType,
                ServerTokenResponse = responseType,
                DisableDissonityInfoLogs = disableDissonityInfoLogs,
                Mappings = mappings,
                PatchUrlMappingsConfig = patchConfig,
                DesktopResolution = desktopResolution,
                MobileResolution = mobileResolution,
                BrowserResolution = webResolution,
                SynchronizeUser = synchronizeUser,
                SynchronizeGuildMemberRpc = synchronizeGuildMemberRpc
            };

            return data;
        }
    }
}