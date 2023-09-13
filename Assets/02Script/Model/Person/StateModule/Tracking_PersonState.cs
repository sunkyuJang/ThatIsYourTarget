using System.Collections;
using UnityEngine;

public class Tracking_PersonState : PersonState
{
    bool isAphDone = false;
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
            var aph = GetNewAPH(1, AnimationPointHandler.WalkingState.Run);
            var ap = aph.GetActionPoint(0);
            SetAPs(ap, prepareData.target, PersonAniState.StateKind.Non, 0, true, true);
            SetAPH(aph, true);
            var shouldFixAPLookAt = false;
            isAphDone = false;

            while (!isAphDone)
            {
                var isInSight = IsInSight(prepareData.target);

                if (isInSight)
                {
                    var dist = Vector3.Distance(ActorTransform.position, prepareData.target.position);
                    if (dist < weapon.Range)
                    {
                        stateKinds = StateKinds.Hit;
                        break;
                    }
                    else
                    {
                        SetAPs(ap, prepareData.target, PersonAniState.StateKind.LookAround, 0, true, true);
                    }

                    shouldFixAPLookAt = true;
                }
                else
                {
                    // have to look this again.
                    // the point of this is, the ap rotation should be look at to last way that player go.
                    if (shouldFixAPLookAt)
                    {
                        ap.transform.LookAt(ap.transform.position);
                        shouldFixAPLookAt = false;
                    }
                }
                yield return new WaitForFixedUpdate();
            }
        }
        yield break;
    }

    public override void Exit()
    {
        isAphDone = false;
    }
    protected StateKinds DoAfterAPHDone(out PersonPrepareData prepareData)
    {
        prepareData = new PersonPrepareData(this.prepareData.target);
        isAphDone = true;
        return stateKinds;
    }
}