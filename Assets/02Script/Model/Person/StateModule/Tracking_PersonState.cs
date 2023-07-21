using System.Collections;
using UnityEngine;

public class Tracking_PersonState : PersonState
{
    PrepareData prepareData;
    bool isAphDone = false;
    StateKinds stateKinds;
    public Tracking_PersonState(Person person) : base(person) { }

    public void PrepareState(PrepareData data)
    {
        prepareData = data;
    }
    public override bool IsReadyForEnter()
    {
        return prepareData != null;
    }
    public override void EnterToException() { }
    protected override void DoEnter()
    {
        person.StartCoroutine(DoTrackingPoint());
    }

    private IEnumerator DoTrackingPoint()
    {
        var weapon = person.weapon;

        if (weapon != null)
        {
            var aph = person.GetNewAPH(1, prepareData.walkingState);
            var ap = aph.GetActionPoint(0);
            person.SetAPs(ap, prepareData.target, PersonAniState.StateKind.Non, false, 0, true, true);
            person.SetAPH(aph, AfterAPHDone);

            while (!isAphDone)
            {
                var isInSight = person.modelHandler.IsHitToTarget(prepareData.target);
                if (isInSight)
                {
                    var dist = Vector3.Distance(prepareData.target.position, person.modelHandler.transform.position);
                    if (dist < weapon.Range)
                    {
                        stateKinds = StateKinds.Hit;
                        break;
                    }
                    else
                    {
                        person.SetAPs(ap, prepareData.target, PersonAniState.StateKind.LookAround, false, 0, true, true);
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
    protected override StateKinds DoAfterDone()
    {
        isAphDone = true;

        if (stateKinds == StateKinds.Hit)
        {
            var hitModule = person.moduleHandler.GetModule(StateKinds.Hit);
            var hitPrepareData = new Hit_PersonState.PrepareData();
            hitPrepareData.target = prepareData.target;
            (hitModule as Hit_PersonState).SetPrepareData(hitPrepareData);
        }
        else
        {
            stateKinds = StateKinds.Normal;
        }

        return stateKinds;
    }

    public class PrepareData
    {
        public Transform target { set; get; }
        public ActionPointHandler.WalkingState walkingState { set; get; }
    }
}