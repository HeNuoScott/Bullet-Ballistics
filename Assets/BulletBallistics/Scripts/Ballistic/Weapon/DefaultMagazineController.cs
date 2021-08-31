using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// 弹夹管理控制器
/// </summary>
public class DefaultMagazineController : MagazineController
{
    public int MagCount;//弹夹数量
    public int BulletsPerMag;//单个弹夹最大容量
    [HideInInspector]
    public int StoredBullets;//子弹储存量
    private int CurrentBulletAmount;//当前子弹数量
    public Action OnMagEmptie;//子弹为空时的事件

    void Awake()
    {
        StoredBullets = BulletsPerMag * MagCount;
        Reload();
    }

    public override bool IsBulletAvailable()
    {
        return CurrentBulletAmount > 0;
    }

    public bool Reload()
    {
        if (CurrentBulletAmount > 0)
        {
            //增加剩余弹药库存。
            StoredBullets += CurrentBulletAmount;
        }
        //把多余的弹药储存起来
        if (StoredBullets > BulletsPerMag)
        {
            StoredBullets -= BulletsPerMag;
            CurrentBulletAmount = BulletsPerMag;
        }
        else
        {
            if (StoredBullets <= 0) return false;
            else
            {
                CurrentBulletAmount += StoredBullets;
                StoredBullets = 0;
            }
        }
        return true;
    }

    public int GetBulletsInMag()
    {
        return CurrentBulletAmount;
    }

    /// <summary>
    /// called from the wheapon when the weapon shoots
    /// </summary>
    public void OnShoot()
    {
        CurrentBulletAmount -= 1;
        if (CurrentBulletAmount <= 0)
        {
            CurrentBulletAmount = 0;
        }
    }
}
