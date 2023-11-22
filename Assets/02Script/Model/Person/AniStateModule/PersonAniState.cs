using System.Collections.Generic;
using UnityEngine;

public abstract class PersonAniState : StateModule
{
    //*if you need change state, you have to controll here frist*/
    public enum StateKind
    {
        Reset,
        Walk,
        //Run,
        Standing,
        LookAround,
        Sitting,
        Surprize,
        DrawWeapon,
        Attack,
        // Avoid,
        TurnAround,
        Dead,
        Non,
    }
    public PersonAniState(PersonAniStateModuleHandler moduleHandler) => ModuleHandler = moduleHandler;
    protected PersonAniStateModuleHandler ModuleHandler { set; get; }
    protected Animator Animator { get { return ModuleHandler.Animator; } }
    protected PersonAnimationPoint ap;
    public override bool IsReady() { return ap != null && Animator != null; }
    public void SetAP(PersonAnimationPoint ap) => this.ap = ap;

    public static List<StateModule> GetStatesList(PersonAniStateModuleHandler moduleHandler)
    {
        if (moduleHandler != null)
        {
            var stateList = new List<StateModule>();

            StateModule state = null;
            for (StateKind stateKind = StateKind.Reset; stateKind <= StateKind.Non; stateKind++)
            {
                switch (stateKind)
                {
                    case StateKind.Non: state = new Non_PersonAniState(moduleHandler); break;
                    case StateKind.Reset: state = new Reset_PersonAniState(moduleHandler); break;
                    case StateKind.Walk: state = new Walk_PersonAniState(moduleHandler); break;
                    case StateKind.Standing: state = new Standing_PersonAniState(moduleHandler); break;
                    case StateKind.LookAround: state = new LookAround_PersonAniState(moduleHandler); break;
                    case StateKind.Sitting: state = new Sitting_PersonAniState(moduleHandler); break;
                    case StateKind.Surprize: state = new Surprize_PersonAniState(moduleHandler); break;
                    case StateKind.DrawWeapon: state = new DrawWeapon_PersonAniState(moduleHandler); break;
                    case StateKind.Attack: state = new Attack_PersonAniState(moduleHandler); break;
                    case StateKind.TurnAround: state = new TurnAround_PersonAniState(moduleHandler); break;
                    case StateKind.Dead: state = new Dead_PersonAniState(moduleHandler); break;
                }

                stateList.Add(state);
            }

            return stateList;
        }

        return null;
    }
}
