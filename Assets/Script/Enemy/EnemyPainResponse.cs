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
            Animator.ResetTrigger("hit");
            Animator.SetLayerWeight(1, (float)Damage / MaxDamagePainThreshold);
            Animator.SetTrigger("hit");
        }
    }

    //Death method
    public void HandleDeath()
    {
        Animator.SetTrigger("death");
        _collider.isTrigger = true;
        Animator.applyRootMotion = true;
        
    }
}
