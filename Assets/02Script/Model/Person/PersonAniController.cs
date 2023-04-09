using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonAniController : AniController
{
    public GameObject personNeck;
    public enum StateKind { non, Walk, Standing, LookAround, Sitting, Surprize, PrepareAttack, Fight, Avoid, TurnAround, TurnHead }
    public enum AnimationsWithLevel { WalkAroundLevel = (int)StateKind.Walk, SittingLevel = (int)StateKind.Sitting }
    public enum WalkLevel { Stop, Walk, Run }
    public enum SittingLevel { Non, High, Middle, Low }
    public enum AnimationsWithFloat { TurnDegree = (int)StateKind.TurnAround }
    public enum AnimationsWithBool { ShouldStand = (int)StateKind.Standing, LookAround = (int)StateKind.LookAround, ShouldSurprize = (int)StateKind.Surprize }
    public enum FightType { Gun, Stick }
    private List<StateKind> playStandList = new List<StateKind>()
    {
        StateKind.Standing, StateKind.PrepareAttack, StateKind.TurnHead
    };
    public override void StartAni(ActionPoint actionPoint, bool shouldReturnAP = false)
    {
        var ap = actionPoint as PersonActionPoint;
        switch (ap.State)
        {
            case StateKind.Sitting: SetSittingAnimation((SittingLevel)ap.sittingNum); break;
            case StateKind.LookAround: SetLookAroundAnimation(); break;
            case StateKind.Standing: break;
            case StateKind.PrepareAttack: SetPrepareAttack(ap.shouldReadyForBattle, ap.weaponLayer); break;
            case StateKind.Surprize: SetSurprizeAnimation(); break;
            case StateKind.TurnAround: SetTurnAroundAnimation(ap); break;
            case StateKind.TurnHead: SetTurnHead(ap); break;
            default:
                break;
        }

        animator.SetInteger(AnimationsWithLevel.WalkAroundLevel.ToString(), (int)WalkLevel.Stop);
        foreach (var state in playStandList)
        {
            if (state == ap.State)
            {
                animator.SetBool(AnimationsWithBool.ShouldStand.ToString(), true);
                break;
            }
        }
        StartAniTimeCount(ap, shouldReturnAP);
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
    }
    public void SetLookAroundAnimation()
    {
        animator.SetBool(AnimationsWithBool.LookAround.ToString(), true);
    }
    public void SetSurprizeAnimation()
    {
        animator.SetBool(AnimationsWithBool.ShouldSurprize.ToString(), true);
    }
    public void SetTurnAroundAnimation(ActionPoint ap)
    {
        animator.SetFloat(AnimationsWithFloat.TurnDegree.ToString(), ap.targetDegree);
    }
    public void SetTurnHead(ActionPoint ap)
    {
        headFollowTarget = ap.transform;
    }
    protected override IEnumerator DoResetAni(bool shouldReadNextAction)
    {
        for (int i = 1; i < animator.layerCount; i++)
            animator.SetLayerWeight(i, 0);
        animator.SetInteger(AnimationsWithLevel.SittingLevel.ToString(), (int)SittingLevel.Non);
        animator.SetBool(AnimationsWithBool.LookAround.ToString(), false);
        animator.SetBool(AnimationsWithBool.ShouldStand.ToString(), false);
        animator.SetBool(AnimationsWithBool.ShouldSurprize.ToString(), false);
        animator.SetFloat(AnimationsWithFloat.TurnDegree.ToString(), 361f);

        SetWalkState(WalkLevel.Walk);
        yield return new WaitUntil(() => IsWalkState());
        yield return StartCoroutine(base.DoResetAni(shouldReadNextAction));

        ProcResetAni = null;
    }

    bool IsWalkState() =>
        animator.GetCurrentAnimatorStateInfo(0).IsName("WalkAround") ||
        animator.GetCurrentAnimatorStateInfo(0).IsName("RunningAround");


    public void SetWalkState(WalkLevel walkLevel)
    {
        animator.SetInteger(AnimationsWithLevel.WalkAroundLevel.ToString(), (int)walkLevel);
    }

    protected override IEnumerator DoMakeCorrect(ActionPoint ap)
    {
        SetCorrectly(ap);
        yield return new WaitUntil(() => isPositionCorrect && isRotationCorrect);
        yield return new WaitUntil(() => IsWalkState());
        StartAni(ap);
    }

    protected override IEnumerator DoRotationCorrectly(Vector3 dir)
    {
        isRotationCorrect = false;
        var startForward = transform.forward;
        var cross = Vector3.Cross(Vector3.up, startForward);
        var dot = Vector3.Dot(cross, dir);
        var isLeft = dot < 0;
        var lastAngle = Vector3.Angle(transform.forward, dir);

        // make correction for animation float
        {
            var lastAngleABS = Mathf.Abs(lastAngle);
            if (lastAngleABS == 0f || lastAngleABS == 135 || lastAngleABS == 360)
                lastAngle += lastAngle >= 0 ? 1 : -1;
        }

        var limitDegreeOfHead = 80f;
        var shouldBodyTurnWithAnimation = lastAngle >= limitDegreeOfHead;

        if (shouldBodyTurnWithAnimation)
        {
            var ap = MakeTurn(lastAngle);

            var rotateTime = Mathf.Lerp(0, ap.during, 0.45f);
            var totalAngle = Vector3.Angle(transform.forward, dir);
            var eachFrameAngle = totalAngle / (rotateTime / Time.fixedDeltaTime);
            for (float t = 0; t < ap.during; t += Time.fixedDeltaTime)
            {
                if (t < rotateTime)
                    transform.Rotate(isLeft ? Vector3.down : Vector3.up, eachFrameAngle);
                yield return new WaitForFixedUpdate();
            }
        }

        isRotationCorrect = true;
        yield return null;
    }
    new public ActionPoint MakeTurn(float degree)
    {
        var ap = APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.PersonAP).GetNewOne<PersonActionPoint>();
        ap.State = PersonAniController.StateKind.TurnAround;
        ap.targetDegree = degree;
        ap.during = ap.GetLength(GetStateNameByDegree(ap.targetDegree));
        StartAni(ap, true);
        return ap;
    }

    string GetStateNameByDegree(float degree)
    {
        if (degree >= 0)
        {
            return degree > 135f ? "LongTurnR" : "TurnR";
        }
        else
        {
            return degree < -135f ? "TurnL" : "LongTurnL";
        }
    }

    public ActionPoint MakeHeadTurn()
    {
        var ap = APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.PersonAP).GetNewOne<PersonActionPoint>();
        ap.State = PersonAniController.StateKind.TurnHead;
        ap.during = 3f;
        StartAni(ap, true);

        return ap;
    }
}
