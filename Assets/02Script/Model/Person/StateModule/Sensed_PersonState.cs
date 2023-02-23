using UnityEngine;

public class Sensed_PersonState : PersonState
{
    private Transform target;
    public override bool IsReadyForEnter()
    {
        return target != null;
    }
    public override void EnterToException()
    {
        SetNormalState();
    }
    public override void Enter()
    {
        var dist = person.GetDistTo(target);
        var shouldAttack = dist < attackDist;
        if (shouldAttack)
        {

        }
        else
        {
            var state = GetState(StateKinds.Curiousity);
            if (state != null)
            {
                (state as Curiousity_PersonState)?.PrepareState(target);
            }
        }
        SetState(dist < attackDist ? StateKinds.Attack : StateKinds.Curiousity);
    }
    public override void Exit() { target = null; }
    public void SetChangeForThisState(Transform transform)
    {
        target = transform;
        SetState(StateKinds.Curiousity);
    }

    public override void AfterAPHDone() { }
}
