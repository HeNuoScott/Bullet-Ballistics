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

        private void Awake()
        {
            m_Trail = GetComponent<TrailRenderer>();
        }

        public void ReAwake()
        {
            if (m_Trail != null) m_Trail.Clear();
        }
    }
}
