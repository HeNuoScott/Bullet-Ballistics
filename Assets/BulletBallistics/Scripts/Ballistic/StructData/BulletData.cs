using Ballistics;
using UnityEngine;

public class BulletData
{
    public Weapon parentWeapon;
    public Vector3 bulletPos;
    public Vector3 LastPos;
    public Vector3 bulletDir;
    public Vector3 VisualOffset;
    public float lifeTime;
    public float StartLifeTime;
    public Vector3 StartOffset;
    public float Speed;
    public float ySpeed;
    public Transform BulletTrans;

    public float timeSinceLastUpdate;

    public BulletData(Weapon weapon, Vector3 pos, Vector3 offset, Vector3 dir, float life, float speed, Transform bC)
    {
        parentWeapon = weapon;
        bulletPos = pos;
        LastPos = pos;
        StartOffset = offset;
        lifeTime = life;
        StartLifeTime = lifeTime;
        VisualOffset = offset;
        bulletDir = dir;
        Speed = speed;
        ySpeed = 0;
        BulletTrans = bC;
    }
}
