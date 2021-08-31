using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Ballistics;

/// <summary>
/// impactObject that plays a sound + particle system
/// </summary>
public class DefaultImpactObject : ImpactObject {
    public List<AudioClip> HitSounds;
    AudioSource audiosource;
    ParticleSystem particles;
    InvokeDeactivate deactivate;
    private Transform myTrans;
    public float Time = 3f;

    void Awake()
    {
        audiosource = GetComponent<AudioSource>();
        particles = GetComponent<ParticleSystem>();
        deactivate = GetComponent<InvokeDeactivate>();
        myTrans = transform;

        for(int i = 0; i < HitSounds.Count; i++)
        {
            if(HitSounds[i] == null)
            {
                HitSounds.RemoveAt(i);
                i--;
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="data">Material Data of the hit object</param>
    /// <param name="rayHit">RaycastHit of the impact</param>
    public override void Hit(BallisticObjectData data, RaycastHit rayHit)
    {
        myTrans.rotation = Quaternion.LookRotation(rayHit.normal);
        if (audiosource != null && HitSounds.Count > 0)
        {
            audiosource.clip = HitSounds[Random.Range(0, HitSounds.Count)];
            audiosource.Play();
        }

        if(particles != null)
        {
            particles.Simulate(0, true, true);
            ParticleSystem.EmissionModule module = particles.emission;
            module.enabled = true;
            particles.Play(true);
        }

        if(deactivate != null)
        {
            deactivate.Deactivate(Time, data.impactObject.gameObject);
        }

    }
}
