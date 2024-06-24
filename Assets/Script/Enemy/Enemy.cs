using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public EnemyHealth Health;
    public EnemyPainResponse PainResponse;
    public EnemyStatScriptableObject StatScript;

    public Transform player;
    private NavMeshAgent agent;
    private Animator animator;

    private float chaseRange = 10f;
    private float attackRange = 2f;
    private float attackCooldown = 1f;
    private float attackTimer;

    private enum State { Idle, Chasing, Attacking , Die}
    private State currentState;

    private void Start()
    {
        chaseRange = StatScript.ChaseRange;
        attackRange = StatScript.AttackRange;
        attackCooldown = StatScript.AttackCoolDown;
        currentState = State.Idle;
        attackTimer = attackCooldown;

        PainResponse = GetComponent<EnemyPainResponse>();
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        Health.OnTakeDamage += PainResponse.HandlePain;
        Health.OnDeath += Die;
    }

    public void Update()
    {
        float distanceToPlayer = Vector3.Distance(player.position, transform.position);

        switch (currentState)
        {
            case State.Idle:
                if (distanceToPlayer <= chaseRange)
                {
                    currentState = State.Chasing;
                }
                break;

            case State.Chasing:
                if (distanceToPlayer <= attackRange)
                {
                    currentState = State.Attacking;
                    agent.isStopped = true;
                }
                else if (distanceToPlayer > chaseRange)
                {
                    currentState = State.Idle;
                }
                else
                {
                    agent.SetDestination(player.position);
                }
                break;

            case State.Attacking:
                if (distanceToPlayer > attackRange)
                {
                    currentState = State.Chasing;
                    agent.isStopped = false;
                }
                else
                {
                    AttackPlayer();
                }
                break;

            case State.Die:
                break;
        }

        UpdateAnimator();
    }

    void AttackPlayer()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            animator.SetTrigger("attack");
            Debug.Log("Zombie attacks player!");
            attackTimer = attackCooldown;
        }
    }

    void UpdateAnimator()
    {
        animator.SetBool("isChasing", currentState == State.Chasing);
        animator.SetBool("isAttacking", currentState == State.Attacking);
    }

    private void Die (Vector3 Position)
    {
        currentState = State.Die;
        Invoke("DespawnObject", 3f);
        agent.enabled = false;
        PainResponse.HandleDeath();
    }

    private void DespawnObject()
    {
        Lean.Pool.LeanPool.Despawn(this.gameObject);
    }
}
