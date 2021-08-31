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
    [HideInInspector]
    public int currentWeaponId = 0;
    [HideInInspector]
    public WeaponData currentWeaponData;
    private int weaponBefore = -1;

    private void Awake()
    {
        currentWeaponData = Weapons[currentWeaponId];
        for (int i = 0; i < Weapons.Count; i++)
        {
            Weapons[i].weapon.OnShoot += OnShoot;
            ((DefaultMagazineController)Weapons[i].weapon.myMagazineController).OnMagEmptie += OnMagazineEmptie;
        }
    }

    private void Update()
    {
        if (currentWeaponId != -1)
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
        if (currentWeaponData.particle != null)
        {
            currentWeaponData.particle.Simulate(0, true, true);
            ParticleSystem.EmissionModule module = currentWeaponData.particle.emission;
            module.enabled = true;
            currentWeaponData.particle.Play(true);
        }
        //播放声音特效
        if (currentWeaponData.sound != null)
        {
            currentWeaponData.sound.Play();
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
            currentWeaponData.weapon.Shoot();
        }
        if (Input.GetMouseButtonUp(0))
        {
            currentWeaponData.weapon.StopShoot();
        }

        //Aim
        if (Input.GetMouseButtonDown(1))
        {
            currentWeaponData.weapon.Aim(true);
        }
        if (Input.GetMouseButtonUp(1))
        {
            currentWeaponData.weapon.Aim(false);
        }

        //Reload
        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload());
        }
    }

    /// <summary>
    /// 等待武器装弹
    /// </summary>
    /// <param name="myCurrentW">current weapon id<param>
    /// <returns></returns>
    private IEnumerator Reload()
    {
        Debug.Log("Reloading...");
        if (currentWeaponData.weapon != null)
        {
            yield return new WaitForSeconds(currentWeaponData.ReloadTime);
            ((DefaultMagazineController)currentWeaponData.weapon.myMagazineController).Reload();
        }
    }

    /// <summary>
    /// 切换武器
    /// </summary>
    private void SwitchWeapons()
    {
        if (Input.mouseScrollDelta.y > 0)
        {
            currentWeaponId++;
            if (currentWeaponId >= Weapons.Count)
            {
                currentWeaponId = 0;
            }
            currentWeaponData = Weapons[currentWeaponId];
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            currentWeaponId--;
            if (currentWeaponId < 0)
            {
                currentWeaponId = Weapons.Count - 1;
            }
            currentWeaponData = Weapons[currentWeaponId];
        }

        if (weaponBefore != currentWeaponId)
        {
            for (int i = 0; i < Weapons.Count; i++)
            {
                Weapons[i].weapon.gameObject.SetActive(i == currentWeaponId); //activate current Weapon
            }
            if (weaponBefore != -1)
            {
                Weapons[weaponBefore].weapon.StopShoot();
                Weapons[weaponBefore].weapon.Aim(false);
            }

        }

        weaponBefore = currentWeaponId;
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