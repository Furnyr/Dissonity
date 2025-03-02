
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor
{
    /// <summary>
    /// Loady is Dissonity's internal asset management system. <br/> <br/>
    /// 
    /// It handles asset loading such as the WebGL Template or other files both
    /// in local development and installed package scenarios.
    /// </summary>
    public static class Loady
    {
        public const string PackageName = "com.furnyr.dissonity";
        public const string LibraryFolder = "Assets/Library/";

        public static T Load<T>(string path) where T : Object
        {
            // Local development
            string assetsPath = $"{LibraryFolder}/Editor/Assets/" + path;
            T asset = AssetDatabase.LoadAssetAtPath<T>(assetsPath);
            if (asset != null)
            {
                return asset;
            }

            // Installed package
            string packagePath = $"Packages/{PackageName}/Editor/Assets/" + path;
            asset = AssetDatabase.LoadAssetAtPath<T>(packagePath);
            return asset;
        }
    
        public static T[] LoadAll<T>(string folderPath) where T : Object
        {
            List<T> assets = new();

            // Local development
            string assetsPath = $"{LibraryFolder}/Editor/Assets/" + folderPath;
    
            if (Directory.Exists(assetsPath))
            {
                string[] assetPathsInAssets = AssetDatabase.FindAssets("t:" + typeof(T).Name, new[] { assetsPath });
                foreach (string guid in assetPathsInAssets)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                    if (asset != null)
                    {
                        assets.Add(asset);
                    }
                }
            }

            // Installed package
            string packagePath = $"Packages/{PackageName}/Editor/Assets/" + folderPath;

            if (Directory.Exists(packagePath))
            {
                string[] assetPathsInPackages = AssetDatabase.FindAssets("t:" + typeof(T).Name, new[] { packagePath });
                foreach (string guid in assetPathsInPackages)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                    if (asset != null)
                    {
                        assets.Add(asset);
                    }
                }
            }


            return assets.ToArray();
        }
    
        public static string GetPackageRoot()
        {
            // Local development
            TextAsset checkAsset = AssetDatabase.LoadAssetAtPath<TextAsset>($"{LibraryFolder}/Editor/Assets/Check.txt");

            if (checkAsset != null)
            {
                return LibraryFolder;
            }

            // Installed package
            else
            {
                return $"Packages/{PackageName}";
            }
        }
    }
}