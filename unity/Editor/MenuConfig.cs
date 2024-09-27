using UnityEditor;
using UnityEngine;

namespace Dissonity.Editor
{
    internal class MenuConfig : MonoBehaviour
    {
        [MenuItem("Assets/Create/Dissonity/Configuration", priority = 2)]
        static void CreateConfiguration()
        {
            var fileData = Resources.Load<TextAsset>("Dissonity_DefaultUserConfig").ToString();

            ProjectWindowUtil.CreateAssetWithContent(
                "DissonityUserConfiguration.cs",
                fileData);
        }
    }
}