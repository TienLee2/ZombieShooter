using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class RangeEnemyShootAction : Action
{
    private GunSelector GunSelector;
    private GameObject Player;
    private float fleeRange;

    public EnemyStatScriptableObject enemyStat;
    public override void OnAwake()
    {
        GunSelector = GetComponent<GunSelector>();
        Player = GameObject.FindGameObjectWithTag("Player");
        fleeRange = enemyStat.FleeRange;
    }


    public override TaskStatus OnUpdate()
    {
        float distanceToPlayer = Vector3.Distance(Player.transform.position, transform.position);


        if (distanceToPlayer <= fleeRange||
            GunSelector.ActiveGun.AmmoConfig.CurrentClipAmmo == 0)
        {
            return TaskStatus.Success;
        }
        FaceTarget(Player.transform.position);
        GunSelector.ActiveGun.Shoot();
        return TaskStatus.Running;
    }

    private void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 30);
    }
}
