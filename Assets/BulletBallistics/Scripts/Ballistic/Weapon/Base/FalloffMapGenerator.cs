using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ballistics
{
    public class FalloffMapGenerator : MonoBehaviour
    {
        public Weapon TargetWeapon;
        public float ScopeDist = 0.5f;
        public Vector3 BarrelPos = new Vector3(0, -.4f, .7f);
        public int TextureSize = 1024;
        public float Size = 0.5f;

        public List<float> ZeroingDist = new List<float>();


        private Color lineColor = Color.blue;
        private Color ObjColor = Color.red;

        void OnDrawGizmos()
        {
            Gizmos.matrix = transform.localToWorldMatrix;

            // Draw symbols
            Gizmos.color = ObjColor;

            Vector3 ScopePos = new Vector3(0, 0, ScopeDist);

            //Scope
            Gizmos.DrawLine(ScopePos + new Vector3(0, Size, 0), ScopePos - new Vector3(0, Size, 0));
            Gizmos.DrawLine(ScopePos + new Vector3(Size, 0, 0), ScopePos - new Vector3(Size, 0, 0));

            //Eye
            Gizmos.color = lineColor;
            Gizmos.DrawLine(Vector3.zero, ScopePos);
            Gizmos.color = ObjColor;

            //Barrel
            Gizmos.color = lineColor;
            Gizmos.DrawLine(BarrelPos, ScopePos);
            Gizmos.color = ObjColor;
            Gizmos.DrawWireSphere(BarrelPos, Size / 6);
        }
    }
}