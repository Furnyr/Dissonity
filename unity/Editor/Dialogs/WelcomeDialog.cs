using System.IO;
using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor.Dialogs
{
    internal class WelcomeDialog : EditorWindow
    {
        public static void ShowDialog()
        {
            var window = GetWindow<WelcomeDialog>(true, "Dissonity — Create Discord activities with Unity", true);
            window.minSize = new Vector2(600, 600);
        }

        private void OnGUI()
        {
            GUIStyle headerStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold
            };

            GUIStyle subHeaderStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold
            };

            GUIStyle boxStyle = new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = 12
            };

            // Top margin
            GUILayout.Space(20);

            // Left margin
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            
            GUILayout.Label("Welcome to Dissonity!", headerStyle);

            //todo remove in final version
            GUI.backgroundColor = new Color(0.85f, 0.88f, 0.28f);
            GUILayout.Label("This is a beta version. Watch out for unexpected breaking changes!", boxStyle);
            GUI.backgroundColor = Color.white;

            GUILayout.Space(15);

            GUILayout.Label("Next steps:", subHeaderStyle);

            GUILayout.Space(10);

            GUILayout.Label("1. Update your settings in Assets/Dissonity/DissonityConfiguration.cs");
            GUILayout.Label("2. Create a Discord Mock in your scene (Hierarchy > Dissonity > Discord Mock)");
            GUILayout.Label("3. Call and await Dissonity.Api.Initialize once when the game starts");
            GUILayout.Label("4. Use the Dissonity namespace!");

            GUILayout.Space(10);

            GUILayout.Label("You can open this dialog again through Assets/Dissonity/Dialogs.asset", EditorStyles.centeredGreyMiniLabel);

            GUILayout.Space(20);

            if (ConfigurationExists())
            {
                GUILayout.Label("Overwrite your configuration:", subHeaderStyle);
            }

            else GUILayout.Label("Choose your preferred configuration:", subHeaderStyle);

            GUILayout.Space(5);

            GUILayout.Label("Hover for more information.");

            if (GUILayout.Button(new GUIContent("Basic configuration", "Ideal for beginners, only the necessary options."), GUILayout.Height(25)))
            {
                Close();

                string fileData = Loady.Load<TextAsset>("BasicConfig.txt").text;
                SetConfiguration(fileData);
            }

            GUILayout.Space(1);

            if (GUILayout.Button(new GUIContent("Standard configuration", "Adds utility features, but it's not too verbose."), GUILayout.Height(25)))
            {
                Close();

                string fileData = Loady.Load<TextAsset>("StandardConfig.txt").text;
                SetConfiguration(fileData);
            }

            GUILayout.Space(1);

            if (GUILayout.Button(new GUIContent("Advanced configuration", "Includes all available configuration options."), GUILayout.Height(25)))
            {
                Close();

                string fileData = Loady.Load<TextAsset>("AdvancedConfig.txt").text;
                SetConfiguration(fileData);
            }

            GUILayout.Space(15);

            GUILayout.Label("Links", subHeaderStyle);

            if (EditorGUILayout.LinkButton("Documentation"))
            {
                Application.OpenURL("https://dissonity.dev");
            }

            if (EditorGUILayout.LinkButton("Source code"))
            {
                Application.OpenURL("https://github.com/Furnyr/Dissonity");
            }

            if (EditorGUILayout.LinkButton("Issues"))
            {
                Application.OpenURL("https://github.com/Furnyr/Dissonity/issues");
            }

            // Right margin
            GUILayout.EndVertical();
            GUILayout.Space(20);
            GUILayout.EndHorizontal();
            
            // Bottom margin
            GUILayout.Space(20);
        }

        private void SetConfiguration(string fileText)
        {
            string pathToFolder = AddTemplate.CombinePath(Application.dataPath, "Dissonity");
            string pathToFile = AddTemplate.CombinePath(pathToFolder, "DissonityConfiguration.cs");

            // It should exists because it is created before opening the dialog the first time.
            if (!Directory.Exists(pathToFolder))
            {
                Directory.CreateDirectory(pathToFolder);

                Debug.Log("[Dissonity Editor] Created folder: Assets/Dissonity");
            }

            if (File.Exists(pathToFile))
            {
                FileUtil.DeleteFileOrDirectory(pathToFile);
            }

            File.WriteAllText(pathToFile, fileText);

            AssetDatabase.Refresh();
        }

        private bool ConfigurationExists()
        {
            string pathToFolder = AddTemplate.CombinePath(Application.dataPath, "Dissonity");
            string pathToFile = AddTemplate.CombinePath(pathToFolder, "DissonityConfiguration.cs");

            return File.Exists(pathToFile);
        }
    }
}
