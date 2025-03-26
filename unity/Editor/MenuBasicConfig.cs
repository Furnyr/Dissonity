using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor
{
    internal class MenuBasicConfig : MonoBehaviour
    {
        [MenuItem("Assets/Create/Dissonity/Basic Configuration", priority = 0)]
        public static void CreateConfiguration()
        {
            string fileData = Loady.Load<TextAsset>("BasicConfig.txt").text;

            ProjectWindowUtil.CreateAssetWithContent(
                "DissonityConfiguration.cs",
                fileData);
        }
    }
}