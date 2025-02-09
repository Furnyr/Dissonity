using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor
{
    internal class MenuAdvancedConfig : MonoBehaviour
    {
        [MenuItem("Assets/Create/Dissonity/Advanced Configuration", priority = 2)]
        public static void CreateConfiguration()
        {
            string fileData = Resources.Load<TextAsset>("Dissonity_AdvancedConfig").text;

            ProjectWindowUtil.CreateAssetWithContent(
                "DissonityConfiguration.cs",
                fileData);
        }
    }
}