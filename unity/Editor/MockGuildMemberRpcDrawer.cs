using UnityEditor;
using Dissonity.Models.Mock;
using UnityEngine;

namespace Dissonity.Editor
{
    [CustomPropertyDrawer(typeof (MockGuildMemberRpc))]
    public class MockGuildMemberRpcDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PropertyField(position, property);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("UserId", "Participant.Id");
                EditorGUILayout.LabelField("Nickname", "Participant.Nickname");

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }
    }
}