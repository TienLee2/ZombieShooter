using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerDeathResponse : MonoBehaviour
{
    private PlayerHealth Health;
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
        Health = gameObject.GetComponentInChildren<PlayerHealth>();
        Health.OnDeath += Die;
    }

    private void Die(Vector3 Position)
    {
        Invoke("DespawnObject", 6f);
        animator.SetTrigger("death");
    }
}
