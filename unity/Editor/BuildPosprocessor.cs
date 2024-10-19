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

            // Move Build folder to Files/Build
            string filesFolder = Path.Combine(pathToBuiltProject, "Files");
            string buildFolder = Path.Combine(pathToBuiltProject, "Build");
            string destination = Path.Combine(filesFolder, "Build");
            
            if (!Directory.Exists(filesFolder))
            {
                Directory.CreateDirectory(filesFolder);
            }

            else if (Directory.Exists(destination))
            {
                Directory.Delete(destination, true);
            }

            Directory.Move(buildFolder, destination);

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

            string scriptsFolder = Path.Combine(filesFolder, "Scripts");
            string rpcBridgePath = Path.Combine(scriptsFolder, "rpc_bridge.js");
            string indexPath = Path.Combine(pathToBuiltProject, "index.html");

            //# RPC BRIDGE - - - - -
            // Replace user variables
            string fileContent = File.ReadAllText(rpcBridgePath);

            // Client id
            int index = fileContent.IndexOf("[[[ CLIENT_ID ]]]");
            CheckIndex(index);
            int endIndex = fileContent.IndexOf(VariableSeparator, index);
            string substring = fileContent.Substring(index, endIndex - index);
            fileContent = fileContent.Replace(substring, $"[[[ CLIENT_ID ]]] {data.ClientId}");

            // Sdk version
            index = fileContent.IndexOf("[[[ SDK_VERSION ]]]");
            CheckIndex(index);
            endIndex = fileContent.IndexOf(VariableSeparator, index);
            substring = fileContent.Substring(index, endIndex - index);
            fileContent = fileContent.Replace(substring, $"[[[ SDK_VERSION ]]] {Api.SdkVersion}");

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
            if (mappings == "null")
            {
                mappings = "";
            }

            //? Format
            else
            {
                string serializedMappings = "";

                for (int i = 0; i < data.Mappings.Length; i++)
                {
                    var mapping = data.Mappings[i];

                    if (i == 0) serializedMappings = $"{mapping.Prefix}";
                    else serializedMappings += $",{mapping.Prefix}";

                    serializedMappings += $",{mapping.Target}";
                }

                mappings = serializedMappings;
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

            //? Format
            else
            {
                var source = data.PatchUrlMappingsConfig;

                patchConfig = $"patchFetch,{source.PatchFetch},patchWebSocket,{source.PatchWebSocket},patchXhr,{source.PatchXhr},patchSrcAttributes,{source.PatchSrcAttributes}";
            }

            fileContent = fileContent.Replace(substring, $"[[[ PATCH_URL_MAPPINGS_CONFIG ]]] {patchConfig}");

            //\ Write final file
            File.WriteAllText(rpcBridgePath, fileContent);

            //# INDEX - - - - -
            fileContent = File.ReadAllText(indexPath);

            float viewportWidth = Handles.GetMainGameViewSize().x;
            float viewportHeight = Handles.GetMainGameViewSize().y;

            //? Low viewport res warning
            if (!data.DisableDissonityInfoLogs && (viewportWidth < 1366 || viewportHeight < 768))
            {
                //? Using viewport
                if (data.DesktopResolution == ScreenResolution.Viewport
                |   data.MobileResolution == ScreenResolution.Viewport
                |   data.WebResolution == ScreenResolution.Viewport)
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

            // Web resolution
            index = fileContent.IndexOf("[[[ WEB_RESOLUTION ]]]");
            CheckIndex(index);
            endIndex = fileContent.IndexOf(VariableSeparator, index);
            substring = fileContent.Substring(index, endIndex - index);
            fileContent = fileContent.Replace(substring, $"[[[ WEB_RESOLUTION ]]] {(int) data.WebResolution}");

            //\ Write final file
            File.WriteAllText(indexPath, fileContent);

            Debug.Log("[Dissonity Build]: Build post-processed correctly!");
        }

            
    
        private static void CheckIndex(int index)
        {
            if (index != -1) return;

            throw new Exception("It seems you are not using the Dissonity WebGL template, so Dissonity won't work. You can change your WebGL template in [Player Settings > Resolution and Presentation > WebGL Template]");
        }
    }
}