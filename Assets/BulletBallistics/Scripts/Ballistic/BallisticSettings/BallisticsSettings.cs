using System.Collections.Generic;
using UnityEngine;

namespace Ballistics
{
    public class BallisticsSettings : ScriptableObject
    {
        /// <summary> 子弹是否坠落 </summary>
        public bool useBulletdrop = true;
        /// <summary> 子弹是否收到阻力 </summary>
        public bool useBulletdrag = true;
        /// <summary> 子弹是否计算弹道材质 </summary>
        public bool useBallisticMaterials = true;
        /// <summary> 子弹是否散射 </summary>
        public bool useSpreadMaterials = true;

        /// <summary> 风向 </summary>
        public Vector3 WindDirection;

        /// <summary> 空气密度 </summary>
        public float AirDensity;

        //BallisticObjectData
        public List<BallisticObjectData> MaterialData = new List<BallisticObjectData>();
    }
}