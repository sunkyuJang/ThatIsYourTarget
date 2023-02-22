using UnityEngine;

public class Sensed_PersonState : PersonState
{
    private Transform target;
    public override void Enter()
    {
        if (target == null)
        {
            SetNormalState();
            return;
        }

        var dist = person.GetDistTo(target);
        var shouldAttack = dist < attackDist;
        if (shouldAttack)
        {

        }
        else
        {
            var state = GetState(StateKinds.Attack);
            if (state != null)
            {
                (state as Curiousity_PersonState)?.SetChangeForThisState(target);
            }
        }
        SetState(dist < attackDist ? StateKinds.Attack : StateKinds.Curiousity);
    }
    public override void Exit() { target = null; }
    protected override void Update() { }
    public void SetChangeForThisState(Transform transform)
    {
        target = transform;
        SetState(StateKinds.Curiousity);
    }

    public override void AfterAPHDone() { }
}
