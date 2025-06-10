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
            
            GUILayout.Label("v2.1.0 Overview", headerStyle);

            GUILayout.Label("Compatibility improvements, relationship features and bug fixes", centerStyle);

            GUILayout.Space(15);

            GUILayout.Label("Added", subHeaderStyle);

            GUILayout.Space(10);

            GUILayout.Label("- LazyHiRpcLoad option in the Advanced Configuration");
            GUILayout.Label("- Api.Commands.GetRelationships (requires Discord approval)");
            GUILayout.Label("- Api.Subscribe.RelationshipUpdate (requires Discord approval)");
            GUILayout.Label("- Models.Relationship");
            GUILayout.Label("- Relationships section in the Discord Mock");

            GUILayout.Space(15);

            GUILayout.Label("Changed", subHeaderStyle);

            GUILayout.Space(10);

            GUILayout.Label("- Minor performance improvements related to the hiRPC Interface");

            GUILayout.Space(15);

            GUILayout.Label("Fixed", subHeaderStyle);

            GUILayout.Space(10);

            GUILayout.Label("- The game doesn't turn into a black screen when loading the index.html");

            GUILayout.Space(10);

            GUILayout.Label("And more!", italicStyle);

            GUILayout.Space(20);

            if (GUILayout.Button("Got it!", GUILayout.Height(25)))
            {
                Close();
            }

            GUILayout.Space(15);

            GUILayout.Label("Links", subHeaderStyle);

            if (EditorGUILayout.LinkButton("Full changelog"))
            {
                Application.OpenURL("https://github.com/Furnyr/Dissonity/releases/tag/v2.1.0");
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
