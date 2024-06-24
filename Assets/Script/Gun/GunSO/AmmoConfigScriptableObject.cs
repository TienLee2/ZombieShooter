using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ammo Config", menuName ="Guns/Ammo Config", order=3)]
public class AmmoConfigScriptableObject : ScriptableObject
{
    public bool IsRigidbodyBullet;
    public RigidbodyBullet BulletPrefab;
    public GameObject Explosion;
    public float BulletSpawnForce = 100;
    public float ExplosiveTime = 3;
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
