using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEnemySkillAction : Action
{
    //Animator to activate skill
    private Animator animator;
    //if the skill is use or not
    private bool SkillUse;

    public override void OnAwake()
    {
        animator = GetComponent<Animator>();
        SkillUse = false;
    }

    public override TaskStatus OnUpdate()
    {
        //if the skill animation is not 90%, then this task keep running
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

    //call the skill
    public void SkillAction()
    {
        animator.SetTrigger("skill");
        SkillUse = true;
    }
}
