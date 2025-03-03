using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor.Dialogs
{
    internal class UpdateDialog : EditorWindow
    {
        public static void ShowDialog()
        {
            var window = GetWindow<UpdateDialog>(true, "Dissonity — Update Changelog", true);
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

            // Top margin
            GUILayout.Space(20);

            // Left margin
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            
            GUILayout.Label("Beta 1 Overview", headerStyle);

            GUILayout.Label("Bug fixes and hiRPC improvements.");

            GUILayout.Space(15);

            GUILayout.Label("Added", subHeaderStyle);

            GUILayout.Space(10);

            GUILayout.Label("- Api.AccessToken property");
            GUILayout.Label("- Api.LocalStorage subclass");

            GUILayout.Space(15);

            GUILayout.Label("Changed", subHeaderStyle);

            GUILayout.Space(10);

            GUILayout.Label("- HiRpc methods now take the hiRPC channel as the first argument.");
            GUILayout.Label("- The Resources folder is no longer used internally to load assets.");

            GUILayout.Space(15);

            GUILayout.Label("Fixed", subHeaderStyle);

            GUILayout.Space(10);

            GUILayout.Label("- Generated folders are now deleted with the uninstaller.");
            GUILayout.Label("- All commands should now raise exceptions properly.");
            GUILayout.Label("- Serialize enums as integers.");
            GUILayout.Label("And more!");

            GUILayout.Space(20);

            if (GUILayout.Button("Got it!", GUILayout.Height(25)))
            {
                Close();
            }

            GUILayout.Space(15);

            GUILayout.Label("Links", subHeaderStyle);

            if (EditorGUILayout.LinkButton("Full changelog"))
            {
                Application.OpenURL("https://github.com/Furnyr/Dissonity/blob/dev/BETA_CHANGELOG.md");
            }

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
    }
}
