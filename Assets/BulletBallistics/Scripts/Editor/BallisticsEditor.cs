using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using Ballistics;

public class BallisticsEditor : EditorWindow {

    private BallisticsSettings Settings;
    List<bool> MatEnabled = new List<bool>();
    List<bool> SoundEnabled = new List<bool>();
    Vector2 scrollVec;

    [MenuItem("Ballistics/Settings")]
	static void Init()
    {
        BallisticsEditor myWindow = (BallisticsEditor)GetWindow(typeof(BallisticsEditor),false,"Ballistics Settings");
        myWindow.Show();
    }

    void OnEnable()
    {
        BallisticSettingsManager.LoadSettings();
        Settings = BallisticSettingsManager.Settings;

        if (Settings != null)
        {
            for (int i = 0; i < Settings.MaterialData.Count; i++)
            {
                MatEnabled.Add(false);
                SoundEnabled.Add(false);
            }
        }
    }

    void OnInspectorUpdate()
    {
        //Load Ballistic Settings Scriptable Object
        if (BallisticSettingsManager.Settings != Settings)
        {
            Settings = BallisticSettingsManager.Settings;
        }
        if (Settings != null)
        {
            for (int i = 0; i < Settings.MaterialData.Count; i++)
            {
                MatEnabled.Add(false);
                SoundEnabled.Add(false);
            }
        }
    }

    void OnGUI()
    {
        scrollVec = EditorGUILayout.BeginScrollView(scrollVec);
        if (Settings != null)
        {
            // World Settings:
            
            EditorGUILayout.LabelField("World Settings",EditorStyles.largeLabel);
            EditorGUILayout.Separator();

            //Quality Settings
            EditorGUILayout.LabelField("Ballistic Quality:",EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            Settings.useBulletdrop = EditorGUILayout.Toggle("Bulletdrop: ",Settings.useBulletdrop);
            Settings.useBulletdrag = EditorGUILayout.Toggle("Bulletdrag: ", Settings.useBulletdrag);
            Settings.useBallisticMaterials = EditorGUILayout.Toggle("Ballistic Materials: ", Settings.useBallisticMaterials);
            Settings.useSpreadMaterials = EditorGUILayout.Toggle("Ballistic Spread: ", Settings.useSpreadMaterials);

            //________________

            if (Settings.useBulletdrag)
            {
                EditorGUILayout.Separator();
                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("Drag Settings:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                Settings.AirDensity = EditorGUILayout.FloatField("Air Density", Settings.AirDensity);
                Settings.WindDirection = EditorGUILayout.Vector3Field("Wind Direction", Settings.WindDirection);
            }

            //---------------------------------------------------------------------
            //Ballistic Material Editor
            EditorGUILayout.Separator();
            if (Settings.useBallisticMaterials)
            {
                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("Material Settings:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Material Types");

                if (GUILayout.Button("+", EditorStyles.miniButton))
                {
                    BallisticObjectData newData = new BallisticObjectData();
                    newData.RicochetPropability = new AnimationCurve();
                    newData.Name = "New Mat";
                    newData.EnergylossPerUnit = 1500f;
                    newData.RndSpread = 0.1f;
                    newData.RndSpreadRic = 0.2f;
                    newData.RicochetPropability = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(1f, 0.1f) });
                    Settings.MaterialData.Add(newData);
                    MatEnabled.Add(true);
                }

                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel++;

                for (int i = 0; i < Settings.MaterialData.Count; i++)
                {
                    EditorGUILayout.BeginHorizontal();
                    BallisticObjectData data = Settings.MaterialData[i];
                    MatEnabled[i] = EditorGUILayout.Foldout(MatEnabled[i], data.Name);
                    if (GUILayout.Button("-", EditorStyles.miniButton))
                    {
                        Settings.MaterialData.RemoveAt(i);
                        MatEnabled.RemoveAt(i);
                        i--;
                        continue;
                    }
                    EditorGUILayout.EndHorizontal();
                    if (MatEnabled[i])
                    {
                        EditorGUI.indentLevel++;
                        data.Name = EditorGUILayout.TextField("Material Name:", data.Name);
                        if (Settings.useBallisticMaterials)
                        {
                            data.EnergylossPerUnit = BigFloatField("Energyloss Per Unit:", data.EnergylossPerUnit);
                            data.RndSpread = BigFloatField("Random Spreadangle:", data.RndSpread);
                            data.RndSpreadRic = BigFloatField("Random Spreadangle (ricochet):", data.RndSpreadRic);
                            data.RicochetPropability = BigCurveField("Ricochet Propability", data.RicochetPropability);
                            data.impactObject = (ImpactObject)BigObjectField("Impact Game Object:", data.impactObject, typeof(ImpactObject));

                        }
                        EditorGUI.indentLevel--;
                    }

                    Settings.MaterialData[i] = data;
                    EditorGUILayout.Space();
                }
            }
            //-----------------------------------------------------------------------------

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Save", EditorStyles.miniButton))
            {
                EditorUtility.SetDirty(Settings);
                AssetDatabase.SaveAssets();
            }
            EditorGUILayout.LabelField("");
            EditorGUILayout.EndHorizontal();
        }
        else
        {
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField("Assign Ballistic Settings in Ballistic Manager!");
        }

        EditorGUILayout.EndScrollView();
    }

    float BigFloatField(string Text, float inVal)
    {
        int indent = EditorGUI.indentLevel;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(Text);
        EditorGUI.indentLevel = 0;
        float outVal = EditorGUILayout.FloatField("", inVal);
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel = indent;
        return outVal;
    }

    AnimationCurve BigCurveField(string Text, AnimationCurve inVal)
    {
        int indent = EditorGUI.indentLevel;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(Text);
        EditorGUI.indentLevel = 0;
        AnimationCurve outVal = EditorGUILayout.CurveField("", inVal);
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel = indent;
        return outVal;
    }

    Object BigObjectField(string Text, Object inVal,System.Type myType)
    {
        int indent = EditorGUI.indentLevel;
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(Text);
        EditorGUI.indentLevel = 0;
        Object outVal = EditorGUILayout.ObjectField("", inVal,myType,false);
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel = indent;
        return outVal;
    }
}
