using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBossHit : MonoBehaviour
{
    public LayerMask hitLayers;
    public GameObject DamageParent;
    public EnemyStatScriptableObject enemyStat;
    public ParticleSystem skillEffect;

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
