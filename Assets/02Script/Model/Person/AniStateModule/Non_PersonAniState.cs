using UnityEngine;
public class Non_PersonAniState : PersonAniState
{
    public Non_PersonAniState(Animator ani) : base(ani)
    {

    }
    protected override void StartModule()
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