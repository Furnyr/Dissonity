using UnityEditor;
using Dissonity.Models.Mock;
using UnityEngine;

namespace Dissonity.Editor
{
    [CustomPropertyDrawer(typeof (MockChannel))]
    public class MockChannelDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            string channelName = property.FindPropertyRelative(nameof(MockChannel.Name)).stringValue;
            EditorGUI.PropertyField(position, property, new GUIContent(channelName));

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("VoiceStates", "Id == Query.ChannelId ? VoiceStates");

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }
    }
}