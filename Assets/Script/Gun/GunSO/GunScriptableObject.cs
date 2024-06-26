using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;


[CreateAssetMenu(fileName ="Gun", menuName ="Guns/Gun", order =0)]
public class GunScriptableObject : ScriptableObject
{
    //The Type of chosen gun
    public GunType Type;
    //Gun's name
    public string Name;
    //The gun model 
    public GameObject ModelPrefab;
    //The position of the spawn gun
    public Vector3 SpawnPoint;
    //The Rotation of the spawn gun
    public Vector3 SpawnRotation;

    //some reference to get stat from
    public ShootingConfigurationScriptableObject ShootConfig;
    public TrailConfigScriptableObject TrailConfig;
    public AmmoConfigScriptableObject AmmoConfig;

    //Because this is Scriptable object, we need a monobehavior to count time
    private MonoBehaviour ActiveMonoBehaviour;
    //The model that take from the gun prefab
    private GameObject Model;
    //Check the last shoot time
    private float LastShootTime;
    //Gun shoot effect
    private ParticleSystem ShootSystem;

    //Add object pooling for performance
    private ObjectPool<RigidbodyBullet> BulletPool;
    private ObjectPool<TrailRenderer> TrailPool;

    //Spawn the gun to player hand, also serve as start
    public void Spawn(Transform Parent, MonoBehaviour ActiveMonoBehaviour)
    {
        this.ActiveMonoBehaviour = ActiveMonoBehaviour;
        LastShootTime = 0;
        TrailPool = new ObjectPool<TrailRenderer> (CreateTrail);
        if(Type == GunType.Bazooka)
        {
            BulletPool = new ObjectPool<RigidbodyBullet>(CreateBullet);
        }

        Model = Instantiate(ModelPrefab);
        Model.transform.SetParent(Parent, false);
        Model.transform.localPosition = SpawnPoint;
        Model.transform.localRotation = Quaternion.Euler(SpawnRotation);

        ShootSystem = Model.GetComponentInChildren<ParticleSystem>();
    }

    //Shoot Function
    public void Shoot()
    {
        //If no ammo then return
        if (AmmoConfig.CurrentClipAmmo <= 0) return;

        //Handle the fire rate
        if(Time.time > ShootConfig.FireRate + LastShootTime)
        {
            LastShootTime = Time.time;

            ShootSystem.Play();
            var ShootConfigX = Random.Range(-ShootConfig.Spread.x, ShootConfig.Spread.x);
            var ShootConfigY = Random.Range(-ShootConfig.Spread.y, ShootConfig.Spread.y);
            var ShootConfigZ = Random.Range(-ShootConfig.Spread.z, ShootConfig.Spread.z);

            Vector3 shootDirection = ShootSystem.transform.forward 
                + new Vector3(ShootConfigX, ShootConfigY,ShootConfigZ);
            shootDirection.Normalize();

            AmmoConfig.CurrentClipAmmo--;

            if(Type == GunType.Bazooka)
            {
                DoProjectileShoot(shootDirection);
            }
            else
            {
                DoRayCastShoot(shootDirection);
            }
        }
    }

    public bool CanReload()
    {
        return AmmoConfig.CanReload();
    }

    public void EndReload()
    {
        AmmoConfig.Reload();
    }

    //Shoot with bullet gun
    private void DoProjectileShoot(Vector3 shootDirection)
    {
        RigidbodyBullet bullet = BulletPool.Get();
        bullet.gameObject.SetActive(true);
        bullet.transform.position = ShootSystem.transform.position;
        bullet.Spawn(shootDirection * AmmoConfig.BulletSpawnForce);

        MonoInstance.instance.StartCoroutine(ExplodeGrenade(bullet));

        TrailRenderer trail = TrailPool.Get();
        if (trail != null)
        {
            trail.transform.SetParent(bullet.transform, false);
            trail.transform.localPosition = Vector3.zero;
            trail.emitting = true;
            trail.gameObject.SetActive(true);
        }
    }

