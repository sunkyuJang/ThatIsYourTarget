using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersonAniController : AniController
{
    public GameObject personNeck;
    Coroutine PlayingAni { set; get; }
    new public bool IsPlayingAni { get { return PlayingAni != null; } }
    public enum AnimationsWithLevel { WalkAroundLevel = 0, SittingLevel, }
    public enum AnimationsWithFloat { TurnDegree }
    public enum WalkLevel { Stop = 0, Walk, Run }
    public enum SittingLevel { Non = 0, High, Middle, Low }
    public enum AnimationsWithBool { ShouldStand, LookAround, ShouldSurprize }

    public override void StartAni(ActionPoint actionPoint, bool shouldReturnAP = false)
    {
        var ap = actionPoint as PersonActionPoint;
        switch (ap.State)
        {
            case PersonActionPoint.StateKind.Sitting: SetSittingAnimation((SittingLevel)ap.sittingNum); break;
            case PersonActionPoint.StateKind.LookAround: SetLookAroundAnimation(); break;
            case PersonActionPoint.StateKind.Standing: StopMove(); break;
            case PersonActionPoint.StateKind.PrepareAttack: SetPrepareAttack(ap.shouldReadyForBattle, ap.weaponLayer); break;
            case PersonActionPoint.StateKind.Surprize: SetSurprizeAnimation(); break;
            case PersonActionPoint.StateKind.TurnAround: SetTurnAroundAnimation(ap); break;
            case PersonActionPoint.StateKind.TurnHead: SetTurnHead(ap); break;
            default:
                break;
        }

        StartAniTimeCount(ap.during, shouldReturnAP);

        if (shouldReturnAP)
            APHManager.Instance.GetObjPooler(APHManager.PoolerKinds.PersonAP).ReturnTargetObj(ap.gameObject);
    }

    void StartAniTimeCount(float during, bool shouldReturnAP)
    {
        if (IsPlayingAni)
            StopCoroutine(PlayingAni);

        PlayingAni = StartCoroutine(DoAnimationTimeCount(during, shouldReturnAP));
    }

    IEnumerator DoAnimationTimeCount(float during, bool shouldReturnAP = false)
    {
        if (during < -1) yield return null;
        var maxTime = Mathf.Lerp(0, during, animationPlayLimit);
        for (float time = 0f; time < maxTime; time += Time.fixedDeltaTime)
        {
            yield return new WaitForFixedUpdate();
        }
        PlayingAni = null;

        MakeResetAni(!shouldReturnAP);
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

    public void SetTurnAroundAnimation(ActionPoint ap)
    {
        animator.SetFloat(AnimationsWithFloat.TurnDegree.ToString(), ap.targetDegree);
        StopMove(false);
    }
    public void SetTurnHead(ActionPoint ap)
    {
        headFollowTarget = ap.transform;
        StopMove(false);
    }

    public void MakeResetAni(bool shouldReadNextAction = true)
    {
        StartCoroutine(DoResetAni(shouldReadNextAction));
    }

    IEnumerator DoResetAni(bool shouldReadNextAction)
    {
        var wasStanding = animator.GetInteger(AnimationsWithLevel.SittingLevel.ToString()) != 0;

        for (int i = 1; i < animator.layerCount; i++)
            animator.SetLayerWeight(i, 0);
        animator.SetInteger(AnimationsWithLevel.SittingLevel.ToString(), (int)SittingLevel.Non);
        animator.SetBool(AnimationsWithBool.LookAround.ToString(), false);
        animator.SetBool(AnimationsWithBool.ShouldStand.ToString(), false);
        animator.SetBool(AnimationsWithBool.ShouldSurprize.ToString(), false);
        animator.SetFloat(AnimationsWithFloat.TurnDegree.ToString(), 361f);

        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime < animationPlayLimit);
        SetWalkState(WalkLevel.Walk);
        yield return new WaitUntil(() => IsWalkState());

        if (shouldReadNextAction)
            modelPhysicsController.ReadNextAction();
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
        ap.State = PersonActionPoint.StateKind.TurnAround;
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
        ap.State = PersonActionPoint.StateKind.TurnHead;
        ap.during = 3f;
        StartAni(ap, true);

        return ap;
    }
}
