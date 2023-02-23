using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Normal_PersonState : PersonState
{
    public override bool IsReadyForEnter()
    {
        return true;
    }
    public override void EnterToException() { }
    public override void Enter()
    {
        person.SetOriginalAPH();
    }
    public override void Exit() { }
    public override void AfterAPHDone() { }
}
