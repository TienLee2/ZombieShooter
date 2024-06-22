
using UnityEngine;
using static UnityEngine.ParticleSystem;

[CreateAssetMenu(fileName ="Shoot Config", menuName ="Guns/Shoot Configuration",order =2)]
public class ShootingConfigurationScriptableObject : ScriptableObject
{
    public LayerMask HitMask;
    public MinMaxCurve DamageCurve;
    public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);
    public float FireRate = 0.25f;

    private void Reset()
    {
        DamageCurve.mode = ParticleSystemCurveMode.Curve;        
    }

    public int GetDamage(float Distance = 0)
    {
        return Mathf.CeilToInt(DamageCurve.Evaluate(Distance, Random.value));
    }
}
