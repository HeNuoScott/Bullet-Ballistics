using UnityEngine;
using System.Collections;

namespace Ballistics
{
    /// <summary>
    /// inherit from ImpactObject to set up impact particles etc. ( also check DefaultImpactObject )
    /// </summary>
    public abstract class ImpactObject : MonoBehaviour
    {
        /// <summary>
        /// called when this object awakes at an impact point
        /// </summary>
        /// <param name="data">material data of the hit object</param>
        /// <param name="rayHit">RaycastHit of the impact</param>
        public abstract void Hit(BallisticObjectData data, RaycastHit rayHit);
    }
}
