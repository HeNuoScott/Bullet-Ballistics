// -----------------------------------------------
// Copyright © Sirius. All rights reserved.
// CreateTime: 2021/9/2   19:45:35
// -----------------------------------------------
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ballistics
{
    public class VRWeaponInput : MonoBehaviour
    {
        public WeaponController CurrentWeapon;
        private bool isFireing = false;
        private void Awake()
        {
            CurrentWeapon.OnShoot += OnShoot;
            CurrentWeapon.myMagazineController.OnMagEmptie += OnMagazineEmptie;
        }

        private void Update()
        {
            //Shoot
            if (OVRInput.GetDown(OVRInput.RawButton.RIndexTrigger))
            {
                isFireing = true;
            }
            if (isFireing) CurrentWeapon.Shoot();

            if (OVRInput.GetUp(OVRInput.RawButton.RIndexTrigger))
            {
                isFireing = false;
                CurrentWeapon.StopShoot();
            }

            //Reload
            if (Input.GetKeyDown(KeyCode.R))
            {
                StartCoroutine(Reload());
            }
        }

        /// <summary>
        /// 武器开火回调
        /// </summary>
        private void OnShoot()
        {
            //播放枪口火花特效
            if (CurrentWeapon.particle != null)
            {
                CurrentWeapon.particle.Simulate(0, true, true);
                ParticleSystem.EmissionModule module = CurrentWeapon.particle.emission;
                module.enabled = true;
                CurrentWeapon.particle.Play(true);
            }
            //播放声音特效
            if (CurrentWeapon.sound != null)
            {
                CurrentWeapon.sound.Play();
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
        /// 等待武器装弹
        /// </summary>
        /// <param name="myCurrentW">current weapon id<param>
        /// <returns></returns>
        private IEnumerator Reload()
        {
            Debug.Log("Reloading...");
            if (CurrentWeapon != null)
            {
                yield return new WaitForSeconds(CurrentWeapon.ReloadTime);
                CurrentWeapon.myMagazineController.Reload();
            }
        }

        /// <summary>
        /// 触碰物体补充子弹
        /// </summary>
        /// <param name="col"></param>
        private void OnTriggerEnter(Collider col)
        {
            if (col.tag == "supply")
            {
                CurrentWeapon.myMagazineController.StoredBullets = 1000;
            }
        }
    }
}