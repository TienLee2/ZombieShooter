using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "Enemy Stat Config", menuName = "Enemy/Stat Configuration", order = 0)]
public class EnemyStatScriptableObject : ScriptableObject
{
    [EnumToggleButtons]
    public EnemyType enemyType;


    public int MaxHealth;
    [HideIf("enemyType", EnemyType.Range)]
    public int Damage;
    public float MovementSpeed;
    public float ChaseRange;
    public float AttackRange;
    [HideIf("enemyType", EnemyType.Range)]
    public float AttackCoolDown;
    [HideIf("enemyType", EnemyType.Melee)]
    public float FleeRange;
    [ShowIf("enemyType", EnemyType.Boss)]
    public float BossSkillRange;
    [ShowIf("enemyType", EnemyType.Boss)]
    public int BossSkillDamage;
}

public enum EnemyType
{
    Melee,
    Range,
    Boss
}
