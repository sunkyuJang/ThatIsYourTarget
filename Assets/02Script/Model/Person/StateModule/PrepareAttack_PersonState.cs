using System.Collections;
using UnityEngine;

public class PrepareAttack_PersonState : PersonState
{
    public enum State { trace, hit, miss, non }
    PrepareData preparingData { set; get; }
    public static float attackDist = 1f;
    protected PersonWeapon weapon { get { return person.weapon; } }
    public PrepareAttack_PersonState(Person person) : base(person) { }

    public void PrepareState(PrepareData param)
    {
        if (preparingData == null)
        {
            preparingData = param;
        }
    }
    public override bool IsReadyForEnter()
    {
        return preparingData != null;
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
    public ActionPointHandler GetAttckHandler(Sensed_PersonState.PreparingData preparingData)
    {
        var target = preparingData.target;
        var aph = person.GetNewAPH(1, ActionPointHandler.WalkingState.Run);
        (aph.GetActionPoint(0) as PersonActionPoint).Weapon = person.weapon;
        person.SetAPs(aph.GetActionPoint(0), target, PersonAniState.StateKind.PrepareAttack, true, 0, false, true);

        return aph;
    }

    IEnumerator TraceTarget(PrepareData preparingData, ActionPointHandler aph)
    {
        var target = preparingData.target;
        var targetDmgController = target.GetComponent<IDamageController>();
        var state = State.trace;
        while (!aph.isAPHDone)
        {
            state = IsTargetInRange(target, weapon.Range) ? State.hit : State.trace;
            if (state == State.trace)
            {

            }

            if (IsTargetInRange(target, weapon.Range))
            {
                if (weapon.IsMelee)
                {

                }
                else
                {
                    weapon.Attack();
                }
            }

            yield return new WaitForFixedUpdate();
        }

        yield return null;
    }



    bool IsTargetInRange(Transform target, float range)
    {
        return Vector3.Distance(target.position, person.modelHandler.transform.position) <= range;
    }

    public class PrepareData
    {
        public Transform target;
        public bool isInSight;
    }
}
