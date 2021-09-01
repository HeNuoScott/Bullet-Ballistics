using UnityEngine;

namespace Ballistics
{
    public class LivingEntity : MonoBehaviour
    {
        public float StartHealth;
        private float myHealth = 0;

        public float Health
        {
            get { return myHealth; }
            set
            {
                float before = myHealth;
                if (value > 0f && value <= StartHealth)
                {
                    myHealth = value;
                }
                else
                {
                    if (value > 0f)
                    {
                        myHealth = StartHealth;
                    }
                    else
                    {
                        myHealth = 0;
                        OnDeath();
                    }
                }
                OnHealthChanged(value - before);
            }
        }

        private void Awake()
        {
            myHealth = StartHealth;
        }

        /// <summary>
        /// 死亡时调用
        /// </summary>
        public virtual void OnDeath()
        { 

        }

        /// <summary>
        /// 血量改变时调用
        /// </summary>
        /// <param name="amount">血量改变值</param>
        public virtual void OnHealthChanged(float amount)
        {
            Debug.Log(transform.name + " took " + (-amount).ToString() + " damage");
        }
    }
}