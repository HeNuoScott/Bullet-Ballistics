using UnityEngine;
using System.Collections;

/// <summary>
/// 发射控制器
/// </summary>
public abstract class SpreadController : MonoBehaviour
{
    /// <summary>
    /// 获取当前发射方向
    /// </summary>
    public abstract Vector3 GetCurrentSpread(Transform spawn);

    /// <summary>
    /// 获取弹射角度
    /// </summary>
    public abstract float GetSpreadAngle();


    /// <summary>
    /// 武器开火回调
    /// </summary>
    public abstract void OnShoot();
}
