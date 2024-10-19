
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor
{
    internal class AddTemplate : AssetPostprocessor
    {
        static bool loaded = false;

        static void OnPostprocessAllAssets(string[] _importedAssets, string[] _deletedAssets, string[] _movedAssets, string[] _movedFromAssetPaths, bool _didDomainReload)
        {
            if (loaded) return;
            loaded = true;

            //# DIRECTORIES - - - - -
            string pathToTemplates = Path.Combine(Application.dataPath, "WebGLTemplates");

            if (!Directory.Exists(pathToTemplates))
            {
                Directory.CreateDirectory(pathToTemplates);
            }

            string pathToDissonityTemplate = Path.Combine(pathToTemplates, "Dissonity");

            if (!Directory.Exists(pathToDissonityTemplate))
            {
                Directory.CreateDirectory(pathToDissonityTemplate);
            }

            string pathToFiles = Path.Combine(pathToDissonityTemplate, "Files");

            if (!Directory.Exists(pathToFiles))
            {
                Directory.CreateDirectory(pathToFiles);
            }

            string pathToScripts = Path.Combine(pathToFiles, "Scripts");

            if (!Directory.Exists(pathToScripts))
            {
                Directory.CreateDirectory(pathToScripts);
            }

            //? If all directories existed, consider the template generated
            else
            {
                return;
            }

            //todo remove in the final release
            Debug.LogWarning("[Dissonity]: WARNING! Version 2 isn't released yet. You should only be using this package for testing purposes.");

            Debug.Log("[Dissonity Editor]: Adding WebGL Template to your assets");

            // Target paths
            string targetPath = Path.Combine(Application.dataPath, "WebGLTemplates/Dissonity");
            string targetIndex = Path.Combine(targetPath, "index.html");
            string checkPath = Path.Combine(targetPath, "discord.check.js");
            string targetThumbnail = Path.Combine(targetPath, "thumbnail.png"); //todo thumbnail
            string targetRpcBridge = Path.Combine(targetPath, "Files/Scripts/rpc_bridge.js");
            string targetOfficialUtils = Path.Combine(targetPath, "Files/Scripts/official_utils.js");

            //\ Read everything
            var index = Resources.Load<TextAsset>("WebGLTemplates/Dissonity/index").ToString();
            var check = Resources.Load<TextAsset>("WebGLTemplates/Dissonity/discord.check.js").ToString();
            byte[] thumbnail = Resources.Load<Texture2D>("WebGLTemplates/Dissonity/thumbnail").EncodeToPNG();
            string rpcBridge = Resources.Load<TextAsset>("WebGLTemplates/Dissonity/Files/Scripts/rpc_bridge.js").ToString();
            string officialUtils = Resources.Load<TextAsset>("WebGLTemplates/Dissonity/Files/Scripts/official_utils.js").ToString();

            //\ Write
            File.WriteAllText(targetIndex, index);
            File.WriteAllText(checkPath, check);
            File.WriteAllBytes(targetThumbnail, thumbnail);
            File.WriteAllText(targetRpcBridge, rpcBridge);
            File.WriteAllText(targetOfficialUtils, officialUtils);
        }
    }
}