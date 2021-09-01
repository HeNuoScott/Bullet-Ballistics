﻿using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ballistics
{
    [CustomEditor(typeof(Weapon))]
    public class WeaponEditor : Editor
    {
        Weapon TargetWeapon;

        private bool showBarrelZero;

        void OnEnable()
        {
            TargetWeapon = (Weapon)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Weapon Editor", EditorStyles.largeLabel);
            EditorGUILayout.Separator();

            EditorGUILayout.LabelField("通用设置", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            TargetWeapon.VisualSpawnPoint = (Transform)EditorGUILayout.ObjectField("枪口特效位置", TargetWeapon.VisualSpawnPoint, typeof(Transform), true);
            TargetWeapon.PhysicalBulletSpawnPoint = (Transform)EditorGUILayout.ObjectField("子弹生成位置", TargetWeapon.PhysicalBulletSpawnPoint, typeof(Transform), true);
            TargetWeapon.ScopePos = (Transform)EditorGUILayout.ObjectField("狙击位置", TargetWeapon.ScopePos, typeof(Transform), true);
            TargetWeapon.LifeTimeOfBullets = EditorGUILayout.FloatField("子弹存留时间", TargetWeapon.LifeTimeOfBullets);
            TargetWeapon.MuzzleDamage = EditorGUILayout.FloatField("枪口伤害", TargetWeapon.MuzzleDamage);
            TargetWeapon.HitMask = LayerMaskField("碰撞层级", TargetWeapon.HitMask);

            EditorGUILayout.Space();
            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("子弹设置", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            TargetWeapon.BulletPref = (Transform)EditorGUILayout.ObjectField("子弹预制体", TargetWeapon.BulletPref, typeof(Transform), true);
            TargetWeapon.MaxBulletSpeed = EditorGUILayout.FloatField("子弹初速度", TargetWeapon.MaxBulletSpeed);
            TargetWeapon.randomSpeedOffset = EditorGUILayout.FloatField("初速度偏差", TargetWeapon.randomSpeedOffset);
            TargetWeapon.BulletMass = EditorGUILayout.FloatField("子弹质量", TargetWeapon.BulletMass);

            EditorGUI.indentLevel++;
            EditorGUILayout.LabelField("最大动能:\t " + (0.5f * TargetWeapon.BulletMass * TargetWeapon.MaxBulletSpeed * TargetWeapon.MaxBulletSpeed).ToString() + " J", EditorStyles.miniLabel);
            EditorGUI.indentLevel--;

            TargetWeapon.DragCoefficient = EditorGUILayout.FloatField("阻力系数", TargetWeapon.DragCoefficient);
            TargetWeapon.Diameter = EditorGUILayout.FloatField("子弹直径", TargetWeapon.Diameter);

            EditorGUILayout.Space();

            EditorGUI.indentLevel--;
            EditorGUILayout.LabelField("瞄准归零点", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;
            TargetWeapon.currentBarrelZero = EditorGUILayout.IntField("当前选中的归零点", TargetWeapon.currentBarrelZero);
            EditorGUILayout.BeginHorizontal();
            showBarrelZero = EditorGUILayout.Foldout(showBarrelZero, "瞄准距离");
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
                    TargetWeapon.BarrelZeroingDistances[i] = EditorGUILayout.FloatField("归零点距离 " + i.ToString() + " :", TargetWeapon.BarrelZeroingDistances[i]);
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
}