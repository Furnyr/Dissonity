using UnityEngine;
using UnityEditor;
using Dissonity.Models.Mock;
using System.Collections.Generic;
using System.Linq;
using static Dissonity.Editor.MockEditorUtils;

namespace Dissonity.Editor
{
    [CustomEditor(typeof(JavascriptMock))]
    internal class JavascriptMockEditor : UnityEditor.Editor
    {
        // Main foldouts
        private bool showLocalStorage = false;
        private bool showHiRpc = false;

        // True when the clear menus are open
        private bool clearingLocalStorage = false;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            JavascriptMock mock = (JavascriptMock) target;

            GUIStyle leftButtonStyle; 

            SetButtonStyles(out leftButtonStyle);

            // Shorcut
            bool isPlaying = Application.isPlaying;

            //# LOCAL STORAGE - - - - -
            var localStorageProperty = serializedObject.FindProperty(nameof(JavascriptMock._localStorage));
            showLocalStorage = EditorGUILayout.Foldout(showLocalStorage, "Local Storage");

            if (showLocalStorage)
            {
                EditorGUI.indentLevel++;

                // Draw every item
                for (int i = 0; i < mock._localStorage.Count; i++)
                {
                    // Draw the item element
                    var item = localStorageProperty.GetArrayElementAtIndex(i);
                    string itemKey = item.FindPropertyRelative(nameof(MockStorageItem.Key)).stringValue;
                    
                    EditorGUILayout.PropertyField(item, new GUIContent (itemKey), false);

                    if (item.isExpanded)
                    {
                        EditorGUI.indentLevel++;

                        DrawChildrenRecursively(item);

                        EditorGUI.indentLevel--;
                    }
                }
                
                EditorGUI.indentLevel--;

                // Draw add item button
                StartSpace(20);

                if (GUILayout.Button("Add item", leftButtonStyle))
                {
                    var item = new MockStorageItem
                    {
                        // Default key
                        Key = $"mock-item-{mock._localStorage.Count + 1}",
                    };

                    mock._localStorage.Add(item);
                }

                TintButtonBlue();

                if (!clearingLocalStorage && GUILayout.Button("Clear", leftButtonStyle))
                {
                    clearingLocalStorage = true;
                }

                ResetButtonTint();

                if (clearingLocalStorage)
                {
                    TintButtonDark();

                    if (GUILayout.Button("Cancel clear", leftButtonStyle))
                    {
                        clearingLocalStorage = false;
                    }

                    ResetButtonTint();
                    TintButtonRed();

                    if (GUILayout.Button("Confirm clear", leftButtonStyle))
                    {
                        clearingLocalStorage = false;
                        localStorageProperty.ClearArray();
                    }

                    ResetButtonTint();
                }

                EndSpace();
            }

            else if (clearingLocalStorage) clearingLocalStorage = false;

            //# HIRPC - - - - -
            showHiRpc = EditorGUILayout.Foldout(showHiRpc, "hiRPC");

            if (showHiRpc)
            {
                EditorGUI.indentLevel++;

                // hiRPC Log Js Messages
                var hiRpcLogJsMessages = serializedObject.FindProperty(nameof(JavascriptMock._hiRpclogJsMessages));
                EditorGUILayout.PropertyField(hiRpcLogJsMessages, new GUIContent("Log JS Messages"));

                // hiRPC App Hash
                var hiRpcAppHash = serializedObject.FindProperty(nameof(JavascriptMock._hiRpcAppHash));
                EditorGUILayout.PropertyField(hiRpcAppHash, new GUIContent("App Hash"));

                // hiRPC Channel
                var hiRpcChannelProperty = serializedObject.FindProperty(nameof(JavascriptMock._hiRpcChannel));
                EditorGUILayout.PropertyField(hiRpcChannelProperty, new GUIContent("Channel"));

                GUILayout.Space(5);

                // Send Json To Unity
                var hiRpcJsonProperty = serializedObject.FindProperty(nameof(JavascriptMock._hiRpcSendJsonToUnity));

                EditorGUILayout.LabelField("Serialized JSON");

                hiRpcJsonProperty.stringValue = EditorGUILayout.TextArea(
                    hiRpcJsonProperty.stringValue
                );

                // Draw send button
                GUILayout.Space(5);

                StartSpace(15);

                if (GUILayout.Button("Send JSON Data to Unity", leftButtonStyle))
                {
                    //? Not in runtime
                    if (!isPlaying)
                    {
                        Debug.Log("[Dissonity Editor] You can only dispatch messages during runtime!");
                        return;
                    }

                    //? Not initialized
                    if (!Api.Ready)
                    {
                        Debug.Log("[Dissonity Editor] You can only receive hiRPC messages once initialized!");
                        return;
                    }

                    mock.DispatchMessage();
                }

                EndSpace();

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}