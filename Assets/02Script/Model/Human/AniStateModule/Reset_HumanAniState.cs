using UnityEngine;
public class Reset_HumanAniState : HumanAniState
{
    public Reset_HumanAniState(HumanAniStateModuleHandler moduleHandler) : base(moduleHandler)
    {
    }
    protected override void StartModule()
    {
        // for (int i = 1; i < ani.layerCount; i++)
        //     ani.SetLayerWeight(i, 0);
        // ani.SetInteger(PersonAniState.SittingLevel.ToString(), (int)SittingLevel.Non);
        // ani.SetBool(PersonAniState.StateKind.LookAround.ToString(), false);
        // ani.SetBool(PersonAniState.StateKind.ShouldStand.ToString(), false);
        // ani.SetBool(PersonAniState.StateKind.ShouldSurprize.ToString(), false);
        // ani.SetFloat(PersonAniState.StateKind.TurnDegree.ToString(), 361f);

        // SetWalkState(WalkLevel.Walk);
        // yield return new WaitUntil(() => IsWalkState());
        // yield return StartCoroutine(base.DoResetAni(shouldReadNextAction));

        // ProcResetAni = null;
    }

    public override void EnterToException()
    {

    }

    public override void Exit()
    {

    }
}