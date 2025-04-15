using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor
{
    internal class MenuAdvancedConfig : MonoBehaviour
    {
        [MenuItem("Assets/Create/Dissonity/Advanced Configuration", priority = 2)]
        public static void CreateConfiguration()
        {
            string fileData = Loady.Load<TextAsset>("AdvancedConfig.txt").text;

            ProjectWindowUtil.CreateAssetWithContent(
                "DissonityConfiguration.cs",
                fileData);
        }
    }
}