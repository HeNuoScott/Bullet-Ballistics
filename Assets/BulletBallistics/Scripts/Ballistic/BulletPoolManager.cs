 using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ballistics
{
    public interface IPoolingObject
    {
        void ReAwake();
    }
    [RequireComponent(typeof(BulletHandler))]
    public class BulletPoolManager : MonoBehaviour
    {
        public static BulletPoolManager Instance = null;

        private void Awake()
        {
            Instance = this;
        }

        // 类型id 对应类型队列
        public Dictionary<int, Queue<GameObject>> Pool = new Dictionary<int, Queue<GameObject>>();
        // 弹痕类型
        public List<MaterialObjectType> ImpactTypes = new List<MaterialObjectType>();
        // 弹痕预制体
        public List<ImpactObject> ImpactObjects = new List<ImpactObject>();

        /// <summary>
        /// 添加一个物体进对象池
        /// </summary>
        /// <param name="ID">哪个物体产生的对象</param>
        /// <param name="obj">实际添加到对象池中的对象</param>
        public void AddObject(GameObject ID, GameObject obj)
        {
            int id = ID.GetInstanceID();
            if (Pool.ContainsKey(id))
            {
                Pool[id].Enqueue(obj);
            }
            else
            {
                Pool.Add(id, new Queue<GameObject>());
                Pool[id].Enqueue(obj);
            }
        }

        /// <summary>
        /// 获取对象
        /// </summary>
        /// <param name="ID">对象实例化的基础游戏对象</param>
        /// <returns></returns>
        public GameObject GetNextGameObject(GameObject ID)
        {
            int id = ID.GetInstanceID();

            if (Pool.ContainsKey(id))
            {
                if (Pool[id].Count > 0)
                {
                    GameObject obj = Pool[id].Dequeue();
                    IPoolingObject pObj = obj.GetComponent<IPoolingObject>();
                    if (pObj != null) pObj.ReAwake();
                    return obj;
                }
            }
            return null;
        }

        public GameObject GetImpactPerfab(MaterialObjectType type)
        {
            if (ImpactTypes.Contains(type))
            {
                for (int i = 0; i < ImpactTypes.Count; i++)
                {
                    if (ImpactTypes[i] == type && ImpactObjects[i] != null)
                    {
                        return ImpactObjects[i].gameObject;
                    }
                }
            }
            return null;
        }
    }
}
