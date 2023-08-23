using UnityEngine;

public class PrepareAttack_PersonState : PersonState
{
    public static float prepareAttackDist = 1f;
    protected PersonWeapon weapon { get { return person.weapon; } }
    public PrepareAttack_PersonState(Person person) : base(person) { }

    public override bool IsReady()
    {
        return prepareData != null;
    }
    protected override void StartModule()
    {
        SetNormalState();
        //person.StartCoroutine(TraceTarget(GetAttckAPHHandler()));
    }
    public override void EnterToException()
    {
        SetNormalState();
        Debug.Log("there have some problem");
    }

    public AnimationPointHandler GetAttckAPHHandler()
    {
        var aph = person.GetNewAPH(1, AnimationPointHandler.WalkingState.Run);
        (aph.GetActionPoint(0) as PersonAnimationPoint).Weapon = person.weapon;
        person.SetAPs(aph.GetActionPoint(0), prepareData.target.modelPhysicsHandler.transform, PersonAniState.StateKind.PrepareAttack, true, 0, false, true);

        return aph;
    }
    //IEnumerator TraceTarget(AnimationPointHandler aph)
    //{
    //    var targetDmgController = prepareData.target.GetComponent<IDamageController>();
    //    var state = State.trace;
    //    while (!aph.isAPHDone)
    //    {
    //        state = IsTargetInHitRange(prepareData.target.modelPhysicsHandler.transform, weapon.Range) ? State.hit : State.trace;

    //        switch (state)
    //        {
    //            case State.trace:
    //                break;
    //            case State.hit:

    //                break;
    //        }

    //        yield return new WaitForFixedUpdate();
    //    }

    //    yield return null;
    //}
    void TransitionState(AnimationPointHandler aph)
    {
        //var targetDmgController = prepareData.target.GetComponent<IDamageController>();
        var state = IsTargetInHitRange(prepareData.target.modelPhysicsHandler.transform, weapon.Range) ? StateKinds.Hit : StateKinds.Tracking;
        var targetPrepareData = new PersonPrepareData(prepareData.target);
        SetState(state, targetPrepareData);
    }

    bool IsTargetInHitRange(Transform target, float range)
    {
        return Vector3.Distance(target.position, person.modelPhysicsHandler.transform.position) <= range;
    }
}
