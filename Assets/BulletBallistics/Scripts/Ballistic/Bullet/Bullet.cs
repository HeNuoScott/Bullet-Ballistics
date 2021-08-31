using UnityEngine;
using System.Collections;

namespace Ballistics
{
    /// <summary>
    /// 子弹的行为
    /// </summary>
    public class Bullet : MonoBehaviour, IPoolingObject
    {
        private TrailRenderer m_Trail;//拖尾
#if UNITY_EDITOR
        private BulletDebugger m_BulletDebugger;
#endif

        void Awake()
        {
            m_Trail = GetComponent<TrailRenderer>();
#if UNITY_EDITOR
            m_BulletDebugger = GetComponent<BulletDebugger>();
#endif
        }

        public void ReAwake()
        {
            if (m_Trail != null) m_Trail.Clear();
#if UNITY_EDITOR
            if (m_BulletDebugger != null) m_BulletDebugger.Clear();
#endif
        }
    }
}
