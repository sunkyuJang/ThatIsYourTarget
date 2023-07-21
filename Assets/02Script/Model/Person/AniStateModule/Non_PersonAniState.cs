using UnityEngine;
public class Non_PersonAniState : PersonAniState
{
    public Non_PersonAniState(Animator ani) : base(ani)
    {

    }
    protected override void DoEnter()
    {
        Debug.Log("isIn");
    }

    public override void EnterToException()
    {

    }

    public override void Exit()
    {

    }

}