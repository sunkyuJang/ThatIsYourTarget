using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PersonState : StateModule
{
    public enum StateKinds { Normal, Sensed, Curiousity, Warn, Follow, Wait, Attack, Avoid, Dead, Non }
    protected Person person;
    protected float attackDist = 1f;
    protected void SetNormalState() => SetState(StateKinds.Normal);
    protected void SetState(StateKinds kinds) => person.ChangedState((int)kinds);
    protected PersonState GetState(StateKinds kinds) => person.GetState(kinds);
    public static Dictionary<StateKinds, PersonState> GetNewStateList()
    {
        var dic = new Dictionary<StateKinds, PersonState>();

        dic.Add(StateKinds.Normal, new Normal_PersonState());
        dic.Add(StateKinds.Sensed, new Sensed_PersonState());
        dic.Add(StateKinds.Curiousity, new Curiousity_PersonState());

        return dic;
    }
    public static int GetStateCount() => Enum.GetValues(typeof(StateKinds)).Length;
}
