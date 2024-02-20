using System.Collections.Generic;
using UnityEngine;

public abstract class HumanAniState : StateModule
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
        KeepingWeapon,
        HoldingWeapon,
        UsingWeapon,
        Attack,
        // Avoid,
        TurnAround,
        Dead,
        Non,
    }
    // fixed duration list
    readonly public static List<StateKind> FixedDuringStateKinds = new List<StateKind>()
    {
        HumanAniState.StateKind.LookAround,
        HumanAniState.StateKind.Surprize,
        HumanAniState.StateKind.Attack,
        HumanAniState.StateKind.TurnAround
    };

    readonly public static List<StateKind> FixedDuringAfterRuntime = new List<StateKind>()
    {
        HumanAniState.StateKind.Attack,
    };

    readonly public static List<StateKind> AttackKind = new List<StateKind>()
    {
        HumanAniState.StateKind.Attack,
    };

    public static bool IsStateDuringFixed(StateKind kind) => FixedDuringStateKinds.Contains(kind);
    // public static bool IsStateDuringFixedAfterRuntime(StateKind kind) => FixedDuringAfterRuntime.Contains(kind);
    //public static bool IsWeaponRuntimeState(StateKind kind) => WeaponRuntimState.Contains(kind);
    public static bool IsAttackKind(StateKind kind) => AttackKind.Contains(kind);

    // fixed position list
    readonly public static List<StateKind> shouldPlaySamePositions = new List<StateKind>()
    {
        HumanAniState.StateKind.Sitting,
    };
    readonly public static StateKind replacebleState = StateKind.Standing;

    public HumanAniState(HumanAniStateModuleHandler moduleHandler) => ModuleHandler = moduleHandler;
    protected HumanAniStateModuleHandler ModuleHandler { set; get; }
    protected Animator Animator { get { return ModuleHandler.Animator; } }
    protected HumanAnimationPoint ap;
    public override bool IsReady()
    {
        return ap != null && Animator != null;
    }
    public void SetAP(HumanAnimationPoint ap) => this.ap = ap;

    public static List<StateModule> GetStatesList(HumanAniStateModuleHandler moduleHandler)
    {
        if (moduleHandler != null)
        {
            var stateList = new List<StateModule>();

            StateModule state = null;
            for (StateKind stateKind = StateKind.Reset; stateKind <= StateKind.Non; stateKind++)
            {
                switch (stateKind)
                {
                    case StateKind.Non: state = new Non_HumanAniState(moduleHandler); break;
                    case StateKind.Reset: state = new Reset_HumanAniState(moduleHandler); break;
                    case StateKind.Walk: state = new Walk_HumanAniState(moduleHandler); break;
                    case StateKind.Standing: state = new Standing_HumanAniState(moduleHandler); break;
                    case StateKind.LookAround: state = new LookAround_HumanAniState(moduleHandler); break;
                    case StateKind.Sitting: state = new Sitting_HumanAniState(moduleHandler); break;
                    case StateKind.Surprize: state = new Surprize_HumanAniState(moduleHandler); break;
                    case StateKind.KeepingWeapon: state = new KeepingWeapon_HumanAniState(moduleHandler); break;
                    case StateKind.HoldingWeapon: state = new HoldingWeapon_HumanAniState(moduleHandler); break;
                    case StateKind.UsingWeapon: state = new UsingWeapon_HumanAniState(moduleHandler); break;
                    case StateKind.Attack: state = new Attack_HumanAniState(moduleHandler); break;
                    case StateKind.TurnAround: state = new TurnAround_HumanAniState(moduleHandler); break;
                    case StateKind.Dead: state = new Dead_HumanAniState(moduleHandler); break;
                }

                stateList.Add(state);
            }

            return stateList;
        }

        return null;
    }
}
