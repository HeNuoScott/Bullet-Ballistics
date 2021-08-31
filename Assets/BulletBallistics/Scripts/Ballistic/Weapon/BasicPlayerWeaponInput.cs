using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 武器的基本输入
/// </summary>
public class BasicPlayerWeaponInput : MonoBehaviour
{
    [Tooltip("行走时准心散开幅度")]// 行走时准心散开幅度
    public float WeaponSpreadWalking;

    public List<WeaponData> Weapons;
    public int currentWeapon = 0;
    private int weaponBefore = -1;

    private void Awake()
    {
        for (int i = 0; i < Weapons.Count; i++)
        {
            Weapons[i].weapon.OnShoot += OnShoot;
            ((DefaultMagazineController)Weapons[i].weapon.myMagazineController).OnMagEmptie += OnMagazineEmptie;
        }
    }

    private void Update()
    {
        if (currentWeapon != -1)
        {
            HandleWeaponInput();
        }
        SwitchWeapons();
    }

    /// <summary>
    /// 武器开火回调
    /// </summary>
    private void OnShoot()
    {
        //播放枪口火花特效
        if (Weapons[currentWeapon].particle != null)
        {
            Weapons[currentWeapon].particle.Simulate(0, true, true);
            ParticleSystem.EmissionModule module = Weapons[currentWeapon].particle.emission;
            module.enabled = true;
            Weapons[currentWeapon].particle.Play(true);
        }
        //播放声音特效
        if (Weapons[currentWeapon].sound != null)
        {
            Weapons[currentWeapon].sound.Play();
        }
    }

    /// <summary>
    /// 当弹夹子弹打完时调用
    /// </summary>
    private void OnMagazineEmptie()
    {
        Debug.Log("You need to press 'r' to reload!");
    }

    /// <summary>
    /// 武器输入调用
    /// </summary>
    private void HandleWeaponInput()
    {
        //Shoot
        if (Input.GetMouseButton(0))
        {
            Weapons[currentWeapon].weapon.Shoot();
        }
        if (Input.GetMouseButtonUp(0))
        {
            Weapons[currentWeapon].weapon.StopShoot();
        }

        //Aim
        if (Input.GetMouseButtonDown(1))
        {
            Weapons[currentWeapon].weapon.Aim(true);
        }
        if (Input.GetMouseButtonUp(1))
        {
            Weapons[currentWeapon].weapon.Aim(false);
        }

        //Reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload(currentWeapon));
        }
    }

    /// <summary>
    /// 等待武器装弹
    /// </summary>
    /// <param name="myCurrentW">current weapon id<param>
    /// <returns></returns>
    private IEnumerator Reload(int myCurrentW)
    {
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(Weapons[myCurrentW].ReloadTime);
        if (myCurrentW == currentWeapon)
        {
            ((DefaultMagazineController)Weapons[currentWeapon].weapon.myMagazineController).Reload();
        }
    }

    /// <summary>
    /// 切换武器
    /// </summary>
    private void SwitchWeapons()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            currentWeapon++;
            if (currentWeapon >= Weapons.Count)
            {
                currentWeapon = 0;
            }
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            currentWeapon--;
            if (currentWeapon < 0)
            {
                currentWeapon = Weapons.Count - 1;
            }
        }

        if (weaponBefore != currentWeapon)
        {
            for (int i = 0; i < Weapons.Count; i++)
            {
                Weapons[i].weapon.gameObject.SetActive(i == currentWeapon); //activate current Weapon
            }
            if (weaponBefore != -1)
            {
                Weapons[weaponBefore].weapon.StopShoot();
                Weapons[weaponBefore].weapon.Aim(false);
            }

        }

        weaponBefore = currentWeapon;
    }

    /// <summary>
    /// 触碰物体补充子弹
    /// </summary>
    /// <param name="col"></param>
    private void OnTriggerEnter(Collider col)
    {
        if (col.tag == "supply")
        {
            for (int i = 0; i < Weapons.Count; i++)
            {
                DefaultMagazineController defaultCont = ((DefaultMagazineController)Weapons[i].weapon.myMagazineController);
                defaultCont.StoredBullets = defaultCont.MagCount * defaultCont.BulletsPerMag;
            }
        }
    }
}