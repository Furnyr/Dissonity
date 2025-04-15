using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor.Dialogs
{
    [CustomEditor(typeof(DevDialogAsset))]
    internal class DevDialogEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("Regenerate WebGL Template"))
            {
                AddTemplate.DeleteWebGLTemplate();

                EditorUtility.RequestScriptReload();
            }
        }
    }
}
