using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor
{
    internal class MenuDiscordMock : MonoBehaviour
    {
        [MenuItem("GameObject/Dissonity/Discord Mock", priority = 2)]
        public static bool CreateDiscordMock() {

            DiscordMock existing = FindAnyObjectByType<DiscordMock>();

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
        public static bool CheckDiscordMock() {

            DiscordMock existing = FindAnyObjectByType<DiscordMock>();

            //? Mock found
            if (existing != null) {
                return false;
            }

            return true;
        }
    }
}