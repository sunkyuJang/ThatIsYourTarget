using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonAniController : AniController
{
    Coroutine PlayingAni { set; get; }
    public bool IsPlayingAni { get { return PlayingAni != null; } }
    public enum AnimationsWithLevel { WalkAroundLevel = 0, SittingLevel, }
    public enum WalkLevel { Stop = 0, Walk, Run }
    public enum SittingLevel { Non = 0, High, Middle, Low }
    public enum AnimationsWithBool { ShouldStand, ShouldTurn, LookAround, ShouldSurprize, ShouldTurnL, ShouldTurnR }

    public override void StartAni(ActionPoint actionPoint, bool shouldReturnAP = false)
    {
        var ap = actionPoint as PersonActionPoint;
        switch (ap.state)
        {
            case (int)PersonActionPoint.StateKind.Sitting: SetSittingAnimation((SittingLevel)ap.sittingNum); break;
            case (int)PersonActionPoint.StateKind.LookAround: SetLookAroundAnimation(); break;
            case (int)PersonActionPoint.StateKind.Standing: StopMove(); break;
            case (int)PersonActionPoint.StateKind.PrepareAttack: SetPrepareAttack(ap.shouldReadyForBattle, ap.weaponLayer); break;
            case (int)PersonActionPoint.StateKind.Surprize: SetSurprizeAnimation(); break;
            case (int)PersonActionPoint.StateKind.TurnAround: SetTurnAroundAnimation(ap.shouldTurnLeft); break;
            default:
                break;
        }

        if (IsPlayingAni)
            StopCoroutine(PlayingAni);

        PlayingAni = StartCoroutine(DoAnimationTimeCount(ap));
    }

    IEnumerator DoAnimationTimeCount(ActionPoint actionPoint, bool shouldReturnAP = false)
    {
        var time = 0f;
        while (time < actionPoint.during)
        {
            time += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        PlayingAni = null;

        if (shouldReturnAP)
            APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.PersonAP).ReturnTargetObj(actionPoint.gameObject);

        MakeResetAni();
    }

    public void SetPrepareAttack(bool shouldPrepare, int weaponLayer)
    {
        if (shouldPrepare)
        {
            animator.SetLayerWeight(weaponLayer, 1);
        }
        else
        {
            MakeResetAni();
        }
    }

    public void SetSittingAnimation(SittingLevel sittingLevel)
    {
        animator.SetInteger(AnimationsWithLevel.SittingLevel.ToString(), (int)sittingLevel);
        StopMove();
    }

    public void SetLookAroundAnimation()
    {
        animator.SetBool(AnimationsWithBool.LookAround.ToString(), true);
        StopMove();
    }

    public void SetSurprizeAnimation()
    {
        animator.SetBool(AnimationsWithBool.ShouldSurprize.ToString(), true);
        StopMove(false);
    }

    void StopMove(bool shoulPlayStand = true)
    {
        animator.SetInteger(AnimationsWithLevel.WalkAroundLevel.ToString(), (int)WalkLevel.Stop);
        if (shoulPlayStand)
            animator.SetBool(AnimationsWithBool.ShouldStand.ToString(), true);
    }

    public void SetTurnAroundAnimation(bool isLeft)
    {
        animator.SetBool(isLeft ? AnimationsWithBool.ShouldTurnL.ToString() : AnimationsWithBool.ShouldTurnR.ToString(), true);
        StopMove(false);
    }

    public void MakeResetAni()
    {
        StartCoroutine(DoResetAni());
    }

    IEnumerator DoResetAni()
    {
        var wasStanding = animator.GetInteger(AnimationsWithLevel.SittingLevel.ToString()) != 0;

        for (int i = 1; i < animator.layerCount; i++)
            animator.SetLayerWeight(i, 0);
        animator.SetInteger(AnimationsWithLevel.SittingLevel.ToString(), (int)SittingLevel.Non);
        animator.SetBool(AnimationsWithBool.LookAround.ToString(), false);
        animator.SetBool(AnimationsWithBool.ShouldStand.ToString(), false);
        animator.SetBool(AnimationsWithBool.ShouldSurprize.ToString(), false);

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.95f);
        SetWalkState(WalkLevel.Walk);
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("WalkAround") ||
                                            animator.GetCurrentAnimatorStateInfo(0).IsName("RunningAround"));
        modelPhysicsController.ReadNextAction();
    }

    public void SetWalkState(WalkLevel walkLevel)
    {
        animator.SetInteger(AnimationsWithLevel.WalkAroundLevel.ToString(), (int)walkLevel);
    }
}
