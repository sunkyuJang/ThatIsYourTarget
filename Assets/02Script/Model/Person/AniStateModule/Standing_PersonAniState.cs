using UnityEngine;
internal class Standing_PersonAniState : PersonAniState
{
    public Standing_PersonAniState(Animator animator) : base(animator)
    {

    }

    protected override void StartModule()
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