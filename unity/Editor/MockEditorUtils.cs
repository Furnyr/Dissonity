
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor
{
    internal static class MockEditorUtils
    {
        // Used to draw the children of a property. If Unity does it automatically, indentation breaks between versions.
        // exclude is used to prevent properties from rendering
        // tooltipMap is used to draw tooltips for specific properties
        public static void DrawChildrenRecursively(SerializedProperty property, string[] exclude = null, Dictionary<string, string> tooltipMap = null)
        {
            SerializedProperty endProperty = property.GetEndProperty();

            property.NextVisible(true);

            while (!SerializedProperty.EqualContents(property, endProperty))
            {
                if (exclude == null || !exclude.Contains(property.name))
                {
                    //? Tooltip
                    if (tooltipMap != null && tooltipMap.ContainsKey(property.name))
                    {
                        GUIContent content = new (property.name, tooltipMap[property.name]);
                        EditorGUILayout.PropertyField(property, content, property.isArray);
                    }

                    else EditorGUILayout.PropertyField(property, property.isArray);

                    if (property.hasVisibleChildren && property.isExpanded)
                    {
                        EditorGUI.indentLevel++;
                        DrawChildrenRecursively(property, exclude, tooltipMap);
                        EditorGUI.indentLevel--;

                        continue;
                    }
                }
                
                property.NextVisible(false);
            }
            
        }

        public static void SetButtonStyles(out GUIStyle leftButtonStyle)
        {
            // Normal button style
            leftButtonStyle = new GUIStyle(GUI.skin.button);
            leftButtonStyle.alignment = TextAnchor.MiddleLeft;
        }
    
        public static void StartSpace(int space)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(space);
            EditorGUILayout.BeginVertical();
        }

        public static void EndSpace()
        {
            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    
        // Danger
        public static void TintButtonRed()
        {
            GUI.backgroundColor = new Color(0.78f, 0.29f, 0.23f);
        }

        // Warning (currently unused)
        public static void TintButtonYellow()
        {
            GUI.backgroundColor = new Color(1f, 0.8f, 0f);
        }

        // Close
        public static void TintButtonDark()
        {
            GUI.backgroundColor = new Color(0.78f, 0.78f, 0.78f);
        }

        // Open
        public static void TintButtonBlue()
        {
            GUI.backgroundColor = new Color(0.6f, 1f, 0.96f);
        }

        // Unreleased
        public static void TintButtonDisabled()
        {
            GUI.backgroundColor = new Color(0.46f, 0.45f, 0.447f);
        }

        public static void ResetButtonTint()
        {
            GUI.backgroundColor = Color.white;
        }
    }
}