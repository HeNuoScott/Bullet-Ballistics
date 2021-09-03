// -----------------------------------------------
// Copyright © Sirius. All rights reserved.
// CreateTime: 2021/9/3   9:26:39
// -----------------------------------------------

using UnityEditor;
using UnityEngine;
using System.IO;

namespace Ballistics
{
    public class BallisticsSettings : ScriptableObject
    {
        const string path = "Assets/Resources/";
        const string fullPath = "Assets/Resources/BallisticsSetting.asset";
        private static BallisticsSettings instance = null;
        public static BallisticsSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load(fullPath) as BallisticsSettings;
                }

                if (instance == null)
                {
#if UNITY_EDITOR
                    instance = ScriptableObject.CreateInstance<BallisticsSettings>();
                    instance.name = "BallisticsSetting";
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    AssetDatabase.CreateAsset(instance, fullPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
#else
                    Debug.LogError("缺少 BallisticsSetting.asset 资源");
#endif
                }

                return instance;
            }
        }

        /// <summary> 子弹是否坠落 </summary>
        public bool UseBulletdrop = true;
        /// <summary> 子弹是否收到阻力 </summary>
        public bool UseBulletdrag = true;
        /// <summary> 子弹是否计算弹道材质 </summary>
        public bool UseBallisticMaterials = true;
        /// <summary> 子弹是否散射 </summary>
        public bool UseSpreadMaterials = false;
        /// <summary> 编辑器动态调节 </summary>
        public bool IsDynamicEditor = false;
        /// <summary> 空气密度 </summary>
        public float AirDensity = 1f;
        /// <summary> 每帧计算子弹的最大数量 </summary>
        public int MaxBulletUpdatesPerFrame = 500;
        /// <summary> 子弹可视化到计算出的虚拟子弹所需的时间  </summary>
        public float VisualBulletToRealBulletMovementTime = 0.01f;
        /// <summary> 风向 </summary>
        public Vector3 WindDirection = new Vector3(0, 0, 0);
    }
}