using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_PersonState : PersonState
{
    Sensed_PersonState.PreparingData preparingData { set; get; }
    public static float attackDist = 1f;
    protected Weapon weapon { get { return person.weapon; } }
    public Attack_PersonState(Person person) : base(person) { }
    public void PrepareState(Sensed_PersonState.PreparingData param)
    {
        if (preparingData == null)
        {
            preparingData = param as Sensed_PersonState.PreparingData;
        }

        SetState(StateKinds.Attack);
    }
    public override bool IsReadyForEnter()
    {
        return preparingData != null;
    }
    public override void Enter()
    {

    }
    public override void EnterToException()
    {
        SetState(StateKinds.Normal);
        Debug.Log("there have some problem");
    }
    public override void AfterAPHDone() { }
    public override void Exit() { }

    void SetDmgToTarget(IDamageController target)
    {
        //var isDead = target.SetDamage(weapon.dmg);
    }
    public ActionPointHandler GetAttckHandler(Sensed_PersonState.PreparingData preparingData)
    {
        var target = preparingData.target;
        var aph = person.GetNewAPH(4);
        // person.SetAPs(aph.GetActionPoint(0), target, PersonAniState.StateKind.PrepareAttack, true, 0, false, true);
        // person.SetAPs(aph.GetActionPoint(1), target, PersonAniController.StateKind.Run, false, 0, false, false);
        // person.SetAPs(aph.GetActionPoint(2), target, PersonAniController.StateKind.Fight, false, 0, false, false);
        // person.SetAPs(aph.GetActionPoint(3), target, PersonAniController.StateKind.LookAround, true, 0, false, false);

        return aph;
    }

    IEnumerator TraceTarget(Sensed_PersonState.PreparingData preparingData, ActionPointHandler aph)
    {
        var target = preparingData.target;
        var targetDmgController = target.GetComponent<IDamageController>();
        while (!aph.isAPHDone)
        {
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
}
