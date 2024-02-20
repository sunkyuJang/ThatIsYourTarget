using UnityEngine;

internal class TurnAround_HumanAniState : HumanAniState
{
    string TurnDegree { get { return "TurnDegree"; } }
    float defaultDegree = 361f;
    public TurnAround_HumanAniState(HumanAniStateModuleHandler moduleHandler) : base(moduleHandler)
    {
    }

    protected override void StartModule()
    {
        Animator.SetFloat(TurnDegree, ap.animationPointData.targetDegree);
    }

    public override void EnterToException()
    {

    }

    public override void Exit()
    {
        Animator.SetFloat(TurnDegree, defaultDegree);
    }
}