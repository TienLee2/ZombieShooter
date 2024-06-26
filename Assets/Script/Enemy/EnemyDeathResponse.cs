using BehaviorDesigner.Runtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyDeathResponse : MonoBehaviour
{
    //Get the health component
    public EnemyHealth Health;
    //Get the pain response component
    private EnemyPainResponse PainResponse;
    //Nav mesh agent
    private NavMeshAgent agent;
    private BehaviorTree tree;

    private void Start()
    {
        tree = GetComponent<BehaviorTree>();
        agent = GetComponent<NavMeshAgent>();
        PainResponse = GetComponent<EnemyPainResponse>();
        Health.OnTakeDamage += PainResponse.HandlePain;
        Health.OnDeath += Die;
    }

    private void Die(Vector3 Position)
    {
        Invoke("DespawnObject", 6f);
        tree.enabled = false;
        agent.enabled = false;
        PainResponse.HandleDeath();
    }

    private void DespawnObject()
    {
        Lean.Pool.LeanPool.Despawn(this.gameObject);
    }
}
