using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager;

namespace Dissonity.Editor.Dialogs
{
    internal class UninstallerDialog : EditorWindow
    {
        public static void ShowDialog()
        {
            var window = GetWindow<UninstallerDialog>(true, "Dissonity — Uninstaller", true);
            window.minSize = new Vector2(600, 600);
        }

        private void OnGUI()
        {
            GUIStyle headerStyle = new(EditorStyles.largeLabel)
            {
                fontSize = 20,
                font = EditorStyles.boldFont
            };

            // Top margin
            GUILayout.Space(20);

            // Left margin
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            
            GUILayout.Label("Uninstaller", headerStyle);

            GUILayout.Space(15);

            GUILayout.Label("Dissonity adds two folders to the project upon installation: Assets/Dissonity and Assets/WebGLTemplates/Dissonity. If you use this uninstaller, the package will be removed after deleting the generated folders.", EditorStyles.wordWrappedLabel);

            GUI.backgroundColor = new Color(0.78f, 0.29f, 0.23f);

            GUILayout.Space(15);

            if (GUILayout.Button("Uninstall Dissonity", GUILayout.Height(25)))
            {
                Close();

                DeleteFolders();

                UninstallPackage();
            }

            GUI.backgroundColor = Color.white;

            // Right margin
            GUILayout.EndVertical();
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
            
            // Bottom margin
            GUILayout.Space(20);
        }

        private void DeleteFolders()
        {
            string pathToDissonity = AddTemplate.CombinePath(Application.dataPath, "Dissonity");
            string pathToDissonityMeta = AddTemplate.CombinePath(Application.dataPath, "Dissonity.meta");

            string pathToTemplates = AddTemplate.CombinePath(Application.dataPath, "WebGLTemplates");
            string pathToTemplatesMeta = AddTemplate.CombinePath(Application.dataPath, "WebGLTemplates.meta");

            string pathToDissonityTemplate = AddTemplate.CombinePath(pathToTemplates, "Dissonity");
            string pathToDissonityTemplateMeta = AddTemplate.CombinePath(pathToTemplates, "Dissonity.meta");

            if (Directory.Exists(pathToDissonity))
            {
                FileUtil.DeleteFileOrDirectory(pathToDissonity);
                FileUtil.DeleteFileOrDirectory(pathToDissonityMeta);
            }

            //? Only using the Dissonity template
            int templates = Directory.GetFiles(pathToTemplates).Length;

            if (Directory.Exists(pathToDissonityTemplate) && templates <= 1)
            {
                FileUtil.DeleteFileOrDirectory(pathToTemplates);
                FileUtil.DeleteFileOrDirectory(pathToTemplatesMeta);
            }

            //? Using the Dissonity template and others
            else if (Directory.Exists(pathToDissonityTemplate))
            {
                FileUtil.DeleteFileOrDirectory(pathToDissonityTemplate);
                FileUtil.DeleteFileOrDirectory(pathToDissonityTemplateMeta);
            }

            // In any other case, the Dissonity template is already deleted.

            AssetDatabase.Refresh();
        }
    
        private void UninstallPackage()
        {
            Client.Remove(Loady.PackageName);
        }
    }
}
