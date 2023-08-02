using System.Collections;
using UnityEngine;

public class PrepareAttack_PersonState : PersonState
{
    public enum State { trace, hit, miss, non }
    public static float attackDist = 1f;
    protected PersonWeapon weapon { get { return person.weapon; } }
    public PrepareAttack_PersonState(Person person) : base(person) { }

    public override bool IsReadyForEnter()
    {
        return targetModel != null;
    }
    protected override void DoEnter()
    {

    }
    public override void EnterToException()
    {
        SetState(StateKinds.Normal);
        Debug.Log("there have some problem");
    }

    public override void Exit() { }

    void SetDmgToTarget(IDamageController target)
    {
        //var isDead = target.SetDamage(weapon.dmg);
    }
    public ActionPointHandler GetAttckAPHHandler()
    {
        var aph = person.GetNewAPH(1, ActionPointHandler.WalkingState.Run);
        (aph.GetActionPoint(0) as PersonActionPoint).Weapon = person.weapon;
        person.SetAPs(aph.GetActionPoint(0), targetModel.transform, PersonAniState.StateKind.PrepareAttack, true, 0, false, true);

        return aph;
    }

    IEnumerator TraceTarget(ActionPointHandler aph)
    {
        var targetDmgController = targetModel.GetComponent<IDamageController>();
        var state = State.trace;
        while (!aph.isAPHDone)
        {
            state = IsTargetInHitRange(targetModel.transform, weapon.Range) ? State.hit : State.trace;

            switch (state)
            {
                case State.trace:

                    break;
                case State.hit:
                    if (weapon.IsMelee)
                    {

                    }
                    else
                    {
                        weapon.Attack();
                    }
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
