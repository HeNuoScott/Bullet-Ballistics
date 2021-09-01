using UnityEngine;
using System.Collections;

namespace Ballistics
{
    public class LivingEntityCollider : MonoBehaviour
    {
        //伤害系数
        public float DamageMultiplier = 1;

        // 当LivingEntityCollider作为子物体时, 可以用LivingEntity按钮直接添加赋值
        public LivingEntity ParentLivingEntity;
    }
}
