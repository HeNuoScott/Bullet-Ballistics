using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Ballistics
{
    /// <summary>
    /// 弹痕物体
    /// 播放声效 + 播放粒子特效
    /// </summary>
    public class ImpactObject : MonoBehaviour
    {
        public float Time = 3f;
        public List<AudioClip> HitSounds;
        
        private AudioSource audiosource;
        private ParticleSystem particles;
        private InvokeDeactivate deactivate;
        private Transform myTrans;

        private void Awake()
        {
            audiosource = GetComponent<AudioSource>();
            particles = GetComponent<ParticleSystem>();
            deactivate = GetComponent<InvokeDeactivate>();
            myTrans = transform;

            for (int i = 0; i < HitSounds.Count; i++)
            {
                if (HitSounds[i] == null)
                {
                    HitSounds.RemoveAt(i);
                    i--;
                }
            }
        }

        public virtual void Hit(RaycastHit rayHit, GameObject owner)
        {
            myTrans.rotation = Quaternion.LookRotation(rayHit.normal);
            if (audiosource != null && HitSounds.Count > 0)
            {
                audiosource.clip = HitSounds[Random.Range(0, HitSounds.Count)];
                audiosource.Play();
            }

            if (particles != null)
            {
                particles.Simulate(0, true, true);
                ParticleSystem.EmissionModule module = particles.emission;
                module.enabled = true;
                particles.Play(true);
            }

            if (deactivate != null)
            {
                deactivate.Deactivate(Time, owner);
            }

        }
    }
}
