using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR
/// <summary>
/// 子弹的轨迹追踪器。 只有在UnityEditor中运行
/// </summary>
public class BulletDebugger : MonoBehaviour
{
    private Transform m_SelfTransform;
    private List<Vector3> m_PoseList = new List<Vector3>();
    public Color LineColor;
    public int MaxFrames;//显示帧数

    void Awake()
    {
        m_SelfTransform = this.transform;
        m_PoseList.Add(m_SelfTransform.position);
    }

    void Update()
    {
        for (int i = 1; i < m_PoseList.Count; i++)
        {
            Debug.DrawLine(m_PoseList[i - 1], m_PoseList[i], LineColor);
        }
        if (m_PoseList.Count > MaxFrames)
        {
            m_PoseList.RemoveRange(0, m_PoseList.Count - MaxFrames);
        }
    }

    public void AddPos(Vector3 pos)
    {
        m_PoseList.Add(pos);
    }

    public void Clear()
    {
        m_PoseList.Clear();
    }
}
#endif
