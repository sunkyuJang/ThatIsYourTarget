using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AnimationPointData
{
    public virtual bool HasAction { get { return true; } }
    [HideInInspector]
    public int state = 0;
    [HideInInspector]
    public float during = 0;
    public bool IsUnLimited { get { return during <= -1; } }
    [HideInInspector]
    public float targetDegree = 0;
    public Vector3 CorrectedPosition { set; get; } = Vector3.zero;
    public bool ShouldContinuousReadState { set; get; } = false;
    public bool CanAnimationCancle { set; get; } = true;

    // event 
    public Action<int> EventTrigger { set; get; } = null;
    public Action whenAnimationStart { set; get; } = null;
    public Action whenAnimationEnd { set; get; } = null;
    public Action whenAnimationExitTime { set; get; } = null;

    // positioning
    public bool CanYield { set; get; } = true;

    // aiming detail
    public InteractionObj InteractionObj { set; get; } = null;
    public Weapon Weapon { get { return InteractionObj as Weapon; } }
    public Transform LookAtTransform { set; get; } = null;
    public Transform AimTarget { get { return Weapon?.AimTransform; } }
    public bool ShouldTrackingTarget { get { return LookAtTransform != null; } }
    public float StoppingDistance { set; get; }

    // SkillAnimationName
    public SkillData SkillData { set; get; } = null;
}
