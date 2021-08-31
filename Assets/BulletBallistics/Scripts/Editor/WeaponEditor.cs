using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using Ballistics;

[CustomEditor(typeof(Weapon))]
[CanEditMultipleObjects]
public class WeaponEditor : Editor
{
    Weapon TargetWeapon;

    private bool showBarrelZero;

    private BallisticsSettings Settings;

    void OnEnable()
    {
        TargetWeapon = (Weapon)target;
        BallisticSettingsManager.LoadSettings();
        Settings = BallisticSettingsManager.Settings;
    }

    public override void OnInspectorGUI()
    {
        if (Settings != null)
        {
            EditorGUILayout.LabelField("Weapon Editor", EditorStyles.largeLabel);
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("General Settings:", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            EditorGUI.BeginChangeCheck();
            TargetWeapon.VisualSpawnPoint = (Transform)EditorGUILayout.ObjectField("Visual Spawn Point:", TargetWeapon.VisualSpawnPoint, typeof(Transform), true);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object obj in targets)
                {
                    ((Weapon)obj).VisualSpawnPoint = TargetWeapon.VisualSpawnPoint;
                }
            }

            EditorGUI.BeginChangeCheck();
            TargetWeapon.PhysicalBulletSpawnPoint = (Transform)EditorGUILayout.ObjectField("Bullet Spawn Point:", TargetWeapon.PhysicalBulletSpawnPoint, typeof(Transform), true);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object obj in targets)
                {
                    ((Weapon)obj).PhysicalBulletSpawnPoint = TargetWeapon.PhysicalBulletSpawnPoint;
                }
            }

            EditorGUI.BeginChangeCheck();
            TargetWeapon.LifeTimeOfBullets = EditorGUILayout.FloatField("Lifetime of Bullet:", TargetWeapon.LifeTimeOfBullets);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object obj in targets)
                {
                    ((Weapon)obj).LifeTimeOfBullets = TargetWeapon.LifeTimeOfBullets;
                }
            }

            EditorGUI.BeginChangeCheck();
            TargetWeapon.MuzzleDamage = EditorGUILayout.FloatField("Muzzle Damage:", TargetWeapon.MuzzleDamage);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object obj in targets)
                {
                    ((Weapon)obj).MuzzleDamage = TargetWeapon.MuzzleDamage;
                }
            }

            EditorGUI.BeginChangeCheck();
            TargetWeapon.HitMask = LayerMaskField("Hit Mask:", TargetWeapon.HitMask);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object obj in targets)
                {
                    ((Weapon)obj).HitMask = TargetWeapon.HitMask;
                }
            }


            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("Bullet:", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;


            EditorGUI.BeginChangeCheck();
            TargetWeapon.BulletPref = (Transform)EditorGUILayout.ObjectField("Bullet Prefab:", TargetWeapon.BulletPref, typeof(Transform), true);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object obj in targets)
                {
                    ((Weapon)obj).BulletPref = TargetWeapon.BulletPref;
                }
            }

            EditorGUI.BeginChangeCheck();
            TargetWeapon.MaxBulletSpeed = EditorGUILayout.FloatField("Bullet Speed:", TargetWeapon.MaxBulletSpeed);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object obj in targets)
                {
                    ((Weapon)obj).MaxBulletSpeed = TargetWeapon.MaxBulletSpeed;
                }
            }

            EditorGUI.BeginChangeCheck();
            TargetWeapon.randomSpeedOffset = EditorGUILayout.FloatField("Random Speed:", TargetWeapon.randomSpeedOffset);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object obj in targets)
                {
                    ((Weapon)obj).randomSpeedOffset = TargetWeapon.randomSpeedOffset;
                }
            }

            EditorGUI.BeginChangeCheck();
            TargetWeapon.BulletMass = EditorGUILayout.FloatField("Mass of Bullet:", TargetWeapon.BulletMass);
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object obj in targets)
                {
                    ((Weapon)obj).BulletMass = TargetWeapon.BulletMass;
                }
            }

            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("maximal kinetic energy:\t " + (0.5f * TargetWeapon.BulletMass * TargetWeapon.MaxBulletSpeed * TargetWeapon.MaxBulletSpeed).ToString() + " J", EditorStyles.miniLabel);
            EditorGUI.indentLevel--;

            if (Settings.useBulletdrag)
            {
                EditorGUI.BeginChangeCheck();
                TargetWeapon.DragCoefficient = EditorGUILayout.FloatField("Drag Coefficient:", TargetWeapon.DragCoefficient);
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (Object obj in targets)
                    {
                        ((Weapon)obj).DragCoefficient = TargetWeapon.DragCoefficient;
                    }
                }

                EditorGUI.BeginChangeCheck();
                TargetWeapon.Diameter = EditorGUILayout.FloatField("Diameter:", TargetWeapon.Diameter);
                if (EditorGUI.EndChangeCheck())
                {
                    foreach (Object obj in targets)
                    {
                        ((Weapon)obj).Diameter = TargetWeapon.Diameter;
                    }
                }

            }

            EditorGUILayout.Space();

            if (Settings.useBulletdrop)
            {
                EditorGUI.indentLevel--;
                EditorGUILayout.LabelField("Zeroing:", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;

                EditorGUILayout.BeginHorizontal();
                showBarrelZero = EditorGUILayout.Foldout(showBarrelZero, "Barrel Zerodistances");
                if (GUILayout.Button("+"))
                {
                    TargetWeapon.BarrelZeroingDistances.Add(150);
                    showBarrelZero = true;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel++;
                if (showBarrelZero)
                {
                    for (int i = 0; i < TargetWeapon.BarrelZeroingDistances.Count; i++)
                    {
                        EditorGUILayout.BeginHorizontal();
                        TargetWeapon.BarrelZeroingDistances[i] = EditorGUILayout.FloatField("Zero distance " + i.ToString() + " :", TargetWeapon.BarrelZeroingDistances[i]);
                        if (GUILayout.Button("-"))
                        {
                            TargetWeapon.BarrelZeroingDistances.RemoveAt(i);
                            i--;
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                }
                EditorGUI.indentLevel -= 2;
                EditorGUILayout.Space();
            }
        }
        else
        {
            EditorGUILayout.LabelField("No Ballistic Settings found! Check 'Ballistic Settings Manager'");
        }
    }

    public static LayerMask LayerMaskField(string label, LayerMask selected)
    {
        return LayerMaskField(label, selected, true);
    }

    public static LayerMask LayerMaskField(string label, LayerMask selected, bool showSpecial)
    {

        List<string> layers = new List<string>();
        List<int> layerNumbers = new List<int>();

        string selectedLayers = "";

        for (int i = 0; i < 32; i++)
        {

            string layerName = LayerMask.LayerToName(i);

            if (layerName != "")
            {
                if (selected == (selected | (1 << i)))
                {

                    if (selectedLayers == "")
                    {
                        selectedLayers = layerName;
                    }
                    else
                    {
                        selectedLayers = "Mixed";
                    }
                }
            }
        }

        if (Event.current.type != EventType.MouseDown && Event.current.type != EventType.ExecuteCommand)
        {
            if (selected.value == 0)
            {
                layers.Add("Nothing");
            }
            else if (selected.value == -1)
            {
                layers.Add("Everything");
            }
            else
            {
                layers.Add(selectedLayers);
            }
            layerNumbers.Add(-1);
        }

        if (showSpecial)
        {
            layers.Add((selected.value == 0 ? "[X] " : "     ") + "Nothing");
            layerNumbers.Add(-2);

            layers.Add((selected.value == -1 ? "[X] " : "     ") + "Everything");
            layerNumbers.Add(-3);
        }

        for (int i = 0; i < 32; i++)
        {

            string layerName = LayerMask.LayerToName(i);

            if (layerName != "")
            {
                if (selected == (selected | (1 << i)))
                {
                    layers.Add("[X] " + layerName);
                }
                else
                {
                    layers.Add("     " + layerName);
                }
                layerNumbers.Add(i);
            }
        }

        bool preChange = GUI.changed;

        GUI.changed = false;

        int newSelected = 0;

        if (Event.current.type == EventType.MouseDown)
        {
            newSelected = -1;
        }

        newSelected = EditorGUILayout.Popup(label, newSelected, layers.ToArray(), EditorStyles.layerMaskField);

        if (GUI.changed && newSelected >= 0)
        {
            //newSelected -= 1;

            //Debug.Log(lastEvent + " " + newSelected + " " + layerNumbers[newSelected]);

            if (showSpecial && newSelected == 0)
            {
                selected = 0;
            }
            else if (showSpecial && newSelected == 1)
            {
                selected = -1;
            }
            else
            {

                if (selected == (selected | (1 << layerNumbers[newSelected])))
                {
                    selected &= ~(1 << layerNumbers[newSelected]);
                    //Debug.Log ("Set Layer "+LayerMask.LayerToName (LayerNumbers[newSelected]) + " To False "+selected.value);
                }
                else
                {
                    //Debug.Log ("Set Layer "+LayerMask.LayerToName (LayerNumbers[newSelected]) + " To True "+selected.value);
                    selected = selected | (1 << layerNumbers[newSelected]);
                }
            }
        }
        else
        {
            GUI.changed = preChange;
        }

        return selected;
    }
}
