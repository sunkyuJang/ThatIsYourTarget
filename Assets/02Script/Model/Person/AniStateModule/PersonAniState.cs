using System.Collections.Generic;
using UnityEngine;

public abstract class PersonAniState : StateModule
{
    //*if you need change state, you have to controll here frist*/
    public enum StateKind
    {
        Non,
        Reset,
        Walk,
        //Run,
        Standing,
        LookAround,
        Sitting,
        Surprize,
        // PrepareAttack,
        // Fight,
        // Avoid,
        TurnAround,
        //TurnHead,
    }
    public static int ConverStateToInt(PersonAniState.StateKind stateKind) => (int)stateKind;
    public PersonAniState(Animator aniController) => this.Animator = aniController;
    protected override bool IsReadyForEnter() { return ap != null && Animator != null; }
    protected Animator Animator { set; get; }
    protected PersonActionPoint ap;
    public void SetAP(PersonActionPoint ap) => this.ap = ap;

    public static List<StateModule> GetStatesList(Animator animator)
    {
        if (animator != null)
        {
            var stateList = new List<StateModule>();

            StateModule state = null;
            for (StateKind stateKind = StateKind.Non; stateKind <= StateKind.TurnAround; stateKind++)
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
                    case StateKind.TurnAround: state = new TurnAround_PersonAniState(animator); break;
                }

                stateList.Add(state);
            }

            return stateList;
        }

        return null;
    }
}
