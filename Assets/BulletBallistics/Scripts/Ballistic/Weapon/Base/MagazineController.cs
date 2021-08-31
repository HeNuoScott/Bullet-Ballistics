using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 弹药控制器
/// </summary>
public abstract class MagazineController : MonoBehaviour
{
    /// <summary>
    /// 是否有弹药 可以射击
    /// </summary>
    /// <returns></returns>
	public abstract bool IsBulletAvailable();
}
