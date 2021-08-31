using UnityEngine;
using System.Collections;

namespace Ballistics
{
    public class InvokeDeactivate : MonoBehaviour
    {
        private PoolManager myPool;
        private GameObject InstanceOf;

        public void Deactivate(float t, GameObject _instanceOf)
        {
            InstanceOf = _instanceOf;
            Invoke("invokeCall", t);
        }

        protected virtual void invokeCall()
        {
            this.gameObject.SetActive(false);
            if (myPool == null)
            {
                myPool = PoolManager.Instance;
            }
            myPool.AddObject(InstanceOf, this.gameObject);
        }
    }
}