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
        PrepareAttack,
        Tracking,
        Hit,
        Avoid,
        Dead,
        Non
    }

    protected Person person;
    public PersonState(Person person) => this.person = person;
    public static int ConvertStateKindToInt(StateKinds kinds) => (int)kinds;
    protected void SetState(StateKinds kinds)
    {
        person.personInfoUI.StateModule.text = kinds.ToString();
        person.SetState(ConvertStateKindToInt(kinds));
    }
    protected void SetNormalState() => SetState(StateKinds.Normal);
    public void AfterAPHDone()
    {
        var state = DoAfterDone();
        SetState(state);
    }
    protected virtual StateKinds DoAfterDone()
    {
        return StateKinds.Normal;
    }
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
                    case StateKinds.PrepareAttack: list.Add(new PrepareAttack_PersonState(person)); break;
                    case StateKinds.Tracking: list.Add(new Tracking_PersonState(person)); break;
                    case StateKinds.Hit: list.Add(new Hit_PersonState(person)); break;
                    case StateKinds.Dead: list.Add(new Dead_PersonState(person)); break;
                    default: list.Add(null); break;
                }
            }

            return list;
        }

        return null;
    }
}
