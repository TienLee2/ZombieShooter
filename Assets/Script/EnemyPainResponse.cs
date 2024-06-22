using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Animator))]

public class EnemyPainResponse : MonoBehaviour
{
    [SerializeField]
    private EnemyHealth Health;
    private Animator Animator;
    private CapsuleCollider _collider;
    [SerializeField]
    [Range(1, 100)]
    private int MaxDamagePainThreshold = 5;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        _collider = GetComponentInChildren<CapsuleCollider>();
    }

    public void HandlePain(int Damage)
    {
        if (Health.CurrentHealth != 0)
        {
            // you can do some cool stuff based on the
            // amount of damage taken relative to max health
            // here we're simply setting the additive layer
            // weight based on damage vs max pain threshhold
            Animator.ResetTrigger("Hit");
            Animator.SetLayerWeight(1, (float)Damage / MaxDamagePainThreshold);
            Animator.SetTrigger("Hit");
        }
    }

    public void HandleDeath()
    {
        Animator.SetTrigger("death");
        _collider.isTrigger = true;
        Animator.applyRootMotion = true;
        
    }
}
