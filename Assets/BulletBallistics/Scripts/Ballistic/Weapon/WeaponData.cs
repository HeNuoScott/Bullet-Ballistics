using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct WeaponData
{
    public BasicWeaponController weapon;
    public AudioSource sound;
    public ParticleSystem particle;
    public Transform ScopePos;
    public float ReloadTime;
    public float RecoilAmount;
}