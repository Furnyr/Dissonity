
using System.IO;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor
{
    internal class AddTemplate : AssetPostprocessor
    {
        static bool loaded = false;
        static string pathToPackage = "";

        private class PackageData
        {
            [JsonProperty("version")]
            public string Version { get; set; }
        }

        private class VersionFileData
        {
            [JsonProperty("dissonity")]
            public string Dissonity { get; set; }
        }

        static void OnPostprocessAllAssets(string[] _importedAssets, string[] _deletedAssets, string[] _movedAssets, string[] _movedFromAssetPaths, bool _didDomainReload)
        {
            if (loaded) return;
            loaded = true;

            ExecuteProcess(_importedAssets, _deletedAssets, _movedAssets, _movedFromAssetPaths, _didDomainReload);
        }

        static void ExecuteProcess(string[] _importedAssets, string[] _deletedAssets, string[] _movedAssets, string[] _movedFromAssetPaths, bool _didDomainReload)
        {
            // To access package.json we need the folder where Dissonity is.
            // This is achieved by accessing the resources folder.
            pathToPackage = "";
            string indexAssetPath = AssetDatabase.GetAssetPath(Resources.Load<TextAsset>("WebGLTemplateSource/Dissonity/index.html"));
            pathToPackage = GetPathToPackage(indexAssetPath);

            // Target where the WebGL Template should be
            string targetPath = CombinePath(Application.dataPath, "WebGLTemplates/Dissonity");
            string metaTargetPath = CombinePath(Application.dataPath, "WebGLTemplates/Dissonity.meta");

            //# DIRECTORIES - - - - -
            string pathToTemplates = CombinePath(Application.dataPath, "WebGLTemplates");

            if (!Directory.Exists(pathToTemplates))
            {
                Directory.CreateDirectory(pathToTemplates);
            }

            string pathToDissonityTemplate = CombinePath(pathToTemplates, "Dissonity");

            if (!Directory.Exists(pathToDissonityTemplate))
            {
                Directory.CreateDirectory(pathToDissonityTemplate);
            }

            string pathToBridge = CombinePath(pathToDissonityTemplate, "Bridge");

            if (!Directory.Exists(pathToBridge))
            {
                Directory.CreateDirectory(pathToBridge);
            }

            // If all directories existed, the template is considered generated.
            // We only need to regenerate it if the version differs.
            else
            {
                //\ Get current template version (in the user project)
                string versionFileText = File.ReadAllText(CombinePath(targetPath, "version.json"));
                VersionFileData versionFileData = JsonConvert.DeserializeObject<VersionFileData>(versionFileText);

                //\ Get package version
                string packageText = File.ReadAllText(CombinePath(pathToPackage, "package.json"));
                PackageData packageData = JsonConvert.DeserializeObject<PackageData>(packageText);  // packageData.Version = "2.0.0";

                if (packageData.Version != versionFileData.Dissonity)
                {
                    Debug.Log($"[Dissonity Editor] The installed Dissonity version is {packageData.Version} but your WebGL template is {versionFileData.Dissonity}");

                    FileUtil.DeleteFileOrDirectory(targetPath);
                    FileUtil.DeleteFileOrDirectory(metaTargetPath);

                    ExecuteProcess(_importedAssets, _deletedAssets, _movedAssets, _movedFromAssetPaths, _didDomainReload);
                }

                return;
            }

            //todo remove in the final release
            Debug.LogWarning("[Dissonity] WARNING! Version 2 isn't released yet. You should only be using this package for testing purposes.");

            Debug.Log("[Dissonity Editor] Adding WebGL Template to your assets.");

            LoadTextAssets("WebGLTemplateSource/Dissonity", targetPath);
            LoadPngAssets("WebGLTemplateSource/Dissonity", targetPath);
        }

        static void LoadTextAssets(string source, string target)
        {
            //source = WebGLTemplateSource/Dissonity

            TextAsset[] assets = Resources.LoadAll<TextAsset>(source);

            foreach (var asset in assets)
            {
                string sourcePath = AssetDatabase.GetAssetPath(asset);
                string targetPath = TargetPath(target, sourcePath);

                if (sourcePath.Contains(".wasm"))
                {
                    byte[] resource = asset.bytes;
                
                    EnsureDirectoriesExist(targetPath);
                    
                    File.WriteAllBytes(targetPath, resource);
                }
                
                else
                {
                    string resource = asset.ToString();
                
                    EnsureDirectoriesExist(targetPath);
                    
                    File.WriteAllText(targetPath, resource);
                }
            }

            //\ Write version.json
            string packageText = File.ReadAllText(CombinePath(pathToPackage, "package.json"));

            PackageData packageData = JsonConvert.DeserializeObject<PackageData>(packageText);

            VersionFileData versionFileData = new VersionFileData {
                Dissonity = packageData.Version
            };

            File.WriteAllText(CombinePath(target, "version.json"), JsonConvert.SerializeObject(versionFileData));
        }

        static void LoadPngAssets(string source, string target)
        {
            //source = WebGLTemplateSource/Dissonity

            Texture2D[] assets = Resources.LoadAll<Texture2D>(source);

            foreach (var asset in assets)
            {
                string sourcePath = AssetDatabase.GetAssetPath(asset);

                string targetPath = TargetPath(target, sourcePath);

                byte[] resource = asset.EncodeToPNG();
            
                EnsureDirectoriesExist(targetPath);
                
                File.WriteAllBytes(targetPath + ".png", resource);
            }
        }

        static string TargetPath(string target, string source)
        {
            // .../WebGLTemplateSource/Dissonity/index.html.txt -> ...Assets/WebGLTemplateSource/Dissonity/index.html

            return CombinePath(target, RelativePath(source));
        }

        static string CombinePath(string path1, string path2)
        {
            return Path.Combine(path1, path2).Replace("\\", "/");
        }

        static string RelativePath(string source)
        {
            // .../WebGLTemplateSource/Dissonity/index.html.txt -> index.html

            string file = Path.GetFileNameWithoutExtension(source);
            string dir = Path.GetDirectoryName(source);

            string fullPath = CombinePath(dir, file);

            int firstIndexOfResources = fullPath.LastIndexOf("Dissonity");
            int lastIndexOfResources = firstIndexOfResources + "Dissonity".Length + 1;

            return fullPath.Substring(lastIndexOfResources, fullPath.Length - lastIndexOfResources);
        }

        static void EnsureDirectoriesExist(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }

        static string GetPathToPackage(string pathToAsset)
        {
            int lastIndex = pathToAsset.IndexOf(CombinePath("Resources", "WebGLTemplateSource"));

            return pathToAsset.Substring(0, lastIndex);
        }
    }
}