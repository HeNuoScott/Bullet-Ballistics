using UnityEngine;

namespace Ballistics
{
    [System.Serializable]
    public struct BallisticObjectData
    {
        /// <summary>
        /// 材质名称
        /// </summary>
        public string Name;

        /// <summary>
        /// 子弹穿过一单位这种材料时的能量损失 
        /// </summary>
        public float EnergylossPerUnit;

        /// <summary>
        /// 子弹穿过这片材料时随机扩散  
        /// </summary>
        public float RndSpread;

        /// <summary>
        /// 子弹被这种材料反射时随机扩散
        /// </summary>
        public float RndSpreadRic;

        /// <summary>
        /// curve describes from x: 0 - 1 ( -> 0° - 90° impacat angle)
        /// the propability y: 0 - 1 ( -> 0% - 100% ) that the bullet becomes a ricochet when it hits this material
        /// 曲线描述从x: 0 - 1(-> 0°- 90°冲击角)  
        /// 当子弹击中这种材料时弹开的概率y: 0 - 1 (-> 0% - 100%)  
        /// </summary>
        public AnimationCurve RicochetPropability;

        /// <summary>
        /// 材料对应的弹痕
        /// </summary>
        public ImpactObject impactObject;
    }
}