using System.Collections;
using UnityEngine;

public class Tracking_PersonState : PersonState
{
    bool isAphDone = false;
    bool shouldFixedLookAt = false;
    AnimationPointHandler aph = null;
    StateKinds stateKinds;
    public Tracking_PersonState(Person person) : base(person) { }
    public override bool IsReady()
    {
        return prepareData != null;
    }
    public override void EnterToException() { }
    protected override void StartModule()
    {
        StartCoroutine(DoTrackingPoint());
    }

    private IEnumerator DoTrackingPoint()
    {
        var weapon = Weapon;

        if (weapon != null)
        {
            aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Run);
            var ap = aph.GetActionPoint(0);
            SetAPs(ap, prepareData.target, PersonAniState.StateKind.Non, 0f, true, true);
            SetAPH(aph, true);

            isAphDone = false;

            TracingTargetInSightProcess(prepareData.target, () => isAphDone);
        }
        yield break;
    }

    protected override bool ShouldStopAfterHit(bool whenHit)
    {
        var ap = aph.GetActionPoint(0);
        if (whenHit)
        {
            SetAPs(ap, prepareData.target, PersonAniState.StateKind.Non, 0, false, true);
            var dist = Vector3.Distance(ActorTransform.position, prepareData.target.position);
            if (dist < Weapon.Range)
            {
                stateKinds = StateKinds.Attack;
                // cause have to attack in position that actor stand.
                return true;
            }
            else
            {
                shouldFixedLookAt = true;
            }
        }
        else
        {
            if (shouldFixedLookAt)
            {
                shouldFixedLookAt = false;
                SetAPs(ap, prepareData.target, PersonAniState.StateKind.LookAround, 0f, false, true);
            }
        }

        return false;
    }

    public override void Exit()
    {
        isAphDone = true;
    }
    protected override StateKinds AfterAPHDone(out PersonPrepareData prepareData)
    {
        prepareData = new PersonPrepareData(this.prepareData.target);
        isAphDone = true;
        return stateKinds;
    }
}