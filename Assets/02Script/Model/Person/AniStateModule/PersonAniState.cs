using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PersonAniState : StateModule
{
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
        PrepareAttack,
        Fight,
        Avoid,
        TurnAround,
        TurnHead,
    }

    protected PersonAniController aniController;
    protected PersonActionPoint ap;
    public void SetAP(PersonActionPoint ap) => this.ap = ap;
    public override bool IsReadyForEnter() { return ap != null; }
    protected void SetNormalState() => SetState(StateKind.Standing);
    protected void SetState(StateKind kind) => person.SetState((int)kind);
    protected PersonAniState GetState(StateKind kind) => person.GetState(kind);
    public PersonAniState(PersonAniController aniController) => this.aniController = aniController;

    public static Dictionary<StateKind, PersonAniState> GetNewStateList(PersonAniController aniController)
    {
        var dic = new Dictionary<StateKind, PersonAniState>();

        dic.Add(StateKind.Non, new Non_PersonAniState(aniController));
        dic.Add(StateKind.Reset, new Reset_PersonAniState(aniController));
        dic.Add(StateKind.Walk, new Walk_PersonAniState(aniController));
        dic.Add(StateKind.Run, new Run_PersonAniState(aniController));
        dic.Add(StateKind.Standing, new Standing_PersonAniState(aniController));
        dic.Add(StateKind.LookAround, new LookAround_PersonAniState(aniController));
        dic.Add(StateKind.Sitting, new Sitting_PersonAniState(aniController));
        dic.Add(StateKind.Surprize, new Surprize_PersonAniState(aniController));
        dic.Add(StateKind.PrepareAttack, new PrepareAttack_PersonAniState(aniController));
        dic.Add(StateKind.Fight, new Fight_PersonAniState(aniController));
        dic.Add(StateKind.Avoid, new Avoid_PersonAniState(aniController));
        dic.Add(StateKind.TurnAround, new TurnAround_PersonAniState(aniController));
        dic.Add(StateKind.TurnHead, new TurnHead_PersonAniState(aniController));

        return dic;
    }

    public static int GetStateCount() => Enum.GetValues(typeof(StateKind)).Length;
}
