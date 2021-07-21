using UnityEngine;

namespace Crossp
{
    public class CrosspSettings : ScriptableObject
    {
        [SerializeField]
        private string serverURL = default;
        public string ServerURL => serverURL;

        private string apiVersion = "1.0";
        public string ApiVersion => apiVersion;

        [SerializeField]
        private string externalId = default;
        public string ExternalId => externalId;

        [SerializeField]
        private RenderTexture renderTexture = default;
        public RenderTexture RenderTexture => renderTexture;


        //private const string CrosspSettingsDir = "Assets/Crossp";

        //private const string CrosspSettingsResDir = CrosspSettingsDir + "/Resources";

        //private const string CrosspSettingsFile = CrosspSettingsResDir + "/CrosspSettings.asset";

        //private static CrosspSettings instance;
        //public static CrosspSettings Instance
        //{
        //    get
        //    {
        //        if (instance == null)
        //        {
        //            if (!AssetDatabase.IsValidFolder(CrosspSettingsResDir))
        //            {
        //                AssetDatabase.CreateFolder(CrosspSettingsDir, "Resources");
        //            }

        //            instance = (CrosspSettings)AssetDatabase.LoadAssetAtPath(
        //                CrosspSettingsFile, typeof(CrosspSettings));

        //            if (instance == null)
        //            {
        //                instance = ScriptableObject.CreateInstance<CrosspSettings>();
        //                AssetDatabase.CreateAsset(instance, CrosspSettingsFile);
        //            }
        //        }
        //        return instance;
        //    }
        //}

        //[MenuItem("Edit/Crossp")]
        //public static void OpenInspector()
        //{
        //    Selection.activeObject = Instance;
        //}

    }
}
