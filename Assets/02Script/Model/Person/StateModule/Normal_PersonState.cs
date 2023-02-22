using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Normal_PersonState : PersonState
{
    public override void Enter()
    {
        person.SetOriginalAPH();
    }

    public override void Exit() { }

    protected override void Update() { }
    public override void AfterAPHDone() { }
}
