using UnityEngine;
internal class Standing_PersonAniState : PersonAniState
{
    public Standing_PersonAniState(Animator animator) : base(animator)
    {

    }

    public override void Enter()
    {
        // when model stop moving, it playing idle.
        // idle == standing
    }

    public override void EnterToException()
    {

    }

    public override void Exit()
    {

    }
}