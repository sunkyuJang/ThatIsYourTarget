using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dead_PersonState : PersonState
{
    public Dead_PersonState(Person person) : base(person)
    {

    }
    public override bool IsReadyForEnter()
    {
        return true;
    }
    public override void EnterToException() { }
    public override void Enter()
    {
        person.modelHandler.SetDead();
    }
    public override void Exit() { }
    public override void AfterAPHDone() { }
}
