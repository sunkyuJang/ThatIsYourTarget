using System;
using System.Collections.Generic;

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
    public PersonState(Person person) => this.person = person;
    public static int ConvertStateKindToInt(StateKinds kinds) => (int)kinds;
    protected void SetState(StateKinds kinds) => person.SetState(ConvertStateKindToInt(kinds));
    protected void SetNormalState() => SetState(StateKinds.Normal);
    public virtual void AfterAPHDone() { }
    public static int GetStateCount() => Enum.GetValues(typeof(StateKinds)).Length;
    public static List<StateModule> GetStatesList(Person person)
    {
        if (person is Person)
        {
            var list = new List<StateModule>();
            for (StateKinds kinds = StateKinds.Normal; kinds != StateKinds.Non; kinds++)
            {
                switch (kinds)
                {
                    case StateKinds.Normal: list.Add(new Normal_PersonState(person)); break;
                    case StateKinds.Sensed: list.Add(new Sensed_PersonState(person)); break;
                    case StateKinds.Curiousity: list.Add(new Curiousity_PersonState(person)); break;
                    case StateKinds.Attack: list.Add(new Attack_PersonState(person)); break;
                    case StateKinds.Dead: list.Add(new Dead_PersonState(person)); break;
                    default: list.Add(null); break;
                }
            }

            return list;
        }

        return null;
    }
}
