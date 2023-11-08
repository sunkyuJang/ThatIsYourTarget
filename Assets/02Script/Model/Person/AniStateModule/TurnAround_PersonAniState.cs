using UnityEngine;

internal class TurnAround_PersonAniState : PersonAniState
{
    string TurnDegree { get { return "TurnDegree"; } }
    float defaultDegree = 361f;
    public TurnAround_PersonAniState(PersonAniStateModuleHandler moduleHandler) : base(moduleHandler)
    {
    }

    protected override void StartModule()
    {
        Animator.SetFloat(TurnDegree, ap.targetDegree);
    }

    public override void EnterToException()
    {

    }

    public override void Exit()
    {
        Animator.SetFloat(TurnDegree, defaultDegree);
    }
}