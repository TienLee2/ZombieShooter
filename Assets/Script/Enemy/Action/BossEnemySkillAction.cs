using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemySkillAction : Action
{
    private Animator animator;
    private bool SkillUse;

    public override void OnAwake()
    {
        animator = GetComponent<Animator>();
        SkillUse = false;
    }

    public override TaskStatus OnUpdate()
    {

        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9)
        {
            SkillUse = false;
            return TaskStatus.Success;
        }
        if(SkillUse == false)
        {
            SkillAction();
        }
        return TaskStatus.Running;
    }

    public void SkillAction()
    {
        animator.SetTrigger("skill");
        SkillUse = true;
    }
}
