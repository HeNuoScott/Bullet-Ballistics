using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using Ballistics;

public class BallisticSettingsManagerEditor : EditorWindow
{
    BallisticsSettings Settings;

    [MenuItem("Ballistics/Manager")]
    static void Init()
    {
        BallisticSettingsManagerEditor myWindow = (BallisticSettingsManagerEditor)GetWindow(typeof(BallisticSettingsManagerEditor), false, "Ballistic Manager");
        myWindow.Show();
    }

    void OnEnable()
    {
        BallisticSettingsManager.LoadSettings();
        Settings = BallisticSettingsManager.Settings;
    }

    void OnGUI()
    {
        EditorGUILayout.Separator();
        EditorGUILayout.Space();
        Settings = (BallisticsSettings)EditorGUILayout.ObjectField("Ballistic Settings:", Settings, typeof(BallisticsSettings), false);
        EditorGUILayout.Space();

        if (GUILayout.Button("New Ballistic Settings"))
        {
            string Path = CreateAsset<BallisticsSettings>();
            EditorPrefs.SetString("SettingsPath", Path);
            Settings = (BallisticsSettings)AssetDatabase.LoadAssetAtPath(Path, typeof(BallisticsSettings));
        }
        if (GUILayout.Button("None Ballistic Settings"))
        {
            EditorPrefs.DeleteKey("SettingsPath");
            Settings = null;
        }
        if (Settings != null)
        {
            EditorUtility.SetDirty(Settings);
            EditorPrefs.SetString("SettingsPath", AssetDatabase.GetAssetPath((Object)Settings));
        }
        BallisticSettingsManager.Settings = Settings;
    }

    static string CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (Path.GetExtension(path) != "")
        {
            path = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;

        return assetPathAndName;
    }
}
