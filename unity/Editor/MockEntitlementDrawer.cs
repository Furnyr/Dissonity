
using UnityEditor;
using Dissonity.Models.Mock;
using UnityEngine;

namespace Dissonity.Editor
{
    [CustomPropertyDrawer(typeof (MockEntitlement))]
    public class MockEntitlementDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            string name = property.FindPropertyRelative(nameof(MockEntitlement._mock_name)).stringValue;
            EditorGUI.PropertyField(position, property, new GUIContent(name));

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.LabelField("UserId", "Activity.CurrentPlayer.Participant.Id");

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }
    }
}