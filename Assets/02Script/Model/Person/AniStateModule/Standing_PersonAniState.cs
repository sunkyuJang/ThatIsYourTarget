using UnityEngine;
internal class Standing_PersonAniState : PersonAniState
{
    public Standing_PersonAniState(PersonAniStateModuleHandler moduleHandler) : base(moduleHandler)
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