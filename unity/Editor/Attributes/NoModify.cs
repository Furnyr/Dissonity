using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dissonity
{
    public class NoModifyAttribute : PropertyAttribute {}

    [CustomPropertyDrawer(typeof(NoModifyAttribute))]
    public class NoModifyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, true);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, true);
            GUI.enabled = true;
        }
    }
}