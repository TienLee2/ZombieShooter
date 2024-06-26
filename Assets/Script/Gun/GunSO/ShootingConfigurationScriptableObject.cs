
using UnityEngine;
using static UnityEngine.ParticleSystem;

[CreateAssetMenu(fileName ="Shoot Config", menuName ="Guns/Shoot Configuration",order =2)]
public class ShootingConfigurationScriptableObject : ScriptableObject
{
    //Target layer to hit
    public LayerMask HitMask;
    //The damage of gun
    public int Damage;
    //How much does the gun spread
    public Vector3 Spread = new Vector3(0.1f, 0.1f, 0.1f);
    //The firerate of the weapon
    public float FireRate = 0.25f;

}
