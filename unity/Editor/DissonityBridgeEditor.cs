using UnityEditor;

namespace Dissonity.Editor
{
    [CustomEditor(typeof(DissonityBridge))]
    internal class DissonityBridgeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("", "This object receives data sent by Discord. You don't need to interact with this script at all.");
        }
    }
}