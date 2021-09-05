// -----------------------------------------------
// Copyright © Sirius. All rights reserved.
// CreateTime: 2021/9/2   16:15:1
// -----------------------------------------------
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Ballistics
{
    [CustomEditor(typeof(BulletPoolManager))]
    public class BulletPoolManagerEditor : Editor
    {
        BulletPoolManager bulletPool;
        private bool addMaterialing = false;
        private MaterialObjectType AddType = MaterialObjectType.Stone;
        private ImpactObject AddImpact = null;

        void OnEnable()
        {
            bulletPool = (BulletPoolManager)target;
            addMaterialing = false;
        }
        public override void OnInspectorGUI()
        {
            EditorGUILayout.Separator();

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("弹痕储存器", EditorStyles.largeLabel);
 
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < bulletPool.ImpactTypes.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(bulletPool.ImpactTypes[i].ToString(), EditorStyles.largeLabel);
                //EditorGUILayout.EnumFlagsField(bulletPool.ImpactTypes[i]);
                ImpactObject impactValue = (ImpactObject)EditorGUILayout.ObjectField(bulletPool.ImpactObjects[i], typeof(ImpactObject), true);
                if (impactValue != bulletPool.ImpactObjects[i])
                {
                    bulletPool.ImpactObjects[i] = impactValue;
                }
                if (GUILayout.Button("-"))
                {
                    bulletPool.ImpactTypes.RemoveAt(i);
                    bulletPool.ImpactObjects.RemoveAt(i);
                    i--;
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (!addMaterialing)
            {
                if (GUILayout.Button("添加一个弹痕类型"))
                {
                    addMaterialing = true;
                    AddImpact = null;
                }
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                AddType = (MaterialObjectType)EditorGUILayout.EnumPopup(AddType);
                AddImpact = (ImpactObject)EditorGUILayout.ObjectField(AddImpact, typeof(ImpactObject), true);
                if (GUILayout.Button("确定添加"))
                {
                    addMaterialing = false;
                    if (bulletPool.ImpactTypes.Contains(AddType))
                    {
                        Debug.LogError("这个材质类型已经存在!!");
                    }
                    else
                    {
                        bulletPool.ImpactTypes.Add(AddType);
                        bulletPool.ImpactObjects.Add(AddImpact);
                    }
                }
                if (GUILayout.Button("取消添加"))
                {
                    addMaterialing = false;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (GUI.changed) EditorUtility.SetDirty(bulletPool);
        }

    }
}