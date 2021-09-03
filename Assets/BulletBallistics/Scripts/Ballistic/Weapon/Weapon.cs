using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ballistics
{
    public class Weapon : MonoBehaviour
    {
        /// <summary>
        /// 子弹可视化生成点(枪口)
        /// </summary>
        public Transform VisualSpawnPoint;
        public Transform AimStartPoint;
        public Transform AimEndPoint;
        /// <summary>
        /// 子弹存留时间
        /// </summary>
        public float LifeTimeOfBullets = 6;
        /// <summary>
        /// 碰撞层
        /// </summary>
        public LayerMask HitMask = new LayerMask();
        /// <summary>
        /// 初始化伤害
        /// </summary>
        public float MuzzleDamage = 80;
        public float AimDistOffset = 5;
        //Bullet
        /// <summary>
        /// 初始化速度
        /// </summary>
        public float MaxBulletSpeed = 550;
        /// <summary>
        /// 子弹质量
        /// </summary>
        public float BulletMass = 0.0065f;
        /// <summary>
        /// 子弹口径
        /// </summary>
        public float Diameter = 0.01f;
        /// <summary>
        /// 阻力系数
        /// </summary>
        public float DragCoefficient = 0.4f;
        /// <summary>
        /// 子弹预制体
        /// </summary>
        public Transform BulletPrefab;
        //Zeroing
        /// <summary>
        /// 瞄准镜提供的档次 500m/1000m/1500m
        /// </summary>
        public List<float> BarrelZeroingDistances = new List<float>();
        /// <summary>
        /// 每个距离瞄准点对应的 矫正角度
        /// </summary>
        public List<float> BarrelZeroingCorrections = new List<float>();
        /// <summary>
        /// 当前的瞄准点档次 -1时没瞄准
        /// </summary>
        public int currentBarrelZero = -1; //-1 equals no Correction
        public int BarrelZeroCount { get { return BarrelZeroingCorrections.Count; } }

        private BulletHandler bulletHandler;
        private BulletPoolManager myPool;
        private BallisticsSettings settings;

        //储存预先就计算的阻力以节省性能
        public float PreDrag;
        private float area;

        private void Start()
        {
            settings = BallisticsSettings.Instance;
            myPool = BulletPoolManager.Instance;
            bulletHandler = BulletHandler.Instance;
            if (bulletHandler == null) return;
            RecalculatePrecalculatedValues();
        }

        private void OnDrawGizmos()
        {
            if (BallisticsSettings.Instance.IsDynamicEditor)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawLine(AimStartPoint.position, AimStartPoint.position + AimStartPoint.forward * 1000f);
            }
        }
        /// <summary>
        /// 计算基本不变的值
        /// </summary>
        public void RecalculatePrecalculatedValues()
        {
            area = Mathf.Pow(Diameter / 2, 2) * Mathf.PI;
            PreDrag = (0.5f * settings.AirDensity * area * DragCoefficient) / BulletMass;

            if (BarrelZeroingDistances.Count > 0)
            {
                BarrelZeroingDistances.Sort();
                CalculateBarrelZeroCorrections();
            }
        }

        /// <summary>
        /// 计算每个瞄准档次的矫正角度
        /// </summary>
        public void CalculateBarrelZeroCorrections()
        {
            BarrelZeroingCorrections.Clear();
            for (int i = 0; i < BarrelZeroingDistances.Count; i++)
            {
                float FlightDistance = BarrelZeroingDistances[i] + AimDistOffset;
                float FlightTime;
                float drop;

                if (settings.UseBulletdrag)
                {
                    float k = (settings.AirDensity * DragCoefficient * Mathf.PI * (Diameter * 0.5f) * (Diameter * 0.5f)) / (2 * BulletMass);
                    FlightTime = (Mathf.Exp(k * FlightDistance) - 1) / (k * MaxBulletSpeed);
                }
                else
                {
                    FlightTime = FlightDistance / MaxBulletSpeed;
                }

                drop = (0.5f * -Physics.gravity.y * Mathf.Pow(FlightTime, 2));

                //scope height above barrel
                float result = 360f - Mathf.Atan(drop / FlightDistance) * Mathf.Rad2Deg;

                BarrelZeroingCorrections.Add(result);
            }
        }

        /// <summary>
        /// 实例化子弹 并将子弹交给BulletHandler计算
        /// </summary>
        /// <param name="ShootDirection">子弹发射的方向(通常你想要使用‘PhysicalBulletSpawnPoint’。 向前，这个方向)  </param>
        public void ShootBullet()
        {
            Transform bClone = null;
            if (BulletPrefab != null)
            {
                GameObject cGO = myPool.GetNextGameObject(BulletPrefab.gameObject);
                if (cGO == null)
                {
                    bClone = (Transform)Instantiate(BulletPrefab, VisualSpawnPoint.position, Quaternion.identity);
                    bClone.SetParent(myPool.transform);
                }
                else
                {
                    bClone = cGO.transform;
                    bClone.position = VisualSpawnPoint.position;
                }
                bClone.gameObject.SetActive(true);
            }
            if (settings.IsDynamicEditor) CalculateBarrelZeroCorrections();
            //calculte in zeroing corrections
            //Vector3 dir = (currentBarrelZero != -1 ? Quaternion.AngleAxis(BarrelZeroingCorrections[currentBarrelZero], AimStartPoint.right) * AimStartPoint.forward : AimStartPoint.forward);
            Vector3 dir = (currentBarrelZero != -1 ? Quaternion.AngleAxis(BarrelZeroingCorrections[currentBarrelZero], Vector3.right) * AimStartPoint.forward : AimStartPoint.forward);
            //bulletHandler.Bullets.Enqueue(new BulletData(this, VisualSpawnPoint.position, Vector3.zero, dir, LifeTimeOfBullets, MaxBulletSpeed, bClone));
            bulletHandler.Bullets.Enqueue(new BulletData(this, AimStartPoint.position, VisualSpawnPoint.position - AimStartPoint.position, dir, LifeTimeOfBullets, MaxBulletSpeed, bClone));
        }

        /// <summary>
        /// sets currentBarrelZero to zeroID
        /// </summary>
        /// <param name="zeroID">distance id from the BarrelZeroingDistances list</param>
        /// <returns>found zeroing id</returns>
        public bool SetZeroingTo(int zeroID)
        {
            if (zeroID < BarrelZeroCount)
            {
                currentBarrelZero = zeroID;
                return true;
            }
            return false;
        }

        /// <summary>
        /// sets currentBarrelZero to the id that equals the distance
        /// </summary>
        /// <param name="distance">distance to zero the weapon to</param>
        /// <returns>found distance in BarrelZeroingDistances list</returns>
        public bool SetZeroingTo(float distance)
        {
            if (BarrelZeroingDistances.Contains(distance))
            {
                currentBarrelZero = BarrelZeroingDistances.IndexOf(distance);
                return true;
            }
            return false;
        }
    }
}