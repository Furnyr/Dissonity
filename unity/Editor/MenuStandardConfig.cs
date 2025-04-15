using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor
{
    internal class MenuStandardConfig : MonoBehaviour
    {
        [MenuItem("Assets/Create/Dissonity/Standard Configuration", priority = 1)]
        public static void CreateConfiguration()
        {
            string fileData = Loady.Load<TextAsset>("StandardConfig.txt").text;

            ProjectWindowUtil.CreateAssetWithContent(
                "DissonityConfiguration.cs",
                fileData);
        }
    }
}