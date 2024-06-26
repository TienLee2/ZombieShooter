using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossHit : MonoBehaviour
{
    
    public LayerMask hitLayers;
    //The parent gameobject that do damage
    public GameObject DamageParent;
    //Get the stat from scriptable object
    public EnemyStatScriptableObject enemyStat;
    //The boss skill effect
    public ParticleSystem skillEffect;

    //Same attack method as Melee enemy
    public void AttackPlayerEvent()
    {
        Collider[] hitColliders = Physics.OverlapSphere(DamageParent.transform.position, 2f, hitLayers);

        foreach (var hitCollider in hitColliders)
        {

            if (hitCollider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(enemyStat.Damage);
            }
        }
    }

    //Skill method is the same as melee attack
    public void SkillEvent()
    {
        skillEffect.Play();
        Collider[] hitColliders = Physics.OverlapSphere(DamageParent.transform.position, enemyStat.BossSkillRange, hitLayers);

        foreach (var hitCollider in hitColliders)
        {

            if (hitCollider.TryGetComponent(out IDamageable damageable))
            {
                damageable.TakeDamage(enemyStat.BossSkillDamage);
            }
        }
    }
}
