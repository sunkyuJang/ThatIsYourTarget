using System.Collections;
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
        Run,
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

    protected Animator Animator { set; get; }
    protected PersonActionPoint ap;
    public void SetAP(PersonActionPoint ap) => this.ap = ap;
    public override bool IsReadyForEnter() { return ap != null && Animator != null; }
    public PersonAniState(Animator aniController) => this.Animator = aniController;

    public static Dictionary<StateKind, PersonAniState> GetNewStateList(Animator animator)
    {
        var dic = new Dictionary<StateKind, PersonAniState>();

        dic.Add(StateKind.Non, new Non_PersonAniState(animator));
        dic.Add(StateKind.Reset, new Reset_PersonAniState(animator));
        dic.Add(StateKind.Walk, new Walk_PersonAniState(animator));
        dic.Add(StateKind.Standing, new Standing_PersonAniState(animator));
        dic.Add(StateKind.LookAround, new LookAround_PersonAniState(animator));
        dic.Add(StateKind.Sitting, new Sitting_PersonAniState(animator));
        dic.Add(StateKind.Surprize, new Surprize_PersonAniState(animator));
        // dic.Add(StateKind.PrepareAttack, new PrepareAttack_PersonAniState(animator));
        // dic.Add(StateKind.Fight, new Fight_PersonAniState(animator));
        // dic.Add(StateKind.Avoid, new Avoid_PersonAniState(animator));
        dic.Add(StateKind.TurnAround, new TurnAround_PersonAniState(animator));
        // dic.Add(StateKind.TurnHead, new TurnHead_PersonAniState(animator));

        return dic;
    }
}
