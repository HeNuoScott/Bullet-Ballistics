using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using Ballistics;

[CustomEditor(typeof(FalloffMapGenerator))]
public class FalloffMapGeneratorEditor : Editor {

    FalloffMapGenerator t;
    bool ShowZeroList = true;
    private BallisticsSettings Settings;

    public void OnSceneGUI()
    {
        Handles.matrix = t.transform.localToWorldMatrix;
        t.BarrelPos = Handles.PositionHandle(t.BarrelPos, Quaternion.LookRotation(Vector3.forward));
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("Weapon Zeroing Settings", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        if (t.TargetWeapon == null)
        {
            EditorGUILayout.LabelField("Set TargetWeapon.");
        }
        else if (Settings == null)
        {
            EditorGUILayout.LabelField("No Settings file connected! Check Ballistics Manager.");
        }

        EditorGUILayout.LabelField("General", EditorStyles.miniBoldLabel);
        EditorGUI.indentLevel++;
        t.TargetWeapon = (Weapon)EditorGUILayout.ObjectField("My Weapon:", (Object)t.TargetWeapon, typeof(Weapon), true);
        t.Size = EditorGUILayout.Slider("Gizmo Size:", t.Size, 0.02f, 1.2f);

        EditorGUILayout.Space();
        EditorGUI.indentLevel--;
        EditorGUILayout.LabelField("Zeroing", EditorStyles.miniBoldLabel);
        EditorGUI.indentLevel++;

        t.ScopeDist = Mathf.Clamp(EditorGUILayout.FloatField("Scope distance:", t.ScopeDist),0.05f,float.MaxValue);
        t.BarrelPos = EditorGUILayout.Vector3Field("Barrel Position:", t.BarrelPos);
        t.TextureSize = Mathf.Clamp((EditorGUILayout.IntField("Texture size:", t.TextureSize) / 2)*2,128,2048);


        EditorGUILayout.BeginHorizontal();
        ShowZeroList = EditorGUILayout.Foldout(ShowZeroList, "Scope Zeroings");
        if (GUILayout.Button("+"))
        {
            t.ZeroingDist.Add(150);
            ShowZeroList = true;
        }
        EditorGUILayout.EndHorizontal();
        EditorGUI.indentLevel++;
        if (ShowZeroList)
        {
            for (int i = 0; i < t.ZeroingDist.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                t.ZeroingDist[i] = Mathf.Clamp(EditorGUILayout.FloatField("Zero distance " + i.ToString() + " :", t.ZeroingDist[i]),10,float.MaxValue);
                if (GUILayout.Button("-"))
                {
                    t.ZeroingDist.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        if(t.ZeroingDist.Count > 0)
        {
            if(GUILayout.Button("Generate Falloff Texture"))
            {
                GenerateFalloffTexture();
            }
        }
        

        if (GUI.changed)
        {
            SceneView.RepaintAll();
        }
    }


     void OnEnable()
     {
         t = (FalloffMapGenerator)target;
        BallisticSettingsManager.LoadSettings();
        Settings = BallisticSettingsManager.Settings;
     }

    public void GenerateFalloffTexture()
    {
        if (Settings != null)
        {
            if (!Settings.useBulletdrop)
            {
                Debug.LogAssertion("Bulletdrop is not enabled! Check 'Ballistic Settings' window.");
            }
            else {
                if (t.ZeroingDist.Count > 0)
                {
                    t.ZeroingDist.Sort();
                    int texSize = t.TextureSize;
                    Texture2D falloffmap = new Texture2D(texSize, texSize);
                    falloffmap.wrapMode = TextureWrapMode.Clamp;
                    List<float> drops = new List<float>(t.ZeroingDist.Count);

                    float FlightTime;
                    for (int i = 0; i < t.ZeroingDist.Count; i++)
                    {
                        drops.Add(0);
                        //Zeroing through Scope Points
                        if (Settings.useBulletdrag)
                        {
                            float k = (Settings.AirDensity * t.TargetWeapon.DragCoefficient * Mathf.PI * (t.TargetWeapon.Diameter * .5f) * (t.TargetWeapon.Diameter * .5f)) / (2 * t.TargetWeapon.BulletMass);
                            FlightTime = (Mathf.Exp(k * t.ZeroingDist[i]) - 1) / (k * t.TargetWeapon.MaxBulletSpeed);
                        }
                        else
                        {
                            FlightTime = (t.ZeroingDist[i]) / t.TargetWeapon.MaxBulletSpeed;
                        }
                        drops[i] = (.5f * Physics.gravity.y * Mathf.Pow(FlightTime, 2));

                        //scope height above barrel
                        drops[i] -= t.BarrelPos.y;

                        //Zeroing Dot Position
                        drops[i] = Mathf.Abs(drops[i]) * (t.ScopeDist / t.ZeroingDist[i]);
                    }

                    drops.Sort();
                    float Max = drops[drops.Count - 1];
                    int y = texSize / 2;
                    Color[] c = new Color[texSize * texSize];
                    Color bg = new Color(0, 0, 0, 0);
                    Color line = Color.red;
                    for (int i = 0; i < c.Length; i++)
                    {
                        c[i] = bg;
                    }

                    for (int i = 0; i < texSize; i++)
                    {
                        c[texSize / 2 + i * texSize] = line;
                        c[texSize / 2 * texSize + i] = line;
                    }
                    falloffmap.SetPixels(c);

                    for (int i = 0; i < drops.Count; i++)
                    {
                        y = texSize / 2 - (int)((drops[i] / Max) * texSize / 2);
                        for (int x = -texSize / 10; x < texSize / 10; x++)
                        {
                            falloffmap.SetPixel(texSize / 2 + x, y, line);
                        }
                    }
                    falloffmap.Apply();

                    if (!AssetDatabase.IsValidFolder("Assets/ScopeFalloffMaps"))
                    {
                        AssetDatabase.CreateFolder("Assets", "ScopeFalloffMaps");
                    }
                    if (File.Exists(Application.dataPath + "/ScopeFalloffMaps/" + t.TargetWeapon.name + ".png"))
                    {
                        File.Delete(Application.dataPath + "/ScopeFalloffMaps/" + t.TargetWeapon.name + ".png");
                    }
                    File.WriteAllBytes(Application.dataPath + "/ScopeFalloffMaps/" + t.TargetWeapon.name + ".png", falloffmap.EncodeToPNG());
                    AssetDatabase.Refresh(ImportAssetOptions.Default);
                    EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<Texture>("Assets/ScopeFalloffMaps/" + t.TargetWeapon.name + ".png"));

                    Transform FalloffTexPlane = t.transform.Find("FalloffTex");
                    if (FalloffTexPlane != null)
                    {
                        FalloffTexPlane.GetComponent<Renderer>().sharedMaterial.mainTexture = falloffmap;
                        FalloffTexPlane.localPosition = new Vector3(0, 0, t.ScopeDist);
                        FalloffTexPlane.localScale = new Vector3(Max * 2, Max * 2, Max * 2);
                    }
                    else
                    {
                        FalloffTexPlane = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
                        FalloffTexPlane.name = "FalloffTex";
                        FalloffTexPlane.SetParent(t.transform);
                        FalloffTexPlane.localRotation = Quaternion.Euler(0, 0, 0);
                        FalloffTexPlane.localPosition = new Vector3(0, 0, t.ScopeDist);
                        Material m = new Material(Shader.Find("Unlit/Transparent"));
                        m.mainTexture = falloffmap;
                        FalloffTexPlane.GetComponent<Renderer>().sharedMaterial = m;
                        FalloffTexPlane.localScale = new Vector3(Max * 2, Max * 2, Max * 2);
                        DestroyImmediate(FalloffTexPlane.GetComponent<Collider>());
                    }
                }
                else
                {
                    Debug.LogAssertion("No set Scopezeroing Distances...");
                }
            }
        }
        else
        {
            Debug.LogAssertion("No BallisticSettings assigned in BallisticsManager!");
        }
    }
}
