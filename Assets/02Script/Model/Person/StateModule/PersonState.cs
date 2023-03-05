using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PersonState : StateModule
{
    public enum StateKinds
    {
        Normal,
        Sensed,
        Curiousity,
        Warn,
        Follow,
        Wait,
        Attack,
        Avoid,
        Dead,
        Non
    }

    protected Person person;
    protected void SetNormalState() => SetState(StateKinds.Normal);
    protected void SetState(StateKinds kinds) => person.SetState((int)kinds);
    protected PersonState GetState(StateKinds kinds) => person.GetState(kinds);
    public PersonState(Person person) => this.person = person;
    public static Dictionary<StateKinds, PersonState> GetNewStateList(Person person)
    {
        var dic = new Dictionary<StateKinds, PersonState>();

        dic.Add(StateKinds.Normal, new Normal_PersonState(person));
        dic.Add(StateKinds.Sensed, new Sensed_PersonState(person));
        dic.Add(StateKinds.Curiousity, new Curiousity_PersonState(person));

        return dic;
    }
    public static int GetStateCount() => Enum.GetValues(typeof(StateKinds)).Length;
}
