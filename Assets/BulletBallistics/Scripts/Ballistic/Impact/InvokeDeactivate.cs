using UnityEngine;
using System.Collections;

namespace Ballistics
{
    /// <summary>
    /// 延迟回收
    /// </summary>
    public class InvokeDeactivate : MonoBehaviour
    {
        private BulletPoolManager myPool;
        private GameObject myOwner;

        public void Deactivate(float t, GameObject owner)
        {
            myOwner = owner;
            Invoke("invokeCall", t);
        }

        protected virtual void invokeCall()
        {
            this.gameObject.SetActive(false);
            if (myPool == null) myPool = BulletPoolManager.Instance;
            myPool.AddObject(myOwner, this.gameObject);
        }
    }
}