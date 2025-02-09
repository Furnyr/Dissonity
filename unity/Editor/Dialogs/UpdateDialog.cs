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
                font = EditorStyles.boldFont
            };

            GUIStyle subHeaderStyle = new GUIStyle(EditorStyles.largeLabel)
            {
                fontSize = 17,
                font = EditorStyles.boldFont
            };

            // Top margin
            GUILayout.Space(20);

            // Left margin
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            
            GUILayout.Label("Alpha 5 Overview", headerStyle);

            GUILayout.Label("Quality of Life changes, hiRPC functionality and bug fixes.");

            GUILayout.Space(15);

            GUILayout.Label("Added", subHeaderStyle);

            GUILayout.Space(10);

            GUILayout.Label("- Api.HiRpc subclass methods");
            GUILayout.Label("- Api.OnReady method");
            GUILayout.Label("- Api.Commands.ShareLink command");
            GUILayout.Label("- Configuration options");
            GUILayout.Label("- Dialogs");

            GUILayout.Space(15);

            GUILayout.Label("Changed", subHeaderStyle);

            GUILayout.Space(10);

            GUILayout.Label("- Default desktop and mobile resolution");
            GUILayout.Label("- SubscriptionReference is now DiscordSubscription");
            GUILayout.Label("- WebResolution is now BrowserResolution");

            GUILayout.Space(15);

            GUILayout.Label("Fixed", subHeaderStyle);

            GUILayout.Space(10);

            GUILayout.Label("- Missing SetOrientationLockState command arguments");
            GUILayout.Label("- When using Max resolution, the game should resize correctly");

            GUILayout.Space(20);

            if (GUILayout.Button("Got it!", GUILayout.Height(25)))
            {
                Close();
            }

            GUILayout.Space(15);

            GUILayout.Label("Links", subHeaderStyle);

            if (EditorGUILayout.LinkButton("Full changelog"))
            {
                Application.OpenURL("https://github.com/Furnyr/Dissonity/blob/dev/ALPHA_CHANGELOG.md");
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
