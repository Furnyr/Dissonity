
using System.IO;
using Dissonity.Editor.Dialogs;
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
            // SessionState isn't used because we need this to execute again on package updates.
            if (loaded) return;
            loaded = true;

            ExecuteProcess(_importedAssets, _deletedAssets, _movedAssets, _movedFromAssetPaths, _didDomainReload, false);
        }

        static void ExecuteProcess(string[] _importedAssets, string[] _deletedAssets, string[] _movedAssets, string[] _movedFromAssetPaths, bool _didDomainReload, bool updating)
        {
            // To access package.json we need the folder where Dissonity is.
            // This is achieved by accessing the resources folder.
            pathToPackage = Loady.GetPackageRoot();

            // Target where the WebGL Template should be
            string targetPath = CombinePath(Application.dataPath, "WebGLTemplates/Dissonity");
            string metaTargetPath = CombinePath(Application.dataPath, "WebGLTemplates/Dissonity.meta");

            //# ASSETS/DISSONITY - - - - -
            string pathToFolder = CombinePath(Application.dataPath, "Dissonity");
            string pathToDialogs = CombinePath(pathToFolder, "Dialogs.asset");

            // If Assets/Dissonity doesn't exist, create it.
            if (!Directory.Exists(pathToFolder))
            {
                Directory.CreateDirectory(pathToFolder);

                Debug.Log("[Dissonity Editor] Created folder: Assets/Dissonity");
            }

            // If Assets/Dissonity/Dialogs.asset doesn't exist, create it.
            if (!File.Exists(pathToDialogs))
            {
                //\ Create Dialogs object
                // For AssetDatabase, the path needs to be relative to the project folder.
                string relativePathToDialogs = CombinePath("Assets/Dissonity", "Dialogs.asset");

                DialogAsset asset = ScriptableObject.CreateInstance<DialogAsset>();
                AssetDatabase.CreateAsset(asset, relativePathToDialogs);
            }

            //# WEBGL TEMPLATE - - - - -
            string pathToTemplates = CombinePath(Application.dataPath, "WebGLTemplates");

            if (!Directory.Exists(pathToTemplates))
            {
                Directory.CreateDirectory(pathToTemplates);
            }

            string pathToDissonityTemplate = CombinePath(pathToTemplates, "Dissonity");

            // The welcome dialog is triggered if Assets/WebGLTemplates/Dissonity doesn't exist.
            if (!Directory.Exists(pathToDissonityTemplate))
            {
                Directory.CreateDirectory(pathToDissonityTemplate);

                if (!updating)
                {
                    Debug.Log("[Dissonity Editor] Created folder: Assets/WebGLTemplates/Dissonity.");

                    WelcomeDialog.ShowDialog();
                }
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
                    UpdateDialog.ShowDialog();

                    FileUtil.DeleteFileOrDirectory(targetPath);
                    FileUtil.DeleteFileOrDirectory(metaTargetPath);

                    ExecuteProcess(_importedAssets, _deletedAssets, _movedAssets, _movedFromAssetPaths, _didDomainReload, true);
                }

                return;
            }

            LoadTextAssets("Template/Dissonity", targetPath);
            LoadPngAssets("Template/Dissonity", targetPath);

            AssetDatabase.Refresh();
        }

        static void LoadTextAssets(string source, string target)
        {
            //source = Template/Dissonity

            TextAsset[] assets = Loady.LoadAll<TextAsset>(source);

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

            File.WriteAllText(CombinePath(target, "version.json"), JsonConvert.SerializeObject(versionFileData, Formatting.Indented));
        }

        static void LoadPngAssets(string source, string target)
        {
            //source = Template/Dissonity

            Texture2D[] assets = Loady.LoadAll<Texture2D>(source);

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
            // .../Template/Dissonity/index.html.txt -> ...Assets/Template/Dissonity/index.html

            return CombinePath(target, RelativePath(source));
        }

        // This method is used in other files related to file generation
        public static string CombinePath(string path1, string path2)
        {
            return Path.Combine(path1, path2).Replace("\\", "/");
        }

        static string RelativePath(string source)
        {
            // .../Template/Dissonity/index.html.txt -> index.html

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
    }
}