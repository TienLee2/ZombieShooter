using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Stat Config", menuName = "Enemy/Stat Configuration", order = 0)]
public class EnemyStatScriptableObject : ScriptableObject
{
    public int MaxHealth;
    public int Damage;
    public float MovementSpeed;
    public float ChaseRange;
    public float AttackRange;
    public float AttackCoolDown;
}
