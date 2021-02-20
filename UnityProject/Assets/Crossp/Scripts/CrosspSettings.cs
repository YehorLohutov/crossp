using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Crossp
{
    public class CrosspSettings : ScriptableObject
    {
        [SerializeField]
        private string serverURL = default;
        public string ServerURL => serverURL;

        [SerializeField]
        private string externalId = default;
        public string ExternalId => externalId;







        private static CrosspSettings instance = default;
        public static CrosspSettings Instance
        {
            get
            {
                if (instance == null)
                    instance = Resources.FindObjectsOfTypeAll(typeof(CrosspSettings)).First() as CrosspSettings;
                return instance;
            }
        }

        [MenuItem("Edit/Crossp")]
        static void OpenCrosspSettings()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{nameof(CrosspSettings)}");
            switch(guids.Length)
            {
                case 0:
                    CrosspSettings crosspSettings = ScriptableObject.CreateInstance<CrosspSettings>();
                    AssetDatabase.CreateAsset(crosspSettings, "Assets/Crossp/CrosspSettings.asset");
                    AssetDatabase.SaveAssets();

                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = crosspSettings;
                    break;
                case 1:
                    string crosspSettingsGUID = guids[0];
                    Debug.Log(AssetDatabase.GUIDToAssetPath(crosspSettingsGUID));

                    EditorUtility.FocusProjectWindow();
                    Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(crosspSettingsGUID));
                    break;
                default:
                    Debug.LogError($"{nameof(CrosspSettings)} assets > 1");
                    break;
            }
        }
    }
}
