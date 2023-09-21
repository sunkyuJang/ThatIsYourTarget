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
        //TurnHead,
        Non,
    }
    readonly public static List<StateKind> FixedDuringStateKinds = new List<StateKind>() { StateKind.LookAround, StateKind.Surprize, StateKind.DrawWeapon, StateKind.Attack, StateKind.TurnAround };
    public static bool IsStateDuringFixed(StateKind kind) => FixedDuringStateKinds.Contains(kind);
    public static int ConverStateToInt(StateKind stateKind) => (int)stateKind;
    public PersonAniState(Animator aniController) => this.Animator = aniController;
    public override bool IsReady() { return ap != null && Animator != null; }
    protected Animator Animator { set; get; }
    protected PersonAnimationPoint ap;
    public void SetAP(PersonAnimationPoint ap) => this.ap = ap;

    public static List<StateModule> GetStatesList(Animator animator)
    {
        if (animator != null)
        {
            var stateList = new List<StateModule>();

            StateModule state = null;
            for (StateKind stateKind = StateKind.Reset; stateKind <= StateKind.Non; stateKind++)
            {
                switch (stateKind)
                {
                    case StateKind.Non: state = new Non_PersonAniState(animator); break;
                    case StateKind.Reset: state = new Reset_PersonAniState(animator); break;
                    case StateKind.Walk: state = new Walk_PersonAniState(animator); break;
                    case StateKind.Standing: state = new Standing_PersonAniState(animator); break;
                    case StateKind.LookAround: state = new LookAround_PersonAniState(animator); break;
                    case StateKind.Sitting: state = new Sitting_PersonAniState(animator); break;
                    case StateKind.Surprize: state = new Surprize_PersonAniState(animator); break;
                    case StateKind.DrawWeapon: state = new DrawWeapon_PersonAniState(animator); break;
                    case StateKind.Attack: state = new Attack_PersonAniState(animator); break;
                    case StateKind.TurnAround: state = new TurnAround_PersonAniState(animator); break;
                }

                stateList.Add(state);
            }

            return stateList;
        }

        return null;
    }
}
