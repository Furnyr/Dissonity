using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor
{
    internal class MenuStandardConfig : MonoBehaviour
    {
        [MenuItem("Assets/Create/Dissonity/Standard Configuration", priority = 2)]
        public static void CreateConfiguration()
        {
            string fileData = Resources.Load<TextAsset>("Dissonity_StandardConfig").text;

            ProjectWindowUtil.CreateAssetWithContent(
                "DissonityConfiguration.cs",
                fileData);
        }
    }
}