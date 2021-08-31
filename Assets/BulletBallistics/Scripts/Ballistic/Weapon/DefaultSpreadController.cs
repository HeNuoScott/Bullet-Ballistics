using UnityEngine;
using System.Collections;
using Ballistics;
/// <summary>
/// 发射控制器 
/// 控制开火间隔 开火稳定性
/// </summary>
public class DefaultSpreadController : SpreadController
{
    /// <summary>
    /// (x=0) -> start spread ; (x=1) -> max spread (in degree)
    /// x=0 抖动,x=1 抖动幅度最大
    /// </summary>
    public AnimationCurve SpreadAngleCurve = new AnimationCurve();

    /// <summary>
    /// Time to go from start- to max-spread
    /// 攻击时间 用于计算抖动
    /// </summary>
    public float AttackTime;

    /// <summary>
    /// Time until spread release
    /// </summary>
    public float HoldTime;

    /// <summary>
    /// Time to go from max- to start-spread
    /// </summary>
    public float ReleaseTime;

    /// <summary>
    /// between 0-1; is the current "time" at the SpreadAngleCurve
    /// </summary>
    [HideInInspector]
    public float currentSpread;

    private float shootTimer = 0;

    [HideInInspector]
    private float baseSpread;

    /// <summary>
    /// Calculations for defaultSpreadModel
    /// </summary>
    private void SpreadCalc()
    {
        currentSpread = Mathf.Clamp01(shootTimer > 0 ? currentSpread + Time.deltaTime / AttackTime : currentSpread - Time.deltaTime / ReleaseTime);
        shootTimer -= Time.deltaTime;
    }

    BulletHandler bulletHandler = null;

    private void Awake()
    {
        bulletHandler = BulletHandler.instance;
    }

    private void Update()
    {
        SpreadCalc();
    }

    /// <summary>
    /// called from the weapon when it shoots
    /// </summary>
    public override void OnShoot()
    {
        shootTimer = HoldTime;
    }

    public override Vector3 GetCurrentSpread(Transform spawn)
    {
        if (bulletHandler != null && bulletHandler.Settings.useSpreadMaterials)
        {
            float spread = SpreadAngleCurve.Evaluate(currentSpread) / 2 + baseSpread;
            return (Quaternion.AngleAxis(Random.Range(0, 360), spawn.forward) * (Quaternion.AngleAxis(Random.Range(0, spread), spawn.right)) * spawn.forward);
        }
        else return spawn.forward;
    }

    public override float GetSpreadAngle()
    {
        return SpreadAngleCurve.Evaluate(currentSpread);
    }

    public void SetBaseSpread(float spread)
    {
        baseSpread = spread;
    }
}
