using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor
{
    internal class MenuJavascriptMock : MonoBehaviour
    {
        [MenuItem("GameObject/Dissonity/Javascript Mock", priority = 2)]
        public static bool CreateJavascriptMock() {

            JavascriptMock existing = FindAnyObjectByType<JavascriptMock>();

            //? Mock found
            if (existing != null) {
                return false;
            }

            GameObject obj = new GameObject("@JavascriptMock");

            obj.AddComponent<JavascriptMock>();

            Undo.RegisterCreatedObjectUndo(obj, "Create " + obj.name);
            Selection.activeObject = obj;

            return true;
        }

        [MenuItem("GameObject/Dissonity/Javascript Mock", true, 2)]
        public static bool CheckJavascriptMock() {

            JavascriptMock existing = FindAnyObjectByType<JavascriptMock>();

            //? Mock found
            if (existing != null) {
                return false;
            }

            return true;
        }
    }
}