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

            if (data.ClientId == "")
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

                // Replace user variables
                string rpcBridgePath = Path.Combine(scriptsFolder, "rpc_bridge.js");
                string fileContent = File.ReadAllText(rpcBridgePath);

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

                Debug.Log("[Dissonity Build]: Build post-processed correctly!");
            }

            
    
        private static void CheckIndex(int index)
        {
            if (index != -1) return;

            throw new Exception("It seems you are not using a Dissonity WebGL template, so Dissonity won't work. You can change your WebGL template in [Player Settings > Resolution and Presentation > WebGL Template]");
        }
    }
}