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
        person.StartCoroutine(DoTrackingPoint());
    }

    private IEnumerator DoTrackingPoint()
    {
        var weapon = person.weapon;

        if (weapon != null)
        {
            var aph = person.GetNewAPH(1, AnimationPointHandler.WalkingState.Run);
            var ap = aph.GetActionPoint(0);
            person.SetAPs(ap, prepareData.target.modelPhysicsHandler.transform, PersonAniState.StateKind.Non, false, 0, true, true);
            person.SetAPH(aph, AfterAPHDone);
            var shouldFixAPLookAt = false;
            isAphDone = false;

            while (!isAphDone)
            {
                var isInSight = person.modelPhysicsHandler.IsInSight(prepareData.target.modelPhysicsHandler.transform);

                if (isInSight)
                {
                    var dist = Vector3.Distance(prepareData.target.modelPhysicsHandler.transform.position, person.modelPhysicsHandler.transform.position);
                    if (dist < weapon.Range)
                    {
                        stateKinds = StateKinds.Hit;
                        break;
                    }
                    else
                    {
                        person.SetAPs(ap, prepareData.target.modelPhysicsHandler.transform, PersonAniState.StateKind.LookAround, false, 0, true, true);
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
    protected override StateKinds DoAfterDone(out PersonPrepareData prepareData)
    {
        prepareData = new PersonPrepareData(this.prepareData.target);
        isAphDone = true;
        return stateKinds;
    }
}