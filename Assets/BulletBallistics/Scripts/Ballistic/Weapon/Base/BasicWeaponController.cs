using UnityEngine;
using System.Collections;
using System;
using Ballistics;

/// <summary>
/// 武器控制器
/// </summary>
public class BasicWeaponController : MonoBehaviour
{
    //General

    public Weapon TargetWeapon;
    public float ShootDelay = 0.25f;
    public ShootingType WeaponType = ShootingType.SingleShot;


    //Savles / Burst

    /// <summary>
    /// amount of bullets per shot in salve- / burst- mode
    /// </summary>
    public int BulletsPerShot = 3;

    /// <summary>
    /// Additional random spread to each bullet in a burst
    /// </summary>
    public float BurstSpreadAngle = 5;
    //--

    //Salves
    /// <summary>
    /// delay between shots in a salve
    /// </summary>
    public float SalveBulletShootDelay = 0.04f;
    //--

    //Spread
    public SpreadController mySpreadController;
    //--

    //MagazineController
    public MagazineController myMagazineController;
    //--

    //private variables
    private bool shootReset = true;
    private int SalvesBulletCounter;
    private float CooldownTimer = 0;

    //--

    public bool isAiming;

    public Action OnShoot;

    private void Update()
    {
        CooldownTimer -= Time.deltaTime;
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
            switch (WeaponType)
            {
                case ShootingType.Auto://自动模式
                    TargetWeapon.ShootBullet(mySpreadController.GetCurrentSpread(TargetWeapon.PhysicalBulletSpawnPoint));
                    CallOnShoot();
                    break;
                case ShootingType.Burst://单次多发(散弹枪)
                    if (shootReset)
                    {
                        for (int i = 0; i < BulletsPerShot; i++)
                        {
                            TargetWeapon.ShootBullet(mySpreadController.GetCurrentSpread(TargetWeapon.PhysicalBulletSpawnPoint));
                        }
                        CallOnShoot();
                        shootReset = false;
                    }
                    break;
                case ShootingType.Salves://单次连发

                    if (shootReset)
                    {
                        TargetWeapon.ShootBullet(mySpreadController.GetCurrentSpread(TargetWeapon.PhysicalBulletSpawnPoint));
                        SalvesBulletCounter++;
                        CallOnShoot();
                        shootReset = false;
                        StartCoroutine(ShootSalves());
                    }
                    break;
                case ShootingType.SingleShot://单发
                    if (shootReset)
                    {
                        TargetWeapon.ShootBullet(mySpreadController.GetCurrentSpread(TargetWeapon.PhysicalBulletSpawnPoint));
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
            TargetWeapon.ShootBullet(mySpreadController.GetCurrentSpread(TargetWeapon.PhysicalBulletSpawnPoint));
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
        ((DefaultMagazineController)myMagazineController).OnShoot();
        mySpreadController.OnShoot();
        OnShoot?.Invoke();
    }

    public void StopShoot()
    {
        shootReset = true;
    }
}

public enum ShootingType
{
    SingleShot,
    Salves,
    Auto,
    Burst
}
