using Dissonity.Editor.Dialogs;
using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor
{
    internal class MenuDevDialog : MonoBehaviour
    {
        [MenuItem("Assets/Create/Dissonity/Development/Dev Dialogs Asset", priority = 10)]
        public static void CreateConfiguration()
        {
            DevDialogAsset asset = ScriptableObject.CreateInstance<DevDialogAsset>();

            ProjectWindowUtil.CreateAsset(
                asset,
                "DevDialogs.asset"
            );
        }
    }
}