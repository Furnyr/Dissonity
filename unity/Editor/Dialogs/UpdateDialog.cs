using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor.Dialogs
{
    internal class UpdateDialog : EditorWindow
    {
        public static void ShowDialog()
        {
            var window = GetWindow<UpdateDialog>(true, "Dissonity — Update Changelog", true);
            window.minSize = new Vector2(600, 750);
        }

        private void OnGUI()
        {
            GUIStyle headerStyle = new(EditorStyles.largeLabel)
            {
                fontSize = 23,
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleCenter
            };

            GUIStyle centerStyle = new(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter
            };

            GUIStyle subHeaderStyle = new(EditorStyles.largeLabel)
            {
                fontSize = 17,
                fontStyle = FontStyle.Bold
            };

            GUIStyle italicStyle = new(EditorStyles.label)
            {
                fontStyle = FontStyle.Italic,
            };

            // Top margin
            GUILayout.Space(20);

            // Left margin
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
            
            GUILayout.Label("v1-v2.0.0 Overview", headerStyle);

            GUILayout.Label("Overhauled, improved and extended SDK.", centerStyle);

            GUILayout.Space(15);

            GUILayout.Label("Added", subHeaderStyle);

            GUILayout.Space(10);

            GUILayout.Label("- Dissonity.Api methods");
            GUILayout.Label("- Dissonity.Api.Commands methods");
            GUILayout.Label("- Dissonity.Api.Subscribe methods");
            GUILayout.Label("- Dissonity.Api.LocalStorage methods");
            GUILayout.Label("- Dissonity.Api.Proxy methods");
            GUILayout.Label("- Dissonity.Api.HiRpc methods");
            GUILayout.Label("- Dissonity.Utils methods");
            GUILayout.Label("- Dissonity.Models classes");
            GUILayout.Label("- @DiscordMock object");
            GUILayout.Label("- @JavascriptMock object");

            GUILayout.Space(15);

            GUILayout.Label("Changed", subHeaderStyle);

            GUILayout.Space(10);

            GUILayout.Label("- Playtest your Discord activity within Unity!");
            GUILayout.Label("- Not limited to Node.js servers.");
            GUILayout.Label("- You must call and await Api.Initialize once per runtime.");

            GUILayout.Space(15);

            GUILayout.Label("Fixed", subHeaderStyle);

            GUILayout.Space(10);

            GUILayout.Label("- Fixed check to detect absolute urls in proxy requests.");
            GUILayout.Label("- Added hiRPC mock runtime limitations.");

            GUILayout.Space(10);

            GUILayout.Label("And more!", italicStyle);

            GUILayout.Space(20);

            if (GUILayout.Button("Got it!", GUILayout.Height(25)))
            {
                Close();
            }

            GUILayout.Space(15);

            GUILayout.Label("Links", subHeaderStyle);

            //todo remove after initial release
            if (EditorGUILayout.LinkButton("Full Alpha changelog"))
            {
                Application.OpenURL("https://github.com/Furnyr/Dissonity/blob/v2/.github/ALPHA_CHANGELOG.md");
            }

            if (EditorGUILayout.LinkButton("Full Beta changelog"))
            {
                Application.OpenURL("https://github.com/Furnyr/Dissonity/blob/v2/.github/BETA_CHANGELOG.md");
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
