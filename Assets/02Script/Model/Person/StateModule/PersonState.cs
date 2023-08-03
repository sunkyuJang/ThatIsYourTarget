using System;
using System.Collections.Generic;

public abstract class PersonState : StateModule
{
    public enum StateKinds
    {
        Normal,
        Sensed,
        Curiousity,
        //Warn,
        //Follow,
        //Wait,
        PrepareAttack,
        Tracking,
        Patrol,
        Hit,
        //Avoid,
        Dead,
        Non
    }

    protected Person person;
    public PersonPrepareData prepareData { protected set; get; } = null;
    public PersonState(Person person)
    {
        this.person = person;
    }
    public void AfterAPHDone()
    {
        var state = DoAfterDone(out PersonPrepareData prepareData);
        SetState(state, prepareData);
    }
    public override void Exit()
    {
        prepareData = null;
    }
    protected virtual StateKinds DoAfterDone(out PersonPrepareData prepareData)
    {
        prepareData = null;
        return StateKinds.Normal;
    }
    protected bool IsTargetModelSame(PersonState stateModule)
    {
        return prepareData.target == stateModule.prepareData.target;
    }
    public static int ConvertStateKindToInt(StateKinds kinds)
    {
        return (int)kinds;
    }
    protected void SetState(StateKinds kinds, PersonPrepareData prepareData)
    {
        person.personInfoUI.StateModule.text = kinds.ToString();
        person.SetState(ConvertStateKindToInt(kinds), prepareData);
    }
    protected void SetNormalState() => SetState(StateKinds.Normal, null);
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
                    case StateKinds.Patrol: list.Add(new Patrol_PersonState(person)); break;
                    case StateKinds.Hit: list.Add(new Hit_PersonState(person)); break;
                    case StateKinds.Dead: list.Add(new Dead_PersonState(person)); break;
                    default: list.Add(null); break;
                }
            }

            return list;
        }

        return null;
    }

    public class PersonPrepareData : PrepareData
    {
        public Model target { private set; get; }
        public PersonPrepareData(Model target) { this.target = target; }
    }
}
