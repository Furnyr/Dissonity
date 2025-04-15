using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor.Dialogs
{
    [CustomEditor(typeof(DialogAsset))]
    internal class DialogEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Show Welcome Dialog"))
            {
                WelcomeDialog.ShowDialog();
            }

            if (GUILayout.Button("Show Update Dialog"))
            {
                UpdateDialog.ShowDialog();
            }

            if (GUILayout.Button("Show Uninstaller Dialog"))
            {
                UninstallerDialog.ShowDialog();
            }
        }
    }
}
