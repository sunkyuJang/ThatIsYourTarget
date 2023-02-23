using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_PersonState : PersonState
{
    public static float attackDist = 1f;
    public override bool IsReadyForEnter()
    {

        return true;
    }
    public override void Enter() { }
    public override void EnterToException()
    {
    }
    public override void AfterAPHDone() { }
    public override void Exit() { }
}
