using System;
using UnityEngine;
using System.Collections;

public class Attack_HumanState : HumanState
{
    private enum APHDoneState { Attacking, Delaying, Non }
    private APHDoneState aphDoneState = APHDoneState.Non;
    public Attack_HumanState(Human person) : base(person)
    {

    }
    public override bool IsReady()
    {
        return prepareData != null;
    }
    public override void EnterToException() { }
    protected override void StartModule()
    {
        if (Weapon == null)
        {
            // cant attack
            SetNormalState();
        }

        aphDoneState = APHDoneState.Attacking;
        if (Weapon.CanAttack(prepareData.target, out Weapon.CanAttackStateError attackError))
        {
            SetAttack();
        }
        else if (attackError == global::Weapon.CanAttackStateError.Resource)
        {
            // reloading
        }
        else if (attackError == global::Weapon.CanAttackStateError.Range)
        {
            SetState(StateKinds.Tracking, prepareData);
        }
    }

    void SetAttack()
    {
        var aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Run);

        var AttackAP = aph.GetAnimationPoint<HumanAnimationPoint>(0);
        SetAPs(AttackAP, prepareData.target, HumanAniState.StateKind.Attack, 0, false, true);
        var skillToken = skillLoader.UseSkill();
        AttackAP.animationPointData.SkillData = skillToken.SkillData;
        AttackAP.animationPointData.CanAnimationCancle = false;
        skillToken.curLoopCount++;

        AttackAP.animationPointData.EventTrigger += (int i) => { AttackTrigger(AttackAP.animationPointData.SkillData, i); };
        AttackAP.animationPointData.LookAtTransform = prepareData.target;
        AttackAP.animationPointData.StoppingDistance = Weapon.range;

        if (skillToken.SkillData.canLoop)
        {
            if (skillToken.maxLoopCount == 0)
            {
                skillToken.maxLoopCount = UnityEngine.Random.Range(0, 2);
            }
        }

        SetAPH(aph, true);
    }

    public void AttackTrigger(SkillData skillData, int num)
    {
        //skillData.
    }

    protected override void AfterAPHDone()
    {
        switch (aphDoneState)
        {
            case APHDoneState.Attacking:
                if (IsInSight(prepareData.target))
                {
                    StartModule();
                }
                else
                {
                    SetState(StateKinds.Tracking, new PersonPrepareData(prepareData.target));
                }

                break;

            case APHDoneState.Delaying:
                StartModule();
                break;
        }

    }
    public override void Exit()
    {
        prepareData = null;
        base.Exit();
    }
}
