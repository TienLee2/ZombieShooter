using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunFromPlayerAction : Action
{
    // Component references
    private GameObject Player;
    private float fleeRange;
    private NavMeshAgent navMeshAgent;
    private Animator _animator;

    public EnemyStatScriptableObject enemyStat;

    public override void OnAwake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        Player = GameObject.FindGameObjectWithTag("Player");
        fleeRange = enemyStat.FleeRange;
        _animator = GetComponent<Animator>();
    }


    public override TaskStatus OnUpdate()
    {
        float distanceToPlayer = Vector3.Distance(Player.transform.position, transform.position);

        if (distanceToPlayer >= fleeRange*2)
        {
            return TaskStatus.Success;
        }
        _animator.SetFloat("speed", 1f);
        FleeFromPlayer();
        return TaskStatus.Running;
    }

    void FleeFromPlayer()
    {
        // Calculate the direction away from the player
        Vector3 fleeDirection = transform.position - Player.transform.position;
        fleeDirection.Normalize();

        // Calculate the new target position
        Vector3 newFleePosition = transform.position + fleeDirection * fleeRange;
        navMeshAgent.isStopped = false;
        navMeshAgent.SetDestination(newFleePosition);
    }
}
