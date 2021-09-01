using System.Collections;
using UnityEngine;

namespace Ballistics
{
    /// <summary>
    /// 武器的基本输入
    /// </summary>
    public class BasicPlayerWeaponInput : MonoBehaviour
    {
        public WeaponController CurrentWeapon;

        private void Awake()
        {
            CurrentWeapon.OnShoot += OnShoot;
            CurrentWeapon.myMagazineController.OnMagEmptie += OnMagazineEmptie;
        }

        private void Update()
        {
            //Shoot
            if (Input.GetMouseButton(0))
            {
                CurrentWeapon.Shoot();
            }
            if (Input.GetMouseButtonUp(0))
            {
                CurrentWeapon.StopShoot();
            }

            //Aim
            if (Input.GetMouseButtonDown(1))
            {
                CurrentWeapon.Aim(true);
            }
            if (Input.GetMouseButtonUp(1))
            {
                CurrentWeapon.Aim(false);
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