using UnityEngine;
using System;

namespace Ballistics
{
    /// <summary>
    /// 弹夹控制器
    /// </summary>
    public class MagazineController : MonoBehaviour
    {
        public int StoredBullets;//子弹储存量
        public int BulletsPerMag;//单个弹夹最大容量
        public int CurrentBulletAmount;//当前子弹数量
        public event Action OnMagEmptie;//子弹为空时的事件

        /// <summary>
        /// 是否有弹药 可以射击
        /// </summary>
        public virtual bool IsBulletAvailable()
        {
            return CurrentBulletAmount > 0;
        }

        public virtual void OnShoot()
        {
            CurrentBulletAmount -= 1;
            if (CurrentBulletAmount <= 0)
            {
                CurrentBulletAmount = 0;
                OnMagEmptie?.Invoke();
            }
        }

        public virtual bool Reload()
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
    }
}