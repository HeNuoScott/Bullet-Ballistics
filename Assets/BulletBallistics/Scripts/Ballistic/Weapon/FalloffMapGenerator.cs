using System.Collections.Generic;
using UnityEngine;

namespace Ballistics
{
    /// <summary>
    /// 瞄准镜十字交叉刻度图 生成器
    /// </summary>
    public class FalloffMapGenerator : MonoBehaviour
    {
        public Weapon TargetWeapon;//武器
        public float ScopeDist = 0.5f;//两个瞄准点之间的距离
        public float AimDist = 100f;//两个瞄准点之间的距离
        public Vector3 BarrelPos = new Vector3(0, -.4f, .7f);//枪管的位置
        public int TextureSize = 1024;
        public float Size = 0.5f;

        public List<float> ZeroingDist = new List<float>();
        private Color lineColor = Color.blue;
        private Color ObjColor = Color.red;

        private void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;
            // Draw symbols
            Gizmos.color = ObjColor;

            Vector3 ScopePos = new Vector3(0, 0, ScopeDist);

            //Scope
            Gizmos.DrawLine(ScopePos + new Vector3(0, Size, 0), ScopePos - new Vector3(0, Size, 0));
            Gizmos.DrawLine(ScopePos + new Vector3(Size, 0, 0), ScopePos - new Vector3(Size, 0, 0));

            //// Eye
            //Gizmos.color = lineColor;
            //Gizmos.DrawLine(Vector3.zero, ScopePos);
            //Gizmos.color = ObjColor;

            //// Barrel
            //Gizmos.color = lineColor;
            //Gizmos.DrawLine(BarrelPos, ScopePos);
            //Gizmos.color = ObjColor;
            //Gizmos.DrawWireSphere(BarrelPos, Size / 6);

            // Aim Line
            Gizmos.color = Color.green;
            Gizmos.DrawLine(Vector3.zero, new Vector3(0, 0, AimDist));
        }
    }
}