using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Dissonity
{
    internal class EditorMenus : MonoBehaviour
    {
        [MenuItem("GameObject/Dissonity/Discord Bridge", priority = 2)]
        static bool CreateDiscordBridge() {

            //? DiscordBridge found
            GameObject existing = GameObject.Find("DiscordBridge");
            if (existing != null) {

                //? Has the script
                if (existing.GetComponent<DiscordBridge>() != null) {
                    return false;
                }
            }

            GameObject obj = new GameObject("DiscordBridge");

            obj.AddComponent<DiscordBridge>();

            Undo.RegisterCreatedObjectUndo(obj, "Create " + obj.name);
            Selection.activeObject = obj;

            return true;
        }

        [MenuItem("GameObject/Dissonity/Discord Bridge", true, 2)]
        static bool CheckDiscordBridge() {

            //? DiscordBridge found
            GameObject existing = GameObject.Find("DiscordBridge");
            if (existing != null) {

                //? Has the script
                if (existing.GetComponent<DiscordBridge>() != null) {
                    return false;
                }
            }

            return true;
        }
    }
}