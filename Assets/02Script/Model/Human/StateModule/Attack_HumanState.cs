using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;

public class Attack_HumanState : HumanState
{
    private enum APHDoneState { Attacking, Delaying, Non }
    private APHDoneState aphDoneState = APHDoneState.Non;
    private DateTime lastEnteredTime;
    private float minGraceTime = 0.5f;
    public Attack_HumanState(Human person) : base(person)
    {

    }
    public override bool IsReady()
    {
        return prepareData != null;
    }
    public override void EnterToException() { }
    protected override void OnStartModule()
    {
        if (Weapon == null)
        {
            // cant attack
            SetNormalState();
        }

        var passedTime = DateTime.Now - TimeSpan.FromSeconds(minGraceTime);
        if (DateTime.Now > lastEnteredTime + TimeSpan.FromSeconds(minGraceTime))
        {
            lastEnteredTime = DateTime.Now;
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
        Debug.Log("isIn");
        var aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Run);

        var AttackAP = aph.GetAnimationPoint<HumanAnimationPoint>(0);
        SetAPs(AttackAP, prepareData.target, HumanAniState.StateKind.Attack, 0, false, true);
        var skillToken = skillLoader.UseSkill();
        AttackAP.animationPointData.SkillData = skillToken.SkillData;
        AttackAP.animationPointData.CanAnimationCancle = false;
        skillToken.curLoopCount++;

        AttackAP.animationPointData.EventTrigger += (int i) => { AttackTrigger(skillToken, i); };
        AttackAP.animationPointData.LookAtTransform = prepareData.target;
        AttackAP.animationPointData.StoppingDistance = Weapon.range;

        if (skillToken.SkillData.CanLoop)
        {
            if (skillToken.maxLoopCount == 0)
            {
                skillToken.maxLoopCount = UnityEngine.Random.Range(0, 2);
            }
        }

        SetAPH(aph, true);
    }

    public void AttackTrigger(SkillLoader.SkillToken skillToken, int num)
    {
        var detector = skillToken.GetTargetDetector(skillToken.SkillData);
        switch (skillToken.SkillData.SkillDetectorStartPoint)
        {
            case SkillData.SkillEffectStartPointList.Model:
                detector.StartDetection(
                    ActorTransform.position,
                    ActorTransform.forward,
                    ActorTransform,
                    (hitTargets) => { WhenHitTarget(hitTargets, skillToken, num); },
                    () => { skillToken.RestoreTargetDetector(skillToken.SkillData, detector); });
                break;
            case SkillData.SkillEffectStartPointList.Weapon:
                detector.StartDetection(
                    Weapon.SkillDectectorStartPoint.position,
                    Weapon.SkillDectectorStartPoint.forward,
                    ActorTransform,
                    (hitTargets) => { WhenHitTarget(hitTargets, skillToken, num); },
                    () => { skillToken.RestoreTargetDetector(skillToken.SkillData, detector); });
                break;
        }
    }

    public void WhenHitTarget(List<RaycastHit> hitTargets, SkillLoader.SkillToken skillToken, int num)
    {
        var hitter = skillToken.GetTargetHitter(skillToken.SkillData);
        hitTargets.ForEach(hit =>
        {
            switch (skillToken.SkillData.SkillHitterStartPoint)
            {
                case SkillData.SkillEffectStartPointList.Model:
                    hitter.StartEffect(
                        hit.point,
                        hit.normal,
                        () => { skillToken.RestoreTargetHitter(skillToken.SkillData, hitter); },
                        hit.transform);
                    break;
                case SkillData.SkillEffectStartPointList.Non:
                    hitter.StartEffect(
                        hit.point,
                        hit.normal,
                        () => { skillToken.RestoreTargetHitter(skillToken.SkillData, hitter); });
                    break;
            }
        });


    }

    protected override void AfterAPHDone()
    {
        switch (aphDoneState)
        {
            case APHDoneState.Attacking:
                if (IsInSight(prepareData.target))
                {
                    Debug.Log("after done to attacking again");
                    StartModule();
                }
                else
                {
                    Debug.Log("after done to tracking");
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
