using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeHitPlayerAction : Action
{
    private GunSelector GunSelector;
    private GameObject Player;
    public GameObject DamageParent;
    private NavMeshAgent agent;
    private Animator animator;

    private float attackTimer;

    public EnemyStatScriptableObject enemyStat;

    public int count;

    public override void OnAwake()
    {
        GunSelector = GetComponent<GunSelector>();
        Player = GameObject.FindGameObjectWithTag("Player");
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }


    public override TaskStatus OnUpdate()
    {
        float distanceToPlayer = Vector3.Distance(Player.transform.position, transform.position);


        if (count>3)
        {
            GunSelector.ActiveGun.EndReload();
            count = 0;
            return TaskStatus.Success;
        }
        agent.isStopped = false;
        agent.SetDestination(Player.transform.position);
        animator.SetFloat("speed", 1f);
        if (distanceToPlayer <= 1.7f)
        {
            AttackPlayer();
            agent.isStopped = true;
        }
        return TaskStatus.Running;
    }

    

    void AttackPlayer()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
        {
            animator.SetTrigger("attack");
            count++;
            attackTimer = enemyStat.AttackCoolDown;
        }
    }
}
