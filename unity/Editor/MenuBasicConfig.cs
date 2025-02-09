using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor
{
    internal class MenuBasicConfig : MonoBehaviour
    {
        [MenuItem("Assets/Create/Dissonity/Basic Configuration", priority = 2)]
        public static void CreateConfiguration()
        {
            string fileData = Resources.Load<TextAsset>("Dissonity_BasicConfig").text;

            ProjectWindowUtil.CreateAssetWithContent(
                "DissonityConfiguration.cs",
                fileData);
        }
    }
}