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
        private GameObject InstanceOf;

        public void Deactivate(float t, GameObject _instanceOf)
        {
            InstanceOf = _instanceOf;
            Invoke("invokeCall", t);
        }

        protected virtual void invokeCall()
        {
            this.gameObject.SetActive(false);
            if (myPool == null) myPool = BulletPoolManager.Instance;
            myPool.AddObject(InstanceOf, this.gameObject);
        }
    }
}