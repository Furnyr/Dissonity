using System;
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Dissonity.Editor
{
    internal class BuildPosprocessor
    {
        public const string VariableSeparator = "§"; //alt 21 win
        
        [PostProcessBuild(0)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            if (target != BuildTarget.WebGL) return;

            Debug.Log("[Dissonity Build]: Now post-processing build, hold on...");

            //\ Validation
            var data = DissonityConfigAttribute.GetUserConfig();

            if (data.ClientId == 0)
            {
                throw new Exception("[Dissonity Build]: Client id is empty");
            }
            if (data.OauthScopes.Length == 0)
            {
                throw new Exception("[Dissonity Build]: Invalid scope, array has no elements");
            }
            if (!data.TokenRequestPath.StartsWith("/"))
            {
                throw new Exception("[Dissonity Build]: Token request path must start with /");
            }

            string bridgeFolder = Path.Combine(pathToBuiltProject, "Bridge");
            string buildVariablesPath = Path.Combine(bridgeFolder, "dissonity_build_variables.js");
            string appLoaderPath = Path.Combine(pathToBuiltProject, "app_loader.js");

            //# BUILD VARIABLES - - - - -
            string fileContent = File.ReadAllText(buildVariablesPath);

            // Client id
            int index = fileContent.IndexOf("[[[ CLIENT_ID ]]]");
            CheckIndex(index);
            int endIndex = fileContent.IndexOf(VariableSeparator, index);
            string substring = fileContent.Substring(index, endIndex - index);
            fileContent = fileContent.Replace(substring, $"[[[ CLIENT_ID ]]] {data.ClientId}");

            // Disable console log override
            index = fileContent.IndexOf("[[[ DISABLE_CONSOLE_LOG_OVERRIDE ]]]");
            CheckIndex(index);
            endIndex = fileContent.IndexOf(VariableSeparator, index);
            substring = fileContent.Substring(index, endIndex - index);
            fileContent = fileContent.Replace(substring, $"[[[ DISABLE_CONSOLE_LOG_OVERRIDE ]]] {data.DisableConsoleLogOverride}");

            // Oauth scopes
            index = fileContent.IndexOf("[[[ OAUTH_SCOPES ]]]");
            CheckIndex(index);
            endIndex = fileContent.IndexOf(VariableSeparator, index);
            substring = fileContent.Substring(index, endIndex - index);
            fileContent = fileContent.Replace(substring, $"[[[ OAUTH_SCOPES ]]] {string.Join(",", data.OauthScopes)}");

            // Token request path
            index = fileContent.IndexOf("[[[ TOKEN_REQUEST_PATH ]]]");
            CheckIndex(index);
            endIndex = fileContent.IndexOf(VariableSeparator, index);
            substring = fileContent.Substring(index, endIndex - index);
            fileContent = fileContent.Replace(substring, $"[[[ TOKEN_REQUEST_PATH ]]] {data.TokenRequestPath}");

            // Server request
            index = fileContent.IndexOf("[[[ SERVER_REQUEST ]]]");
            CheckIndex(index);
            endIndex = fileContent.IndexOf(VariableSeparator, index);
            substring = fileContent.Substring(index, endIndex - index);

            var requestInstance = Activator.CreateInstance(data.GetRequestType());
            string stringifiedRequest = JsonConvert.SerializeObject(requestInstance);

            fileContent = fileContent.Replace(substring, $"[[[ SERVER_REQUEST ]]] {stringifiedRequest}");

            // Disable info logs
            index = fileContent.IndexOf("[[[ DISABLE_INFO_LOGS ]]]");
            CheckIndex(index);
            endIndex = fileContent.IndexOf(VariableSeparator, index);
            substring = fileContent.Substring(index, endIndex - index);
            fileContent = fileContent.Replace(substring, $"[[[ DISABLE_INFO_LOGS ]]] {data.DisableDissonityInfoLogs}");

            // Mappings
            index = fileContent.IndexOf("[[[ MAPPINGS ]]]");
            CheckIndex(index);
            endIndex = fileContent.IndexOf(VariableSeparator, index);
            substring = fileContent.Substring(index, endIndex - index);

            var mappings = JsonConvert.SerializeObject(data.Mappings);

            //? Null
            bool emptyMappings = data.Mappings.Length == 0;
            bool onlyNullMapping = data.Mappings.Length == 1 && data.Mappings[0].Prefix == null;
            if (mappings == "null" || emptyMappings || onlyNullMapping)
            {
                mappings = "";
            }

            fileContent = fileContent.Replace(substring, $"[[[ MAPPINGS ]]] {mappings}");

            // Patch url mappings config
            index = fileContent.IndexOf("[[[ PATCH_URL_MAPPINGS_CONFIG ]]]");
            CheckIndex(index);
            endIndex = fileContent.IndexOf(VariableSeparator, index);
            substring = fileContent.Substring(index, endIndex - index);

            var patchConfig = JsonConvert.SerializeObject(data.PatchUrlMappingsConfig);

            //? Null
            if (patchConfig == "null")
            {
                patchConfig = "";
            }

            fileContent = fileContent.Replace(substring, $"[[[ PATCH_URL_MAPPINGS_CONFIG ]]] {patchConfig}");

            //\ Write final file
            File.WriteAllText(buildVariablesPath, fileContent);

            //# APP LOADER - - - - -
            fileContent = File.ReadAllText(appLoaderPath);

            float viewportWidth = Handles.GetMainGameViewSize().x;
            float viewportHeight = Handles.GetMainGameViewSize().y;

            //? Low viewport res warning
            if (!data.DisableDissonityInfoLogs && (viewportWidth < 1366 || viewportHeight < 768))
            {
                //? Using viewport
                if (data.DesktopResolution == ScreenResolution.Viewport
                |   data.MobileResolution == ScreenResolution.Viewport
                |   data.BrowserResolution == ScreenResolution.Viewport)
                {
                    Debug.LogWarning("[Dissonity Build]: Your viewport resolution is low, so the activity may appear blurry on the platforms using ScreenResolution.Viewport");
                }
            }

            // Width
            index = fileContent.IndexOf("[[[ WIDTH ]]]");
            CheckIndex(index);
            endIndex = fileContent.IndexOf(VariableSeparator, index);
            substring = fileContent.Substring(index, endIndex - index);
            fileContent = fileContent.Replace(substring, $"[[[ WIDTH ]]] {viewportWidth}");

            // Height
            index = fileContent.IndexOf("[[[ HEIGHT ]]]");
            CheckIndex(index);
            endIndex = fileContent.IndexOf(VariableSeparator, index);
            substring = fileContent.Substring(index, endIndex - index);
            fileContent = fileContent.Replace(substring, $"[[[ HEIGHT ]]] {viewportHeight}");

            // Desktop resolution
            index = fileContent.IndexOf("[[[ DESKTOP_RESOLUTION ]]]");
            CheckIndex(index);
            endIndex = fileContent.IndexOf(VariableSeparator, index);
            substring = fileContent.Substring(index, endIndex - index);
            fileContent = fileContent.Replace(substring, $"[[[ DESKTOP_RESOLUTION ]]] {(int) data.DesktopResolution}");

            // Mobile resolution
            index = fileContent.IndexOf("[[[ MOBILE_RESOLUTION ]]]");
            CheckIndex(index);
            endIndex = fileContent.IndexOf(VariableSeparator, index);
            substring = fileContent.Substring(index, endIndex - index);
            fileContent = fileContent.Replace(substring, $"[[[ MOBILE_RESOLUTION ]]] {(int) data.MobileResolution}");

            // Browser resolution
            index = fileContent.IndexOf("[[[ BROWSER_RESOLUTION ]]]");
            CheckIndex(index);
            endIndex = fileContent.IndexOf(VariableSeparator, index);
            substring = fileContent.Substring(index, endIndex - index);
            fileContent = fileContent.Replace(substring, $"[[[ BROWSER_RESOLUTION ]]] {(int) data.BrowserResolution}");

            //\ Write final file
            File.WriteAllText(appLoaderPath, fileContent);

            Debug.Log("[Dissonity Build]: Build post-processed correctly!");
        }

        private static void CheckIndex(int index)
        {
            if (index != -1) return;

            throw new Exception("It seems you are not using the Dissonity WebGL template, so Dissonity won't work. You can change your WebGL template in [Player Settings > Resolution and Presentation > WebGL Template]");
        }
    }
}