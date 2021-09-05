using System.Collections;
using UnityEngine;
using System;

namespace Ballistics
{
    /// <summary>
    /// 武器控制器
    /// </summary>
    public class WeaponController : MonoBehaviour
    {
        public Weapon TargetWeapon;

        public AudioSource sound;
        public ParticleSystem particle;
        /// <summary>
        /// 瞄准位置
        /// </summary>
        public Transform ScopePos;
        /// <summary>
        /// Time to go from start- to max-spread
        /// 从静止到 上翘最大值所需时间
        /// </summary>
        public float AttackTime;
        /// <summary>
        /// Time until spread release
        /// 枪口在上翘最高点停留时间
        /// </summary>
        public float HoldTime;
        /// <summary>
        /// Time to go from max- to start-spread
        /// 从上翘最大值 恢复静止所需时间
        /// </summary>
        public float ReleaseTime;
        /// <summary>
        /// 装弹时间
        /// </summary>
        public float ReloadTime;
        /// <summary>
        /// 后坐力
        /// </summary>
        public float RecoilAmount;
        private float currentSpread;
        private float shootTimer = 0;
        public float ShootDelay = 0.25f;
        public ShootingType WeaponType = ShootingType.SingleShot;

        /// <summary>
        /// 单次射击数量
        /// </summary>
        public int BulletsPerShot = 3;
        /// <summary>
        /// 散射时 子弹与子弹之间的角度
        /// </summary>
        public float BurstSpreadAngle = 5;
        /// <summary>
        /// 连发之间间隔
        /// </summary>
        public float SalveBulletShootDelay = 0.04f;

        public MagazineController myMagazineController;

        private bool shootReset = true;
        private int SalvesBulletCounter;
        private float CooldownTimer = 0;
        public bool isAiming;

        public event Action OnShoot;

        private void Update()
        {
            CooldownTimer -= Time.deltaTime;
            currentSpread = Mathf.Clamp01(shootTimer > 0 ? currentSpread + Time.deltaTime / AttackTime : currentSpread - Time.deltaTime / ReleaseTime);
            shootTimer -= Time.deltaTime;
        }

        /// <summary>
        /// 武器正在瞄准
        /// </summary>
        public void Aim(bool active)
        {
            isAiming = active;
        }

        /// <summary>
        /// Fire the gun. Call this every frame the trigger is held down.
        /// </summary>
        public void Shoot()
        {
            if (myMagazineController.IsBulletAvailable() && CooldownTimer <= 0)
            {
                Vector3 rotation = Vector3.zero;
                switch (WeaponType)
                {
                    case ShootingType.Auto://自动模式
                        TargetWeapon.ShootBullet(rotation);
                        CallOnShoot();
                        break;
                    case ShootingType.Burst://单次多发(散弹枪)
                        if (shootReset)
                        {
                            int max = BulletsPerShot / 2;
                            int min = -max;
                            
                            for (int i = 0; i < BulletsPerShot; i++)
                            {
                                rotation.x = (rotation.x + UnityEngine.Random.Range(-BurstSpreadAngle, BurstSpreadAngle)) % 360;
                                rotation.y = (rotation.y + UnityEngine.Random.Range(-BurstSpreadAngle, BurstSpreadAngle)) % 360;
                                rotation = rotation * 0.01f;
                                TargetWeapon.ShootBullet(rotation);
                            }
                            CallOnShoot();
                            shootReset = false;
                        }
                        break;
                    case ShootingType.Salves://单次连发

                        if (shootReset)
                        {
                            TargetWeapon.ShootBullet(rotation);
                            SalvesBulletCounter++;
                            CallOnShoot();
                            shootReset = false;
                            StartCoroutine(ShootSalves());
                        }
                        break;
                    case ShootingType.SingleShot://单发
                        if (shootReset)
                        {
                            TargetWeapon.ShootBullet(rotation);
                            CallOnShoot();
                            shootReset = false;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// 单次连发
        /// </summary>
        private IEnumerator ShootSalves()
        {
            while (SalvesBulletCounter < BulletsPerShot)
            {
                yield return new WaitForSeconds(SalveBulletShootDelay);
                TargetWeapon.ShootBullet(Vector3.zero);
                SalvesBulletCounter++;
                CallOnShoot();
                if (SalvesBulletCounter >= BulletsPerShot)
                {
                    CooldownTimer = ShootDelay;
                    SalvesBulletCounter = 0;
                    break;
                }
            }
        }

        private void CallOnShoot()
        {
            CooldownTimer = ShootDelay;
            myMagazineController.OnShoot();
            shootTimer = HoldTime;
            OnShoot?.Invoke();
        }

        public void StopShoot()
        {
            shootReset = true;
        }

        /// <summary>
        /// 后坐力矫正时间
        /// </summary>
        /// <returns></returns>
        public float RecoilCorrectionTime()
        {
            return ReleaseTime;
        }
    }

    public enum ShootingType
    {
        SingleShot,
        Salves,
        Auto,
        Burst
    }
}