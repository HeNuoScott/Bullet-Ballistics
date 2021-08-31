using UnityEngine;
using System.Collections;

namespace Ballistics
{
    public abstract class BallisticObject : MonoBehaviour
    {
        [HideInInspector]
        public int MatID;

        public bool isStatic;

        /// <summary>
        /// initializes bullet impact at a rayhit
        /// </summary>
        /// <param name="rayHit">Impact raycasthit</param>
        /// <param name="myBObj">material data of my materialID</param>
        public abstract void BulletImpact(RaycastHit rayHit, BallisticObjectData myBObj);
    }
}
