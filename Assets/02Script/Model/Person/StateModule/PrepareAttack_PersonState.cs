using System.Collections;
using UnityEngine;

public class PrepareAttack_PersonState : PersonState
{
    public enum State { trace, hit, miss, non }
    public static float attackDist = 1f;
    protected PersonWeapon weapon { get { return person.weapon; } }
    public PrepareAttack_PersonState(Person person) : base(person) { }

    public override bool IsReady()
    {
        return prepareData != null;
    }
    protected override void StartModule()
    {

    }
    public override void EnterToException()
    {
        SetNormalState();
        Debug.Log("there have some problem");
    }

    public ActionPointHandler GetAttckAPHHandler()
    {
        var aph = person.GetNewAPH(1, ActionPointHandler.WalkingState.Run);
        (aph.GetActionPoint(0) as PersonActionPoint).Weapon = person.weapon;
        person.SetAPs(aph.GetActionPoint(0), prepareData.target.transform, PersonAniState.StateKind.PrepareAttack, true, 0, false, true);

        return aph;
    }

    IEnumerator TraceTarget(ActionPointHandler aph)
    {
        var targetDmgController = prepareData.target.GetComponent<IDamageController>();
        var state = State.trace;
        while (!aph.isAPHDone)
        {
            state = IsTargetInHitRange(prepareData.target.transform, weapon.Range) ? State.hit : State.trace;

            switch (state)
            {
                case State.trace:

                    break;
                case State.hit:

                    break;
            }

            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }

    bool IsTargetInHitRange(Transform target, float range)
    {
        return Vector3.Distance(target.position, person.modelHandler.transform.position) <= range;
    }
}
