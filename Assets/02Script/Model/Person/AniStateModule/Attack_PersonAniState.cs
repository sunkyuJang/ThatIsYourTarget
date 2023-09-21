using UnityEngine;
public class Attack_PersonAniState : PersonAniState
{
    public string Attack { get { return "Attack"; } }
    public Attack_PersonAniState(Animator ani) : base(ani)
    {

    }
    protected override void StartModule()
    {
        Animator.SetBool(Attack, true);
    }

    public override void EnterToException()
    {

    }

    public override void Exit()
    {
        Animator.SetBool(Attack, false);
    }

}