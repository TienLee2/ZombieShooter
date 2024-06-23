using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UIElements;


[CreateAssetMenu(fileName ="Gun", menuName ="Guns/Gun", order =0)]
public class GunScriptableObject : ScriptableObject
{
    public GunType Type;
    public string Name;
    public GameObject ModelPrefab;
    public Vector3 SpawnPoint;
    public Vector3 SpawnRotation;

    public ShootingConfigurationScriptableObject ShootConfig;
    public TrailConfigScriptableObject TrailConfig;
    public AmmoConfigScriptableObject AmmoConfig;


    private MonoBehaviour ActiveMonoBehaviour;
    private GameObject Model;
    private float LastShootTime;
    private ParticleSystem ShootSystem;

    private ObjectPool<RigidbodyBullet> BulletPool;
    private ObjectPool<TrailRenderer> TrailPool;

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

    public void Shoot()
    {
        if (AmmoConfig.CurrentClipAmmo <= 0) return;

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

    private void DoProjectileShoot(Vector3 shootDirection)
    {
        RigidbodyBullet bullet = BulletPool.Get();
        bullet.gameObject.SetActive(true);
        //bullet.OnCollsion += HandleBulletCollision;
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

    //To make the bullet damage on touch
    /*private void HandleBulletCollision(RigidbodyBullet Bullet, Collision Collision)
    {
        TrailRenderer trail = Bullet.GetComponentInChildren<TrailRenderer>();
        if (trail != null)
        {
            trail.transform.SetParent(null, true);
            ActiveMonoBehaviour.StartCoroutine(DelayedDisableTrail(trail));
        }

        Bullet.gameObject.SetActive(false);
        BulletPool.Release(Bullet);

        if (Collision != null)
        {
            ContactPoint contactPoint = Collision.GetContact(0);
            float DistanceTraveled = Vector3.Distance(contactPoint.point, Bullet.SpawnLocation);
            Collider HitCollider = contactPoint.otherCollider;

            if (HitCollider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(ShootConfig.GetDamage(DistanceTraveled));
            }
        }
    }*/

    private IEnumerator ExplodeGrenade(RigidbodyBullet Bullet)
    {
        yield return new WaitForSeconds(3f);
        Bullet.gameObject.SetActive(false);
        BulletPool.Release(Bullet);
        Debug.Log("Grenade go boom");
    }

    private IEnumerator PlayTrail(Vector3 StartPoint, Vector3 EndPoint, RaycastHit Hit)
    {
        TrailRenderer instance = TrailPool.Get();
        instance.gameObject.SetActive( true );
        instance.transform.position = StartPoint;
        yield return null;

        instance.emitting = true;

        float distance = Vector3.Distance(StartPoint, EndPoint);
        float remainingDistance = distance;
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

        if(Hit.collider!=null)
        {
            if(Hit.collider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(ShootConfig.GetDamage(distance));
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
