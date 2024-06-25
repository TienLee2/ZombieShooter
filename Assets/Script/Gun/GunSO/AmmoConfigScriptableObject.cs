using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ammo Config", menuName ="Guns/Ammo Config", order=3)]
public class AmmoConfigScriptableObject : ScriptableObject
{
    [EnumToggleButtons]
    public GunShootType shootType;

    [ShowIf("shootType", GunShootType.Rigidbody)]
    public RigidbodyBullet BulletPrefab;
    [ShowIf("shootType", GunShootType.Rigidbody)]
    public GameObject Explosion;
    [ShowIf("shootType", GunShootType.Rigidbody)]
    public float BulletSpawnForce = 100;
    [ShowIf("shootType", GunShootType.Rigidbody)]
    public float ExplosiveTime = 3;
    [ShowIf("shootType", GunShootType.Rigidbody)]
    public int ExplosiveRange = 5;

    public int MaxAmmo = 120;
    public int CurrentAmmo = 120;

    public int ClipSize = 30;
    public int CurrentClipAmmo = 30;

    public void Reload()
    {
        int maxReloadAmount = Mathf.Min(ClipSize, CurrentAmmo);
        int availableBulletsInCurrentClip = ClipSize - CurrentClipAmmo;
        int reloadAmount = Mathf.Min(maxReloadAmount, availableBulletsInCurrentClip);

        CurrentClipAmmo = CurrentClipAmmo + reloadAmount;
        CurrentAmmo -= reloadAmount;
    }

    public bool CanReload()
    {
        return CurrentClipAmmo < ClipSize && CurrentAmmo > 0;
    }

}

public enum GunShootType
{
    Raycast,
    Rigidbody
}
