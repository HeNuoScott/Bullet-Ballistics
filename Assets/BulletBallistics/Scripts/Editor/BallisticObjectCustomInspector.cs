using UnityEngine;
using System.Collections;
using UnityEditor;
using Ballistics;

[CustomEditor(typeof(BallisticObject),true)]
[CanEditMultipleObjects]
public class BallisticObjectCustomInspector : Editor
{
    string[] Names;
    BallisticsSettings Settings;

    public override void OnInspectorGUI()
    {
        BallisticObject myTarget = (BallisticObject)target;
        if (Settings != null)
        {
            EditorGUI.BeginChangeCheck();
            myTarget.MatID = EditorGUILayout.Popup("Materialtype:",myTarget.MatID, Names);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object obj in targets)
                {
                    ((BallisticObject)obj).MatID = myTarget.MatID;
                }
            }

            EditorGUI.BeginChangeCheck();
            myTarget.isStatic = EditorGUILayout.Toggle("is Static Object:", myTarget.isStatic);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object obj in targets)
                {
                    ((BallisticObject)obj).isStatic = myTarget.isStatic;
                }
            }
        }
        else
        {
            EditorGUILayout.LabelField("No Settings Found!");
            EditorGUILayout.LabelField("Check Ballistics Editor!");
        }
    }

    void OnEnable()
    {
        BallisticObject myTarget = (BallisticObject)target;

        BallisticSettingsManager.LoadSettings();
        Settings = BallisticSettingsManager.Settings;

        if (Settings != null)
        {
            Names = new string[(Settings.MaterialData.Count == 0) ? 1 : Settings.MaterialData.Count];
            if(Settings.MaterialData.Count == 0)
            {
                Names[0] = "Add custom materials in the 'Ballistic Settings' window";
            }
            for (int i = 0; i < Settings.MaterialData.Count; i++)
            {
                Names[i] = Settings.MaterialData[i].Name;
            }

            if (Settings.MaterialData.Count <= myTarget.MatID)
            {
                myTarget.MatID = 0;
            }
        }
    }
}
