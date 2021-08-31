using System.Collections;
using UnityEditor;
using Ballistics;

public static class BallisticSettingsManager
{
    public static BallisticsSettings Settings;

    public static void LoadSettings()
    {
        if (Settings == null)
        {
            if (EditorPrefs.HasKey("SettingsPath"))
            {
                string path = EditorPrefs.GetString("SettingsPath");
                Settings = (BallisticsSettings)AssetDatabase.LoadAssetAtPath(path, typeof(BallisticsSettings));
            }
        }
    }
}
