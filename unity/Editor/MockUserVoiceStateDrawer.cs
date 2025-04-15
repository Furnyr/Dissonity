using UnityEditor;
using Dissonity.Models.Mock;
using UnityEngine;

namespace Dissonity.Editor
{
    [CustomPropertyDrawer(typeof (MockUserVoiceState))]
    public class MockUserVoiceStateDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.PropertyField(position, property);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("Nickname", "Participant.Nickname");
                EditorGUILayout.LabelField("User", "Participant as User");

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }
    }
}