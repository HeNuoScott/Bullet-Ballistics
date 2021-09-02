using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Ballistics
{
    public class BulletHandler : MonoBehaviour
    {
        /// <summary> 子弹是否坠落 </summary>
        public static bool UseBulletdrop = true;
        /// <summary> 子弹是否收到阻力 </summary>
        public static bool UseBulletdrag = true;
        /// <summary> 子弹是否计算弹道材质 </summary>
        public static bool UseBallisticMaterials = true;
        /// <summary> 子弹是否散射 </summary>
        public static bool UseSpreadMaterials = false;
        /// <summary> 空气密度 </summary>
        public static float AirDensity = 1f;
        /// <summary> 风向 </summary>
        public static Vector3 WindDirection = new Vector3(0,0,0);
        /// <summary> 编辑器动态调节 </summary>
        public static bool IsDynamicEditor = false;

        [HideInInspector]
        public Queue<BulletData> Bullets = new Queue<BulletData>();

        /// <summary>
        /// 每帧计算子弹的最大数量
        /// </summary>
        public int MaxBulletUpdatesPerFrame = 500;

        /// <summary>
        /// 子弹可视化到计算出的虚拟子弹所需的时间  
        /// </summary>
        public float VisualBulletToRealBulletMovementTime = 0.1f;

        private static BulletHandler instance;
        public static BulletHandler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<BulletHandler>();
                    if (instance == null)
                    {
                        GameObject instanceGO = new GameObject("BulletHandler");
                        instance = instanceGO.AddComponent<BulletHandler>();
                    }
                }
                return instance;
            }
        }
        

        private BulletPoolManager myPool;

        //private
        private float g;
        private BulletData[] bulletListTmp;

        private void Start()
        {
            g = Physics.gravity.y;
            myPool = BulletPoolManager.Instance;

            if (!Application.isEditor) IsDynamicEditor = false;
        }

        private void LateUpdate()
        {
            UpdateBullets();
        }

        /// <summary>
        /// 计算子弹飞行
        /// </summary>
        private void UpdateBullets()
        {
            float deltaTime = IsDynamicEditor ? 0.0166667f : Time.deltaTime;

            //predefined variables
            BulletData cBullet;
            Weapon cWeapon;
            Ray ray;
            RaycastHit hit;
            Transform hitTrans;
            if (Bullets.Count > MaxBulletUpdatesPerFrame)
            {
                bulletListTmp = Bullets.ToArray();
            }

            Rigidbody hitRigid;
            LivingEntityCollider hitLivingEntity;
            MaterialObject hitBallisticObject;

            //is windy?
            bool isWindy = WindDirection.sqrMagnitude > 0;

            float leftOverFlyTime = 0;
            float myDeltaTime = 0;
            bool processAgain = true;

            //Process each Bullet
            for (int i = 0; i < Bullets.Count; i++)
            {
                if (i < MaxBulletUpdatesPerFrame)
                {
                    cBullet = Bullets.Dequeue();

                    cWeapon = cBullet.parentWeapon;

                    processAgain = true;
                    while (processAgain)
                    {
                        //when current bullet passed processing already but has 'flytime over'
                        if (leftOverFlyTime == 0)
                        {
                            myDeltaTime = deltaTime;
                        }
                        else
                        {
                            myDeltaTime = leftOverFlyTime;
                            leftOverFlyTime = 0;
                        }

                        //when this bullet had to wait in the query for more than 1 frame
                        if (cBullet.timeSinceLastUpdate > 0)
                        {
                            myDeltaTime += cBullet.timeSinceLastUpdate;
                            cBullet.timeSinceLastUpdate = 0;
                        }

                        //decrease Lifetime
                        cBullet.lifeTime -= myDeltaTime;
                        if (cBullet.lifeTime <= 0)
                        {
                            //remove Bullet if dead
                            DeactivateBullet(cWeapon, cBullet.BulletTrans);
                            processAgain = false;
                            continue;
                        }

                        if (UseBulletdrag)
                        {
                            //Air resistence
                            cBullet.Speed -= cWeapon.PreDrag * ((cBullet.Speed * cBullet.Speed)) * myDeltaTime;
                            //Wind Influence
                            if (isWindy)
                            {
                                cBullet.bulletDir += ((WindDirection * cWeapon.DragCoefficient * AirDensity) / cBullet.Speed) * myDeltaTime;
                                cBullet.bulletDir.Normalize();
                            }
                        }


                        //Move Bullet
                        cBullet.bulletPos += cBullet.bulletDir * cBullet.Speed * myDeltaTime;

                        if (UseBulletdrop)
                        {
                            //Bulletdrop
                            cBullet.ySpeed += g * myDeltaTime;

                            if (UseBulletdrag)
                            {
                                cBullet.ySpeed += cWeapon.PreDrag * ((cBullet.ySpeed * cBullet.ySpeed)) * myDeltaTime;
                            }

                            cBullet.bulletPos.y += cBullet.ySpeed * myDeltaTime;
                        }

                        //Hitcheck------------------
                        Vector3 dir = cBullet.bulletPos - cBullet.LastPos;
                        float dirMag = dir.magnitude;
                        //bullet speed + fall speed
                        float rBulletSpeed = dirMag / myDeltaTime;
                        float BulletEnergy = (0.5f * cWeapon.BulletMass * rBulletSpeed * rBulletSpeed);
                        ray = new Ray(cBullet.LastPos, dir);

                        if (Physics.Raycast(ray, out hit, dirMag, cWeapon.HitMask))
                        {
                            hitTrans = hit.transform;

                            hitRigid = null;
                            hitLivingEntity = null;
                            hitBallisticObject = null;

                            hitBallisticObject = UseBallisticMaterials ? hitTrans.GetComponent<MaterialObject>() : null;


                            if (hitBallisticObject == null)
                            {
                                //if the hit object has the tag 'static' it will not search for attached Rigidbody / LivingEntityCollider
                                if (hitTrans.tag != "static")
                                {
                                    hitRigid = hitTrans.GetComponent<Rigidbody>();
                                    hitLivingEntity = hitTrans.GetComponent<LivingEntityCollider>();
                                }

                                BulletHit(cWeapon, hitLivingEntity, hitRigid, cBullet.bulletDir, hit.point, rBulletSpeed, BulletEnergy * 2f);
                
                                //Stop Bullet
                                DeactivateBullet(cWeapon, cBullet.BulletTrans);
                                processAgain = false;
                                continue;
                            }
                            else
                            {
                                //if the BallisticObject is marked static, it will not search for attached Rigidbody / LivingEntityCollider
                                if (!hitBallisticObject.isStatic || hitTrans.tag != "static")
                                {
                                    hitRigid = hitTrans.GetComponent<Rigidbody>();
                                    hitLivingEntity = hitTrans.GetComponent<LivingEntityCollider>();
                                }

                                //call BulletImpact on the hitBallisticObject
                                hitBallisticObject.BulletImpact(hit);

                                if (UseSpreadMaterials)
                                {
                                    Vector3 dirNorm = dir.normalized;
                                    //does this bullet ricochet=
                                    if (hitBallisticObject.RicochetPropability.Evaluate(Vector3.Angle(hit.normal, -ray.direction) / 90f) < Random.Range(0f, 1f))
                                    {

                                        //maximal material penetration of bullet
                                        float MaxRange = BulletEnergy / hitBallisticObject.EnergylossPerUnit;
                                        //float MaxRange = (rBulletSpeed * rBulletSpeed) / (2 * (hitData.Slowdown / cWeapon.Mass));

                                        //backtrace bulletpath to find out if the bullet went through the material
                                        ray = new Ray(hit.point + dirNorm * MaxRange, -dirNorm);
                                        RaycastHit[] hits = Physics.RaycastAll(ray, MaxRange, cWeapon.HitMask);

                                        hits.OrderByDescending(d => d.distance);

                                        int OutIndex = -1;
                                        RaycastHit outHit = new RaycastHit();
                                        for (int n = 0; n < hits.Length; n++)
                                        {
                                            if (hits[n].transform == hitTrans)
                                            {
                                                OutIndex = n;
                                                outHit = hits[OutIndex];
                                                break;
                                            }
                                        }

                                        if (OutIndex != -1)
                                        {
                                            //Shoot through

                                            hitBallisticObject.BulletImpact(outHit);

                                            //slowdown
                                            float dist = (outHit.point - hit.point).magnitude;
                                            float afterSpeed = cBullet.Speed * (1 - (dist / MaxRange));
                                            cBullet.ySpeed *= afterSpeed / cBullet.Speed;
                                            cBullet.Speed = afterSpeed;
                                            float diffSpeed = rBulletSpeed - afterSpeed;


                                            //Spread
                                            cBullet.bulletDir = hitBallisticObject.RndSpread > 0 ? (Quaternion.AngleAxis(Random.Range(0f, 360f), cBullet.bulletDir) * Quaternion.AngleAxis(Random.Range(0f, hitBallisticObject.RndSpread), Vector3.Cross(Vector3.up, cBullet.bulletDir)) * cBullet.bulletDir) : cBullet.bulletDir;
                                            //cBullet.bulletDir = (cBullet.bulletDir * rBulletSpeed * cWeapon.BulletMass + ((Random.onUnitSphere + hit.normal) / 2) * hitData.RndSpread).normalized;


                                            //process Bullet again
                                            leftOverFlyTime = ((cBullet.bulletPos - outHit.point).magnitude / (myDeltaTime * rBulletSpeed)) * myDeltaTime;
                                            BulletHit(cWeapon, hitLivingEntity, hitRigid, cBullet.bulletDir, hit.point, diffSpeed, (0.5f * cWeapon.BulletMass * diffSpeed * diffSpeed) / dist);
                                            cBullet.bulletPos = cBullet.LastPos = outHit.point;

                                            processAgain = true;
                                            continue;
                                        }
                                        else
                                        {
                                            //Bullet stuck in object

                                            BulletHit(cWeapon, hitLivingEntity, hitRigid, cBullet.bulletDir, hit.point, rBulletSpeed, BulletEnergy / MaxRange);

                                            //Stop Bullet
                                            DeactivateBullet(cWeapon, cBullet.BulletTrans);
                                            processAgain = false;
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        //Ricochet
                                        //Spread
                                        Vector3 dirBefore = dirNorm;

                                        //Reflect bullet
                                        cBullet.bulletDir = Vector3.Reflect(dirNorm, hit.normal);

                                        cBullet.bulletDir = hitBallisticObject.RndSpreadRic > 0 ? (Quaternion.AngleAxis(Random.Range(0f, 360f), cBullet.bulletDir) * Quaternion.AngleAxis(Random.Range(0f, hitBallisticObject.RndSpreadRic), Vector3.Cross(Vector3.up, cBullet.bulletDir)) * cBullet.bulletDir) : cBullet.bulletDir;
                                        //cBullet.bulletDir = (cBullet.bulletDir * rBulletSpeed * cWeapon.BulletMass + ((Random.onUnitSphere + hit.normal)/2) * hitData.RndSpreadRic).normalized;

                                        //Slowdown
                                        float fac = 1 - (Vector3.Angle(dirBefore, cBullet.bulletDir) / 180f);
                                        cBullet.Speed = rBulletSpeed * fac;
                                        cBullet.ySpeed = 0;

                                        leftOverFlyTime = ((hit.point - cBullet.bulletPos).magnitude / (myDeltaTime * rBulletSpeed)) * myDeltaTime;

                                        float deltaSpeed = rBulletSpeed * (1 - fac);
                                        BulletHit(cWeapon, hitLivingEntity, hitRigid, dirBefore, hit.point, deltaSpeed, cWeapon.BulletMass * deltaSpeed * deltaSpeed);

                                        //process Bullet again
                                        cBullet.bulletPos = cBullet.LastPos = hit.point;

                                        processAgain = true;
                                        continue;
                                    }
                                }

                                //not using Ballistic Materials
                                else
                                {
                                    BulletHit(cWeapon, hitLivingEntity, hitRigid, cBullet.bulletDir, hit.point, rBulletSpeed, BulletEnergy * 2);

                                    //stop bullet
                                    DeactivateBullet(cWeapon, cBullet.BulletTrans);
                                    processAgain = false;
                                    continue;
                                }
                            }
                        }
                        processAgain = false;

                        cBullet.LastPos = cBullet.bulletPos;
                        //Update Transform
                        if (cBullet.BulletTrans != null)
                        {
                            if (cBullet.StartLifeTime - cBullet.lifeTime < VisualBulletToRealBulletMovementTime)
                            {
                                cBullet.VisualOffset = Vector3.Lerp(cBullet.StartOffset, Vector3.zero, (cBullet.StartLifeTime - cBullet.lifeTime) / VisualBulletToRealBulletMovementTime);
                            }
                            else
                            {
                                cBullet.VisualOffset = Vector3.zero;
                            }
                            cBullet.BulletTrans.position = cBullet.bulletPos + cBullet.VisualOffset;
                            cBullet.BulletTrans.rotation = Quaternion.LookRotation(dir);
                        }
                        //Enqueue at End
                        Bullets.Enqueue(cBullet);
                    }
                }
                else
                {
                    bulletListTmp[i - MaxBulletUpdatesPerFrame].timeSinceLastUpdate += deltaTime; 
                }
            }
        }

        /// <summary>
        /// register impact
        /// </summary>
        /// <param name="weapon">parent weapon of bullet</param>
        /// <param name="hitEntity">living entity component of hit GameObject</param>
        /// <param name="hitRigid">rigidbody component of hit GameObject</param>
        /// <param name="bDir">impact direction</param>
        /// <param name="hitPoint">impact point</param>
        /// <param name="bSpeed">bullet speed at impact</param>
        /// <param name="impactForce">Force of the impact</param>
        private void BulletHit(Weapon weapon, LivingEntityCollider hitEntity, Rigidbody hitRigid, Vector3 bDir, Vector3 hitPoint, float bSpeed, float impactForce)
        {
            //apply damage
            if (hitEntity != null)
            {
                if (hitEntity.ParentLivingEntity != null)
                {
                    //damage: ( baseBulletDamage ) * ( bulletSpeed / MaxBulletSpeed ) * ( DamageMultiplier of the LivingEntityCollider )
                    hitEntity.ParentLivingEntity.Health -= (bSpeed / weapon.MaxBulletSpeed) * weapon.MuzzleDamage * hitEntity.DamageMultiplier;
                }
            }
            //Force at Rigidbody
            if (hitRigid != null)
            {
                hitRigid.AddForceAtPosition(bDir * impactForce, hitPoint, ForceMode.Impulse);
            }
        }

        /// <summary>
        /// 添加一个子弹回对象池
        /// </summary>
        /// <param name="weapon"></param>
        /// <param name="bulletTrans"></param>
        private void DeactivateBullet(Weapon weapon, Transform bulletTrans)
        {
            if (bulletTrans != null)
            {
                GameObject go = bulletTrans.gameObject;
                go.SetActive(false);
                myPool.AddObject(weapon.BulletPrefab.gameObject, go);
            }
        }
    }
}
