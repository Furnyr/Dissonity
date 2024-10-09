using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor
{
    internal class MenuMock : MonoBehaviour
    {
        [MenuItem("GameObject/Dissonity/Discord Mock", priority = 2)]
        static bool CreateDiscordMock() {

            DiscordMock existing = FindObjectOfType<DiscordMock>();

            //? Mock found
            if (existing != null) {
                return false;
            }

            GameObject obj = new GameObject("@DiscordMock");

            obj.AddComponent<DiscordMock>();

            Undo.RegisterCreatedObjectUndo(obj, "Create " + obj.name);
            Selection.activeObject = obj;

            return true;
        }

        [MenuItem("GameObject/Dissonity/Discord Mock", true, 2)]
        static bool CheckDiscordMock() {

            DiscordMock existing = FindObjectOfType<DiscordMock>();

            //? Mock found
            if (existing != null) {
                return false;
            }

            return true;
        }
    }
}