    //Shoot with raycast gun
    private void DoRayCastShoot(Vector3 shootDirection)
    {
        if (Physics.Raycast(
                ShootSystem.transform.position,
                shootDirection,
                out RaycastHit hit,
                float.MaxValue,
                ShootConfig.HitMask
                ))
        {
            ActiveMonoBehaviour.StartCoroutine(
                PlayTrail(ShootSystem.transform.position, hit.point, hit)
            );

        }
        else
        {
            ActiveMonoBehaviour.StartCoroutine(
                PlayTrail(
                    ShootSystem.transform.position,
                    ShootSystem.transform.position + (shootDirection * TrailConfig.MissDistance),
                    new RaycastHit()
                )
            );
        }
    }

    //The bullet will explode after few second
    private IEnumerator ExplodeGrenade(RigidbodyBullet Bullet)
    {
        yield return new WaitForSeconds(3f);

        //Generate a explosion effect, then detach it's parent
        GameObject BulletExplosion = Instantiate(AmmoConfig.Explosion);
        BulletExplosion.transform.parent = null;
        BulletExplosion.transform.position = Bullet.transform.position;

        //Apply a sphere to collect nearby enemy collider
        Collider[] hitColliders = Physics.OverlapSphere(Bullet.transform.position, AmmoConfig.ExplosiveRange,ShootConfig.HitMask);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.TryGetComponent(out IDamageable damageable))
            {
                //Do damage to the enemy surround area
                damageable.TakeDamage(ShootConfig.Damage);
            }
        }

        //Afer explode, deactive bullet
        Bullet.gameObject.SetActive(false);
        BulletPool.Release(Bullet);
        
    }

    private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit Hit)
    {
        //Get trail from pool
        TrailRenderer instance = TrailPool.Get();
        instance.gameObject.SetActive( true );
        instance.transform.position = StartPoint;
        yield return null;

        instance.emitting = true;

        //Get travel distance of bullet to limit it's range
        float distance = Vector3.Distance(StartPoint, EndPoint);
        float remainingDistance = distance;
        //When slowly get to the end, reduce the intense of trail
        while( remainingDistance > 0 )
        {
            instance.transform.position = Vector3.Lerp(
                StartPoint, 
                EndPoint, 
                Mathf.Clamp01(1-(remainingDistance/distance)) 
            );

            remainingDistance -= TrailConfig.SimulationSpeed * Time.deltaTime;

            yield return null;
        }

        instance.transform.position = EndPoint;

        //if the bullet hit something and it has health, subtract health 
        if(Hit.collider!=null)
        {
            if(Hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(ShootConfig.Damage);
            }
        }

        yield return new WaitForSeconds(TrailConfig.Duration);
        yield return null;
        instance.emitting = false;
        instance.gameObject.SetActive ( false );
        TrailPool.Release(instance);
    }

    private IEnumerator DelayedDisableTrail(TrailRenderer Trail)
    {
        yield return new WaitForSeconds(TrailConfig.Duration);
        yield return null;
        Trail.emitting = false;
        Trail.gameObject.SetActive(false);
        TrailPool.Release(Trail);
    }

    private TrailRenderer CreateTrail()
    {
        GameObject instance = new GameObject("Bullet Trail");
        TrailRenderer trail = instance.AddComponent<TrailRenderer>();
        trail.colorGradient = TrailConfig.Color;
        trail.material = TrailConfig.Material;
        trail.widthCurve = TrailConfig.WidthCurve;
        trail.time = TrailConfig.Duration;
        trail.minVertexDistance = TrailConfig.MinVertexDistance;
        trail.emitting = false;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        
        return trail;
    }

    private RigidbodyBullet CreateBullet()
    {
        return Instantiate(AmmoConfig.BulletPrefab);
    }

}
